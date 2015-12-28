using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Patronage2016WP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        List<StorageFile> images;
        StorageFile currentImage;

        public MainPage()
        {
            this.InitializeComponent();
        }
        
        private async void DownloadButtonClick(object sender, RoutedEventArgs e)
        {
            ImagesLoading.Visibility = Visibility.Visible;
            ImagesLoading.IsActive = true;

            Information.Visibility = Visibility.Collapsed;
            ImagePanel.Visibility = Visibility.Collapsed;

            try
            {
                images = new List<StorageFile>();
                StorageFolder folder = KnownFolders.PicturesLibrary;

                await GetAllImages(images, folder);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }

            currentImage = images.FirstOrDefault();
            if (currentImage == null)
            {
                ShowErrorMessage("There is no picture in the library.");
            }
            else
            {
                SetImageSource(currentImage);
                ImagePanel.Visibility = Visibility.Visible;
                Information.Visibility = Visibility.Collapsed;
            }

            ImagesLoading.Visibility = Visibility.Collapsed;
            ImagesLoading.IsActive = false;
        }

        private async void SetImageSource(StorageFile image)
        {
            var stream = await image.OpenReadAsync();
            var bitmapImage = new BitmapImage();
            bitmapImage.SetSource(stream);
            ImageToShow.Source = bitmapImage;

            ImageProperties imageProperties = await image.Properties.GetImagePropertiesAsync();
            Size.Text = "Size: " + imageProperties.Width + " x " + imageProperties.Height;
            Date.Text = "Date: " + (imageProperties.DateTaken.ToString().Substring(0, 8) == "1/1/1601" ? image.DateCreated.ToString() : imageProperties.DateTaken.ToString());
            Longitude.Text = imageProperties.Longitude.HasValue ? "Longitude: " + imageProperties.Longitude.Value.ToString() : "Longitude: no information";
            Latitude.Text = imageProperties.Latitude.HasValue ? "Latitude: " + imageProperties.Latitude.Value.ToString() : "Latitude: no information";
        }

        private async Task GetAllImages(List<StorageFile> list, StorageFolder folder)
        {
            foreach (var item in await folder.GetFilesAsync())
            {
                if (item.FileType == ".jpg" || item.FileType == ".jpeg" || item.FileType == ".png" || item.FileType == ".bmp")
                {
                    list.Add(item);
                }
            }

            foreach (var item in await folder.GetFoldersAsync())
            {
                await GetAllImages(list, item);
            }
        }

        private void ShowErrorMessage(string message)
        {
            Information.Text = message;
            Information.Visibility = Visibility.Visible;
            ImagePanel.Visibility = Visibility.Collapsed;
        }

        private async void ChangeImage(object sender, TappedRoutedEventArgs e)
        {
            int index = images.IndexOf(currentImage);
            int nextIndex = index + 1;
            if (nextIndex >= images.Count || (currentImage = images[nextIndex]) == null)
            {
                var dialog = new Windows.UI.Popups.MessageDialog("There are no more pictures in the library.");
                await dialog.ShowAsync();
                currentImage = images.First();
            }
            SetImageSource(currentImage);
        }
    }
}
