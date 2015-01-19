using BassPlayer.Classes;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace BassPlayer.Controls
{
    /// <summary>
    /// Interaction logic for Player.xaml
    /// </summary>
    public partial class Player : UserControl, IDisposable
    {
        private AudioEngine _engine;
        private bool _loaded;
        private DispatcherTimer _timer;
        private float _vol;

        public PlayList PlayList { get; set; }

        public Player()
        {
            InitializeComponent();
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            _loaded = false;
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.IsEnabled = false;
            _timer.Tick += _timer_Tick;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            _engine = new AudioEngine();
            var devs = _engine.GetDevices();
            foreach (var dev in devs)
            {
                CbDeviceList.Items.Add(dev);
            }
            VolSlider.Value = _engine.Volume;
            _loaded = true;
        }

        private void CbDeviceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string devicename = CbDeviceList.SelectedItem.ToString();
            _engine.ChangeDevice(devicename);
            VolSlider.Value = _engine.Volume;
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            var elen = _engine.Length;
            var epos = _engine.Position;
            TimeSpan len = TimeSpan.FromSeconds(elen);
            TimeSpan pos = TimeSpan.FromSeconds(epos);
            TbPosition.Text = string.Format("{0} / {1}", pos.ToShortTime(), len.ToShortTime());
            TbArtistTitle.Text = _engine.Tags;
            if (_engine.MediaType == MediaType.Stream && elen == 0) return;
            SPosition.Value = epos;
            double progress = epos / elen;
            if (elen - epos < 1) PlayList.DoNextTrack();
            App.SetTaskbarProgress(progress);
        }

        public void Load(string file)
        {
            _engine.FileName = file;
            _engine.Play();
            if (_engine.MediaType == MediaType.Stream && _engine.Length == 0) App.PlayUndetTaskbar();
            else App.PlayTaskbar();
            SPosition.Maximum = _engine.Length;
            _timer.IsEnabled = (bool)!BtnPlayPause.IsChecked;
            PlayList.SetCoverImage(_engine.ImageTag);
        }

        private void BtnPlayPause_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)BtnPlayPause.IsChecked)
            {
                _engine.Pause();
                App.PauseTaskbar();
            }
            else
            {
                _engine.Play();
                App.PlayTaskbar();
            }
            _timer.IsEnabled = (bool)!BtnPlayPause.IsChecked;
        }

        private void BtnStrop_Click(object sender, RoutedEventArgs e)
        {
            _engine.Stop();
        }

        private void SPosition_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            _timer.IsEnabled = false;
            _engine.Position = SPosition.Value;
            SPosition.Value = _engine.Position;
            _timer.IsEnabled = true;
        }

        private void VolSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_loaded) return;
            _engine.Volume = (float)VolSlider.Value;
        }

        private void BtnMute_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)BtnMute.IsChecked)
            {
                _vol = _engine.Volume;
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
            _engine.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
