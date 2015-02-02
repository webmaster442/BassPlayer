using BassPlayer.Classes;
using System;
using System.Windows;
using System.Windows.Controls;

namespace BassPlayer.Controls
{
    /// <summary>
    /// Interaction logic for OSD.xaml
    /// </summary>
    public partial class OSD : UserControl
    {
        private bool _showremain;
        private bool _loaded;

        public OSD()
        {
            InitializeComponent();
            _showremain = false;
            _loaded = false;
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

    }
}
