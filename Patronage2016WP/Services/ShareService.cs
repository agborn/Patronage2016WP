using System.Collections.Generic;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Storage;

namespace Patronage2016WP.Services
{
    public class ShareService
    {
        #region Private Fields
        private static ShareService instance;
        private StorageFile image;
        private DataTransferManager dataTransferManagerForService;
        #endregion

        #region Private Constructor
        private ShareService() { }
        #endregion

        #region Public Properties
        public static ShareService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ShareService();
                    instance.dataTransferManagerForService = DataTransferManager.GetForCurrentView();
                    instance.dataTransferManagerForService.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(ShareStorageItemsHandler);
                }
                return instance;
            }
        }
        #endregion

        #region Public Methods
        public void Share(StorageFile file)
        {
            instance.image = file;
            DataTransferManager.ShowShareUI();
        }
        #endregion

        #region Private Methods
        private static async void ShareStorageItemsHandler(DataTransferManager sender, DataRequestedEventArgs e)
        {
            DataRequest request = e.Request;
            request.Data.Properties.Title = "Share Image";

            DataRequestDeferral deferral = request.GetDeferral();

            try
            {
                List<IStorageItem> storageItems = new List<IStorageItem>();
                storageItems.Add(instance.image);
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
