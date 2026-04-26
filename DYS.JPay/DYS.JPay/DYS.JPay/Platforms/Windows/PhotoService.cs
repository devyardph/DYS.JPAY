using DYS.JPay.Shared.Shared.Extensions;
using DYS.JPay.Shared.Shared.Services;


namespace DYS.JPay.Platforms.Windows
{
    public class PhotoService : IPhotoService
    {
        public Task<string> SaveImageToAlbumAsync(string tempPath, string album)
        {
            return Task.FromResult(tempPath);
        }
        public async Task<string> GetImageDataUriAsync(string tempPath, string photoPath)
        {
            var output = DataExtensions.GenerateImagePath(tempPath);
            return output;
        }
    }
}
