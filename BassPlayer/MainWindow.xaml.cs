using BassEngine;
using BassPlayer.Classes;
using BassPlayer.Controls;
using BassPlayer.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using MahApps.Metro.Controls;

namespace BassPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow, IDisposable
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

        private void SetupWindowSize()
        {
            double left, top, width, height;
            left = Settings.Default.WindowLeft;
            top = Settings.Default.WindowTop;
            width = Settings.Default.WindowWidth;
            height = Settings.Default.WindowHeight;

            if (left < SystemParameters.VirtualScreenWidth && left >= 0) this.Left = left;
            if (top < SystemParameters.VirtualScreenHeight && top >= 0) this.Top = top;
            if (width < SystemParameters.VirtualScreenWidth && width > 30) this.Width = width;
            if (height < SystemParameters.VirtualScreenHeight && height > 30) this.Height = height;
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (IntPtr.Size == 4) this.Title += " | x86";
            else this.Title += " | x64";

            SetupWindowSize();
            App.MiniPlayer = new MiniPlayer();
            App.MiniPlayer.NextClick += ThumbNext_Click;
            App.MiniPlayer.PreviousClick += ThumbPrevious_Click;
            App.MiniPlayer.PlayClick += ThumbPlayPause_Click;
            App.MiniPlayer.StopClick += ThumbStop_Click;
            App.MiniPlayer.MuteClick += ThumbMute_Click;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Settings.Default.WindowLeft = this.Left;
            Settings.Default.WindowTop = this.Top;
            Settings.Default.WindowWidth = this.Width;
            Settings.Default.WindowHeight = this.Height;
            Settings.Default.Save();
            Playlist.SaveRecent();
            e.Cancel = false;
            App.Current.Shutdown();
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

        protected virtual void Dispose(bool native)
        {
            if (_keyboardhook != null)
            {
                _keyboardhook.Dispose();
                _keyboardhook = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}