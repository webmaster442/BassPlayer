using BassPlayer.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;


namespace BassPlayer.Controls
{
    /// <summary>
    /// Interaction logic for PlayList.xaml
    /// </summary>
    public partial class PlayList : UserControl
    {
        private ObservableCollection<PlayListEntry> _playlist;
        private readonly string _supportedformats;

        public Player AudioPlayerControls { get; set; }

        public PlayList()
        {
            InitializeComponent();
            _supportedformats = "*.mp3;*.mp4;*.m4a;*.m4b;*.aac;*.flac;*.ac3;*.wv";
            _playlist = new ObservableCollection<PlayListEntry>();
            LbList.ItemsSource = _playlist;
        }

        private void LbList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (AudioPlayerControls != null)
            {
                var index = LbList.SelectedIndex;
                AudioPlayerControls.Load(_playlist[index].File);
            }
        }

        #region Public Functions
        public void SetCoverImage(ImageSource src)
        {
            CoverImage.Source = src;
        }
        #endregion

        #region Load / Add menu
        private void MenAddFiles_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "Audio Files | " + _supportedformats;
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
            string[] filters = _supportedformats.Split(';');
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.Description = "Select folder to be added";
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                List<string> Files = new List<string>(30);
                foreach (var filter in filters)
                {
                    Files.AddRange(Directory.GetFiles(fbd.SelectedPath, filter));
                }
                Files.Sort();
                foreach (var f in Files)
                {
                    _playlist.Add(PlayListEntry.FromFile(f));
                }
            }
        }

        private void MenLoadPlaylist_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "Playlist Files | *.m3u;*pls;*.txt";
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach (var file in ofd.FileNames)
                {
                    string filedir = System.IO.Path.GetDirectoryName(file);
                    string extenssion = System.IO.Path.GetExtension(file);
                    string line;
                    using (var content = File.OpenText(file))
                    {
                        switch (extenssion)
                        {
                            case ".txt":
                            case ".m3u":
                                do
                                {
                                    line = content.ReadLine();
                                    if (line == null) continue;
                                    if (line.StartsWith("#")) continue;
                                    if (line.StartsWith("http://"))
                                    {
                                        _playlist.Add(PlayListEntry.FromFile(line));
                                    }
                                    else if (line.Contains(":\\") || line.StartsWith("\\\\"))
                                    {
                                        if (!File.Exists(line)) continue;
                                        _playlist.Add(PlayListEntry.FromFile(line));
                                    }
                                    else
                                    {
                                        string f = System.IO.Path.Combine(filedir, line);
                                        if (!File.Exists(f)) continue;
                                        _playlist.Add(PlayListEntry.FromFile(f));
                                    }
                                }
                                while (line != null);
                                break;
                            default:
                                MessageBox.Show("Sorry, PLS is not yet supported");
                                break;
                        }
                    }
                }
            }
        }
        #endregion

        #region Sort menu
        private void MenSortArtistTitle_Click(object sender, RoutedEventArgs e)
        {
            var query = (from i in _playlist orderby i.ArtistTitle ascending select i).ToList();
            _playlist.Clear();
            _playlist.AddRange(query);
        }

        private void MenSortArtist_Click(object sender, RoutedEventArgs e)
        {
            var query = (from i in _playlist orderby i.Artist ascending select i).ToList();
            _playlist.Clear();
            _playlist.AddRange(query);
        }

        private void MenSortTitle_Click(object sender, RoutedEventArgs e)
        {
            var query = (from i in _playlist orderby i.Title ascending select i).ToList();
            _playlist.Clear();
            _playlist.AddRange(query);
        }

        private void MenSortLength_Click(object sender, RoutedEventArgs e)
        {
            var query = (from i in _playlist orderby i.Time ascending select i).ToList();
            _playlist.Clear();
            _playlist.AddRange(query);
        }

        private void MenSortFileName_Click(object sender, RoutedEventArgs e)
        {
            var query = (from i in _playlist orderby i.File ascending select i).ToList();
            _playlist.Clear();
            _playlist.AddRange(query);
        }

        private void MenSortRandom_Click(object sender, RoutedEventArgs e)
        {
            var query = (from i in _playlist orderby Guid.NewGuid() select i).ToList();
            _playlist.Clear();
            _playlist.AddRange(query);
        }

        private void MenSortReverse_Click(object sender, RoutedEventArgs e)
        {
            var query = _playlist.Reverse().ToList();
            _playlist.Clear();
            _playlist.AddRange(query);
        }
        #endregion
    }
}
