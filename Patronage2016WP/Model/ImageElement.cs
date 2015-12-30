using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Media.Imaging;

namespace Patronage2016WP.Model
{
    public class ImageElement
    {
        public BitmapImage Source { get; set; }
        public StorageItemThumbnail Thumbnail { get; set; }
        public string Name { get; set; }
    }
}
