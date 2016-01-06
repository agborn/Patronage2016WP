using Patronage2016WP.Common;
using Patronage2016WP.Model;
using Patronage2016WP.Services;
using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Media.Imaging;

namespace Patronage2016WP.ViewModels
{
    public class ImageDetailsViewModel : BaseObservableObject
    {
        #region Private Fields
        private NavigationService _navigationService = new NavigationService();
        private ImageElement _currentImage;
        private BitmapImage _currentBitmapImage;
        private string _message;
        private bool _isMessageVisible = false;
        private RelayCommand<object> _loadPage;
        private RelayCommand<object> _takePhoto;
        private RelayCommand<object> _goback;
        private RelayCommand<object> _showNextPhoto;
        private RelayCommand<object> _share;
        #endregion

        #region Public Properties
        public ImageElement CurrentImage
        {
            get
            {
                return _currentImage;
            }
            set
            {
                _currentImage = value;
                RefreshData();
            }
        }

        public ObservableCollection<ImageElement> Images
        {
            get
            {
                return ImageManagementService.Instance.Images;
            }
            set
            {
                ImageManagementService.Instance.Images = value;
                RaisePropertyChanged("Images");
            }
        }

        public BitmapImage CurrentBitmapImage
        {
            get
            {
                return _currentBitmapImage;
            }
            set
            {
                _currentBitmapImage = value;
                RaisePropertyChanged("CurrentBitmapImage");
            }
        }

        public string Size
        {
            get
            {
                if (_currentImage != null)
                {
                    return "Size: " + _currentImage.Width + " x " + _currentImage.Height;
                }
                return string.Empty;
            }
        }

        public string Date
        {
            get
            {
                if (_currentImage != null)
                {
                    return "Date: " + _currentImage.Date.ToString();
                }
                return string.Empty;
            }
        }

        public string Longitude
        {
            get
            {
                if (_currentImage != null)
                {
                    return "Longitude: " + (_currentImage.Longitude == 0.00 ? "no information" : _currentImage.Longitude.ToString());
                }
                return string.Empty;
            }
        }

        public string Latitude
        {
            get
            {
                if (_currentImage != null)
                {
                    return "Latitude: " + (_currentImage.Latitude == 0.00 ? "no information" : _currentImage.Latitude.ToString());
                }
                return string.Empty;
            }
        }

        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                RaisePropertyChanged("Message");
                IsMessageVisible = !string.IsNullOrEmpty(_message);
            }
        }

        public bool IsMessageVisible
        {
            get
            {
                return _isMessageVisible;
            }
            set
            {
                _isMessageVisible = value;
                RaisePropertyChanged("IsMessageVisible");
            }
        }

        public RelayCommand<object> LoadPage
        {
            get
            {
                return _loadPage ?? (_loadPage = new RelayCommand<object>(LoadCurrentItem));
            }
        }

        public RelayCommand<object> TakePhoto
        {
            get
            {
                return _takePhoto ?? (_takePhoto = new RelayCommand<object>(TakeNewPhoto));
            }
        }

        public RelayCommand<object> GoBack
        {
            get
            {
                return _goback ?? (_goback = new RelayCommand<object>(GobackToListView));
            }
        }

        public RelayCommand<object> ShowNextPhoto
        {
            get
            {
                return _showNextPhoto ?? (_showNextPhoto = new RelayCommand<object>(ChangeImage));
            }
        }

        public RelayCommand<object> Share
        {
            get
            {
                return _share ?? (_share = new RelayCommand<object>(ShareImage));
            }
        }
        #endregion

        #region Private Methods
        private void ShareImage(object obj)
        {
            ShareService.Instance.Share(_currentImage.File);
        }

        private void LoadCurrentItem(object obj)
        {
            Message = string.Empty;
            ImageElement image = obj as ImageElement;
            if (image != null)
            {
                LoadImage(image);
            }
            else
            {
                Message = "The image could not be presented in details.";
            }
        }

        private void RefreshData()
        {
            RaisePropertyChanged("CurrentImage");
            RaisePropertyChanged("Size");
            RaisePropertyChanged("Date");
            RaisePropertyChanged("Longitude");
            RaisePropertyChanged("Latitude");
        }

        private void ChangeImage(object obj)
        {
            int index = Images.IndexOf(_currentImage);
            int nextIndex = index + 1;

            if (nextIndex >= Images.Count)
            {
                nextIndex = 0;
            }

            LoadImage(Images[nextIndex]);
        }

        private async void LoadImage(ImageElement image)
        {
            CurrentImage = image;
            CurrentBitmapImage = await ImageManagementService.Instance.GetBitmapImageFromStorageFile(image.File);
        }

        private void GobackToListView(object obj)
        {
            _navigationService.GoBack();
        }

        private async void TakeNewPhoto(object obj)
        {
            string message = string.Empty;
            try
            {
                await ImageManagementService.Instance.TakeNewPhoto();
                message = "The new photo was succesully added to your library!";
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            finally
            {
                MessageDialogService.ShowMessageDialog(message);
            }
        } 
        #endregion
    }
}
