using BassPlayer.SongSources;
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

        public MediaLib()
        {
            InitializeComponent();
            _db = new TrackDb();
        }

        private async void MediaAddFiles_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ProcessProgress.Visibility = System.Windows.Visibility.Visible;
                await _db.ProcessFiles(ofd.FileNames);
                _db.Save();
                ProcessProgress.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void MediaAddFolder_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void MediaRemove_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void MediaBackupLib_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }
    }
}
