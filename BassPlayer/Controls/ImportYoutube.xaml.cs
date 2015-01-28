using BassPlayer.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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

        private async Task<PlayListEntry[]> GetEntrys()
        {
            string[] lines = TbUrls.Text.Split('\n');
            List<PlayListEntry> ret = new List<PlayListEntry>(lines.Length);

            foreach (var line in lines)
            {
                var quality = YouTubeDownloader.GetYouTubeVideoUrls(line);
                var good = (from i in quality where i.Extention == "mp4" && i.Dimension.Width == 640 select i).FirstOrDefault();
                
            }

            return ret.ToArray();

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
