using System.Windows.Input;

namespace BassPlayer.Controls
{
    internal static class CustomCommands
    {
        public static readonly RoutedUICommand SortArtistTitle = new RoutedUICommand("By Artist & Title", "SortArtistTitle", typeof(CustomCommands));

        public static readonly RoutedUICommand SortArtist = new RoutedUICommand("By Artist", "SortArtist", typeof(CustomCommands));

        public static readonly RoutedUICommand SortTitle = new RoutedUICommand("By Title", "SortTitle", typeof(CustomCommands));

        public static readonly RoutedUICommand SortDate = new RoutedUICommand("By Play Date", "SortDate", typeof(CustomCommands));

        public static readonly RoutedUICommand SortLength = new RoutedUICommand("By Length", "SortLength", typeof(CustomCommands));

        public static readonly RoutedUICommand SortFileName = new RoutedUICommand("By File name", "SortFileName", typeof(CustomCommands));

        public static readonly RoutedUICommand SortRandom = new RoutedUICommand("Random", "SortRandom", typeof(CustomCommands));

        public static readonly RoutedUICommand SortReverse = new RoutedUICommand("Reverse", "SortReverse", typeof(CustomCommands));

        public static readonly RoutedUICommand ManageDelete = new RoutedUICommand("Delete Selected", "ManageDelete", typeof(CustomCommands));

        public static readonly RoutedUICommand ManageClear = new RoutedUICommand("Clear List", "ManageClear", typeof(CustomCommands));

        public static readonly RoutedUICommand ContextAddPlaylist = new RoutedUICommand("Add to Playlist", "ContextAddPlaylist", typeof(CustomCommands));

        public static readonly RoutedUICommand ContextCopyToDevice = new RoutedUICommand("Copy to Device...", "ContextCopyToDevice", typeof(CustomCommands));

        public static readonly RoutedUICommand ContextRefresh = new RoutedUICommand("Refresh", "ContextRefresh", typeof(CustomCommands));

        public static readonly RoutedUICommand MediaTest = new RoutedUICommand("Test", "MediaTest", typeof(CustomCommands));
    }
}
