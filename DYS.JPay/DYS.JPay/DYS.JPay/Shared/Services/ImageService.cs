using DYS.JPay.Shared.Shared.Helpers;
using DYS.JPay.Shared.Shared.Services;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Services
{
    public class ImageService: IImageService
    { 
        private readonly IPhotoService _photoService;
        public ImageService(IPhotoService photoService)
        {
            _photoService = photoService;
        }
        public async Task<(string tempPath, string photoPath)> PickAndResizeAsync(int targetSize)
        {
            try
            {
                FileResult? result = null;
                if (DeviceInfo.Platform == DevicePlatform.WinUI)
                {
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {

                        var result = await FilePicker.Default.PickAsync(new PickOptions
                        {
                            PickerTitle = "Select a photo",
                            FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                            {
                                { DevicePlatform.WinUI, new[] { ".jpg", ".jpeg", ".png" } },
                                { DevicePlatform.Android, new[] { "image/*" } },
                                { DevicePlatform.iOS, new[] { "public.image" } }
                            })
                        });
                    });
                }
                else
                {
                    // Mobile/mac platforms
                    result = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
                    {
                        Title = "Select a photo"
                    });
                }

                if (result == null) return ("","");

                var inputPath = Path.Combine(FileSystem.AppDataDirectory, result.FileName);
                using (var stream = await result.OpenReadAsync())
                using (var fileStream = File.Create(inputPath))
                    await stream.CopyToAsync(fileStream);

                var uniqueId = Guid.NewGuid().ToString();
                var outputPath = Path.Combine(FileSystem.AppDataDirectory, $"{uniqueId}.jpg");

                using var input = File.OpenRead(inputPath);
                using var bitmap = SKBitmap.Decode(input);

                // --- Scale to fill square ---
                float widthRatio = (float)targetSize / bitmap.Width;
                float heightRatio = (float)targetSize / bitmap.Height;
                float scale = Math.Max(widthRatio, heightRatio); // use Max so we fill the square

                int scaledWidth = (int)(bitmap.Width * scale);
                int scaledHeight = (int)(bitmap.Height * scale);

                // Resize the bitmap
                using var resized = bitmap.Resize(new SKImageInfo(scaledWidth, scaledHeight), SKFilterQuality.High);

                // --- Crop center to square ---
                int cropX = (scaledWidth - targetSize) / 2;
                int cropY = (scaledHeight - targetSize) / 2;

                var cropRect = new SKRectI(cropX, cropY, cropX + targetSize, cropY + targetSize);

                using var surface = SKSurface.Create(new SKImageInfo(targetSize, targetSize));
                surface.Canvas.DrawBitmap(resized, cropRect, new SKRect(0, 0, targetSize, targetSize));
                surface.Canvas.Flush();

                using var image = surface.Snapshot();
                using var data = image.Encode(SKEncodedImageFormat.Jpeg, 90);

                using var output = File.Open(outputPath, FileMode.Create, FileAccess.Write);
                data.SaveTo(output);

                var identifier = await _photoService.SaveImageToAlbumAsync(outputPath, "jpay");
                return (outputPath,identifier);
            }
            catch (Exception ex)
            {
                return (ex.Message, ex.Message);
            }
        }
    }
}
