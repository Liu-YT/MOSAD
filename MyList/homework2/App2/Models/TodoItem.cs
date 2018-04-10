using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using System.ComponentModel;
using Windows.UI.Xaml.Media;

namespace App2.Models
{
    
    public class TodoItem
    {
        private string id;

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTimeOffset ItemDate { get; set; }

        public ImageSource image { get; set; }

        public double picSize { get; set; }

        public bool Completed { get; set; }
        
        public void UpdateItem(ImageSource image, double picSize, string title, string description, DateTimeOffset date)
        {
            this.image = (image == null ? new BitmapImage(new Uri("Assets/pic_5.jpg")) : image); ;
            this.Title = title;
            this.Description = description;
            this.ItemDate = date;
            this.Completed = false; //默认未完成
            this.picSize = picSize;
        }

        public TodoItem(ImageSource _image, double picSize, string title, string description, DateTimeOffset itemDate)
        {
            this.id = Guid.NewGuid().ToString(); //生成id
            this.image = (_image == null ? new BitmapImage(new Uri("Assets/pic_5.jpg")) : _image);
            this.picSize = picSize;
            this.Title = title;
            this.Description = description;
            this.ItemDate = itemDate;
            this.Completed = false; //默认未完成
        } 
    }
}
