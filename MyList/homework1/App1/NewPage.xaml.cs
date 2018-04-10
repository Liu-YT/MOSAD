using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Popups;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace App1
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class NewPage : Page
    {
        public NewPage()
        {
            this.InitializeComponent();
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
            if (Message == "")
                Message = "创建成功!";
            var msgbox = new MessageDialog(Message);
            var result = msgbox.ShowAsync();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            titleText.Text = "";
            detailText.Text = "";
            DatePicker.Date = DateTimeOffset.Now.LocalDateTime;
        }
    }
}
