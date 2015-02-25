using BassEngine;
using BassPlayer.Classes;
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
        private KeyboardHook _keyboardhook;

        public MainWindow()
        {
            InitializeComponent();
            Playlist.AudioPlayerControls = Player;
            Player.PlayList = Playlist;
            _keyboardhook = new KeyboardHook();
            _keyboardhook.KeyPressed += _keyboardhook_KeyPressed;
            if (Settings.Default.MediaKeys)
            {
                try
                {
                    _keyboardhook.RegisterHotKey(ModifierKeys.None, System.Windows.Forms.Keys.MediaPlayPause);
                    _keyboardhook.RegisterHotKey(ModifierKeys.None, System.Windows.Forms.Keys.MediaStop);
                    _keyboardhook.RegisterHotKey(ModifierKeys.None, System.Windows.Forms.Keys.MediaNextTrack);
                    _keyboardhook.RegisterHotKey(ModifierKeys.None, System.Windows.Forms.Keys.MediaPreviousTrack);
                }
                catch (Exception ex)
                {
                    Helpers.ErrorDialog(ex, "Media keys are in use by another application.\r\nTo Use media key functions please close other apps that may use the keys, then restart the player");
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var size = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            if (Settings.Default.WindowLeft < size.Width && Settings.Default.WindowLeft >= 0)
            {
                this.Left = Settings.Default.WindowLeft;
            }
            if (Settings.Default.WindowTop < size.Height && Settings.Default.WindowTop >= 0)
            {
                this.Top = Settings.Default.WindowTop;
            }
            if (IntPtr.Size == 4) this.Title += " | x86";
            else this.Title += " | x64";
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Settings.Default.WindowLeft = this.Left;
            Settings.Default.WindowTop = this.Top;
            Settings.Default.Save();
            Playlist.SaveRecent();
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

        private void _keyboardhook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            switch (e.Key)
            {
                case System.Windows.Forms.Keys.MediaPreviousTrack:
                    Player.DoThumbCommand(Controls.Player.ThumbCommands.Previous);
                    break;
                case System.Windows.Forms.Keys.MediaNextTrack:
                    Player.DoThumbCommand(Controls.Player.ThumbCommands.Next);
                    break;
                case System.Windows.Forms.Keys.MediaPlayPause:
                    Player.DoThumbCommand(Controls.Player.ThumbCommands.PlayPause);
                    break;
                case System.Windows.Forms.Keys.MediaStop:
                    Player.DoThumbCommand(Controls.Player.ThumbCommands.Stop);
                    break;
            }
        }

        private void ThumbPrevious_Click(object sender, EventArgs e)
        {
            Player.DoThumbCommand(Controls.Player.ThumbCommands.Previous);
        }

        private void ThumbPlayPause_Click(object sender, EventArgs e)
        {
            Player.DoThumbCommand(Controls.Player.ThumbCommands.PlayPause);
            ThumbPlayPause.ImageSource = Player.GetPlayPauseIcon();
        }

        private void ThumbStop_Click(object sender, EventArgs e)
        {
            Player.DoThumbCommand(Controls.Player.ThumbCommands.Stop);
        }

        private void ThumbNext_Click(object sender, EventArgs e)
        {
            Player.DoThumbCommand(Controls.Player.ThumbCommands.Next);
        }

        private void ThumbMute_Click(object sender, EventArgs e)
        {
            Player.DoThumbCommand(Controls.Player.ThumbCommands.MuteUnMute);
        }
    }
}