using SQLite.Net;
using SQLite.Net.Platform.WinRT;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using MyList.Models;
using System.Threading;
using SQLite.Net.Interop;

namespace MyList.Service
{
    public class TodoDB
    {
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Todos.db");

        public static SQLiteConnection db;

        public TodoDB()
        {
            try
            {
                db = new SQLiteConnection(new SQLitePlatformWinRT(), path);
                db.CreateTable<TodoItem>();
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        ~TodoDB()
        {
            db.Close();
        }

        //获得数据库中的Item
        public List<TodoItem> getALLItem()
        {
            var list = db.Table<Models.TodoItem>().ToList();
            return list;
        }
        
        //在数据库中添加Item
        public void addItem(Models.TodoItem item)
        {
            try
            {
                Debug.WriteLine(item.itemDateToString);
                db.Insert(item);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        //更新数据库中的Item
        public void updateItem(Models.TodoItem item)
        {
            try
            {
                db.Update(item);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        //删除数据库中的某个Item
        public void deleteItem(Models.TodoItem item)
        {
            try
            {
                db.Delete(item);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        //获取Item通过其id
        public TodoItem GetItemById(string id)
        {
            return db.Get<TodoItem>(id);
        }

        //清空
        public void clear()
        {
            db.DeleteAll<TodoItem>();
        }

        //查询
        public List<TodoItem> queryItems(string info)
        {
            info = "%" + info + "%";
            string[] _info = { info, info, info };
            List<TodoItem> list = db.Query<TodoItem>("SELECT * FROM TodoItem WHERE (TodoItem.Title LIKE ?) OR (TodoItem.Description LIKE ?) OR (TodoItem.itemDateToString LIKE ?) ", _info); 
            return list;
        }
    }
}
