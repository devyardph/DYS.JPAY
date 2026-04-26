using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Services
{
    public interface IPhotoService
    {
        Task<string> SaveImageToAlbumAsync(string filePath, string albumName);
    }
}
