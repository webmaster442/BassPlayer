using BassPlayer.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace BassPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string[] _filters, _lists;

        public MainWindow()
        {
            InitializeComponent();
            Playlist.AudioPlayerControls = Player;
            Player.PlayList = Playlist;
            _filters = App.Formats.Replace("*", "").Split(';');
            _lists = App.Playlists.Replace("*", "").Split(';');
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
            foreach (var file in args)
            {
                var extension = Path.GetExtension(file);
                if (_filters.Contains(extension)) Playlist.AppendFile(file);
                else if (_lists.Contains(extension)) Playlist.AppendPlaylist(file);
            }

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Settings.Default.WindowLeft = this.Left;
            Settings.Default.WindowTop = this.Top;
            Settings.Default.Save();
            e.Cancel = false;
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                ProcessArguments(files);
            }
        }
    }
}