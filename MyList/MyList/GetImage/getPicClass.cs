using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace MyList.GetImage
{
    class getPicClass
    {
        public static string imgName;
        public async void selectPic(Image pic)
        {
            var fop = new FileOpenPicker();
            fop.ViewMode = PickerViewMode.Thumbnail;
            fop.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            fop.FileTypeFilter.Add(".jpg");
            fop.FileTypeFilter.Add(".jpeg");
            fop.FileTypeFilter.Add(".png");
            fop.FileTypeFilter.Add(".gif");

            Windows.Storage.StorageFile file = await fop.PickSingleFileAsync();
            try
            {
                using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    await bitmapImage.SetSourceAsync(fileStream);
                    pic.Source = bitmapImage;
                    imgName = file.Path.Substring(file.Path.LastIndexOf('\\') + 1);
                    await file.CopyAsync(ApplicationData.Current.LocalFolder, imgName, NameCollisionOption.ReplaceExisting);
                    Debug.WriteLine(imgName);
                }
            }
            catch (Exception)
            {
                return;
            }
        }

    }
}
