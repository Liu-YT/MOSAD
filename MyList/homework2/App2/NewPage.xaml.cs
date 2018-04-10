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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace App2
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>

    class getPicClass
    {
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
                }
            }
            catch (Exception)
            {
                return;
            }
        }
    }

    public sealed partial class NewPage : Page
    {
        public NewPage()
        {
            this.InitializeComponent();
        }

        public Models.TodoItem ViewModel;

        private void selectPic(object sender, RoutedEventArgs e)
        {
            var getPicClass = new getPicClass();
            getPicClass.selectPic(pic);
        }

        
        private void DeleteAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            MainPage.allItem.RemoveTodoItem("");
            DeleteAppBarButton.Visibility = Visibility.Collapsed;
            var msgbox = new MessageDialog("删除成功!");
            var result = msgbox.ShowAsync();
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(MainPage));
        }
        

        private void AddAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            //修改Item中，直接新建Item
            if(this.ViewModel != null)
            {
                MainPage.allItem.selectedItem = null;
                this.ViewModel = null;
                DeleteAppBarButton.Visibility = Visibility.Collapsed;
                MySlider.Value = 0.5;
                titleText.Text = "" ;
                detailText.Text = "";
                DatePicker.Date = DateTimeOffset.Now;
                createButton.Content = "Create";
            }
            else
            {
                if (Window.Current.Bounds.Width < 800 && SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility == AppViewBackButtonVisibility.Collapsed)
                {
                    Frame rootFrame = Window.Current.Content as Frame;
                    rootFrame.Navigate(typeof(NewPage));
                }         
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.ViewModel = e.Parameter as Models.TodoItem;
            if (this.ViewModel!= null)
            {
                DeleteAppBarButton.Visibility = Visibility.Visible;
                MySlider.Value = this.ViewModel.picSize;
                titleText.Text = this.ViewModel.Title;;
                detailText.Text = this.ViewModel.Description;
                DatePicker.Date = this.ViewModel.ItemDate;
                pic.Source = this.ViewModel.image;
                createButton.Content = "Update";
            }
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
                MainPage.allItem.AddTodoItem(pic.Source as BitmapImage, MySlider.Value, titleText.Text, detailText.Text, DatePicker.Date);
                Message = "创建成功!";
                Frame rootFrame = Window.Current.Content as Frame;
                rootFrame.Navigate(typeof(MainPage));
            }
            else if (Message == "")
            {
                MainPage.allItem.UpdateTodaItem("", pic.Source as BitmapImage, MySlider.Value, titleText.Text, detailText.Text, DatePicker.Date);
                this.ViewModel = null;
                Message = "更新成功!";
                Frame rootFrame = Window.Current.Content as Frame;
                rootFrame.Navigate(typeof(MainPage));
            }
            var msgbox = new MessageDialog(Message);
            var result = msgbox.ShowAsync();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if(this.ViewModel == null)
            {
                titleText.Text = "";
                detailText.Text = "";
                DatePicker.Date = DateTimeOffset.Now.LocalDateTime;
            }
            else
            {
                MySlider.Value = this.ViewModel.picSize;
                titleText.Text = this.ViewModel.Title; ;
                detailText.Text = this.ViewModel.Description;
                DatePicker.Date = this.ViewModel.ItemDate;
                pic.Source = this.ViewModel.image;
            }   
        }
    }
}

