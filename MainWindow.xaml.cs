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
using System.Windows.Shapes;

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
            ProcessArguments();
        }

        public void ProcessArguments(IEnumerable<string> args = null)
        {
            if (args == null) args = Environment.GetCommandLineArgs();

            foreach (var file in args)
            {

            }

        }
    }
}
