using BassPlayer.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.IO;

namespace BassPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Playlist.AudioPlayerControls = Player;
            Player.PlayList = Playlist;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.WindowLeft >= 0) this.Left = Settings.Default.WindowLeft;
            if (Settings.Default.WindowTop >= 0) this.Top = Settings.Default.WindowTop;
            ProcessArguments();
            if (IntPtr.Size == 4) this.Title += " | x86";
            else this.Title += " | x64";
        }

        public void ProcessArguments(IEnumerable<string> args = null)
        {
            if (args == null) args = Environment.GetCommandLineArgs();
            string[] filters = App.Formats.Replace("*", "").Split(';');
            string[] lists = App.Playlists.Replace("*", "").Split(';');
            foreach (var file in args)
            {
                var extension = Path.GetExtension(file);
                if (filters.Contains(extension)) Playlist.AppendFile(file);
                else if (lists.Contains(extension)) Playlist.AppendPlaylist(file);
            }

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Settings.Default.WindowLeft = this.Left;
            Settings.Default.WindowTop = this.Top;
            Settings.Default.Save();
            e.Cancel = false;
        }
    }
}