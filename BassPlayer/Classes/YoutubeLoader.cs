using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Web;
using System.Windows.Media.Imaging;
using System.Xml;

namespace BassPlayer.Classes
{
    public class YoutubeItem
    {
        public string Title { get; set; }
        public string VideoId { get; set; }
        public DateTimeOffset Date { get; set; }
        public BitmapImage Thumbnail { get; set; }
    }

    internal static class YoutubeLoader
    {
        private static BitmapImage DownloadImage(string id)
        {
            WebClient wc = new WebClient();
            byte[] data = wc.DownloadData(string.Format("http://img.youtube.com/vi/{0}/mqdefault.jpg", id));
            MemoryStream ms = new MemoryStream(data);
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = ms;
            bitmap.EndInit();
            return bitmap;
        }

        public static IEnumerable<YoutubeItem> Search(string s)
        {
            string query = HttpUtility.UrlEncode(s);
            
            XmlReader reader = XmlReader.Create("https://gdata.youtube.com/feeds/api/videos?q="+query);

            SyndicationFeed feed = SyndicationFeed.Load(reader);
            
            //http://img.youtube.com/vi/6GO1MEYVpkM/mqdefault.jpg

            var q = from item in feed.Items
                            select new YoutubeItem
                            {
                                Title = item.Title.Text,
                                Date = item.PublishDate,
                                VideoId = item.Id.Replace("http://gdata.youtube.com/feeds/api/videos/", ""),
                                Thumbnail = DownloadImage(item.Id.Replace("http://gdata.youtube.com/feeds/api/videos/", ""))
                            };


            return q;
        }

    }
}
