using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Windows.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace App2.ViewModels
{

    public class TodoItemViewModel
    {
        private ObservableCollection<Models.TodoItem> allItems = new ObservableCollection<Models.TodoItem>();

        public ObservableCollection<Models.TodoItem> AllItems { get { return this.allItems; } }

        public void AddTodoItem(ImageSource _image, double picSize, string title, string description, DateTimeOffset dateTime)
        {
            this.allItems.Add(new Models.TodoItem(_image, picSize, title, description, dateTime));
        }

        private Models.TodoItem _selectedItem;

        public Models.TodoItem selectedItem
        {
            get { return _selectedItem; }
            set { this._selectedItem = value; }
        }

        public void RemoveTodoItem(string id)
        {
            //DIY

            //set selectedItem to null after remove
            //this.selectedItem = null;
            if(this.selectedItem != null)
            {
                AllItems.Remove(selectedItem);
                this.selectedItem = null;
            }    
        }

        public void UpdateTodaItem(string id, ImageSource image, double picSize, string title, string description, DateTimeOffset date)
        {
            //DIY

            //set selectedItem to null after remove
            //this.selectedItem = null;
            if (this.selectedItem != null)
            {
                if (selectedItem != null) this.selectedItem.UpdateItem(image, picSize,title, description, date);
                this.selectedItem = null;
            }
        }
    }
}
