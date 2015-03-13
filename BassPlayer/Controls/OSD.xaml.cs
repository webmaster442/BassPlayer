using BassPlayer.Classes;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Un4seen.Bass;
using BassEngine;
using BassPlayer.SongSources;

namespace BassPlayer.Controls
{
    /// <summary>
    /// Interaction logic for OSD.xaml
    /// </summary>
    public partial class OSD : UserControl
    {
        private bool _showremain;
        private bool _loaded;
        private DispatcherTimer _visualTimer;
        private Polyline _vline;

        public OSD()
        {
            InitializeComponent();
            _showremain = false;
            _loaded = false;
            _visualTimer = new DispatcherTimer();
            _visualTimer.Interval = TimeSpan.FromMilliseconds(40);
            _vline = new Polyline();
            _vline.Stroke = new SolidColorBrush(Colors.Black);
            _vline.StrokeThickness = 1;
            _vline.Stroke = SystemColors.HotTrackBrush;
            _vline.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            Visual.Children.Add(_vline);
            _visualTimer.Tick += _visualTimer_Tick;
        }

        private void _visualTimer_Tick(object sender, EventArgs e)
        {
            if (App.Engine.MixerHandle == 0) return;
            int length = (int)Bass.BASS_ChannelSeconds2Bytes(App.Engine.MixerHandle, 0.01);
            short[] data = new short[length / 2];
            length = Bass.BASS_ChannelGetData(App.Engine.MixerHandle, data, length);
            _vline.Points.Clear();
            double xscale = Visual.ActualWidth / data.Length;
            double yscale = (Visual.ActualHeight * -0.35) / (short.MaxValue);
            for (int i=1; i<data.Length; i+=2)
            {
                _vline.Points.Add(new Point(i * xscale, data[i] * yscale));
            }
        }

        public static DependencyProperty TagsProperty = DependencyProperty.Register("Tags", typeof(PlayListEntry), typeof(OSD));
        public static DependencyProperty SongLengthProperty = DependencyProperty.Register("SongLength", typeof(double), typeof(OSD), new PropertyMetadata(0.0d));
        public static DependencyProperty SongPositionProperty = DependencyProperty.Register("SongPosition", typeof(double), typeof(OSD), new PropertyMetadata(0.0d));

        public PlayListEntry Tags
        {
            get { return (PlayListEntry)GetValue(TagsProperty); }
            set 
            { 
                SetValue(TagsProperty, value);
                UpdateTitle();
            }
        }

        public double SongLength
        {
            get { return (double)GetValue(SongLengthProperty); }
            set
            {
                SetValue(SongLengthProperty, value);
                UpdateTime();
            }
        }

        public double SongPosition
        {
            get { return (double)GetValue(SongPositionProperty); }
            set
            {
                SetValue(SongPositionProperty, value);
                UpdateTime();
            }
        }

        private void UpdateTime()
        {
            if (!_loaded) return;
            TimeSpan l = TimeSpan.FromSeconds(SongLength);
            TimeSpan p = TimeSpan.FromSeconds(SongPosition);

            if (_showremain) TimeText.Text = string.Format("{0} / {1}", l.ToShortTime(), (l - p).ToShortTime());
            else TimeText.Text = string.Format("{0} / {1}", l.ToShortTime(), p.ToShortTime());
        }

        private void UpdateTitle()
        {
            if (!_loaded) return;
            TitleText.Text = Tags.ArtistTitle;
        }

        private void TimeText_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!_loaded) return;
            _showremain = !_showremain;
            UpdateTime();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _loaded = true;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _visualTimer.IsEnabled = (OSDTab.SelectedIndex == 1);
        }

    }
}
