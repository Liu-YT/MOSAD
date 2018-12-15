using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using System.ComponentModel;
using Windows.UI.Xaml.Media;
using System.Xml.Linq;
using SQLite.Net.Attributes;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Diagnostics;

namespace MyList.Models
{
    public class TodoItem
    {
        [PrimaryKey]
        public string id { get; set; }

        public string imgName { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTimeOffset ItemDate { get; set; }

        public double picSize { get; set; }
        
        public bool? isChecked { get; set; }

        public string itemDateToString { get; set; }

        [Ignore]
        public ImageSource image { get; set; }

        public async Task setImg()
        {
            if(imgName != null && imgName != "")
            {
                Debug.WriteLine(imgName);
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync(imgName);
                IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read);
                BitmapImage bitmapImage = new BitmapImage();
                await bitmapImage.SetSourceAsync(fileStream);
                this.image = bitmapImage;
            }
            else
            {
                this.image = new BitmapImage(new Uri("ms-appx:///Assets/pic_6.jpg"));
            }
        }

        public void UpdateItem(ImageSource image, double picSize, string imgName, string title, string description, DateTimeOffset date, bool? isChecked)
        {
            this.image = (image == null ? new BitmapImage(new Uri("Assets/pic_5.jpg")) : image);
            this.Title = title;
            this.Description = description;
            this.ItemDate = date;
            this.picSize = picSize;
            this.isChecked = isChecked;
            this.imgName = imgName;
            itemDateToString = ItemDate.ToString();
        }

        public TodoItem() { }

        public TodoItem(string id, ImageSource _image, double picSize, string imgName, string title, string description, DateTimeOffset itemDate, bool? isChecked = false)
        {
            this.id = id;
            this.image = ((_image == null) ? new BitmapImage(new Uri("ms-appx:///Assets/pic_6.jpg")) :_image);
            this.picSize = picSize;
            this.Title = title;
            this.Description = description;
            this.ItemDate = itemDate;
            this.isChecked = isChecked;
            this.imgName = imgName;
            itemDateToString = ItemDate.ToString();
        }

        public TodoItem(TodoItem item)
        {
            this.id = item.id;
            this.picSize = item.picSize;
            this.Title = item.Title;
            this.Description = item.Description;
            this.ItemDate = item.ItemDate;
            this.isChecked = item.isChecked;
            this.imgName = item.imgName;
            itemDateToString = ItemDate.ToString();
        }
    }
}
