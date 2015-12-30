using Patronage2016WP.Common;
using Patronage2016WP.Model;
using Patronage2016WP.Services;
using System;
using System.Collections.ObjectModel;

namespace Patronage2016WP.ViewModels
{
    public class ListOfPhotosViewModel : BaseObservableObject
    {
        private NavigationService navigationService = new NavigationService();
        
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
                RaisePropertyChanged("ListOfImages");
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

        private RelayCommand<object> showDetailsOfSelectedImage;
        public RelayCommand<object> ShowDetailsOfSelectedImage
        {
            get
            {
                return showDetailsOfSelectedImage ?? (showDetailsOfSelectedImage = new RelayCommand<object>(GoToDetails));
            }
        }

        private RelayCommand<object> loadPage;
        public RelayCommand<object> LoadPage
        {
            get
            {
                return loadPage ?? (loadPage = new RelayCommand<object>(LoadListOfImages));
            }
        }

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
                await ImageManagementService.LoadCollectionOfImageElements();
                ListOfImages = ImageManagementService.Images;
                Message = string.Empty;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
        }
    }
}
