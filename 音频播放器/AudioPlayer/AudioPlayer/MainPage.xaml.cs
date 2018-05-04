using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace AudioPlayer
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        MediaPlayer mediaPlayer = new MediaPlayer();
        MediaTimelineController mediaTimelineController = new MediaTimelineController();
        TimeSpan time;

        public MainPage()
        {
            this.InitializeComponent();
            mediaPlayer.TimelineController = mediaTimelineController;
            mediaPlayer.CommandManager.IsEnabled = false;
        }

        //play the media
        void PlayMedia(object sender, RoutedEventArgs e)
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer;
            timer.Start();
            if (mediaTimelineController.State == MediaTimelineControllerState.Paused)
                mediaTimelineController.Resume();
            else
            {
                mediaTimelineController.Start();//播放加载好的视频文件.
                InitializePropertyValues();
            }
                
            //图片旋转动画开始  
            EllStoryboard.Begin();

            Start.Visibility = Visibility.Collapsed;
            Pause.Visibility = Visibility.Visible;
        }

        //pause the media
        void PauseMedia(object sender, RoutedEventArgs e)
        {

            mediaTimelineController.Pause();//暂停播放

            //图片旋转动画暂停  
            EllStoryboard.Pause();

            Start.Visibility = Visibility.Visible;
            Pause.Visibility = Visibility.Collapsed;

        }

        // Stop the media.
        void StopMedia(object sender, RoutedEventArgs e)
        {

            //停止播放,再次播放会从头开始
            mediaTimelineController.Position = TimeSpan.FromSeconds(0);
            mediaTimelineController.Pause();

            Start.Visibility = Visibility.Visible;
            Pause.Visibility = Visibility.Collapsed;

            //图片旋转动画结束  
            EllStoryboard.Stop();
        }

        void Timer(object sender, object e)
        {
            nowTime.Text = mediaPlayer.PlaybackSession.Position.ToString().Substring(0, 8);
            timeLine.Value = ((TimeSpan)mediaTimelineController.Position).TotalSeconds;
            if (timeLine.Value == timeLine.Maximum)
            {
                mediaTimelineController.Position = TimeSpan.FromSeconds(0);
                mediaTimelineController.Pause();
                EllStoryboard.Stop();
                Pause.Visibility = Visibility.Collapsed;
                Start.Visibility = Visibility.Visible;
            }
        }

        private async void Add_Click(object sender, RoutedEventArgs e)
        {
            var filePicker = new FileOpenPicker();

            filePicker.SuggestedStartLocation = PickerLocationId.VideosLibrary;
            filePicker.FileTypeFilter.Add(".wmv");
            filePicker.FileTypeFilter.Add(".mp4");
            filePicker.FileTypeFilter.Add(".mp3");
            filePicker.FileTypeFilter.Add(".wma");

            StorageFile file = await filePicker.PickSingleFileAsync();
            
            if (file != null)
            {
                int start = file.Path.LastIndexOf('\\') + 1;
                int end = file.Path.LastIndexOf('.');
                string FileName = file.Path.Substring(start, end - start);
                Debug.WriteLine(FileName);
                fileName.Text = FileName;
                var source = MediaSource.CreateFromStorageFile(file);
                source.OpenOperationCompleted += setTimeLine;
                mediaPlayer.Source = source;
                if (file.FileType == ".mp3" || file.FileType == ".wma")
                {
                    LayoutRoot.Visibility = Visibility.Visible;
                    myMediaElement.Visibility = Visibility.Collapsed;
                    myMediaElement.SetMediaPlayer(mediaPlayer);
                }
                else
                {
                    LayoutRoot.Visibility = Visibility.Collapsed;
                    myMediaElement.Visibility = Visibility.Visible;
                }
                timeLine.Value = 0;
                Start.Visibility = Visibility.Visible;
                Pause.Visibility = Visibility.Collapsed;
                StopMedia(null, null);
            }
        }

        private void FullPrint_Click(object sender, RoutedEventArgs e)
        {
            ApplicationView view = ApplicationView.GetForCurrentView();
            if(view.IsFullScreenMode)
            {
                view.ExitFullScreenMode();
            }
            else
            {
                view.TryEnterFullScreenMode();
            }
        }
        
        private void ChangeMediaVolume(object sender, RangeBaseValueChangedEventArgs args)
        {
            mediaPlayer.Volume = (double)VolumnSlider.Value;//更改音量
        }

        //点击开始时设置播放的音量
        void InitializePropertyValues()
        {
            mediaPlayer.Volume = (double)VolumnSlider.Value;
        }

        private async void setTimeLine(MediaSource sender, MediaSourceOpenOperationCompletedEventArgs args)
        {
            time = sender.Duration.GetValueOrDefault();

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Debug.WriteLine(time.TotalSeconds);
                timeLine.Minimum = 0;
                timeLine.Maximum = time.TotalSeconds;
                timeLine.StepFrequency = 1;
                DateTime dt = new DateTime(0).AddSeconds(time.TotalSeconds);
                totalTime.Text = " " + dt.ToString().Substring(9, 7);
                nowTime.Text = "00:00:00";
            });
        }
    }
}
