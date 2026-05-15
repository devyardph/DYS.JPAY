using DYS.JPay.Shared.Shared.Services;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Windows.Storage.Pickers;
using WinRT.Interop;


namespace DYS.JPay.Platforms.Windows
{
    public class FilePickerService: IFilePickerService
    {
        public async Task<Stream?> PickImageAsync()
        {
            try
            {
                var picker = new FileOpenPicker();

                // File type filters
                picker.FileTypeFilter.Add(".jpg");
                picker.FileTypeFilter.Add(".jpeg");
                picker.FileTypeFilter.Add(".png");

                // Suggested start location
                picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;

                // Attach picker to your MAUI window
                InitializeWithWindow.Initialize(picker, DYS.JPay.WinUI.App.MainWindowHandle);

                // Show picker
                var file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    using var stream = await file.OpenStreamForReadAsync();
                    // process stream...
                    return stream;
                }
                return null;
            }
            catch (COMException comEx)
            {
                //var picker = new Windows.Storage.Pickers.FileOpenPicker();
                //picker.FileTypeFilter.Add(".jpg");
                //picker.FileTypeFilter.Add(".png");
                //picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;

                //WinRT.Interop.InitializeWithWindow.Initialize(picker, App.MainWindowHandle);

                //var file = await picker.PickSingleFileAsync();
                //if (file != null)
                //{
                //    return await file.OpenStreamForReadAsync();
                //}
                System.Diagnostics.Debug.WriteLine($"FilePicker failed: {comEx.Message}");
                return null;
            }
        }
    }
}
