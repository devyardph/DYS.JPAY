using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Services
{
    public interface IFilePickerService
    {
        Task<Stream?> PickImageAsync();
    }
}
