using System;
using System.Collections.Generic;
using System.IO;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Core;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Text;
using Windows.UI.Popups;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.ApplicationModel.DataTransfer;
using Windows.System.Diagnostics;
using System.Diagnostics;
using MyList.Models;
using System.Text;



// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace MyList
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            OrignPic = pic.Source as BitmapImage;
        }

        public BitmapImage OrignPic;

        public static ViewModels.TodoItemViewModel allItem = new ViewModels.TodoItemViewModel();

        public Models.TodoItem shareItem;

        // strongly-typed view models enable x:bind
        public ViewModels.TodoItemViewModel ViewModel { get { return allItem; } }

        //leftView
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            DependencyObject parent = VisualTreeHelper.GetParent((CheckBox)sender);
            Line MyLine = (Line)VisualTreeHelper.GetChild(parent, 3);
            CheckBox MyCheckBox = (CheckBox)sender;
            MyLine.Opacity = 1;
            TodoItem item = (sender as FrameworkElement).DataContext as TodoItem;
            if(item != null)
            {
                item.isChecked = true;
                ViewModel.UpdateTodoItemStatus(item);
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            DependencyObject parent = VisualTreeHelper.GetParent((CheckBox)sender);
            Line MyLine = (Line)VisualTreeHelper.GetChild(parent, 3);
            CheckBox MyCheckBox = (CheckBox)sender;
            MyLine.Opacity = 0;
            TodoItem item = (sender as FrameworkElement).DataContext as TodoItem;
            if (item != null)
            {
                item.isChecked = false;
                ViewModel.UpdateTodoItemStatus(item);
            }
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            DeleteAppBarButton.Visibility = Visibility.Visible;
            this.ViewModel.selectedItem = e.ClickedItem as Models.TodoItem;
            GetImage.getPicClass.imgName = this.ViewModel.selectedItem.imgName;
            if (rightView.Visibility == Visibility.Visible)
            {
                MySlider.Value = this.ViewModel.selectedItem.picSize;
                titleText.Text = this.ViewModel.selectedItem.Title; ;
                detailText.Text = this.ViewModel.selectedItem.Description;
                DatePicker.Date = this.ViewModel.selectedItem.ItemDate;
                pic.Source = this.ViewModel.selectedItem.image;
            }
            else
            {
                Frame rootFrame = Window.Current.Content as Frame;
                rootFrame.Navigate(typeof(NewPage));
            }
            createButton.Content = "Update";
        }

        private void DeleteItem(object sender, RoutedEventArgs e)
        {
            var data = (sender as FrameworkElement).DataContext;
            var item = data as Models.TodoItem;
            this.ViewModel.selectedItem = item;
            this.ViewModel.RemoveTodoItem("");
            if (rightView.Visibility == Visibility.Visible)
            {
                MySlider.Value = 0.5;
                titleText.Text = "";
                detailText.Text = "";
                DatePicker.Date = DateTimeOffset.Now;
                pic.Source = OrignPic;
            }
            var msgbox = new MessageDialog("删除成功!");
            var result = msgbox.ShowAsync();
        }

        private void EditItem(object sender, RoutedEventArgs e)
        {
            TodoItem item = (sender as FrameworkElement).DataContext as TodoItem;
            allItem.selectedItem = item;
            if (rightView.Visibility == Visibility.Visible)
            {
                MySlider.Value = allItem.selectedItem.picSize;
                titleText.Text = allItem.selectedItem.Title; ;
                detailText.Text = allItem.selectedItem.Description;
                DatePicker.Date = allItem.selectedItem.ItemDate;
                pic.Source = allItem.selectedItem.image;
            }
            else
            {
                Frame rootFrame = Window.Current.Content as Frame;
                rootFrame.Navigate(typeof(NewPage), allItem.selectedItem);
            }
        }

        private void ShareItem(object sender, RoutedEventArgs e)
        {
            var item = (sender as FrameworkElement).DataContext as Models.TodoItem;
            shareItem = item;
            DataTransferManager.ShowShareUI();
        }

        private async void clearAllItem(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ViewModel.AllItems.Clear();
                App.db.clear();
                var msgbox = new MessageDialog("清空成功！");
                await msgbox.ShowAsync();
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private async void queryItems(object sender, RoutedEventArgs e) {
            if(queryInfo.Text != "")
            {
                StringBuilder items = new StringBuilder();
                List<TodoItem> list = App.db.queryItems(queryInfo.Text);
                if (list.Count == 0) await new MessageDialog("没有相应信息！").ShowAsync();
                else
                {
                    //items.AppendLine($"{"Title\t\t"} {"Description\t\t"} {"Time"}");
                    foreach (var item in list)
                    {
                        items.AppendLine($"{"Title:\t" + item.Title}");
                        items.AppendLine($"{"Description:\t" + item.Description}");
                        items.AppendLine($"{"Time:\t" + item.ItemDate.ToLocalTime() + "\n"}");
                        //items.AppendLine($"{item.Title + "\t\t"} {item.Description + "\t\t"} {item.ItemDate}");
                    }
                    await new MessageDialog(items.ToString()).ShowAsync();
                }
                queryInfo.Text = "";
            }
            else
            {
                var msgbox = new MessageDialog("请输入查询信息");
                await msgbox.ShowAsync();
            }
        }

        // Handle DataRequested event and provide DataPackage
        public async void OnShareDataRequested(DataTransferManager sender, DataRequestedEventArgs args){
            var request = args.Request;
            var deferral = args.Request.GetDeferral();
            if(shareItem.imgName != "" && shareItem.imgName != null)
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync(shareItem.imgName);
                IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read);
                var picStream = RandomAccessStreamReference.CreateFromStream(fileStream);
                request.Data.SetBitmap(picStream);
            }
            else
            {
                var picStream = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/pic_6.jpg"));
                request.Data.SetBitmap(picStream);
            }
            request.Data.Properties.Title = shareItem.Title;
            request.Data.Properties.Description = "A share of Todo";
            request.Data.SetText(shareItem.Description);
            shareItem = null;
            deferral.Complete();
        }

        //rightView
        private void selectPic(object sender, RoutedEventArgs e)
        {
            var getPicClass = new GetImage.getPicClass();
            getPicClass.selectPic(pic);
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            string Message = "";
            if (allItem.selectedItem == null)
            {
                //创建
                if (titleText.Text == "")
                    Message += "请输入标题\n";
                if (detailText.Text == "")
                    Message += "请输入内容详情\n";
                if (DatePicker.Date < DateTimeOffset.Now.LocalDateTime.AddDays(-1))
                    Message += "请选择正确的时间";
                if (Message == "")
                {
                    allItem.AddTodoItem(pic.Source as BitmapImage, MySlider.Value, GetImage.getPicClass.imgName, titleText.Text, detailText.Text, DatePicker.Date);
                    Message = "创建成功!";
                    GetImage.getPicClass.imgName = "";
                    Frame rootFrame = Window.Current.Content as Frame;
                    rootFrame.Navigate(typeof(MainPage));
                } 
            }     
            else
            {
                //更新
                Debug.WriteLine("更新");
                if (titleText.Text == "")
                    Message += "请输入标题\n";
                if (detailText.Text == "")
                    Message += "请输入内容详情\n";
                if (DatePicker.Date < DateTimeOffset.Now.LocalDateTime.AddDays(-1))
                    Message += "请选择正确的时间";
                Debug.WriteLine(DateTimeOffset.Now);
                Debug.WriteLine(DateTime.Now);
                allItem.UpdateTodoItem("", pic.Source as BitmapImage, MySlider.Value, GetImage.getPicClass.imgName, titleText.Text, detailText.Text, DatePicker.Date, allItem.selectedItem.isChecked);
                Message = "更新成功!";
                createButton.Content = "Create";
                GetImage.getPicClass.imgName = "";
                Frame rootFrame = Window.Current.Content as Frame;
                rootFrame.Navigate(typeof(MainPage));
            }
            var msgbox = new MessageDialog(Message);
            var result = msgbox.ShowAsync();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (allItem.selectedItem == null)
            {
                titleText.Text = "";
                detailText.Text = "";
                DatePicker.Date = DateTimeOffset.Now.LocalDateTime;
                MySlider.Value = 0.5;
                pic.Source = OrignPic;
                GetImage.getPicClass.imgName = "";
            }
            else
            {
                MySlider.Value = allItem.selectedItem.picSize;
                titleText.Text = allItem.selectedItem.Title; ;
                detailText.Text = allItem.selectedItem.Description;
                DatePicker.Date = allItem.selectedItem.ItemDate;
                pic.Source = allItem.selectedItem.image;
                GetImage.getPicClass.imgName = allItem.selectedItem.imgName;
            }
        }

        //AppBarButton
        private void AddAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (rightView.Visibility != Visibility.Visible)
            {
                GetImage.getPicClass.imgName = "";
                Frame rootFrame = Window.Current.Content as Frame;
                rootFrame.Navigate(typeof(NewPage));
            }
            else
            {
                
            }
        }

        private void DeleteAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if(allItem.selectedItem != null)
            {
                GetImage.getPicClass.imgName = "";
                allItem.RemoveTodoItem("");
                DeleteAppBarButton.Visibility = Visibility.Collapsed;
                var msgbox = new MessageDialog("删除成功!");
                var result = msgbox.ShowAsync();
                Frame rootFrame = Window.Current.Content as Frame;
                rootFrame.Navigate(typeof(MainPage));
            }
        }

        //navigate
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            DataTransferManager.GetForCurrentView().DataRequested += OnShareDataRequested;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            this.ViewModel.selectedItem = null;
            if (e.NavigationMode == NavigationMode.New)
            {
                //if this is a new navigation, this is a fresh lauch so we can
                //discard any saved state

                ApplicationData.Current.LocalSettings.Values.Remove("MainPage");
            }
            else
            {
                //try to restore state if any, in case we were terminated
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("MainPage"))
                {
                    var composite = ApplicationData.Current.LocalSettings.Values["MainPage"] as ApplicationDataCompositeValue;
                    MySlider.Value = (double)composite["picSize"];
                    titleText.Text = (string)composite["title"];
                    detailText.Text = (string)composite["detail"];
                    DatePicker.Date = (DateTimeOffset)composite["date"];
                    GetImage.getPicClass.imgName = (string)composite["image"];
                    string isSelectedId = (string)composite["selectedId"];
                    if (isSelectedId != null)
                    {
                        createButton.Content = "Update";
                        foreach (var item in this.ViewModel.AllItems)
                        {
                            if (item.id == isSelectedId)
                                this.ViewModel.selectedItem = item;
                        }
                    }
                    if (GetImage.getPicClass.imgName == "" || GetImage.getPicClass.imgName == null)
                        pic.Source = OrignPic;
                    else
                    {
                        var file = await ApplicationData.Current.LocalFolder.GetFileAsync(GetImage.getPicClass.imgName);
                        IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read);
                        BitmapImage bitmapImage = new BitmapImage();
                        await bitmapImage.SetSourceAsync(fileStream);
                        pic.Source = bitmapImage;
                    }
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            DataTransferManager.GetForCurrentView().DataRequested -= OnShareDataRequested;
            bool suspending = ((App)App.Current).issuspend;
            //暂停
            if (suspending)
            {
                ApplicationDataCompositeValue composite = new ApplicationDataCompositeValue();
                if (this.ViewModel.selectedItem == null)
                    composite["selectedId"] = null;
                else
                    composite["selectedId"] = this.ViewModel.selectedItem.id;
                if (GetImage.getPicClass.imgName != "")
                    composite["image"] = GetImage.getPicClass.imgName;
                else 
                    composite["image"] = "";
                composite["picSize"] = MySlider.Value;
                composite["title"] = titleText.Text;
                composite["detail"] = detailText.Text;
                composite["date"] = DatePicker.Date;
                ApplicationData.Current.LocalSettings.Values["MainPage"] = composite;
            }
        }
    }
}
