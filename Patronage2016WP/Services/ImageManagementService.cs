using Patronage2016WP.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Media.Imaging;

namespace Patronage2016WP.Services
{
    public class ImageManagementService
    {
        public static ObservableCollection<ImageElement> Images;

        public async static Task LoadCollectionOfImageElements()
        {
            List<StorageFile> storageImages = new List<StorageFile>();
            StorageFolder folder = KnownFolders.PicturesLibrary;

            await GetAllImages(storageImages, folder);

            Images = new ObservableCollection<ImageElement>();

            foreach (var image in storageImages)
            {
                await AddNewImageElementToCollection(image);
            }
        }

        public static async Task AddNewImageElementToCollection(StorageFile image)
        {
            var stream = await image.OpenReadAsync();
            var bitmapImage = new BitmapImage();
            bitmapImage.SetSource(stream);

            StorageItemThumbnail thumbnail = await image.GetThumbnailAsync(ThumbnailMode.PicturesView);

            ImageProperties imageProperties = await image.Properties.GetImagePropertiesAsync();
            var date = imageProperties.DateTaken.ToString().Substring(0, 8) == "1/1/1601" ? image.DateCreated : imageProperties.DateTaken;
            double lat = imageProperties.Latitude.HasValue ? imageProperties.Latitude.Value : 0;
            double lon = imageProperties.Longitude.HasValue ? imageProperties.Longitude.Value : 0;

            Images.Add(new ImageElement
            {
                File = image,
                Name = image.Name.Substring(0, image.Name.IndexOf('.')),
                Source = bitmapImage,
                Thumbnail = thumbnail,
                Height = (int)imageProperties.Height,
                Width = (int)imageProperties.Width,
                Date = date,
                Latitude = lat,
                Longitude = lon
            });
        }

        private static async Task GetAllImages(List<StorageFile> listOfImages, StorageFolder folder)
        {
            foreach (var item in await folder.GetFilesAsync())
            {
                if (item.FileType == ".jpg" || item.FileType == ".jpeg" || item.FileType == ".png" || item.FileType == ".bmp")
                {
                    listOfImages.Add(item);
                }
            }

            foreach (var item in await folder.GetFoldersAsync())
            {
                await GetAllImages(listOfImages, item);
            }
        }
    }
}
