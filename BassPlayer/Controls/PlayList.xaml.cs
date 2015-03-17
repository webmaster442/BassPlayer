using BassEngine;
using BassPlayer.Classes;
using BassPlayer.SongSources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using System.Xml.Serialization;


namespace BassPlayer.Controls
{
    /// <summary>
    /// Interaction logic for PlayList.xaml
    /// </summary>
    public partial class PlayList : UserControl
    {
        private ObservableCollection<PlayListEntry> _playlist;
        private ObservableCollection<PlayListEntry> _tunes;
        private ObservableCollection<string> _files;
        private ObservableCollection<YoutubeItem> _youtube;
        private RecentPlays _recent;
        private TreeViewItem dummyNode = null;
        private int _index;
        private Random _rgen;
        private iTunesData _itunes;


        public Player AudioPlayerControls { get; set; }

        public bool Repeat { get; set; }
        public bool Shuffle { get; set; }

        public PlayList()
        {
            InitializeComponent();

            _playlist = new ObservableCollection<PlayListEntry>();
            _tunes = new ObservableCollection<PlayListEntry>();
            _files = new ObservableCollection<string>();
            _youtube = new ObservableCollection<YoutubeItem>();
            _recent = new RecentPlays();
            _rgen = new Random();
            _itunes = new iTunesData();

            LbList.ItemsSource = _playlist;
            LbFiles.ItemsSource = _files;
            LbLib.ItemsSource = _tunes;
            LbRecent.ItemsSource = _recent;
            LbYoutube.ItemsSource = _youtube;

            ListItunesData(SpArtists, _itunes.Artists, "Artists");
            ListItunesData(SpAlbums, _itunes.Albums, "Albums");
            ListItunesData(SpCompilations, _itunes.Compilations, "Compilations");
            ListItunesData(SpGenres, _itunes.Genres, "Genres");
            ListItunesData(SpPodcasts, _itunes.Podcasts, "Podcasts");
            ListItunesData(SpPlaylists, _itunes.Playlists, "Playlists");
            TabTunes.IsEnabled = _itunes.isLoaded;
        }

        #region Private Functions
        private Task LoadM3u(string file)
        {
            return Task.Run(() =>
            {
                try
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
                catch (Exception ex) { Helpers.ErrorDialog(ex, "File Load error"); }
            });
        }

        private Task LoadPls(string file)
        {
            return Task.Run(() =>
            {
                try
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
                catch (Exception ex) { Helpers.ErrorDialog(ex, "File Load error"); }
            });
        }

        private Task LoadBPL(string file)
        {
            return Task.Run(() =>
            {
                try
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
                catch (Exception ex) { Helpers.ErrorDialog(ex, "File Load error"); }
            });
        }

        private Task LoadWPL(string file)
        {
            return Task.Run(() =>
            {
                try
                {
                    var doc = XDocument.Load(file).Descendants("body").Elements("seq").Elements("media");
                    foreach (var media in doc)
                    {
                        var src = media.Attribute("src").Value;
                        PlayListEntry entry = PlayListEntry.FromFile(src);
                        _playlist.Add(entry);
                    }
                }
                catch (Exception ex) { Helpers.ErrorDialog(ex, "File Load error"); }
            });
        }

        private void LbList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (AudioPlayerControls != null)
            {
                var index = LbList.SelectedIndex;
                if (index < 0) return;
                _index = index;
                AudioPlayerControls.Load(_playlist[index]);
                _recent.Add(_playlist[index]);
            }
        }

        private void LbLib_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (AudioPlayerControls != null)
            {
                var index = LbLib.SelectedIndex;
                if (index < 0) return;
                _index = index;
                AudioPlayerControls.Load(_tunes[index]);
                _recent.Add(PlayListEntry.FromFile(_tunes[index].FileName));
            }
        }

        private void LbFiles_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (AudioPlayerControls != null)
            {
                var index = LbFiles.SelectedIndex;
                if (index < 0) return;
                AudioPlayerControls.Load(_files[index]);
                _recent.Add(PlayListEntry.FromFile(_files[index]));
            }
        }

        private async void LbYoutube_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (AudioPlayerControls != null)
            {
                var index = LbYoutube.SelectedIndex;
                YtProgress.Visibility = System.Windows.Visibility.Visible;
                PlayListEntry entry = await YoutubeLoader.FromYoutubeItem(_youtube[index]);
                _playlist.Add(entry);
                YtProgress.Visibility = System.Windows.Visibility.Collapsed;
                await Dispatcher.BeginInvoke((Action)(() => TcView.SelectedIndex = 0));
            }
        }

        private void LbRecent_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (AudioPlayerControls != null)
            {
                var index = LbRecent.SelectedIndex;
                AudioPlayerControls.Load(_recent[index]);
                _recent.UpdateItemAtIndex(index);
            }
        }

        #endregion

        #region Public Functions

        public void DoNextTrack()
        {
            var next = 0;
            if (TcView.SelectedIndex == 0)
            {
                //Playlist
                if (_playlist.Count < 1) return;
                if (Repeat) next = _index;
                else if (Shuffle) next = _rgen.Next(0, _playlist.Count);
                else next = _index + 1;
                if (next > _playlist.Count - 1) return;
                AudioPlayerControls.Load(_playlist[next]);
                _index = next;
            }
            else if (TcView.SelectedIndex == 1)
            {
                //Recent
                if (_recent.Count < 1) return;
                if (Repeat) next = LbRecent.SelectedIndex;
                else if (Shuffle) next = _rgen.Next(0, _recent.Count);
                else next = LbRecent.SelectedIndex + 1;
                if (next > _recent.Count - 1) return;
                AudioPlayerControls.Load(_recent[next]);
                LbRecent.SelectedIndex = next;
            }
            else if (TcView.SelectedIndex == 2)
            {
                //File Manager
                if (_files.Count < 1) return;
                if (Repeat) next = LbFiles.SelectedIndex;
                else if (Shuffle) next = _rgen.Next(0, _files.Count);
                else next = LbFiles.SelectedIndex + 1;
                if (next > _files.Count - 1) return;
                AudioPlayerControls.Load(_files[next]);
                LbFiles.SelectedIndex = next;
                _recent.UpdateItemAtIndex(next);
            }
            else if (TcView.SelectedIndex == 3)
            {
                //iTunes
                if (_tunes.Count < 1) return;
                if (Repeat) next = LbLib.SelectedIndex;
                else if (Shuffle) next = _rgen.Next(0, _tunes.Count);
                else next = LbLib.SelectedIndex + 1;
                if (next > _tunes.Count - 1) return;
                AudioPlayerControls.Load(_tunes[next]);
                LbLib.SelectedIndex = next;
            }
        }

        public void DoPreviousTrack()
        {
            var previous = 0;
            if (TcView.SelectedIndex == 0)
            {
                //Playlist
                if (_playlist.Count < 1) return;
                if (Repeat) previous = _index;
                else if (Shuffle) previous = _rgen.Next(0, _playlist.Count);
                else previous = _index - 1;
                if (previous < 0) return;
                AudioPlayerControls.Load(_playlist[previous]);
                _index = previous;
            }
            else if (TcView.SelectedIndex == 1)
            {
                //Recent
                if (_recent.Count < 1) return;
                if (Repeat) previous = LbRecent.SelectedIndex;
                else if (Shuffle) previous = _rgen.Next(0, _recent.Count);
                else previous = LbRecent.SelectedIndex - 1;
                if (previous < 0) return;
                AudioPlayerControls.Load(_recent[previous]);
                LbRecent.SelectedIndex = previous;
                _recent.UpdateItemAtIndex(previous);
            }
            else if (TcView.SelectedIndex == 2)
            {
                //File Manager
                if (_files.Count < 1) return;
                if (Repeat) previous = LbFiles.SelectedIndex;
                else if (Shuffle) previous = _rgen.Next(0, _files.Count);
                else previous = LbFiles.SelectedIndex - 1;
                if (previous < 0) return;
                AudioPlayerControls.Load(_files[previous]);
                LbFiles.SelectedIndex = previous;
            }
            else if (TcView.SelectedIndex == 3)
            {
                //iTunes
                if (_tunes.Count < 1) return;
                if (Repeat) previous = LbLib.SelectedIndex;
                else if (Shuffle) previous = _rgen.Next(0, _tunes.Count);
                else previous = LbLib.SelectedIndex - 1;
                if (previous < 0) return;
                AudioPlayerControls.Load(_tunes[previous]);
                LbLib.SelectedIndex = previous;
            }
        }

        public async void AppendFiles(IEnumerable<string> Files)
        {
            Processing.Visibility = System.Windows.Visibility.Visible;
            List<PlayListEntry> data = await Task.Run(() =>
                {
                    List<PlayListEntry> processed = new List<PlayListEntry>(Files.Count());
                    foreach (var f in Files)
                    {
                        processed.Add(PlayListEntry.FromFile(f));
                    }
                    return processed;
                });
            _playlist.AddRange(data);
            Processing.Visibility = System.Windows.Visibility.Collapsed;
        }

        public async void AppendPlaylist(string file)
        {
            Processing.Visibility = System.Windows.Visibility.Visible;
            string extenssion = Path.GetExtension(file);
            switch (extenssion)
            {
                case ".m3u":
                case ".txt":
                    await LoadM3u(file);
                    break;
                case ".bpl":
                    await LoadBPL(file);
                    break;
                case ".pls":
                    await LoadPls(file);
                    break;
                case ".wpl":
                    await LoadWPL(file);
                    break;
            }
            Processing.Visibility = System.Windows.Visibility.Collapsed;
        }

        public void SaveRecent()
        {
            _recent.Save();
            MediaLib.Save();
        }

        #endregion

        #region Load / Add menu
        private async void MenAddFiles_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "Audio Files | " + App.Formats;
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Processing.Visibility = System.Windows.Visibility.Visible;
                string[] files = ofd.FileNames;
                List<PlayListEntry> items = new List<PlayListEntry>(files.Length);
                await Task.Run(() =>
                    {
                        foreach (var f in files)
                        {
                            items.Add(PlayListEntry.FromFile(f));
                        }
                    });
                _playlist.AddRange(items);
                Processing.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private async void MenAddFolder_Click(object sender, RoutedEventArgs e)
        {
            string[] filters = App.Formats.Split(';');
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.Description = "Select folder to be added";
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                List<PlayListEntry> _items = new List<PlayListEntry>();
                Processing.Visibility = System.Windows.Visibility.Visible;
                await Task.Run(() =>
                    {
                        List<string> Files = new List<string>(30);
                        foreach (var filter in filters)
                        {
                            Files.AddRange(Directory.GetFiles(fbd.SelectedPath, filter));
                        }
                        Files.Sort();
                        foreach (var f in Files)
                        {
                            _items.Add(PlayListEntry.FromFile(f));
                        }
                    });
                _playlist.AddRange(_items);
                Processing.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void MenLoadPlaylist_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "Playlist Files | *.m3u;*.bpl;*.txt;*.pls;*.wpl";
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach (var file in ofd.FileNames)
                {
                    AppendPlaylist(file);
                }
            }
        }

        private void MenLoadCD_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            MenLoadCD.Items.Clear();
            var cds = from cd in DriveInfo.GetDrives() where cd.DriveType == DriveType.CDRom && cd.IsReady select cd.Name;
            foreach (var cd in cds)
            {
                MenuItem drive = new MenuItem();
                drive.Header = cd;
                drive.Click += drive_Click;
                MenLoadCD.Items.Add(drive);
            }
            if (cds.Count() < 1)
            {
                MenuItem drive = new MenuItem();
                drive.Header = "No Discs found";
                MenLoadCD.Items.Add(drive);
            }
        }

        private void drive_Click(object sender, RoutedEventArgs e)
        {
            var drive = ((MenuItem)sender).Header.ToString();
            _playlist.AddRange(AudioEngine.GetCdInfo(drive));
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

        #region Command Bindigns
        private void SortArtistTitle_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (TcView.SelectedIndex == 0)
            {
                var query = (from i in _playlist orderby i.ArtistTitle ascending select i).ToList();
                _playlist.Clear();
                _playlist.AddRange(query);
            }
            else
            {
                var query = (from i in _recent orderby i.Title ascending select i).ToList();
                _recent.Clear();
                _recent.AddRange(query);
            }
        }

        private void SortLength_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (TcView.SelectedIndex == 0)
            {
                var query = (from i in _playlist orderby i.Time ascending select i).ToList();
                _playlist.Clear();
                _playlist.AddRange(query);
            }
            else
            {
                var query = (from i in _recent orderby i.Time ascending select i).ToList();
                _recent.Clear();
                _recent.AddRange(query);
            }
        }

        private void SortFileName_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (TcView.SelectedIndex == 0)
            {
                var query = (from i in _playlist orderby i.FileName ascending select i).ToList();
                _playlist.Clear();
                _playlist.AddRange(query);
            }
            else
            {
                var query = (from i in _recent orderby i.FilePath ascending select i).ToList();
                _recent.Clear();
                _recent.AddRange(query);
            }
        }

        private void SortRandom_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (TcView.SelectedIndex == 0)
            {
                var query = (from i in _playlist orderby Guid.NewGuid() select i).ToList();
                _playlist.Clear();
                _playlist.AddRange(query);
            }
            else
            {
                var query = (from i in _recent orderby Guid.NewGuid() select i).ToList();
                _recent.Clear();
                _recent.AddRange(query);
            }
        }

        private void SortReverse_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (TcView.SelectedIndex == 0)
            {
                var query = _playlist.Reverse().ToList();
                _playlist.Clear();
                _playlist.AddRange(query);
            }
            else
            {
                var query = _recent.Reverse().ToList();
                _recent.Clear();
                _recent.AddRange(query);
            }
        }

        private void ManageClear_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (TcView.SelectedIndex == 0) _playlist.Clear();
            else _recent.Clear();
        }

        private void ManageDelete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (LbList.SelectedItems.Count == 0) return;
            while (LbList.SelectedItems != null)
            {
                _playlist.Remove((PlayListEntry)LbList.SelectedItems[0]);
            }
        }

        private void SortArtist_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var query = (from i in _playlist orderby i.Artist ascending select i).ToList();
            _playlist.Clear();
            _playlist.AddRange(query);
        }

        private void SortTitle_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var query = (from i in _playlist orderby i.Title ascending select i).ToList();
            _playlist.Clear();
            _playlist.AddRange(query);
        }


        private void SortDate_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var query = (from i in _recent orderby i.LastPlayed descending select i).ToList();
            _recent.Clear();
            _recent.AddRange(query);
        }

        private void ContextRefresh_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (TvDirs.SelectedItem == null) return;
            _files.Clear();
            TreeViewItem selected = (TreeViewItem)TvDirs.SelectedItem;
            ListDir(selected.Tag.ToString());
        }

        private void ContextAddPlaylist_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            PlayListEntry[] Items = null;
            if (TcView.SelectedIndex == 1)
            {
                Items = (from RecentItem i in LbRecent.SelectedItems select PlayListEntry.FromFile(i.FilePath)).ToArray();
            }
            else if (TcView.SelectedIndex == 2)
            {
                Items = (from string i in LbFiles.SelectedItems select PlayListEntry.FromFile(i)).ToArray();
            }
            else if (TcView.SelectedIndex == 3)
            {
                Items = (from PlayListEntry i in LbLib.SelectedItems select i).ToArray();
            }
            _playlist.AddRange(Items);
            Dispatcher.BeginInvoke((Action)(() => TcView.SelectedIndex = 0));

        }

        private void ContextCopyToDevice_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string[] files = null;
            if (TcView.SelectedIndex == 0) files = (from PlayListEntry i in LbFiles.SelectedItems select i.FileName).ToArray();
            else if (TcView.SelectedIndex == 1) files = (from RecentItem i in LbRecent.SelectedItems select i.FilePath).ToArray();
            else if (TcView.SelectedIndex == 2) files = (from string i in LbFiles.SelectedItems select i).ToArray();
            else if (TcView.SelectedIndex == 3) files = (from PlayListEntry i in LbLib.SelectedItems select i.FileName).ToArray();

            Process p = new Process();
            p.StartInfo.FileName = "BassDeviceCopy.exe";
            p.StartInfo.Arguments = Helpers.Arguments(files);
            p.StartInfo.UseShellExecute = false;
            p.Start();
        }

        private void ContextConvert_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string[] files = null;
            if (TcView.SelectedIndex == 0) files = (from PlayListEntry i in LbFiles.SelectedItems select i.FileName).ToArray();
            else if (TcView.SelectedIndex == 1) files = (from RecentItem i in LbRecent.SelectedItems select i.FilePath).ToArray();
            else if (TcView.SelectedIndex == 2) files = (from string i in LbFiles.SelectedItems select i).ToArray();
            else if (TcView.SelectedIndex == 3) files = (from PlayListEntry i in LbLib.SelectedItems select i.FileName).ToArray();

            Process p = new Process();
            p.StartInfo.FileName = "FFConverter.exe";
            p.StartInfo.Arguments = Helpers.Arguments(files);
            p.StartInfo.UseShellExecute = false;
            p.Start();
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
                                    var line = entry.FileName.Replace(targetdir + "\\", "");
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
        #endregion

        #region File Explorer

        private Button CreateButton(string texts, string tag, BitmapImage image)
        {
            Button button = new Button();
            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;
            Image icon = new Image();
            icon.Width = 16;
            icon.Height = 16;
            icon.Source = image;
            sp.Children.Add(icon);
            TextBlock text = new TextBlock();
            text.Margin = new Thickness(2, 0, 2, 0);
            text.Text = texts;
            sp.Children.Add(text);
            button.Content = sp;
            button.ToolTip = tag;
            button.Margin = new Thickness(2.5, 0, 2.5, 0);
            button.Click += button_Click;
            return button;
        }

        private void BuildDriveList()
        {
            var drives = DriveInfo.GetDrives();
            SpDriveList.Children.Clear();
            foreach (var drive in drives)
            {
                if (!drive.IsReady) continue;
                BitmapImage icon = null;
                switch (drive.DriveType)
                {
                    case DriveType.CDRom:
                        icon = new BitmapImage(new Uri("pack://application:,,,/BassPlayer;component/Images/filemanager/cd-50.png"));
                        break;
                    case DriveType.Fixed:
                    case DriveType.Ram:
                    case DriveType.NoRootDirectory:
                    case DriveType.Unknown:
                        icon = new BitmapImage(new Uri("pack://application:,,,/BassPlayer;component/Images/filemanager/hdd-50.png"));
                        break;
                    case DriveType.Network:
                        icon = new BitmapImage(new Uri("pack://application:,,,/BassPlayer;component/Images/filemanager/cloud_storage-50.png"));
                        break;
                    case DriveType.Removable:
                        icon = new BitmapImage(new Uri("pack://application:,,,/BassPlayer;component/Images/filemanager/usb_logo-50.png"));
                        break;
                }
                Button button = CreateButton(drive.Name, drive.Name, icon);
                SpDriveList.Children.Add(button);
            }

            foreach (var cloud in CloudDriveProvider.AvailableDrives)
            {
                Button button = CreateButton(cloud.ToString(), CloudDriveProvider.GetPath(cloud), CloudDriveProvider.GetIcon(cloud));
                SpDriveList.Children.Add(button);
            }
        }

        private void ListDir(string path)
        {
            _files.Clear();
            List<string> files = new List<string>();
            foreach (var filter in App.Formats.Split(';'))
            {
                files.AddRange(Directory.GetFiles(path, filter));
            }
            files.Sort();
            foreach (var file in files)
            {
                FileInfo fi = new FileInfo(file);
                if ((fi.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden) continue;
                _files.Add(fi.FullName);
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            string drive = (sender as Button).ToolTip.ToString();
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
            ListDir(drive);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            BuildDriveList();
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            BuildDriveList();
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
            ListDir(selected.Tag.ToString());
        }
        #endregion

        #region Bass Menu
        private void MenBassSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow sw = new SettingsWindow();
            sw.ShowDialog();
        }

        private void MenBassAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutDialog ab = new AboutDialog();
            ab.ShowDialog();
        }

        private void MenBassExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        #endregion

        #region iTunes

        private void ListItunesData(StackPanel target, string[] items, string linkcat)
        {
            if (items == null || target == null) return;
            foreach (var item in items)
            {
                Button b = new Button();
                b.Content = item;
                b.Margin = new Thickness(30, 2, 5, 2);
                b.Click += b_Click;
                b.ToolTip = string.Format("{0}/{1}", linkcat, item);
                target.Children.Add(b);
            }
        }

        private void b_Click(object sender, RoutedEventArgs e)
        {
            var s = ((Button)sender).ToolTip.ToString();
            _tunes.Clear();
            var result = _itunes.Filter(s);
            _tunes.AddRange(result);
        }

        private void BtnListAll_Click(object sender, RoutedEventArgs e)
        {
            _tunes.Clear();
            var result = _itunes.Filter("Songs/Songs");
            _tunes.AddRange(result);
        }

        private void BtnFilter_Click(object sender, RoutedEventArgs e)
        {
            _tunes.Clear();
            var result = _itunes.Search(TbFilter.Text);
            _tunes.AddRange(result);
        }
        #endregion

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            YtProgress.Visibility = System.Windows.Visibility.Visible;
            try
            {
                var results = await YoutubeLoader.Search(TbYoutubeQuery.Text);
                _youtube.Clear();
                _youtube.AddRange(results);
            }
            catch (Exception ex)
            {
                Helpers.ErrorDialog(ex, "Youtube Query failed");
            }
            YtProgress.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void TbYoutubeQuery_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Button_Click_1(null, null);
                e.Handled = true;
            }
        }
    }
}
