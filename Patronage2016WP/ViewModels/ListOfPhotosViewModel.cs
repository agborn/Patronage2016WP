using Patronage2016WP.Common;
using Patronage2016WP.Model;
using Patronage2016WP.Services;
using System;
using System.Collections.ObjectModel;

namespace Patronage2016WP.ViewModels
{
    public class ListOfPhotosViewModel : BaseObservableObject
    {
        #region Private Fields
        private NavigationService _navigationService = new NavigationService();
        private ObservableCollection<ImageElement> _listOfImages;
        private string _message;
        private bool _isMessageVisible = false;
        private bool _isDataLoading;
        private RelayCommand<object> _showDetailsOfSelectedImage;
        private RelayCommand<object> _loadPage;
        #endregion

        #region Public Properties
        public ObservableCollection<ImageElement> ListOfImages
        {
            get
            {
                return _listOfImages ?? new ObservableCollection<ImageElement>();
            }
            set
            {
                _listOfImages = value;
                RaisePropertyChanged("ListOfImages");
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

        public bool IsDataLoading
        {
            get
            {
                return _isDataLoading;
            }
            set
            {
                _isDataLoading = value;
                RaisePropertyChanged("IsDataLoading");
            }
        }

        public RelayCommand<object> ShowDetailsOfSelectedImage
        {
            get
            {
                return _showDetailsOfSelectedImage ?? (_showDetailsOfSelectedImage = new RelayCommand<object>(GoToDetails));
            }
        }

        public RelayCommand<object> LoadPage
        {
            get
            {
                return _loadPage ?? (_loadPage = new RelayCommand<object>(LoadListOfImages));
            }
        }
        #endregion

        #region Private Methods
        private void GoToDetails(object obj)
        {
            var image = obj as ImageElement;
            if (image != null)
            {
                _navigationService.Navigate(typeof(ImageDetailsView), image);
            }
        }

        private async void LoadListOfImages(object obj)
        {
            try
            {
                IsDataLoading = true;
                await ImageManagementService.Instance.LoadCollectionOfImageElements();
                ListOfImages = ImageManagementService.Instance.Images;
                Message = string.Empty;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
            finally
            {
                IsDataLoading = false;
            }
        } 
        #endregion
    }
}
