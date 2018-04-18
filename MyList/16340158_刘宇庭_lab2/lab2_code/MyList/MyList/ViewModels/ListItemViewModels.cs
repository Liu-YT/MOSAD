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
using System.Diagnostics;

namespace MyList.ViewModels
{
    public class TodoItemViewModel
    {

        private Models.TodoItem _selectedItem;

        private ObservableCollection<Models.TodoItem> allItems = new ObservableCollection<Models.TodoItem>();

        public ObservableCollection<Models.TodoItem> AllItems { get { return this.allItems; } }

        public Models.TodoItem selectedItem
        {
            get { return _selectedItem; }
            set { this._selectedItem = value; }
        }

        //创建新Item
        public void AddTodoItem(ImageSource _image, double picSize, string imgName, string title, string description, DateTimeOffset dateTime, bool? isChecked = false, string _id = "")
        {
            string id = Guid.NewGuid().ToString(); //生成id
            AllItems.Add(new Models.TodoItem(id, _image, picSize, imgName, title, description, dateTime, isChecked));
            App.db.addItem(new Models.TodoItem(id, _image, picSize, imgName, title, description, dateTime, isChecked));
            TileService.UpdateTileItem();   //更新磁贴
        }

        //将数据库中的Item导入
        public async Task AddTodoItems(Models.TodoItem item)
        {
            await item.setImg();
            AllItems.Add(item);
            TileService.UpdateTileItem();   //更新磁贴
        }

        //删除Item
        public void RemoveTodoItem(string id)
        {
            //DIY

            //set selectedItem to null after remove
            //this.selectedItem = null;
            if (this.selectedItem != null)
            {
                AllItems.Remove(selectedItem);
                TileService.UpdateTileItem();
                Debug.WriteLine(selectedItem.id);
                App.db.deleteItem(selectedItem);
                this.selectedItem = null;
                TileService.UpdateTileItem();   //更新磁贴
            }
        }

        //更新Item
        public void UpdateTodoItem(string id, ImageSource image, double picSize, string imgName, string title, string description, DateTimeOffset date, bool? isChecked)
        {
            //DIY

            //set selectedItem to null after remove
            //this.selectedItem = null;
            if (this.selectedItem != null)
            {
                this.selectedItem.UpdateItem(image, picSize, imgName, title, description, date, isChecked);
                TileService.UpdateTileItem();
                App.db.updateItem(selectedItem);
                this.selectedItem = null;
                TileService.UpdateTileItem();   //更新磁贴
            }
        }

        //更新Item的status状态
        public void UpdateTodoItemStatus(Models.TodoItem item)
        {
            App.db.updateItem(item);
        }
    }
}
