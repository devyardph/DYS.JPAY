using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Helpers
{
    public interface IImageService
    {
        Task<(string tempPath, string photoPath)> PickAndResizeAsync(int size);
    }
}
