using DYS.JPay.Shared.Shared.Dtos;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Helpers
{
    public interface IFileService
    {
        Task<(string tempPath, string photoPath)> PickAndResizeAsync(int size);
        Task<OutputDto<List<ProductTemplateDto>>> GetFileAsync();
    }
}
