using BassPlayer.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Windows.Shell;
using BassEngine;
using BassPlayer.SongSources;

namespace BassPlayer.Controls
{
    /// <summary>
    /// Interaction logic for Player.xaml
    /// </summary>
    public partial class Player : UserControl, IDisposable
    {
        private bool _loaded;
        private DispatcherTimer _timer;
        private float _vol;
        private string[] _filters, _lists;
        private bool _isDraging;

        private static DependencyProperty AllwaysTopProperty = DependencyProperty.Register("AllwaysTop", typeof(bool?), typeof(Player), new PropertyMetadata(false));

        public bool? AllwaysTop
        {
            get { return (bool?)GetValue(AllwaysTopProperty); }
            set { SetValue(AllwaysTopProperty, value); }
        }

        public PlayList PlayList { get; set; }

        public Player()
        {
            InitializeComponent();
            _isDraging = false;
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            _loaded = false;
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(500);
            _timer.IsEnabled = false;
            _timer.Tick += _timer_Tick;
            _filters = App.Formats.Replace("*", "").Split(';');
            _lists = App.Playlists.Replace("*", "").Split(';');
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            var devs = App.Engine.GetDevices();
            foreach (var dev in devs)
            {
                CbDeviceList.Items.Add(dev);
            }
            VolSlider.Value = App.Engine.ChannelVolume;
            if (VolSlider.Value == 0) VolSlider.Value = App.Engine.MasterVolume;
            _loaded = true;
            ProcessArguments();
        }

        public void ProcessArguments(IEnumerable<string> args = null)
        {
            if (args == null) args = Environment.GetCommandLineArgs();
            List<string> ok = new List<string>();
            foreach (var file in args)
            {
                var extension = Path.GetExtension(file);

                if (_filters.Contains(extension))
                {
                    ok.Add(file);
                }
                else if (_lists.Contains(extension)) PlayList.AppendPlaylist(file);
            }
            if (ok.Count > 0)
            {
                PlayList.AppendFiles(ok);
            }
            App.Current.MainWindow.Activate();
        }

        private void CbDeviceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string devicename = CbDeviceList.SelectedItem.ToString();
            App.Engine.ChangeDevice(devicename);
            VolSlider.Value = App.Engine.MasterVolume;
        }

        public void Load(PlayListEntry entry)
        {
            try
            {
                App.Engine.SetPlayListEntry(entry);
                App.Engine.Play();
                if (App.Engine.MediaType == MediaType.Stream && App.Engine.Length == 0) App.PlayUndetTaskbar();
                else App.PlayTaskbar();
                SPosition.Maximum = App.Engine.Length;
                _timer.IsEnabled = (bool)!BtnPlayPause.IsChecked;
                CoverArt.Source = App.Engine.ImageTag;
                CoverArtLarge.Source = App.Engine.ImageTag;
                App.MiniPlayer.SetCover(App.Engine.ImageTag);
                BtnPlayPause.IsChecked = false;
                Chapterize();
            }
            catch (Exception ex) { Helpers.ErrorDialog(ex, "File Load error"); }
        }

        public void Load(string file)
        {
            try
            {
                App.Engine.SetFileName(file);
                App.Engine.Play();
                if (App.Engine.MediaType == MediaType.Stream && App.Engine.Length == 0) App.PlayUndetTaskbar();
                else App.PlayTaskbar();
                SPosition.Maximum = App.Engine.Length;
                _timer.IsEnabled = (bool)!BtnPlayPause.IsChecked;
                CoverArt.Source = App.Engine.ImageTag;
                CoverArtLarge.Source = App.Engine.ImageTag;
                App.MiniPlayer.SetCover(App.Engine.ImageTag);
                BtnPlayPause.IsChecked = false;
                Chapterize();
            }
            catch (Exception ex) { Helpers.ErrorDialog(ex, "File Load error"); }
        }

        private void Chapterize()
        {
            CmChapters.Items.Clear();
            int cuts = 5;
            if (App.Engine.Length >= 600) cuts = 10;
            else if (App.Engine.Length >= 900) cuts = 15;
            double cutlen = App.Engine.Length / cuts;

            if (cutlen > 300)
            {
                cutlen = 300;
                cuts = (int)(App.Engine.Length / 300);
            }

            for (int i = 1; i < cuts; i++)
            {
                MenuItem men = new MenuItem();
                men.Header = TimeSpan.FromSeconds(i * cutlen).ToShortTime();
                men.ToolTip = i * cutlen;
                men.Click += men_Click;
                CmChapters.Items.Add(men);
            }

        }

        private void men_Click(object sender, RoutedEventArgs e)
        {
            MenuItem s = (MenuItem)sender;
            double time = (double)s.ToolTip;
            _timer.IsEnabled = false;
            SPosition.Value = time;
            App.Engine.Position = time;
            _timer.IsEnabled = true;
        }

        public enum ThumbCommands
        {
            PlayPause,
            Stop,
            Next,
            Previous,
            MuteUnMute
        }

        public void DoThumbCommand(ThumbCommands command)
        {
            switch (command)
            {
                case ThumbCommands.Next:
                    BtnNext_Click(null, null);
                    break;
                case ThumbCommands.Stop:
                    BtnStrop_Click(null, null);
                    break;
                case ThumbCommands.Previous:
                    BtnPrevious_Click(null, null);
                    break;
                case ThumbCommands.MuteUnMute:
                    BtnMute.IsChecked = !BtnMute.IsChecked;
                    BtnMute_Click(null, null);
                    break;
                case ThumbCommands.PlayPause:
                    BtnPlayPause.IsChecked = !BtnPlayPause.IsChecked;
                    BtnPlayPause_Click(null, null);
                    break;
            }
        }

        public BitmapImage GetPlayPauseIcon()
        {
            if ((bool)BtnPlayPause.IsChecked)
            {
                return new BitmapImage(new Uri("pack://application:,,,/BassPlayer;component/Images/play-100.png"));
            }
            return new BitmapImage(new Uri("pack://application:,,,/BassPlayer;component/Images/pause-100.png"));
        }

        private void BtnPlayPause_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)BtnPlayPause.IsChecked)
            {
                App.Engine.Pause();
                App.PauseTaskbar();
                BtnPlayPause.ToolTip = "Play";
            }
            else
            {
                App.Engine.Play();
                App.PlayTaskbar();
                BtnPlayPause.ToolTip = "Pause";
            }
            ImgPlayPause.Source = GetPlayPauseIcon();
            _timer.IsEnabled = (bool)!BtnPlayPause.IsChecked;
        }

        private void BtnStrop_Click(object sender, RoutedEventArgs e)
        {
            App.Engine.Stop();
            BtnPlayPause.IsChecked = false;
            _timer.IsEnabled = false;
            App.SetTaskbarProgress(0);
        }

        private void SPosition_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            App.Engine.Position = SPosition.Value;
            SPosition.Value = App.Engine.Position;
            _isDraging = false;
        }

        private void SPosition_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            _isDraging = true;
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            var elen = App.Engine.Length;
            var epos = App.Engine.Position;
            if (_isDraging)
            {
                OSD.SongPosition = SPosition.Value;
                return;
            }

            if (App.MiniPlayer.Visibility == System.Windows.Visibility.Visible)
            {
                App.MiniPlayer.SongPosition = epos;
                App.MiniPlayer.SongLength = elen;
                App.MiniPlayer.Tags = App.Engine.Tags;
            }
            else
            {
                OSD.SongPosition = epos;
                OSD.SongLength = elen;
                OSD.Tags = App.Engine.Tags;
            }

            if (App.Engine.MediaType == MediaType.Stream && elen == 0) return;
            SPosition.Value = epos;
            double progress = epos / elen;
            if (elen - epos < 1)
            {
                BtnStrop_Click(null, null);
                PlayList.DoNextTrack();
            }
            App.SetTaskbarProgress(progress);
        }

        private void VolSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_loaded) return;
            App.Engine.ChannelVolume = (float)VolSlider.Value;
        }

        private void BtnMute_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)BtnMute.IsChecked)
            {
                _vol = App.Engine.ChannelVolume;
                VolSlider.Value = 0;
            }
            else
            {
                VolSlider.Value = _vol;
            }
        }

        private void BtnPrevious_Click(object sender, RoutedEventArgs e)
        {
            PlayList.DoPreviousTrack();
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            PlayList.DoNextTrack();
        }

        private void BtnRepeat_Click(object sender, RoutedEventArgs e)
        {
            PlayList.Repeat = (bool)BtnRepeat.IsChecked;
        }

        private void BtnSuffle_Click(object sender, RoutedEventArgs e)
        {
            PlayList.Shuffle = (bool)BtnSuffle.IsChecked;
        }

        protected virtual void Dispose(bool disposing)
        {
            App.Engine.Dispose();
        }

        private void BtnMiniView_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Hide();
            App.MiniPlayer.Visibility = System.Windows.Visibility.Visible;
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
