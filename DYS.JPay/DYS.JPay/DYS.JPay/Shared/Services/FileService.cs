using DYS.JPay.Shared.Shared.Dtos;
using DYS.JPay.Shared.Shared.Helpers;
using DYS.JPay.Shared.Shared.Services;
using SkiaSharp;

namespace DYS.JPay.Shared.Services
{
    public class FileService: IFileService
    { 
        private readonly IPhotoService _photoService;
        public FileService(IPhotoService photoService)
        {
            _photoService = photoService;
        }


        public async Task<(string tempPath, string photoPath)> PickAndResizeAsync(int targetSize)
        {
            string outputPath = string.Empty;
            string identifier = string.Empty;

            try
            {
                FileResult? result = null;

                if (DeviceInfo.Platform == DevicePlatform.WinUI)
                {
                    result = await FilePicker.Default.PickAsync(new PickOptions
                    {
                        PickerTitle = "Select a photo",
                        FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                            {
                                { DevicePlatform.WinUI, new[] { ".jpg", ".jpeg", ".png" } },
                                { DevicePlatform.Android, new[] { "image/*" } },
                                { DevicePlatform.iOS, new[] { "public.image" } }
                            })
                    });
                }
                else
                {
                    result = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
                    {
                        Title = "Select a photo"
                    });
                }

                if (result == null)
                    return (string.Empty, string.Empty);

                // Copy file to app data dir
                var inputPath = Path.Combine(FileSystem.AppDataDirectory, result.FileName);
                using (var stream = await result.OpenReadAsync())
                using (var fileStream = File.Create(inputPath))
                    await stream.CopyToAsync(fileStream);

                var uniqueId = Guid.NewGuid().ToString();
                outputPath = Path.Combine(FileSystem.AppDataDirectory, $"{uniqueId}.jpg");

                using var input = File.OpenRead(inputPath);
                using var bitmap = SKBitmap.Decode(input);

                // --- Scale to fill square ---
                float widthRatio = (float)targetSize / bitmap.Width;
                float heightRatio = (float)targetSize / bitmap.Height;
                float scale = Math.Max(widthRatio, heightRatio);

                int scaledWidth = (int)(bitmap.Width * scale);
                int scaledHeight = (int)(bitmap.Height * scale);

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

                identifier = await _photoService.SaveImageToAlbumAsync(outputPath, "jpay");

                return (outputPath, identifier);
            }
            catch (Exception ex)
            {
                // Return error message in photoPath for debugging
                return (string.Empty, ex.Message);
            }
        }

        public async Task<OutputDto<List<ProductTemplateDto>>> GetFileAsync()
        {
            var output = new OutputDto<List<ProductTemplateDto>>();

            try
            {
                FileResult? result = null;

                if (DeviceInfo.Platform == DevicePlatform.WinUI)
                {
                    result = await FilePicker.Default.PickAsync(new PickOptions
                    {
                        PickerTitle = "Select a file",
                        FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.WinUI, new[] { ".csv", ".xlsx" } },
                    { DevicePlatform.Android, new[] { "text/csv", "application/vnd.ms-excel", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" } },
                    { DevicePlatform.iOS, new[] { "public.comma-separated-values-text", "org.openxmlformats.spreadsheetml.sheet", "com.microsoft.excel.xls" } },
                    { DevicePlatform.MacCatalyst, new[] { "public.comma-separated-values-text", "org.openxmlformats.spreadsheetml.sheet", "com.microsoft.excel.xls" } }
                })
                    });
                }
                else
                {
                    // For mobile/mac platforms you can also allow picking CSV/XLSX
                    result = await FilePicker.Default.PickAsync(new PickOptions
                    {
                        PickerTitle = "Select a file",
                        FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.Android, new[] { "text/csv", "application/vnd.ms-excel", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" } },
                    { DevicePlatform.iOS, new[] { "public.comma-separated-values-text", "org.openxmlformats.spreadsheetml.sheet", "com.microsoft.excel.xls" } },
                    { DevicePlatform.MacCatalyst, new[] { "public.comma-separated-values-text", "org.openxmlformats.spreadsheetml.sheet", "com.microsoft.excel.xls" } }
                })
                    });
                }

                if (result != null)
                {
                    using var stream = await result.OpenReadAsync();
                    using var reader = new StreamReader(stream);
                    using var csv = new CsvHelper.CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture);

                    var records = csv.GetRecords<ProductTemplateDto>().ToList();

                    output.Success = true;
                    output.Message = "Product file selected.";
                    output.Entity = records;
                }
                else
                {
                    output.Success = false;
                    output.Message = "No file selected.";
                }
            }
            catch (Exception ex)
            {
                output.Success = false;
                output.Message = ex.Message;
            }

            return output;
        }

    }
}
