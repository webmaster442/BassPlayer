using BassPlayer.Classes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using YouTube_Downloader;

namespace BassPlayer.Controls
{
    /// <summary>
    /// Interaction logic for ImportYoutube.xaml
    /// </summary>
    public partial class ImportYoutube : Window
    {
        public ImportYoutube()
        {
            InitializeComponent();
        }

        private Task<PlayListEntry[]> GetEntrys()
        {
            return Task<PlayListEntry[]>.Run(() =>
            {
                string[] lines = TbUrls.Text.Split('\n');
                List<PlayListEntry> ret = new List<PlayListEntry>(lines.Length);

                foreach (var line in lines)
                {
                    var quality = YouTubeDownloader.GetYouTubeVideoUrls(line);
                    var good = (from i in quality where i.Extention == "mp4" && i.Dimension.Width == 640 select i).FirstOrDefault();
                    PlayListEntry ple = new PlayListEntry();
                    ple.Title = good.VideoTitle;
                    ple.FileName = good.DownloadUrl;
                    ple.Time = good.Length;
                    ret.Add(ple);
                }
                return ret.ToArray();
            });
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            PlayListEntry[] urls = await GetEntrys();
        }
    }
}
