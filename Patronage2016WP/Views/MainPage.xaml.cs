using Patronage2016WP.Model;
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
        ObservableCollection<StorageFile> images;
        BackgroundWorker worker;

        public MainPage()
        {
            this.InitializeComponent();
            worker = new BackgroundWorker();
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null && e.Result is BackgroundWorkerResult)
            {
                var result = e.Result as BackgroundWorkerResult;
                if (!String.IsNullOrEmpty(result.ErrorMessage))
                {
                    ShowErrorMessage(result.ErrorMessage);
                }
                
            }
            else
            {
                var image = images.FirstOrDefault();
                if (image == null)
                {
                    ShowErrorMessage("There is no picture in library.");
                }
                else
                {
                    SetImageSource(image);
                    ImageToShow.Visibility = Visibility.Visible;
                    Information.Visibility = Visibility.Collapsed;
                }
            }
            ImagesLoading.Visibility = Visibility.Collapsed;
            ImagesLoading.IsActive = false;
        }

        private async void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                images = new ObservableCollection<StorageFile>();
                StorageFolder folder = KnownFolders.PicturesLibrary;

                await GetAllImages(images, folder);
            }
            catch(Exception ex)
            {
                BackgroundWorkerResult bwr = new BackgroundWorkerResult();
                bwr.ErrorMessage = ex.Message;
                e.Result = bwr;
            }
            
        }

        private async void DownloadButtonClick(object sender, RoutedEventArgs e)
        {
            ImagesLoading.Visibility = Visibility.Visible;
            ImagesLoading.IsActive = true;

            Information.Visibility = Visibility.Collapsed;
            ImageToShow.Visibility = Visibility.Collapsed;

            worker.RunWorkerAsync();
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
        }
    }
}
