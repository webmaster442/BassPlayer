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
        private bool _loaded;
        private DispatcherTimer _timer;
        private float _vol;

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
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            _loaded = false;
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(500);
            _timer.IsEnabled = false;
            _timer.Tick += _timer_Tick;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            var devs = App.Engine.GetDevices();
            foreach (var dev in devs)
            {
                CbDeviceList.Items.Add(dev);
            }
            VolSlider.Value = App.Engine.Volume;
            _loaded = true;
        }

        private void CbDeviceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string devicename = CbDeviceList.SelectedItem.ToString();
            App.Engine.ChangeDevice(devicename);
            VolSlider.Value = App.Engine.Volume;
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            var elen = App.Engine.Length;
            var epos = App.Engine.Position;
            TimeSpan len = TimeSpan.FromSeconds(elen);
            TimeSpan pos = TimeSpan.FromSeconds(epos);
            TbPosition.Text = string.Format("{0} / {1}", pos.ToShortTime(), len.ToShortTime());
            TbArtistTitle.Text = App.Engine.Tags;
            if (App.Engine.MediaType == MediaType.Stream && elen == 0) return;
            SPosition.Value = epos;
            double progress = epos / elen;
            if (elen - epos < 1) PlayList.DoNextTrack();
            App.SetTaskbarProgress(progress);
        }

        public void Load(string file)
        {
            App.Engine.FileName = file;
            App.Engine.Play();
            if (App.Engine.MediaType == MediaType.Stream && App.Engine.Length == 0) App.PlayUndetTaskbar();
            else App.PlayTaskbar();
            SPosition.Maximum = App.Engine.Length;
            _timer.IsEnabled = (bool)!BtnPlayPause.IsChecked;
            PlayList.SetCoverImage(App.Engine.ImageTag);
        }

        private void BtnPlayPause_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)BtnPlayPause.IsChecked)
            {
                App.Engine.Pause();
                App.PauseTaskbar();
            }
            else
            {
                App.Engine.Play();
                App.PlayTaskbar();
            }
            _timer.IsEnabled = (bool)!BtnPlayPause.IsChecked;
        }

        private void BtnStrop_Click(object sender, RoutedEventArgs e)
        {
            App.Engine.Stop();
            BtnPlayPause.IsChecked = false;
        }

        private void SPosition_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            _timer.IsEnabled = false;
            App.Engine.Position = SPosition.Value;
            SPosition.Value = App.Engine.Position;
            _timer.IsEnabled = true;
        }

        private void VolSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_loaded) return;
            App.Engine.Volume = (float)VolSlider.Value;
        }

        private void BtnMute_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)BtnMute.IsChecked)
            {
                _vol = App.Engine.Volume;
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

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
