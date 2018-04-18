using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;
using Microsoft.Toolkit.Uwp.Notifications;
using MyList.Models;
using System.Diagnostics;

namespace MyList.Service
{
    public class TileService
    {
        static public XmlDocument CreateFiles(TodoItem Item)
        {
            string title = Item.Title;
            string description = Item.Description;
            TileContent content = new TileContent()
            {
                Visual = new TileVisual()
                {
                    TileSmall = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            BackgroundImage = new TileBackgroundImage()
                            {
                                Source = "Assets/pic_1.jpg",
                                HintOverlay = 60
                            },
                            Children =
                            {
                               new AdaptiveText()
                                {
                                    Text = title,
                                    HintStyle = AdaptiveTextStyle.Subtitle
                                },

                                new AdaptiveText()
                                {
                                    Text = description,
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },
                            }
                        }
                    },

                    TileMedium = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            BackgroundImage = new TileBackgroundImage()
                            {
                                Source = "Assets/pic_1.jpg",
                                HintOverlay = 60
                            },
                            Children =
                            {
                               new AdaptiveText()
                                {
                                    Text = title,
                                    HintStyle = AdaptiveTextStyle.Subtitle
                                },

                                new AdaptiveText()
                                {
                                    Text = description,
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },
                            }
                        }
                    },

                    TileWide = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            BackgroundImage = new TileBackgroundImage()
                            {
                                Source = "Assets/pic_1.jpg",
                                HintOverlay = 60
                            },
                            Children =
                            {
                               new AdaptiveText()
                                {
                                    Text = title,
                                    HintStyle = AdaptiveTextStyle.Subtitle
                                },

                                new AdaptiveText()
                                {
                                    Text = description,
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },
                            }
                        }
                    },

                    TileLarge = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            BackgroundImage = new TileBackgroundImage()
                            {
                                Source = "Assets/pic_1.jpg",
                                HintOverlay = 60
                            },
                            Children =
                            {
                               new AdaptiveText()
                                {
                                    Text = title,
                                    HintStyle = AdaptiveTextStyle.Subtitle
                                },

                                new AdaptiveText()
                                {
                                    Text = description,
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },
                            }
                        }
                    }
                }
            };
            XmlDocument xdox = content.GetXml();
            return content.GetXml();
        }

        //磁贴
        static public void UpdateTileItem()
        {
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            updater.EnableNotificationQueue(true);
            updater.Clear();
            if (MainPage.allItem.AllItems.Count == 0)
                return;
            else
            {
                int count = 0;
                foreach (var item in MainPage.allItem.AllItems)
                {
                    ++count;
                    var xmlDoc = TileService.CreateFiles(item);
                    TileNotification notification = new TileNotification(xmlDoc);
                    updater.Update(notification);
                    if (count == 5) break;
                }
            }
        }
    }
}
