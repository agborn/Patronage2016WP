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
        private NavigationService navigationService = new NavigationService();
        private ObservableCollection<ImageElement> listOfImages;
        private string message;
        private bool isMessageVisible = false;
        private bool isDataLoading;
        private RelayCommand<object> showDetailsOfSelectedImage;
        private RelayCommand<object> loadPage;
        #endregion

        #region Public Properties
        public ObservableCollection<ImageElement> ListOfImages
        {
            get
            {
                return listOfImages ?? new ObservableCollection<ImageElement>();
            }
            set
            {
                listOfImages = value;
                RaisePropertyChanged("ListOfImages");
            }
        }

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

        public bool IsDataLoading
        {
            get
            {
                return isDataLoading;
            }
            set
            {
                isDataLoading = value;
                RaisePropertyChanged("IsDataLoading");
            }
        }

        public RelayCommand<object> ShowDetailsOfSelectedImage
        {
            get
            {
                return showDetailsOfSelectedImage ?? (showDetailsOfSelectedImage = new RelayCommand<object>(GoToDetails));
            }
        }

        public RelayCommand<object> LoadPage
        {
            get
            {
                return loadPage ?? (loadPage = new RelayCommand<object>(LoadListOfImages));
            }
        }
        #endregion

        #region Private Methods
        private void GoToDetails(object obj)
        {
            var image = obj as ImageElement;
            if (image != null)
            {
                navigationService.Navigate(typeof(ImageDetailsView), image);
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
