using BassEngine;
using BassPlayer.SongSources;
using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BassPlayer.Controls
{
    /// <summary>
    /// Interaction logic for MiniPlayer.xaml
    /// </summary>
    public partial class MiniPlayer : Window
    {
        private bool _showremain;
        private bool _loaded;

        public MiniPlayer()
        {
            InitializeComponent();
        }

        private void BtnFullView_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            Application.Current.MainWindow.Visibility = System.Windows.Visibility.Visible;
            Application.Current.MainWindow.Activate();
        }

        public event EventHandler PlayClick;
        public event EventHandler StopClick;
        public event EventHandler NextClick;
        public event EventHandler PreviousClick;
        public event EventHandler MuteClick;

        private void HandleEvent(object sender, RoutedEventArgs e)
        {
            ButtonBase s = (ButtonBase)sender;
            switch (s.Name)
            {
                case "BtnPrevious":
                    if (PreviousClick != null) PreviousClick(sender, null);
                    break;
                case "BtnNext":
                    if (NextClick != null) NextClick(sender, null);
                    break;
                case "BtnStrop":
                    if (StopClick != null) StopClick(sender, null);
                    break;
                case "BtnMute":
                    if (MuteClick != null) MuteClick(sender, null);
                    break;
                case "BtnPlayPause":
                    if (PlayClick != null) PlayClick(sender, null);
                    if ((bool)BtnPlayPause.IsChecked)
                    {
                        ImgPlayPause.Source = new BitmapImage(new Uri("pack://application:,,,/BassPlayer;component/Images/play-100.png"));
                    }
                    else ImgPlayPause.Source = new BitmapImage(new Uri("pack://application:,,,/BassPlayer;component/Images/pause-100.png"));
                    break;
            }
        }

        public static DependencyProperty TagsProperty = DependencyProperty.Register("Tags", typeof(PlayListEntry), typeof(MiniPlayer));
        public static DependencyProperty SongLengthProperty = DependencyProperty.Register("SongLength", typeof(double), typeof(MiniPlayer), new PropertyMetadata(0.0d));
        public static DependencyProperty SongPositionProperty = DependencyProperty.Register("SongPosition", typeof(double), typeof(MiniPlayer), new PropertyMetadata(0.0d));

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

        public void SetCover(ImageSource src)
        {
            CoverImage.Source = src;
        }

        private void MiniWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _loaded = true;
        }

        private void Grid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
