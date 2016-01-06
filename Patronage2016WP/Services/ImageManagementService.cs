using Patronage2016WP.Interfaces;
using Patronage2016WP.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Media.Capture;
using System.Linq;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.System.Profile;
using Windows.UI.Xaml.Media.Imaging;

namespace Patronage2016WP.Services
{
    public class ImageManagementService : IImageManagementService
    {
        #region Fields
        public ObservableCollection<ImageElement> Images;
        private static ImageManagementService _instance; 
        #endregion

        #region Private Constructor
        private ImageManagementService() { } 
        #endregion

        #region Public Properties
        public static ImageManagementService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ImageManagementService();
                    _instance.Images = new ObservableCollection<ImageElement>();
                }
                return _instance;
            }
        } 
        #endregion

        #region Public Methods
        public async Task LoadCollectionOfImageElements()
        {
            List<StorageFile> storageImages = new List<StorageFile>();
            StorageFolder folder = KnownFolders.PicturesLibrary;

            await GetAllImages(storageImages, folder);

            _instance.Images.Clear();

            foreach (var image in storageImages)
            {
                await AddNewImageElementToCollection(image);
            }
        }

        public async Task AddNewImageElementToCollection(StorageFile image)
        {
            StorageItemThumbnail thumbnail = await image.GetThumbnailAsync(ThumbnailMode.PicturesView);

            ImageProperties imageProperties = await image.Properties.GetImagePropertiesAsync();
            DateTime date = imageProperties.DateTaken.LocalDateTime.Year.ToString() == "1601" ? image.DateCreated.LocalDateTime : imageProperties.DateTaken.LocalDateTime;
            double lat = imageProperties.Latitude.HasValue ? imageProperties.Latitude.Value : 0.00;
            double lon = imageProperties.Longitude.HasValue ? imageProperties.Longitude.Value : 0.00;

            Images.Add(new ImageElement
            {
                File = image,
                Name = image.Name.Substring(0, image.Name.IndexOf('.')),
                Thumbnail = thumbnail,
                Height = (int)imageProperties.Height,
                Width = (int)imageProperties.Width,
                Date = date,
                Latitude = lat,
                Longitude = lon
            });
        }

        public async Task<BitmapImage> GetBitmapImageFromStorageFile(StorageFile image)
        {
            var stream = await image.OpenReadAsync();
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.SetSource(stream);

            return bitmapImage;
        }

        public async Task TakeNewPhoto()
        {
            string device = GetDeviceInfo();
            bool isCameraAvailable = await CanTakePhoto();
            if (!isCameraAvailable)
            {
                throw new Exception("There is no camera to take a photo!");
            }

            StorageFile file = await OpenCameraAndTakePicture();

            if (file != null)
            {
                StorageFile renamedFile = await RenamePhoto(file, device);
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

        public RandomAccessStreamReference CreateRandomAccessStreamReferenceFromImage(StorageFile image)
        {
            RandomAccessStreamReference reference = RandomAccessStreamReference.CreateFromFile(image);

            return reference;
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

        private async Task<bool> CanTakePhoto()
        {
            bool isCamera = false;
            var devices = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(Windows.Devices.Enumeration.DeviceClass.VideoCapture);

            if (devices.Count < 1)
            {
                return false;
            }
            var enabledDevide = devices.FirstOrDefault(x => x.IsEnabled == true);
            if (enabledDevide != null)
            {
                isCamera = true;
            }

            return isCamera;
        }

        private async Task<StorageFile> RenamePhoto(StorageFile file, string device)
        {
            string newName = GenerateDefaultFileName(device);
            await file.RenameAsync(newName);
            return file;
        }

        private async Task<StorageFile> SaveFileOnDesktop(StorageFile file)
        {
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.FileTypeChoices.Add("JPEG-Image", new List<string>() { ".jpg" });
            savePicker.SuggestedSaveFile = file;
            savePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            StorageFile savedFile = await savePicker.PickSaveFileAsync();
            
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
            FolderPicker savePicker = new FolderPicker();
            savePicker.FileTypeFilter.Add(".jpg");
            savePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            StorageFolder savedFolder = await savePicker.PickSingleFolderAsync();
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
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            int day = DateTime.Now.Day;
            int hour = DateTime.Now.Hour;
            int minute = DateTime.Now.Minute;
            int second = DateTime.Now.Second;
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
