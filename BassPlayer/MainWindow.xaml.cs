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
            if (IntPtr.Size == 4) this.Title += " | x86";
            else this.Title += " | x64";
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
                Player.ProcessArguments(files);
            }
        }
    }
}