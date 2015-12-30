using System.Collections.Generic;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Storage;

namespace Patronage2016WP.Services
{
    public class ShareService
    {
        private static ShareService instance;
        private StorageFile image;
        private DataTransferManager dataTransferManagerForService;

        private ShareService() { }

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

        public void Share(StorageFile file)
        {
            instance.image = file;
            DataTransferManager.ShowShareUI();
        }

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
    }
}
