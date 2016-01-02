using Patronage2016WP.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.System.Profile;
using Windows.UI.Xaml.Media.Imaging;

namespace Patronage2016WP.Services
{
    public class ImageManagementService
    {
        #region Fields
        public ObservableCollection<ImageElement> Images;
        private static ImageManagementService instance; 
        #endregion

        #region Private Constructor
        private ImageManagementService() { } 
        #endregion

        #region Public Properties
        public static ImageManagementService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ImageManagementService();
                    instance.Images = new ObservableCollection<ImageElement>();
                }
                return instance;
            }
        } 
        #endregion

        #region Public Methods
        public async Task LoadCollectionOfImageElements()
        {
            List<StorageFile> storageImages = new List<StorageFile>();
            StorageFolder folder = KnownFolders.PicturesLibrary;

            await GetAllImages(storageImages, folder);

            instance.Images.Clear();

            foreach (var image in storageImages)
            {
                await AddNewImageElementToCollection(image);
            }
        }

        public async Task AddNewImageElementToCollection(StorageFile image)
        {
            var stream = await image.OpenReadAsync();
            var bitmapImage = new BitmapImage();
            bitmapImage.SetSource(stream);

            StorageItemThumbnail thumbnail = await image.GetThumbnailAsync(ThumbnailMode.PicturesView);

            ImageProperties imageProperties = await image.Properties.GetImagePropertiesAsync();
            var date = imageProperties.DateTaken.LocalDateTime.Year.ToString() == "1601" ? image.DateCreated.LocalDateTime : imageProperties.DateTaken.LocalDateTime;
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

        public async Task TakeNewPhoto()
        {
            string device = GetDeviceInfo();
            var file = await OpenCameraAndTakePicture();

            if (file != null)
            {
                var renamedFile = await RenamePhoto(file, device);
                await Task.Delay(TimeSpan.FromSeconds(2));
                StorageFile savedFile = null;
                if (device == "Windows.Desktop")
                {
                    savedFile = await SaveFileOnDesktop(renamedFile);
                }
                else if (device == "Windows.Mobile")
                {
                    savedFile = await SaveFileOnMobile(renamedFile);
                }

                if (savedFile != null && Images != null)
                {
                    await AddNewImageElementToCollection(savedFile);
                }
                else
                {
                    throw new Exception("New photo could not be saved.");
                }
            }
        }
        #endregion

        #region Private Methods
        private string GetDeviceInfo()
        {
            string device = AnalyticsInfo.VersionInfo.DeviceFamily;
            return device;
        }

        private async Task<StorageFile> OpenCameraAndTakePicture()
        {
            CameraCaptureUI dialog = new CameraCaptureUI();
            StorageFile file = await dialog.CaptureFileAsync(CameraCaptureUIMode.Photo);

            return file;
        }

        private async Task<StorageFile> RenamePhoto(StorageFile file, string device)
        {
            string newName = GenerateDefaultFileName(device);
            await file.RenameAsync(newName);
            return file;
        }

        private async Task<StorageFile> SaveFileOnDesktop(StorageFile file)
        {
            var savePicker = new FileSavePicker();
            savePicker.FileTypeChoices.Add("JPEG-Image", new List<string>() { ".jpg" });
            savePicker.SuggestedSaveFile = file;
            savePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            var savedFile = await savePicker.PickSaveFileAsync();
            
            CachedFileManager.DeferUpdates(savedFile);
            await file.MoveAndReplaceAsync(savedFile);
            Windows.Storage.Provider.FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(savedFile);

            if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
            {
                return savedFile;
            }

            return null;
        }

        private async Task<StorageFile> SaveFileOnMobile(StorageFile file)
        {
            var savePicker = new FolderPicker();
            savePicker.FileTypeFilter.Add(".jpg");
            savePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            var savedFolder = await savePicker.PickSingleFolderAsync();
            if (savedFolder != null)
            {
                await file.MoveAsync(savedFolder);
                return file;
            }

            return null;
        }

        private string GenerateDefaultFileName(string device)
        {
            string name = string.Empty;
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;
            var day = DateTime.Now.Day;
            var hour = DateTime.Now.Hour;
            var minute = DateTime.Now.Minute;
            var second = DateTime.Now.Second;

            string nameOfDevice = (device == "Windows.Desktop") ? "WIN" : "WP";

            name = string.Format("{0}_{1}_{2}_{3}_{4}.jpg", nameOfDevice, year + month + day, hour, minute, second);
            return name;
        }

        private async Task GetAllImages(List<StorageFile> listOfImages, StorageFolder folder)
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
        #endregion

    }
}
