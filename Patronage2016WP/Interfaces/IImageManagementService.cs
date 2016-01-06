using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace Patronage2016WP.Interfaces
{
    public interface IImageManagementService
    {
        Task LoadCollectionOfImageElements();
        Task AddNewImageElementToCollection(StorageFile image);
        Task<BitmapImage> GetBitmapImageFromStorageFile(StorageFile image);
        Task TakeNewPhoto();
        RandomAccessStreamReference CreateRandomAccessStreamReferenceFromImage(StorageFile image);
    }
}
