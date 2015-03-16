using BassPlayer.SongSources;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;

namespace BassPlayer.Controls
{
    /// <summary>
    /// Interaction logic for MediaLib.xaml
    /// </summary>
    public partial class MediaLib : UserControl
    {
        private TrackDb _db;
        private AlbumArtStorage _albumarts;

        public MediaLib()
        {
            InitializeComponent();
            _db = new TrackDb();
            _albumarts = new AlbumArtStorage();
        }

        private async void MediaAddFiles_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "Audio Files | " + App.Formats;
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ProcessProgress.Visibility = System.Windows.Visibility.Visible;
                await _db.ProcessFiles(ofd.FileNames);
                _db.Save();
                ProcessProgress.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private async void MediaAddFolder_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string[] filters = App.Formats.Split(';');
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.Description = "Select folder to be added";
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ProcessProgress.Visibility = System.Windows.Visibility.Visible;
                List<string> Files = new List<string>(30);
                foreach (var filter in filters)
                {
                    Files.AddRange(Directory.GetFiles(fbd.SelectedPath, filter));
                }
                Files.Sort();
                await _db.ProcessFiles(Files);
                _db.Save();
                ProcessProgress.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void MediaRemove_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void MediaBackupLib_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }
    }
}
