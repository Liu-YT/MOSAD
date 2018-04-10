using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;
using Windows.UI.Popups;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace App2
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // public static ViewModels.TodoItemViewModel ViewModel = new ViewModels.TodoItemViewModel();

        public MainPage()
        {
            InitializeComponent();
            rightView.Navigate(typeof(NewPage),null);
        }

        public static ViewModels.TodoItemViewModel allItem = new ViewModels.TodoItemViewModel();

        // strongly-typed view models enable x:bind
        public ViewModels.TodoItemViewModel ViewModel { get { return allItem; } }      

        private void MyCheckBox_Click(object sender, RoutedEventArgs e)
        {
            DependencyObject parent = VisualTreeHelper.GetParent((CheckBox)sender);
            Line MyLine = (Line)VisualTreeHelper.GetChild(parent, 3);
            CheckBox MyCheckBox = (CheckBox)sender;
            if(MyCheckBox.IsChecked == false)
            {
                MyLine.Opacity = 0;
                
            }
            else
            {
                MyLine.Opacity = 1;
            }
        }

        private void AddAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if(rightView.Visibility != Visibility.Visible)
            {
                Frame rootFrame = Window.Current.Content as Frame;
                rootFrame.Navigate(typeof(NewPage));
            }
            else
            {
                rightView.Navigate(typeof(NewPage));
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            this.ViewModel.selectedItem = null;
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            allItem.selectedItem = e.ClickedItem as Models.TodoItem;
            if (rightView.Visibility == Visibility.Visible)
            {
                rightView.Navigate(typeof(NewPage), allItem.selectedItem);
            }
            else
            {
                Frame rootFrame = Window.Current.Content as Frame;
                rootFrame.Navigate(typeof(NewPage), allItem.selectedItem);
            }
        }

        private void DeleteItem(object sender, RoutedEventArgs e)
        {
            var data = (sender as FrameworkElement).DataContext;
            var item = data as Models.TodoItem;
            this.ViewModel.selectedItem = item;
            this.ViewModel.RemoveTodoItem("");
            if(rightView.Visibility == Visibility.Visible)
            {
                rightView.Navigate(typeof(NewPage));
            }
            var msgbox = new MessageDialog("删除成功!");
            var result = msgbox.ShowAsync();
        }

        private void EditItem(object sender, RoutedEventArgs e)
        {
            var data = (sender as FrameworkElement).DataContext;
            var item = data as Models.TodoItem;
            allItem.selectedItem = item;
            if(rightView.Visibility == Visibility.Visible)
            {
                rightView.Navigate(typeof(NewPage), allItem.selectedItem);
            }
            else
            {
                Frame rootFrame = Window.Current.Content as Frame;
                rootFrame.Navigate(typeof(NewPage), allItem.selectedItem);
            }
        }
    }
}
