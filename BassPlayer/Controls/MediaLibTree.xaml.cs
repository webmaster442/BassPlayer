using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace BassPlayer.Controls
{
    /// <summary>
    /// Interaction logic for MediaLibTree.xaml
    /// </summary>
    public partial class MediaLibTree : UserControl
    {
        public MediaLibTree()
        {
            InitializeComponent();
        }

        public event RoutedEventHandler ListAllClick;
        
        public event RoutedEventHandler FilterClick;

        public event RoutedEventHandler ItemClick;

        public static DependencyProperty FilterStringProperty = DependencyProperty.Register("FilterString", typeof(string), typeof(MediaLibTree));

        public static DependencyProperty AlbumsVisibleProperty = DependencyProperty.Register("AlbumsVisible", typeof(Visibility), typeof(MediaLibTree), new PropertyMetadata(Visibility.Visible));

        public static DependencyProperty CompilationsVisibleProperty = DependencyProperty.Register("CompilationsVisible", typeof(Visibility), typeof(MediaLibTree), new PropertyMetadata(Visibility.Visible));
        
        public static DependencyProperty ArtistsVisibleProperty = DependencyProperty.Register("ArtistsVisible", typeof(Visibility), typeof(MediaLibTree), new PropertyMetadata(Visibility.Visible));

        public static DependencyProperty GenresVisibleProperty = DependencyProperty.Register("GenresVisible", typeof(Visibility), typeof(MediaLibTree), new PropertyMetadata(Visibility.Visible));
        
        public static DependencyProperty PodcastsVisibleProperty = DependencyProperty.Register("PodcastsVisible", typeof(Visibility), typeof(MediaLibTree), new PropertyMetadata(Visibility.Visible));
       
        public static DependencyProperty PlaylistsVisibleProperty = DependencyProperty.Register("PlaylistsVisible", typeof(Visibility), typeof(MediaLibTree), new PropertyMetadata(Visibility.Visible));
        
        public static DependencyProperty SongsVisibleProperty = DependencyProperty.Register("SongsVisible", typeof(Visibility), typeof(MediaLibTree), new PropertyMetadata(Visibility.Visible));

        public enum Categories
        {
            Albums, Compilations, Artists, Genres, Podcasts, Playlists
        }
        
        public string FilterString
        {
            get { return (string)GetValue(FilterStringProperty); }
            set { SetValue(FilterStringProperty, value); }
        }

        public Visibility AlbumsVisible
        {
            get { return (Visibility)GetValue(AlbumsVisibleProperty); }
            set { SetValue(AlbumsVisibleProperty, value); }
        }

        public Visibility CompilationsVisible
        {
            get { return (Visibility)GetValue(CompilationsVisibleProperty); }
            set { SetValue(CompilationsVisibleProperty, value); }
        }

        public Visibility ArtistsVisible
        {
            get { return (Visibility)GetValue(ArtistsVisibleProperty); }
            set { SetValue(ArtistsVisibleProperty, value); }
        }

        public Visibility GenresVisible
        {
            get { return (Visibility)GetValue(GenresVisibleProperty); }
            set { SetValue(GenresVisibleProperty, value); }
        }

        public Visibility PodcastsVisible
        {
            get { return (Visibility)GetValue(PodcastsVisibleProperty); }
            set { SetValue(PodcastsVisibleProperty, value); }
        }

        public Visibility PlaylistsVisible
        {
            get { return (Visibility)GetValue(PlaylistsVisibleProperty); }
            set { SetValue(PlaylistsVisibleProperty, value); }
        }

        public Visibility SongsVisible
        {
            get { return (Visibility)GetValue(SongsVisibleProperty); }
            set { SetValue(SongsVisibleProperty, value); }
        }

        public void AddNode(Categories category, IEnumerable<string> items)
        {
            ItemCollection target = null;
            string tag = null;
            switch (category)
            {
                case Categories.Albums:
                    target = NodeAlbums.Items;
                    tag = "Albums/";
                    break;
                case Categories.Artists:
                    target = NodeArtists.Items;
                    tag = "Artists/";
                    break;
                case Categories.Compilations:
                    target = NodeCompilations.Items;
                    tag = "Compilations/";
                    break;
                case Categories.Genres:
                    target = NodeGenres.Items;
                    tag = "Genres/";
                    break;
                case Categories.Playlists:
                    target = NodePlaylists.Items;
                    tag = "Playlists/";
                    break;
                case Categories.Podcasts:
                    target = NodePodcasts.Items;
                    tag = "Podcasts/";
                    break;
            }
            if (items == null) return;
            foreach (var item in items)
            {
                TreeViewItem ti = new TreeViewItem();
                ti.Header = item;
                ti.Tag = tag + item;
                target.Add(ti);
            }
        }

        private void BtnFilter_Click(object sender, RoutedEventArgs e)
        {
            if (FilterClick != null) FilterClick(sender, e);
        }

        private void BtnListAll_Click(object sender, RoutedEventArgs e)
        {
            if (ListAllClick != null) ListAllClick(sender, e);
        }

        private void MediaTree_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ItemClick == null) return;
            if (MediaTree.SelectedItem == null) return;
            ItemClick(sender, new RoutedEventArgs());
        }
    }
}
