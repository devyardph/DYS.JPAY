using Android.Content;
using Android.Graphics;
using Android.Provider;
using DYS.JPay.Shared.Shared.Services;
using Java.IO;
using System.IO;
using System.Threading.Tasks;

namespace DYS.JPay.Platforms.Android
{
    public class PhotoService : IPhotoService
    {
        private readonly Context _context;

        public PhotoService(Context context)
        {
            _context = context;
        }

        public async Task<string> SaveImageToAlbumAsync(string tempPath, string album)
        {
            //var values = new ContentValues();
            //values.Put(MediaStore.Images.Media.InterfaceConsts.Title, Path.GetFileName(tempPath));
            //values.Put(MediaStore.Images.Media.InterfaceConsts.Description, album);
            //values.Put(MediaStore.Images.Media.InterfaceConsts.MimeType, "image/jpeg");
            //values.Put(MediaStore.Images.Media.InterfaceConsts.DateAdded, Java.Lang.JavaSystem.CurrentTimeMillis() / 1000);

            //var uri = _context.ContentResolver.Insert(MediaStore.Images.Media.ExternalContentUri, values);

            //using (var stream = _context.ContentResolver.OpenOutputStream(uri))
            //using (var fileStream = File.OpenRead(tempPath))
            //{
            //    await fileStream.CopyToAsync(stream);
            //}

            // Return the URI string as identifier
            return "";//uri.ToString();
        }

        public async Task<string> GetImageDataUriAsync(string tempPath, string photoPath)
        {
            //var uri = Android.Net.Uri.Parse(photoPath);

            //using (var stream = _context.ContentResolver.OpenInputStream(uri))
            //{
            //    if (stream == null) return null;

            //    using (var ms = new MemoryStream())
            //    {
            //        await stream.CopyToAsync(ms);
            //        var base64 = Convert.ToBase64String(ms.ToArray());
            //        return $"data:image/jpeg;base64,{base64}";
            //    }
            //}
            return "";
        }
    }
}