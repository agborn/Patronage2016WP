using Windows.Storage;

namespace Patronage2016WP.Interfaces
{
    public interface IShareService
    {
        void Share(StorageFile file);
    }
}
