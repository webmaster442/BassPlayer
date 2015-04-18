using BassEngine;
using BassPlayer.Properties;
using BassPlayer.SongSources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Media.Imaging;
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

    /// <summary>
    /// Youtube Downloader & Loader functions
    /// </summary>
    internal static class YoutubeLoader
    {
        private static ProxyConfig GetProxyConfig()
        {
            if (!Settings.Default.ProxyEnabled) return null;
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

            DownloadUrlResolver.ProxyConfiguration = GetProxyConfig();

            var videoinfos = DownloadUrlResolver.GetDownloadUrls("https://www.youtube.com/watch?v=" + item.VideoId);

            var video = (from v in videoinfos where v.VideoType == VideoType.Mp4 && v.Resolution == 360 select v).FirstOrDefault();
            if (video == null) return null;

            if (video.RequiresDecryption) DownloadUrlResolver.DecryptDownloadUrl(video);

            ple.Title = video.Title;
            ple.FileName = video.DownloadUrl;
            return ple;
        }

        private static VideoInfo[] GetInfos(YoutubeItem item)
        {
            DownloadUrlResolver.ProxyConfiguration = GetProxyConfig();
            var videoinfos = DownloadUrlResolver.GetDownloadUrls("https://www.youtube.com/watch?v=" + item.VideoId);
            return videoinfos.ToArray();
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

        /// <summary>
        /// Searches a video and returns found items
        /// </summary>
        /// <param name="s">Video to search for</param>
        /// <returns>results</returns>
        public static Task<YoutubeItem[]> Search(string s)
        {
            return Task.Run(() => SearchFunction(s));
        }

        /// <summary>
        /// Creates a playlist entry from the selected video
        /// </summary>
        /// <param name="item">Selected youtue item</param>
        /// <returns>A playlist entry</returns>
        public static Task<PlayListEntry> FromYoutubeItem(YoutubeItem item)
        {
            return Task.Run(() => FromYoutubeItemFunction(item));
        }

        /// <summary>
        /// Return video infos for download
        /// </summary>
        /// <param name="item">Selected item</param>
        /// <returns>Video infos</returns>
        public static Task<VideoInfo[]> Infos(YoutubeItem item)
        {
            return Task.Run(() => GetInfos(item));
        }
    }
}
