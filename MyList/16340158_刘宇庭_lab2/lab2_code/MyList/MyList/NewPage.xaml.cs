using Windows.UI.Xaml;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Core;
using Windows.UI;
using Windows.Storage.Streams;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Text;
using Windows.UI.Popups;
using Windows.Storage;
using System.Diagnostics;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace MyList
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>

    public sealed partial class NewPage : Page
    {
        public NewPage()
        {
            this.InitializeComponent();
            OrignPic = pic.Source as BitmapImage;
        }

        public BitmapImage OrignPic;

        public Models.TodoItem ViewModel;

        private void selectPic(object sender, RoutedEventArgs e)
        {
            var getPicClass = new GetImage.getPicClass();
            getPicClass.selectPic(pic);
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            string Message = "";
            if (titleText.Text == "")
                Message += "请输入标题\n";
            if (detailText.Text == "")
                Message += "请输入内容详情\n";
            if (DatePicker.Date < DateTimeOffset.Now.LocalDateTime.AddDays(-1))
                Message += "请选择正确的时间";

            if (Message == "" && (this.ViewModel == null))
            {
                MainPage.allItem.AddTodoItem(pic.Source as BitmapImage, MySlider.Value, GetImage.getPicClass.imgName, titleText.Text, detailText.Text, DatePicker.Date);
                Message = "创建成功!";
                GetImage.getPicClass.imgName = "";
                Frame rootFrame = Window.Current.Content as Frame;
                rootFrame.Navigate(typeof(MainPage));
            }
            else if (Message == "")
            {
                MainPage.allItem.UpdateTodoItem("", pic.Source as BitmapImage, MySlider.Value, GetImage.getPicClass.imgName, titleText.Text, detailText.Text, DatePicker.Date, this.ViewModel.isChecked);
                this.ViewModel = null;
                Message = "更新成功!";
                GetImage.getPicClass.imgName = "";
                Frame rootFrame = Window.Current.Content as Frame;
                rootFrame.Navigate(typeof(MainPage));
            }
            var msgbox = new MessageDialog(Message);
            var result = msgbox.ShowAsync();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.ViewModel == null)
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
                MySlider.Value = this.ViewModel.picSize;
                titleText.Text = this.ViewModel.Title; ;
                detailText.Text = this.ViewModel.Description;
                DatePicker.Date = this.ViewModel.ItemDate;
                pic.Source = this.ViewModel.image;
                GetImage.getPicClass.imgName = ViewModel.imgName;
            }
        }

        //ApplicationBar delete
        private void DeleteAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            MainPage.allItem.RemoveTodoItem("");
            DeleteAppBarButton.Visibility = Visibility.Collapsed;
            var msgbox = new MessageDialog("删除成功!");
            var result = msgbox.ShowAsync();
            Frame rootFrame = Window.Current.Content as Frame;
            GetImage.getPicClass.imgName = "";
            rootFrame.Navigate(typeof(MainPage));
        }

        //storage and nagivation
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            bool suspending = ((App)App.Current).issuspend;
            //暂停
            if(suspending == true)
            {
                ApplicationDataCompositeValue composite = new ApplicationDataCompositeValue();
                composite["picSize"] = MySlider.Value;
                composite["title"] = titleText.Text;
                composite["detail"] = detailText.Text;
                composite["date"] = DatePicker.Date;
                composite["isSelectedId"] = (this.ViewModel == null ? null : this.ViewModel.id);
                if (GetImage.getPicClass.imgName != "" || GetImage.getPicClass.imgName != null)
                    composite["image"] = GetImage.getPicClass.imgName;
                else
                    composite["image"] = "";
                ApplicationData.Current.LocalSettings.Values["NewPage"] = composite;

            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.New)
            {
                //if this is a new navigation, this is a fresh lauch so we can
                //discard any saved state
                ApplicationData.Current.LocalSettings.Values.Remove("NewPage");
                this.ViewModel = MainPage.allItem.selectedItem;
                if (this.ViewModel != null)
                {
                    DeleteAppBarButton.Visibility = Visibility.Visible;
                    MySlider.Value = this.ViewModel.picSize;
                    titleText.Text = this.ViewModel.Title; ;
                    detailText.Text = this.ViewModel.Description;
                    DatePicker.Date = this.ViewModel.ItemDate;
                    pic.Source = this.ViewModel.image;
                    createButton.Content = "Update";
                }
            }
            else
            {
                //try to restore state if any, in case we were terminated
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("NewPage"))
                {
                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                    var composite = ApplicationData.Current.LocalSettings.Values["NewPage"] as ApplicationDataCompositeValue;
                    MySlider.Value = (double)composite["picSize"];
                    titleText.Text = (string)composite["title"];
                    detailText.Text = (string)composite["detail"];
                    DatePicker.Date = (DateTimeOffset)composite["date"];
                    GetImage.getPicClass.imgName = (string)composite["image"];
                    string isSelectedId = (string)composite["isSelectedId"];
                    if (isSelectedId != null)
                    {
                        createButton.Content = "Update";
                        foreach (var item in MainPage.allItem.AllItems)
                        {
                            if (item.id == isSelectedId)
                                MainPage.allItem.selectedItem = this.ViewModel = item;
                        }
                        DeleteAppBarButton.Visibility = Visibility.Visible;
                    }
                    if (GetImage.getPicClass.imgName == "" || GetImage.getPicClass.imgName == null)
                        pic.Source = new BitmapImage(new Uri("ms-appx:///Assets/pic_6.jpg"));
                    else
                    {
                        var file = await ApplicationData.Current.LocalFolder.GetFileAsync(GetImage.getPicClass.imgName);
                        IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read);
                        BitmapImage bitmapImage = new BitmapImage();
                        await bitmapImage.SetSourceAsync(fileStream);
                        pic.Source = bitmapImage;
                    }

                    //we have done it, so remove it
                    ApplicationData.Current.LocalSettings.Values.Remove("NewPage");
                }
            }
        }
    }
}

