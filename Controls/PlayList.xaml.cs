using BassPlayer.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;


namespace BassPlayer.Controls
{
    /// <summary>
    /// Interaction logic for PlayList.xaml
    /// </summary>
    public partial class PlayList : UserControl
    {
        private ObservableCollection<PlayListEntry> _playlist;
        private ObservableCollection<string> _files;
        private readonly string _supportedformats;
        private TreeViewItem dummyNode = null;
        private int _index;
        private Random _rgen;


        public Player AudioPlayerControls { get; set; }

        public bool Repeat { get; set; }
        public bool Shuffle { get; set; }

        public PlayList()
        {
            InitializeComponent();
            _supportedformats = "*.mp3;*.mp4;*.m4a;*.m4b;*.aac;*.flac;*.ac3;*.wv;*.wav;*.wma;*.ogg";
            _playlist = new ObservableCollection<PlayListEntry>();
            _files = new ObservableCollection<string>();
            _rgen = new Random();
            LbList.ItemsSource = _playlist;
            LbFiles.ItemsSource = _files;
        }

        #region Private Functions
        private void LoadM3u(string file)
        {
            string filedir = System.IO.Path.GetDirectoryName(file);
            string line;
            using (var content = File.OpenText(file))
            {
                do
                {
                    line = content.ReadLine();
                    if (line == null) continue;
                    if (line.StartsWith("#")) continue;
                    if (line.StartsWith("http://") || line.StartsWith("https://"))
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
            }
        }

        private void LoadPls(string file)
        {
            string filedir = System.IO.Path.GetDirectoryName(file);
            string line;
            string pattern = @"^(File)([0-9])+(=)";
            using (var content = File.OpenText(file))
            {
                do
                {
                    line = content.ReadLine();
                    if (line == null) continue;
                    if (Regex.IsMatch(line, pattern)) line = Regex.Replace(line, pattern, "");
                    else continue;
                    if (line.StartsWith("http://") || line.StartsWith("https://"))
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
            }
        }

        private void LoadBPL(string file)
        {
            var targetdir = Path.GetDirectoryName(file);
            XmlSerializer xs = new XmlSerializer(typeof(PlayListEntry[]));
            using (var content = File.OpenRead(file))
            {
                var array = (PlayListEntry[])xs.Deserialize(content);
                foreach (var item in array)
                {
                    if (item.FileName.StartsWith("http://") || item.FileName.StartsWith("https://")) _playlist.Add(item);
                    else if (item.FileName.Contains(":\\") || item.FileName.StartsWith("\\\\"))
                    {
                        if (!File.Exists(item.FileName)) continue;
                        _playlist.Add(item);
                    }
                    else
                    {
                        var newitem = item;
                        string f = System.IO.Path.Combine(targetdir, item.FileName);
                        newitem.FileName = f;
                        _playlist.Add(newitem);
                    }
                }
            }
        }

        private void LbList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (AudioPlayerControls != null)
            {
                var index = LbList.SelectedIndex;
                AudioPlayerControls.Load(_playlist[index].FileName);
            }
        }

        private void LbFiles_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (AudioPlayerControls != null)
            {
                var index = LbFiles.SelectedIndex;
                _index = index;
                AudioPlayerControls.Load(_files[index]);
            }
        }

        #endregion

        #region Public Functions
        public void SetCoverImage(ImageSource src)
        {
            CoverImage.Source = src;
        }

        public void DoNextTrack()
        {
            var next = 0;
            if (TcView.SelectedIndex == 0)
            {
                if (Repeat) next = _index;
                else if (Shuffle) next = _rgen.Next(0, _playlist.Count);
                else next = _index + 1;
                if (next > _playlist.Count - 1) next = 0;
                AudioPlayerControls.Load(_playlist[next].FileName);
            }
            else
            {
                if (Repeat) next = LbFiles.SelectedIndex;
                else if (Shuffle) next = _rgen.Next(0, _files.Count);
                else next = LbFiles.SelectedIndex + 1;
                if (next > _files.Count - 1) next = 0;
                AudioPlayerControls.Load(_files[next]);
                LbFiles.SelectedIndex = next;
            }
        }

        public void DoPreviousTrack()
        {
            var previous = 0;
            if (TcView.SelectedIndex == 0)
            {
                if (Repeat) previous = _index;
                else if (Shuffle) previous = _rgen.Next(0, _playlist.Count);
                else previous = _index - 1;
                if (previous < 0) previous = _playlist.Count - 1;
                AudioPlayerControls.Load(_playlist[previous].FileName);
            }
            else
            {
                if (Repeat) previous = LbFiles.SelectedIndex;
                else if (Shuffle) previous = _rgen.Next(0, _files.Count);
                else previous = LbFiles.SelectedIndex - 1;
                if (previous < 0) previous = _files.Count - 1;
                AudioPlayerControls.Load(_files[previous]);
                LbFiles.SelectedIndex = previous;
            }
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
            ofd.Filter = "Playlist Files | *.m3u;*.bpl;*.txt;*.pls";
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach (var file in ofd.FileNames)
                {
                    string extenssion = System.IO.Path.GetExtension(file);
                    switch (extenssion)
                    {
                        case ".m3u":
                        case ".txt":
                            LoadM3u(file);
                            break;
                        case ".bpl":
                            LoadBPL(file);
                            break;
                        case ".pls":
                            LoadPls(file);
                            break;
                    }
                }
            }
        }

        private void MenAddUrl_Click(object sender, RoutedEventArgs e)
        {
            AddUrlDialog url = new AddUrlDialog();
            if (url.ShowDialog() == true)
            {
                _playlist.Add(PlayListEntry.FromFile(url.Url));
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
            var query = (from i in _playlist orderby i.FileName ascending select i).ToList();
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

        #region ManageMenu
        private void MenManageSave_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
            sfd.Filter = "BassPlayer List|*.bpl|M3U list|*.m3u";
            sfd.FilterIndex = 0;
            sfd.AddExtension = true;
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var extension = Path.GetExtension(sfd.FileName);
                var targetdir = Path.GetDirectoryName(sfd.FileName);
                switch (extension)
                {
                    case ".m3u":
                        using (var contents = File.CreateText(sfd.FileName))
                        {
                            foreach (var entry in _playlist)
                            {
                                var edir = Path.GetDirectoryName(entry.FileName);
                                if (edir.StartsWith(targetdir))
                                {
                                    var line = edir.Replace(targetdir + "\\", "");
                                    contents.WriteLine(line);
                                }
                                else contents.WriteLine(entry.FileName);
                            }
                        }
                        break;
                    case ".bpl":
                        XmlSerializer xs = new XmlSerializer(typeof(PlayListEntry[]));
                        using (var target = File.Create(sfd.FileName))
                        {
                            var array = _playlist.ToArray();
                            for (int i = 0; i < array.Length; i++)
                            {
                                var fdir = Path.GetDirectoryName(array[i].FileName);
                                if (fdir.StartsWith(targetdir))
                                {
                                    array[i].FileName = array[i].FileName.Replace(targetdir + "\\", "");
                                }
                            }
                            xs.Serialize(target, array);
                        }
                        break;
                }
            }
        }

        private void MenManageClear_Click(object sender, RoutedEventArgs e)
        {
            _playlist.Clear();
        }

        private void MenManageDelete_Click(object sender, RoutedEventArgs e)
        {
            if (LbList.SelectedIndex < 0) return;
            _playlist.RemoveAt(LbList.SelectedIndex);
        }
        #endregion

        #region File Explorer

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var drive in Directory.GetLogicalDrives())
            {
                CbDrives.Items.Add(drive);
            }
        }

        private void CbDrives_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string drive = CbDrives.SelectedItem.ToString();
            TvDirs.Items.Clear();
            string[] dirs = Directory.GetDirectories(drive);
            foreach (var dir in dirs)
            {
                DirectoryInfo di = new DirectoryInfo(dir);
                if ((di.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden) continue;
                TreeViewItem item = new TreeViewItem();
                item.Header = di.Name;
                item.Tag = dir;
                item.Items.Add(dummyNode);
                item.Expanded += item_Expanded;
                TvDirs.Items.Add(item);
            }
        }

        private void item_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;
            if (item.Items.Count == 1 && item.Items[0] == dummyNode)
            {
                item.Items.Clear();
                try
                {
                    foreach (string s in Directory.GetDirectories(item.Tag.ToString()))
                    {
                        TreeViewItem subitem = new TreeViewItem();
                        subitem.Header = s.Substring(s.LastIndexOf("\\") + 1);
                        subitem.Tag = s;
                        subitem.FontWeight = FontWeights.Normal;
                        subitem.Items.Add(dummyNode);
                        subitem.Expanded += item_Expanded;
                        item.Items.Add(subitem);
                    }
                }
                catch (Exception) { }
            }
        }

        private void TvDirs_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (TvDirs.SelectedItem == null) return;
            _files.Clear();
            TreeViewItem selected = (TreeViewItem)TvDirs.SelectedItem;
            List<string> files = new List<string>();
            foreach (var filter in _supportedformats.Split(';'))
            {
                files.AddRange(Directory.GetFiles(selected.Tag.ToString(), filter));
            }
            files.Sort();
            foreach (var file in files)
            {
                FileInfo fi = new FileInfo(file);
                if ((fi.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden) continue;
                _files.Add(fi.FullName);
            }
        }
        #endregion
    }
}
