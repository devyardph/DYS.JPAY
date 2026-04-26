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
       
        public Task<string> SaveImageToAlbumAsync(string filePath, string albumName)
        {
            var tcs = new TaskCompletionSource<string>();
            var nsUrl = NSUrl.FromFilename(filePath);

            PHPhotoLibrary.SharedPhotoLibrary.PerformChanges(() =>
            {
                // Create asset
                var assetRequest = PHAssetCreationRequest.CreationRequestForAsset();
                assetRequest.AddResource(PHAssetResourceType.Photo, nsUrl, new PHAssetResourceCreationOptions());
                var assetPlaceholder = assetRequest.PlaceholderForCreatedAsset;

                // Find album
                var fetchOptions = new PHFetchOptions
                {
                    Predicate = NSPredicate.FromFormat("title = %@", new NSObject[] { new NSString(albumName) })
                };

                var collection = PHAssetCollection.FetchAssetCollections(PHAssetCollectionType.Album, PHAssetCollectionSubtype.Any, fetchOptions);

                if (collection.Count > 0)
                {
                    var album = collection[0] as PHAssetCollection;
                    var albumChangeRequest = PHAssetCollectionChangeRequest.ChangeRequest(album);
                    albumChangeRequest.AddAssets(new[] { assetPlaceholder });
                }
                else
                {
                    // ✅ This is the correct usage
                    var albumRequest = PHAssetCollectionChangeRequest.CreateAssetCollection(albumName);
                    albumRequest.AddAssets(new[] { assetPlaceholder });
                }

            }, (success, error) =>
            {
                if (success)
                {
                    // ✅ Return the LocalIdentifier of the saved asset
                    var fetchOptions = new PHFetchOptions();
                    fetchOptions.SortDescriptors = new[] { new NSSortDescriptor("creationDate", false) };
                    var asset = PHAsset.FetchAssets(PHAssetMediaType.Image, fetchOptions).LastObject;
                    tcs.SetResult(asset?.ToString());
                }
                else
                {
                    tcs.SetException(new NSErrorException(error));
                }
            });

            return tcs.Task;
        }


    }
}
