using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Xml;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace LifeQuery
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void setWeatherText()
        {
            updateTime.Text = "更新时间: \t";
            cityName.Text = "城市: \t\t";
            template.Text = "温度: \t\t";
            wet.Text = "湿度: \t\t";
            sunrise.Text = "日出时间: \t";
            sunset.Text = "日落时间: \t";
            windDirection.Text = "风向： \t\t";
        }

        private void setIpText ()
        {
            IpNum.Text = "Ip: \t";
            country.Text = "国家: \t";
            regionName.Text = "地区: \t";
            city.Text = "城市: \t";
            lat.Text = "纬度: \t";
            lon.Text = "经度: \t";
        }

        private async void queryIp(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            setIpText();

            //Create an HTTP client object
            Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient();

            //Add a user-agent header to the GET request. 
            var headers = httpClient.DefaultRequestHeaders;
            //The safe way to add a header value is to use the TryParseAdd method and verify the return value is true,
            //especially if the header value is coming from user input.
            string header = "ie";
            if (!headers.UserAgent.TryParseAdd(header))
            {
                throw new Exception("Invalid header value: " + header);
            }

            header = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
            if (!headers.UserAgent.TryParseAdd(header))
            {
                throw new Exception("Invalid header value: " + header);
            }

            Uri requestUri = new Uri("http://ip-api.com/json/" + Ip.Text);
            //Send the GET request asynchronously and retrieve the response as a string.
            Windows.Web.Http.HttpResponseMessage httpResponse = new Windows.Web.Http.HttpResponseMessage();
            string httpResponseBody = "";
            try
            {

                //Send the GET request
                httpResponse = await httpClient.GetAsync(requestUri);
                httpResponse.EnsureSuccessStatusCode();
                httpResponseBody = await httpResponse.Content.ReadAsStringAsync();
                //Debug.WriteLine(httpResponseBody);
                JsonObject info = JsonObject.Parse(httpResponseBody);
                //Debug.WriteLine(info);
                if(info["status"].GetString() == "success")
                {
                    IpNum.Text += info["query"].GetString();
                    country.Text += info["country"].GetString();
                    regionName.Text += info["regionName"].GetString();
                    city.Text += info["city"].GetString();
                    Debug.WriteLine(info["lat"]);
                    lat.Text += info["lat"].GetNumber().ToString();
                    lon.Text += info["lon"].GetNumber().ToString();
                    IpDetail.Visibility = Visibility.Visible;
                }
                else
                {
                    await new MessageDialog(info["message"].GetString()).ShowAsync();
                    IpDetail.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                await new MessageDialog("出现错误！请重新输入").ShowAsync();
                httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
            }
        }

        private async void queryWeather(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args) 
        {
            setWeatherText();

            //Create an HTTP client object
            Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient();

            //Add a user-agent header to the GET request. 
            var headers = httpClient.DefaultRequestHeaders;
            //The safe way to add a header value is to use the TryParseAdd method and verify the return value is true,
            //especially if the header value is coming from user input.
            string header = "ie";
            if (!headers.UserAgent.TryParseAdd(header))
            {
                throw new Exception("Invalid header value: " + header);
            }

            header = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
            if (!headers.UserAgent.TryParseAdd(header))
            {
                throw new Exception("Invalid header value: " + header);
            }

            Uri requestUri = new Uri("https://www.sojson.com/open/api/weather/xml.shtml?city=" + Weather.Text);
            //Send the GET request asynchronously and retrieve the response as a string.
            Windows.Web.Http.HttpResponseMessage httpResponse = new Windows.Web.Http.HttpResponseMessage();
            string httpResponseBody = "";

            try
            {
                //Send the GET request
                httpResponse = await httpClient.GetAsync(requestUri);
                httpResponse.EnsureSuccessStatusCode();
                httpResponseBody = await httpResponse.Content.ReadAsStringAsync();
                Debug.WriteLine(httpResponseBody);
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(httpResponseBody);
                XmlNodeList nodelist = xmlDoc.GetElementsByTagName("resp");
                foreach (XmlNode node in nodelist)
                {
                    updateTime.Text += node["updatetime"].InnerText;
                    cityName.Text += node["city"].InnerText;
                    template.Text += node["wendu"].InnerText + "℃";
                    wet.Text += node["shidu"].InnerText;
                    sunrise.Text += node["sunrise_1"].InnerText;
                    sunset.Text += node["sunset_1"].InnerText;
                    windDirection.Text += node["fengxiang"].InnerText;
                }
                WeatherDetail.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                await new MessageDialog("该城市不存在！请重新输入").ShowAsync();
                WeatherDetail.Visibility = Visibility.Collapsed;
                httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
            }
        }

        private void WeatherChecked(object sender, RoutedEventArgs e)
        {
            Weather.Visibility = Visibility.Visible;
            Ip.Visibility = Visibility.Collapsed;
            IpDetail.Visibility = Visibility.Collapsed;
        }

        private void IpChecked(object sender, RoutedEventArgs e)
        {
            Weather.Visibility = Visibility.Collapsed;
            WeatherDetail.Visibility = Visibility.Collapsed;
            Ip.Visibility = Visibility.Visible;
        }
    }
}
