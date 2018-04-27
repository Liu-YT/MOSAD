using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
using Windows.Data.Json;
using System.Xml;


// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace Network
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            ClearWeatherText();
            ClearPhoneText();
        }

        public void ClearPhoneText()
        {
            phoneNumber.Text = "";
            place.Text = "";
            from.Text = "";
        }

        public void ClearWeatherText()
        {
            updateTime.Text = "更新时间: ";
            cityName.Text = "城市: ";
            template.Text = "温度: ";
            wet.Text = "湿度: ";
            sunrise.Text = "日出时间: ";
            sunset.Text = "日落时间: ";
            windDirection.Text = "风向： ";
        }

        public async void httpServiceForWeather()
        {
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

            Uri requestUri = new Uri("https://www.sojson.com/open/api/weather/xml.shtml?city=" + city.Text);
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
                ClearWeatherText();
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
            }
            catch (Exception ex)
            {
                await new MessageDialog("该城市不存在！请重新输入").ShowAsync();
                httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
            }

            Debug.WriteLine(httpResponseBody);
        }

        public async void httpServiceForPhone()
        {
            ClearPhoneText();
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

            Uri requestUri = new Uri("http://tcc.taobao.com/cc/json/mobile_tel_segment.htm?tel=" + phone.Text);
            //Send the GET request asynchronously and retrieve the response as a string.
            Windows.Web.Http.HttpResponseMessage httpResponse = new Windows.Web.Http.HttpResponseMessage();
            string httpResponseBody = "";

            try
            {
                //Send the GET request
                httpResponse = await httpClient.GetAsync(requestUri);
                httpResponse.EnsureSuccessStatusCode();
                httpResponseBody = await httpResponse.Content.ReadAsStringAsync();
                int start = httpResponseBody.IndexOf('{') + 1;
                int end = httpResponseBody.IndexOf('}');
                string info = httpResponseBody.Substring(start, end - start);
                var phoneInfo = new Dictionary<string, string>();
                string[] infoArray = info.Split(',');
                foreach (string str in infoArray)
                {
                    Debug.WriteLine("str" + str);
                    int index = str.IndexOf(':');
                    int strStart = str.LastIndexOf(' ') > str.LastIndexOf('\t') ? str.LastIndexOf(' ') + 1: str.LastIndexOf('\t') + 1;
                    string key = str.Substring(strStart, index - strStart);
                    string value = str.Substring(index + 2, str.LastIndexOf('\'') - index - 2);
                    phoneInfo.Add(key, value);
                }
                if(phoneInfo.ContainsKey("mts"))
                {
                    phoneNumber.Text = phoneInfo["telString"];
                    place.Text = phoneInfo["province"];
                    from.Text = phoneInfo["catName"] + "---" + phoneInfo["carrier"];
                }
                else
                {
                    await new MessageDialog("该号码不存在！请重新输入").ShowAsync();
                }
            }
            catch (Exception ex)
            {
                httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
            }

            
        }

        private async void Button_Click_Weather(object sender, RoutedEventArgs e)
        {
            if(city.Text != "") 
                httpServiceForWeather();
            else
            {
                var msgbox = new MessageDialog("请输入查询的城市");
                await msgbox.ShowAsync();
            }
            city.Text = "";
        }

        private async void Button_Click_Phone(object sender, RoutedEventArgs e)
        {
            if (phone.Text != "")
                httpServiceForPhone();
            else
            {
                var msgbox = new MessageDialog("请输入查询的手机号");
                await msgbox.ShowAsync();
            }
            phone.Text = "";
        }
    }
}
