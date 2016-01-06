using Patronage2016WP.Interfaces;
using System.Collections.Generic;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Storage;

namespace Patronage2016WP.Services
{
    public class ShareService : IShareService
    {
        #region Private Fields
        private static ShareService _instance;
        private StorageFile _image;
        private DataTransferManager _dataTransferManagerForService;
        #endregion

        #region Private Constructor
        private ShareService() { }
        #endregion

        #region Public Properties
        public static ShareService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ShareService();
                    _instance._dataTransferManagerForService = DataTransferManager.GetForCurrentView();
                    _instance._dataTransferManagerForService.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(ShareStorageItemsHandler);
                }
                return _instance;
            }
        }
        #endregion

        #region Public Methods
        public void Share(StorageFile file)
        {
            _instance._image = file;
            DataTransferManager.ShowShareUI();
        }
        #endregion

        #region Private Methods
        private static void ShareStorageItemsHandler(DataTransferManager sender, DataRequestedEventArgs e)
        {
            DataRequest request = e.Request;
            request.Data.Properties.Title = "Share Image";
            DataRequestDeferral deferral = request.GetDeferral();

            try
            {
                var image = ImageManagementService.Instance.CreateRandomAccessStreamReferenceFromImage(_instance._image);
                request.Data.Properties.Thumbnail = image;
                request.Data.SetBitmap(image);

                List<IStorageItem> storageItems = new List<IStorageItem>();
                storageItems.Add(_instance._image);
                request.Data.SetStorageItems(storageItems);
            }
            finally
            {
                deferral.Complete();
            }
        } 
        #endregion
    }
}
