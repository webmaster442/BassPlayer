using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
