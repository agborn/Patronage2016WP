using System.Threading.Tasks;
using Windows.Storage;

namespace Patronage2016WP.Interfaces
{
    public interface IImageManagementService
    {
        Task LoadCollectionOfImageElements();
        Task AddNewImageElementToCollection(StorageFile image);
        Task TakeNewPhoto();
    }
}
