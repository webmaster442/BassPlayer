using BassPlayer.Classes;
using BassPlayer.Controls;
using Microsoft.Shell;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Un4seen.Bass;

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

        public const string Playlists = "*.pls;*.m3u;*.bpl;*.wpl";

        internal static AudioEngine Engine;

        internal static MiniPlayer MiniPlayer;
        
        [STAThread]
        public static void Main()
        {
            if (SingleInstance<App>.InitializeAsFirstInstance(Unique))
            {
                //api key
                BassNet.Registration("", "");

                var application = new App();
                Engine = new AudioEngine();
                application.InitializeComponent();
                application.ShutdownMode = ShutdownMode.OnMainWindowClose;
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
