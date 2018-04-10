using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using System.ComponentModel;
using Windows.UI.Xaml.Media;
using System.Collections.ObjectModel;
using Windows.Storage;
using MyList.Service;

namespace MyList.ViewModels
{
    public class TodoItemViewModel
    {
        private ObservableCollection<Models.TodoItem> allItems = new ObservableCollection<Models.TodoItem>();

        public ObservableCollection<Models.TodoItem> AllItems { get { return this.allItems; } }

        private int count = 0;

        public void AddTodoItem(ImageSource _image, double picSize, string imgName, string title, string description, DateTimeOffset dateTime, bool? isChecked = false)
        {
            this.allItems.Add(new Models.TodoItem(_image, picSize, imgName, title, description, dateTime, isChecked));
            TileService.UpdateTileItem();
            ++count;
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
            if (this.selectedItem != null)
            {
                AllItems.Remove(selectedItem);
                --count;
                this.selectedItem = null;
                TileService.UpdateTileItem();
            }
        }

        public void UpdateTodaItem(string id, ImageSource image, double picSize, string imgName, string title, string description, DateTimeOffset date, bool? isChecked)
        {
            //DIY

            //set selectedItem to null after remove
            //this.selectedItem = null;
            if (this.selectedItem != null)
            {
                if (selectedItem != null) this.selectedItem.UpdateItem(image, picSize, imgName, title, description, date, isChecked);
                this.selectedItem = null;
                TileService.UpdateTileItem();
            }
        }

        public int GetInstantCount()
        {
            return count;
        }
    }
}
