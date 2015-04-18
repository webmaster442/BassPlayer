using BassEngine;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using YoutubeExtractor;

namespace BassPlayer.Controls
{
    /// <summary>
    /// Interaction logic for YoutubeDownload.xaml
    /// </summary>
    public partial class YoutubeDownload : Window
    {
        private string dloadpath;
        private VideoInfo[] _videos;
        private VideoDownloader _dloader;

        public YoutubeDownload(string VideoTitle)
        {
            InitializeComponent();
            TbTitle.Text = VideoTitle;
        }

        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            if (!string.IsNullOrEmpty(dloadpath)) fbd.SelectedPath = dloadpath;
            fbd.Description = "Select video target path";
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                BtnBrowse.Content = fbd.SelectedPath;
                dloadpath = fbd.SelectedPath;
            }
        }

        public VideoInfo[] VideoInfos
        {
            get { return _videos; }
            set
            {
                _videos = value;
                FormatCombo.ItemsSource = _videos;
            }
        }

        private async void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (FormatCombo.SelectedIndex < 0) return;
                if (string.IsNullOrEmpty(dloadpath))
                {
                    MessageBox.Show("Select download directory first!");
                    return;
                }
                var video = VideoInfos[FormatCombo.SelectedIndex];
                FormatCombo.IsEnabled = false;
                BtnBrowse.IsEnabled = false;
                BtnStart.IsEnabled = false;
                await Task.Run(() =>
                {
                    _dloader = new VideoDownloader(video, Path.Combine(dloadpath, video.Title + video.VideoExtension));
                    _dloader.DownloadProgressChanged += _dloader_DownloadProgressChanged;
                    _dloader.Execute();
                });
                MessageBox.Show("Download complete", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
            }
            catch (Exception ex)
            {
                Helpers.ErrorDialog(ex, "Download error");
            }
        }

        private void _dloader_DownloadProgressChanged(object sender, ProgressEventArgs e)
        {
            Dispatcher.Invoke(() => {
                PbProgress.Value = e.ProgressPercentage;
            });
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
