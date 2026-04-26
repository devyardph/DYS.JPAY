using DYS.JPay.Shared.Shared.Services;
using Foundation;
using Photos;
using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Platforms.iOS
{
    public class PhotoService: IPhotoService
    {
        private PHObjectPlaceholder _lastPlaceholder;
        public Task<string> SaveImageToAlbumAsync(string tempPath, string album)
        {
            var tcs = new TaskCompletionSource<string>();
            var nsUrl = NSUrl.FromFilename(tempPath);

            PHPhotoLibrary.SharedPhotoLibrary.PerformChanges(() =>
            {
                // Create asset
                var assetRequest = PHAssetCreationRequest.CreationRequestForAsset();
                assetRequest.AddResource(PHAssetResourceType.Photo, nsUrl, new PHAssetResourceCreationOptions());

                var placeholder = assetRequest.PlaceholderForCreatedAsset;

                // Try to fetch album
                var fetchOptions = new PHFetchOptions
                {
                    Predicate = NSPredicate.FromFormat("title = %@", new NSString(album))
                };
                var collection = PHAssetCollection.FetchAssetCollections(PHAssetCollectionType.Album, PHAssetCollectionSubtype.Any, fetchOptions);

                if (collection.Count > 0)
                {
                    // Existing album
                    var album = collection[0] as PHAssetCollection;
                    var albumChangeRequest = PHAssetCollectionChangeRequest.ChangeRequest(album);
                    albumChangeRequest.AddAssets(new[] { placeholder });
                }
                else
                {
                    // ✅ Create new album
                    var albumRequest = PHAssetCollectionChangeRequest.CreateAssetCollection(album);
                    albumRequest.AddAssets(new[] { placeholder });
                }

                _lastPlaceholder = placeholder;

            }, (success, error) =>
            {
                if (success && _lastPlaceholder != null)
                    tcs.SetResult(_lastPlaceholder.LocalIdentifier);
                else
                    tcs.SetException(new NSErrorException(error));
            });

            return tcs.Task;
        }
        public async Task<string> GetImageDataUriAsync(string tempPath, string photoPath)
        {
            // Fetch the asset by identifier
            var fetchResult = PHAsset.FetchAssetsUsingLocalIdentifiers(new[] { photoPath }, null);

            // ✅ Correctly cast to PHAsset
            var asset = fetchResult.FirstOrDefault() as PHAsset;
            if (asset == null) return null;

            var tcs = new TaskCompletionSource<string>();

            var options = new PHImageRequestOptions
            {
                Synchronous = false,
                NetworkAccessAllowed = true,
                DeliveryMode = PHImageRequestOptionsDeliveryMode.HighQualityFormat
            };

            // Request raw image data
            PHImageManager.DefaultManager.RequestImageData(asset, options, (data, dataUti, orientation, info) =>
            {
                if (data != null)
                {
                    var base64 = Convert.ToBase64String(data.ToArray());
                    var dataUri = $"data:image/jpeg;base64,{base64}";
                    tcs.SetResult(dataUri);
                }
                else
                {
                    tcs.SetResult(null);
                }
            });

            return await tcs.Task;
        }

        }
}
