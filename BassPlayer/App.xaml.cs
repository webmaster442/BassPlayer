﻿using BassPlayer.Classes;
using Microsoft.Shell;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BassPlayer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ISingleInstanceApp
    {
        public static void SetTaskbarProgress(double value)
        {
            MainWindow mw = (MainWindow)App.Current.MainWindow;
            mw.TaskbarItemInfo.ProgressValue = value;
        }

        public static void PauseTaskbar()
        {
            MainWindow mw = (MainWindow)App.Current.MainWindow;
            mw.TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Paused;
        }

        public static void PlayTaskbar()
        {
            MainWindow mw = (MainWindow)App.Current.MainWindow;
            mw.TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Normal;
        }

        public static void PlayUndetTaskbar()
        {
            MainWindow mw = (MainWindow)App.Current.MainWindow;
            mw.TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Indeterminate;
        }

        private const string Unique = "BassPlayer";

        public const string Formats = "*.mp3;*.mp4;*.m4a;*.m4b;*.aac;*.flac;*.ac3;*.wv;*.wav;*.wma;*.ogg";

        public const string Playlists = "*.pls;*.m3u;*.bpl";

        internal static AudioEngine Engine;
        
        [STAThread]
        public static void Main()
        {
            if (SingleInstance<App>.InitializeAsFirstInstance(Unique))
            {
                var application = new App();
                Engine = new AudioEngine();
                application.InitializeComponent();
                application.Run();
                // Allow single instance code to perform cleanup operations
                SingleInstance<App>.Cleanup();
            }
        }

        #region ISingleInstanceApp Members
        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            MainWindow mw = (MainWindow)App.Current.MainWindow;
            mw.Player.ProcessArguments(args);
            return true;
        }
        #endregion
    }
}
