using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using System.ComponentModel;
using Windows.UI.Xaml.Media;
using System.Xml.Linq;

namespace MyList.Models
{
    public class TodoItem
    {
        private string id;

        public string imgName { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTimeOffset ItemDate { get; set; }

        public ImageSource image { get; set; }

        public double picSize { get; set; }
        
        public bool? isChecked { get; set; }

        public void UpdateItem(ImageSource image, double picSize, string imgName, string title, string description, DateTimeOffset date, bool? isChecked)
        {
            this.image = (image == null ? new BitmapImage(new Uri("Assets/pic_5.jpg")) : image); ;
            this.Title = title;
            this.Description = description;
            this.ItemDate = date;
            this.picSize = picSize;
            this.isChecked = isChecked;
            this.imgName = imgName;
        }

        public TodoItem(ImageSource _image, double picSize, string imgName, string title, string description, DateTimeOffset itemDate, bool? isChecked = false)
        {
            this.id = Guid.NewGuid().ToString(); //生成id
            this.image = (_image == null ? new BitmapImage(new Uri("ms-appx:///Assets/pic_6.jpg")) : _image);
            this.picSize = picSize;
            this.Title = title;
            this.Description = description;
            this.ItemDate = itemDate;
            this.isChecked = isChecked;
            this.imgName = imgName;
        }
    }
}
