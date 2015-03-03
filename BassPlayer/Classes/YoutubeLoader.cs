using BassEngine;
using BassPlayer.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml;
using YoutubeExtractor;

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
        private static ProxyConfig GetProxyConfig()
        {
            ProxyConfig config = new ProxyConfig
            {
                URL = Settings.Default.ProxyAddress,
                Port = Convert.ToInt32(Settings.Default.ProxyPort),
                Enabled = Settings.Default.ProxyEnabled,
                Username = Settings.Default.ProxyUser,
                Passwd = Settings.Default.ProxyPassword,
                RequiresAuth = Settings.Default.ProxyAuthReq
            };
            return config;
        }

        private static BitmapImage DownloadImage(string id)
        {
            WebClient wc = Helpers.CreateWebClient(GetProxyConfig());
            byte[] data = wc.DownloadData(string.Format("http://img.youtube.com/vi/{0}/mqdefault.jpg", id));
            MemoryStream ms = new MemoryStream(data);
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = ms;
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }

        private static PlayListEntry FromYoutubeItemFunction(YoutubeItem item)
        {
            PlayListEntry ple = new PlayListEntry();

            var videoinfos = DownloadUrlResolver.GetDownloadUrls("https://www.youtube.com/watch?v=" + item.VideoId);

            var video = (from v in videoinfos where v.VideoType == VideoType.Mp4 && v.Resolution == 360 select v).FirstOrDefault();
            if (video == null) return null;

            if (video.RequiresDecryption) DownloadUrlResolver.DecryptDownloadUrl(video);

            ple.Title = video.Title;
            ple.FileName = video.DownloadUrl;
            return ple;
        }

        private static YoutubeItem[] SearchFunction(string s)
        {
            string query = HttpUtility.UrlEncode(s);

            WebClient wc = Helpers.CreateWebClient(GetProxyConfig());
            byte[] data = wc.DownloadData("https://gdata.youtube.com/feeds/api/videos?q=" + query);
            MemoryStream ms = new MemoryStream(data);

            XmlReader reader = XmlReader.Create(ms);

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

            ms.Close();

            return q.ToArray();
        }

        public static Task<YoutubeItem[]> Search(string s)
        {
            return Task.Run(() => SearchFunction(s));
        }

        public static Task<PlayListEntry> FromYoutubeItem(YoutubeItem item)
        {
            return Task.Run(() => FromYoutubeItemFunction(item));
        }
    }
}
