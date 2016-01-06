using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Patronage2016WP.Interfaces
{
    public interface IImageManagementService
    {
        Task LoadCollectionOfImageElements();
        Task AddNewImageElementToCollection(StorageFile image);
        Task TakeNewPhoto();
        RandomAccessStreamReference CreateRandomAccessStreamReferenceFromImage(StorageFile image);
    }
}
