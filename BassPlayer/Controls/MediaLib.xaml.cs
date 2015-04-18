using BassEngine;
using BassPlayer.SongSources;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
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
        private ObservableCollection<PlayListEntry> _list;

        public event RoutedEventHandler MediaLibSongLoad;

        /// <summary>
        /// Gets the current Selected PlaylistEntry
        /// </summary>
        public PlayListEntry SelectedItem { get; set; }

        public MediaLib()
        {
            InitializeComponent();
            _db = new TrackDb();
            _list = new ObservableCollection<PlayListEntry>();
            LbMediaLib.ItemsSource = _list;
            RefreshTree();
        }

        /// <summary>
        /// Refreshes tree
        /// </summary>
        private void RefreshTree()
        {
            MediaTree.ResetNodes();
            MediaTree.AddNode(MediaLibTree.Categories.Albums, _db.Albums);
            MediaTree.AddNode(MediaLibTree.Categories.Artists, _db.Artists);
            MediaTree.AddNode(MediaLibTree.Categories.Genres, _db.Genres);
            MediaTree.AddNode(MediaLibTree.Categories.Years, _db.Years);
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
            RefreshTree();
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
            RefreshTree();
        }

        private void MediaRemove_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (LbMediaLib.SelectedItems.Count > 0)
            {
                List<PlayListEntry> selection = new List<PlayListEntry>(LbMediaLib.SelectedItems.Count);

                while (LbMediaLib.SelectedItems.Count > 0)
                {
                    var j = (PlayListEntry)LbMediaLib.SelectedItems[0];
                    selection.Add(j);
                    _list.Remove(j);
                }
                _db.DeleteItems(selection);
                RefreshTree();
            }
        }

        private void MediaBackupLib_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _db.DoBackup();
        }

        private void MediaRestoreLib_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _db.DoResore();
            RefreshTree();
        }

        public void Save()
        {
            _db.Save();
        }

        private void MediaTree_ListAllClick(object sender, System.Windows.RoutedEventArgs e)
        {
            _list.Clear();
            _list.AddRange(_db.Query(TrackDb.QueryType.All, null));
        }

        private void MediaTree_FilterClick(object sender, System.Windows.RoutedEventArgs e)
        {
            _list.Clear();
            _list.AddRange(_db.Query(TrackDb.QueryType.Search, MediaTree.FilterString));
        }

        private void MediaTree_ItemClick(object sender, System.Windows.RoutedEventArgs e)
        {
            _list.Clear();
            var s = ((TreeViewItem)sender).Tag.ToString().Split('/');
            PlayListEntry[] result = null;
            if (s.Length > 1)
            {
                switch (s[0])
                {
                    case "Genres":
                        result = _db.Query(TrackDb.QueryType.Genre, s[1]);
                        break;
                    case "Albums":
                        result = _db.Query(TrackDb.QueryType.Album, s[1]);
                        break;
                    case "Artists":
                        result = _db.Query(TrackDb.QueryType.Artist, s[1]);
                        break;
                    case "Years":
                        result = _db.Query(TrackDb.QueryType.Year, s[1]);
                        break;
                }
            }
            _list.AddRange(result);
        }

        private void LbMediaLib_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (MediaLibSongLoad != null)
            {
                if (LbMediaLib.SelectedItem != null)
                {
                    SelectedItem = _list[LbMediaLib.SelectedIndex];
                    MediaLibSongLoad(this, null);
                }
            }
        }

        public void NextTrack()
        {
            int index = LbMediaLib.SelectedIndex;
            if ((index + 1) <= (LbMediaLib.Items.Count - 1))
            {
                index += 1;
                LbMediaLib.SelectedIndex = index;
                SelectedItem = _list[LbMediaLib.SelectedIndex];
            }
        }

        public void PreviousTrack()
        {
            int index = LbMediaLib.SelectedIndex;
            if ((index - 1) >= 0)
            {
                index -= 1;
                LbMediaLib.SelectedIndex = index;
                SelectedItem = _list[LbMediaLib.SelectedIndex];
            }
        }
    }
}
