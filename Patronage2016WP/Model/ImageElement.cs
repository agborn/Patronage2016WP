using System;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Media.Imaging;

namespace Patronage2016WP.Model
{
    public class ImageElement
    {
        public StorageFile File { get; set; }
        public BitmapImage Source { get; set; }
        public StorageItemThumbnail Thumbnail { get; set; }
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public DateTime Date { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
