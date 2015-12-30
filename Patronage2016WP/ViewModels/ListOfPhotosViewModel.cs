using Patronage2016WP.Common;
using Patronage2016WP.Model;
using Patronage2016WP.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Media.Imaging;

namespace Patronage2016WP.ViewModels
{
    public class ListOfPhotosViewModel : BaseObservableObject
    {
        private NavigationService NavigationService = new NavigationService();

        public ListOfPhotosViewModel()
        {
            LoadImages();
        }
        
        private ObservableCollection<ImageElement> listOfImages;
        public ObservableCollection<ImageElement> ListOfImages
        {
            get
            {
                return listOfImages ?? new ObservableCollection<ImageElement>();
            }
            set
            {
                listOfImages = value;
                RefreshImagesData();
            }
        }

        private string message;
        public string Message
        {
            get
            {
                return message;
            }
            set
            {
                message = value;
                RaisePropertyChanged("Message");
                IsMessageVisible = !string.IsNullOrEmpty(message);
            }
        }

        private bool isMessageVisible = false;
        public bool IsMessageVisible
        {
            get
            {
                return isMessageVisible;
            }
            set
            {
                isMessageVisible = value;
                RaisePropertyChanged("IsMessageVisible");
            }
        }

        private async void LoadImages()
        {
            try
            {
                List<StorageFile> images = new List<StorageFile>();
                StorageFolder folder = KnownFolders.PicturesLibrary;

                await RetrievingImagesService.GetAllImages(images, folder);

                listOfImages = new ObservableCollection<ImageElement>();
                foreach (var image in images)
                {
                    var stream = await image.OpenReadAsync();
                    var bitmapImage = new BitmapImage();
                    bitmapImage.SetSource(stream);
                    StorageItemThumbnail thumbnail = await image.GetThumbnailAsync(ThumbnailMode.PicturesView);
                    listOfImages.Add(new ImageElement { Name = image.Name.Substring(0, image.Name.IndexOf('.')), Source = bitmapImage, Thumbnail = thumbnail });
                }

                RefreshImagesData();
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
        }

        private void RefreshImagesData()
        {
            RaisePropertyChanged("ListOfImages");
        }
    }
}
