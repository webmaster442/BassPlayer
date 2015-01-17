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
    public partial class App : Application
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
    }
}
