using DYS.JPay.Shared.Shared.Helpers;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Services
{
    public class ImageService: IImageService
    {
        //public async Task<string> PickAndResizeAsync(int targetWidth, int targetHeight)
        //{
        //    var result = await FilePicker.PickAsync(new PickOptions
        //    {
        //        PickerTitle = "Select an image",
        //        FileTypes = FilePickerFileType.Images
        //    });

        //    if (result == null) return null;

        //    var inputPath = Path.Combine(FileSystem.AppDataDirectory, result.FileName);
        //    using (var stream = await result.OpenReadAsync())
        //    using (var fileStream = File.Create(inputPath))
        //        await stream.CopyToAsync(fileStream);

        //    var uniqueId = Guid.NewGuid().ToString();
        //    var outputPath = Path.Combine(FileSystem.AppDataDirectory, $"{uniqueId}.jpg");

        //    using var input = File.OpenRead(inputPath);
        //    using var bitmap = SKBitmap.Decode(input);

        //    // Calculate proportional scale
        //    float widthRatio = (float)targetWidth / bitmap.Width;
        //    float heightRatio = (float)targetHeight / bitmap.Height;
        //    float scale = Math.Min(widthRatio, heightRatio);

        //    int newWidth = (int)(bitmap.Width * scale);
        //    int newHeight = (int)(bitmap.Height * scale);

        //    // Create surface with target size
        //    using var surface = SKSurface.Create(new SKImageInfo(targetWidth, targetHeight));
        //    surface.Canvas.Clear(SKColors.White);

        //    // Draw resized image centered
        //    var destRect = new SKRect(
        //        (targetWidth - newWidth) / 2,
        //        (targetHeight - newHeight) / 2,
        //        (targetWidth - newWidth) / 2 + newWidth,
        //        (targetHeight - newHeight) / 2 + newHeight
        //    );

        //    surface.Canvas.DrawBitmap(bitmap, destRect);
        //    surface.Canvas.Flush();

        //    using var image = surface.Snapshot();
        //    using var data = image.Encode(SKEncodedImageFormat.Jpeg, 90);

        //    using var output = File.Open(outputPath, FileMode.Create, FileAccess.Write);
        //    data.SaveTo(output);

        //    return outputPath;
        //}

        public async Task<string> PickAndResizeAsync(int targetWidth, int targetHeight)
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Select an image",
                FileTypes = FilePickerFileType.Images
            });

            if (result == null) return null;

            var inputPath = Path.Combine(FileSystem.AppDataDirectory, result.FileName);
            using (var stream = await result.OpenReadAsync())
            using (var fileStream = File.Create(inputPath))
                await stream.CopyToAsync(fileStream);

            var uniqueId = Guid.NewGuid().ToString();
            var outputPath = Path.Combine(FileSystem.AppDataDirectory, $"{uniqueId}.jpg");

            using var input = File.OpenRead(inputPath);
            using var bitmap = SKBitmap.Decode(input);

            // Calculate proportional scale to fit inside square
            float widthRatio = (float)targetWidth / bitmap.Width;
            float heightRatio = (float)targetHeight / bitmap.Height;
            float scale = Math.Min(widthRatio, heightRatio);

            int newWidth = (int)(bitmap.Width * scale);
            int newHeight = (int)(bitmap.Height * scale);

            // Create square canvas
            using var surface = SKSurface.Create(new SKImageInfo(targetWidth, targetHeight));
            surface.Canvas.Clear(SKColors.White);

            // Center the resized image
            int x = (targetWidth - newWidth) / 2;
            int y = (targetHeight - newHeight) / 2;
            var destRect = new SKRect(x, y, x + newWidth, y + newHeight);

            surface.Canvas.DrawBitmap(bitmap, destRect);
            surface.Canvas.Flush();

            using var image = surface.Snapshot();
            using var data = image.Encode(SKEncodedImageFormat.Jpeg, 90);

            using var output = File.Open(outputPath, FileMode.Create, FileAccess.Write);
            data.SaveTo(output);

            return outputPath;
        }
        public async Task<string> PickAndResizeAsync(int targetSize)
        {
            try
            {
                var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Select a photo"
                });

                if (result == null) return null;

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

                return outputPath;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }



    }
}
