using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void DownloadButtonClick(object sender, RoutedEventArgs e)
        {
            var images = new ObservableCollection<StorageFile>();

            try
            {
                StorageFolder folder = KnownFolders.PicturesLibrary;

                await GetAllImages(images, folder);
                SetImageSource(images.First());
                ImageToShow.Visibility = Visibility.Visible;
                Information.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                Information.Text = ex.Message;
            }
            
        }

        private async void SetImageSource(StorageFile image)
        {
            var stream = await image.OpenReadAsync();
            var bitmapImage = new BitmapImage();
            bitmapImage.SetSource(stream);

            ImageToShow.Source = bitmapImage;
        }

        private async Task GetAllImages(ObservableCollection<StorageFile> list, StorageFolder folder)
        {
            foreach (var item in await folder.GetFilesAsync())
            {
                if (item.FileType == ".jpg" || item.FileType == ".png" || item.FileType == ".bmp")
                {
                    list.Add(item);
                }
            }

            foreach (var item in await folder.GetFoldersAsync())
            {
                await GetAllImages(list, item);
            }
        }
    }
}
