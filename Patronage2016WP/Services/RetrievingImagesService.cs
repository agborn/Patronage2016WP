using Patronage2016WP.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace Patronage2016WP.Services
{
    public class RetrievingImagesService
    {
        public static async Task GetAllImages(List<StorageFile> listOfImages, StorageFolder folder)
        {
            foreach (var item in await folder.GetFilesAsync())
            {
                if (item.FileType == ".jpg" || item.FileType == ".jpeg" || item.FileType == ".png" || item.FileType == ".bmp")
                {
                    listOfImages.Add(item);
                }
            }

            foreach (var item in await folder.GetFoldersAsync())
            {
                await GetAllImages(listOfImages, item);
            }
        }
    }
}
