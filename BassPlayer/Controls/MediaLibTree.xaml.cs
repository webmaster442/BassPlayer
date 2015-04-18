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

        public static DependencyProperty YearsVisibleProperty = DependencyProperty.Register("YearsVisible", typeof(Visibility), typeof(MediaLibTree), new PropertyMetadata(Visibility.Visible));
        
        public static DependencyProperty SongsVisibleProperty = DependencyProperty.Register("SongsVisible", typeof(Visibility), typeof(MediaLibTree), new PropertyMetadata(Visibility.Visible));

        public enum Categories
        {
            Albums, Compilations, Artists, Genres, Podcasts, Playlists, Years
        }
        
        public string FilterString
        {
            get { return (string)GetValue(FilterStringProperty); }
            set { SetValue(FilterStringProperty, value); }
        }

        /// <summary>
        /// Gets or sets albums visibility
        /// </summary>
        public Visibility AlbumsVisible
        {
            get { return (Visibility)GetValue(AlbumsVisibleProperty); }
            set { SetValue(AlbumsVisibleProperty, value); }
        }

        /// <summary>
        /// Gets os sets compilations visibility
        /// </summary>
        public Visibility CompilationsVisible
        {
            get { return (Visibility)GetValue(CompilationsVisibleProperty); }
            set { SetValue(CompilationsVisibleProperty, value); }
        }

        /// <summary>
        /// Gets or sets artists visibility
        /// </summary>
        public Visibility ArtistsVisible
        {
            get { return (Visibility)GetValue(ArtistsVisibleProperty); }
            set { SetValue(ArtistsVisibleProperty, value); }
        }

        /// <summary>
        /// Gets or sets genres Visibility
        /// </summary>
        public Visibility GenresVisible
        {
            get { return (Visibility)GetValue(GenresVisibleProperty); }
            set { SetValue(GenresVisibleProperty, value); }
        }

        /// <summary>
        /// Gets or sets podcasts visibility
        /// </summary>
        public Visibility PodcastsVisible
        {
            get { return (Visibility)GetValue(PodcastsVisibleProperty); }
            set { SetValue(PodcastsVisibleProperty, value); }
        }

        /// <summary>
        /// Gets or sets playlists visibility
        /// </summary>
        public Visibility PlaylistsVisible
        {
            get { return (Visibility)GetValue(PlaylistsVisibleProperty); }
            set { SetValue(PlaylistsVisibleProperty, value); }
        }

        /// <summary>
        /// Gets or sets songs visibility
        /// </summary>
        public Visibility SongsVisible
        {
            get { return (Visibility)GetValue(SongsVisibleProperty); }
            set { SetValue(SongsVisibleProperty, value); }
        }

        /// <summary>
        /// Gets or sets years visibility
        /// </summary>
        public Visibility YearsVisible
        {
            get { return (Visibility)GetValue(YearsVisibleProperty); }
            set { SetValue(YearsVisibleProperty, value); }
        }

        /// <summary>
        /// Adds nodes to the specifed category
        /// </summary>
        /// <param name="category">The category to add to</param>
        /// <param name="items">The Items to add</param>
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
                case Categories.Years:
                    target = NodeYears.Items;
                    tag = "Years/";
                    break;
            }
            if (items == null) return;
            foreach (var item in items)
            {
                TreeViewItem ti = new TreeViewItem();
                ti.MouseLeftButtonUp += ti_MouseLeftButtonUp;
                ti.Header = item;
                ti.Tag = tag + item;
                target.Add(ti);
            }
        }

        private void ti_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ItemClick == null) return;
            if (MediaTree.SelectedItem == null) return;
            ItemClick(sender, new RoutedEventArgs());
        }

        /// <summary>
        /// Resets all category nodes to default state
        /// </summary>
        public void ResetNodes()
        {
            NodeAlbums.Items.Clear();
            NodeArtists.Items.Clear();
            NodeCompilations.Items.Clear();
            NodeGenres.Items.Clear();
            NodePlaylists.Items.Clear();
            NodePodcasts.Items.Clear();
            NodeYears.Items.Clear();
        }

        private void BtnFilter_Click(object sender, RoutedEventArgs e)
        {
            if (FilterClick != null) FilterClick(sender, e);
        }

        private void BtnListAll_Click(object sender, RoutedEventArgs e)
        {
            if (ListAllClick != null) ListAllClick(sender, e);
        }
    }
}
