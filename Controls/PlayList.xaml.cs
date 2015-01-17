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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using BassPlayer.Classes;


namespace BassPlayer.Controls
{
    /// <summary>
    /// Interaction logic for PlayList.xaml
    /// </summary>
    public partial class PlayList : UserControl
    {
        private ObservableCollection<PlayListEntry> _playlist;

        public Player AudioPlayerControls { get; set; }

        public PlayList()
        {
            InitializeComponent();
            _playlist = new ObservableCollection<PlayListEntry>();
            LbList.ItemsSource = _playlist;
        }

        public void SetCoverImage(ImageSource src)
        {
            CoverImage.Source = src;
        }

        private void MenAddFiles_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "Audio Files | *.mp3;*.mp4;*.m4a;*.m4b;*.aac;*.flac;*.ac3;*.wv";
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach (var f in ofd.FileNames)
                {
                    _playlist.Add(PlayListEntry.FromFile(f));
                }
            }
        }

        private void MenAddFolder_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LbList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (AudioPlayerControls != null)
            {
                var index = LbList.SelectedIndex;
                AudioPlayerControls.Load(_playlist[index].File);
            }
        }
    }
}
