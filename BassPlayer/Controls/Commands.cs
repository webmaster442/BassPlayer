using System.Windows.Input;

namespace BassPlayer.Controls
{
    /// <summary>
    /// Custom commands for command bindings
    /// </summary>
    internal static class CustomCommands
    {
        public static readonly RoutedUICommand SortArtistTitle = new RoutedUICommand("By Artist & Title", "SortArtistTitle", typeof(CustomCommands));

        public static readonly RoutedUICommand SortArtist = new RoutedUICommand("By Artist", "SortArtist", typeof(CustomCommands));

        public static readonly RoutedUICommand SortTitle = new RoutedUICommand("By Title", "SortTitle", typeof(CustomCommands));

        public static readonly RoutedUICommand SortDate = new RoutedUICommand("By Play Date", "SortDate", typeof(CustomCommands));

        public static readonly RoutedUICommand SortFileName = new RoutedUICommand("By File name", "SortFileName", typeof(CustomCommands));

        public static readonly RoutedUICommand SortRandom = new RoutedUICommand("Random", "SortRandom", typeof(CustomCommands));

        public static readonly RoutedUICommand SortReverse = new RoutedUICommand("Reverse", "SortReverse", typeof(CustomCommands));

        public static readonly RoutedUICommand ManageDelete = new RoutedUICommand("Delete Selected", "ManageDelete", typeof(CustomCommands));

        public static readonly RoutedUICommand ManageClear = new RoutedUICommand("Clear List", "ManageClear", typeof(CustomCommands));

        public static readonly RoutedUICommand ContextAddPlaylist = new RoutedUICommand("Add to Playlist", "ContextAddPlaylist", typeof(CustomCommands));

        public static readonly RoutedUICommand ContextCopyToDevice = new RoutedUICommand("Copy to Device...", "ContextCopyToDevice", typeof(CustomCommands));

        public static readonly RoutedUICommand ContextConvert = new RoutedUICommand("Convert files...", "ContextConvert", typeof(CustomCommands));

        public static readonly RoutedUICommand ContextRefresh = new RoutedUICommand("Refresh", "ContextRefresh", typeof(CustomCommands));

        public static readonly RoutedUICommand MediaAddFiles = new RoutedUICommand("Add Files...", "MediaAddFiles", typeof(CustomCommands));

        public static readonly RoutedUICommand MediaAddFolder = new RoutedUICommand("Add Folder...", "MediaAddFolder", typeof(CustomCommands));

        public static readonly RoutedUICommand MediaRemove = new RoutedUICommand("Remove Selected item...", "MediaRemove", typeof(CustomCommands));

        public static readonly RoutedUICommand MediaBackupLib = new RoutedUICommand("Backup database...", "MediaBackup", typeof(CustomCommands));
    }
}
