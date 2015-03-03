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

namespace BassConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string[] _files;
        private PresetManager _presets;

        public MainWindow()
        {
            InitializeComponent();
            _files = Environment.GetCommandLineArgs();
            #if RELEASE
            if (_files.Length < 2)
            {
                MessageBox.Show("No files selected.\r\nThis Program can be used from BassPlayer.\r\nApplicaton Will now Exit", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                this.Close();
            }
            #endif
            this.Title = string.Format("Bass Audio Converter - {0} files", _files.Length - 1);
            _presets = new PresetManager();
            LbPresets.ItemsSource = _presets;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TcPages.SelectedIndex == 1)
            {
                var preset = _presets[LbPresets.SelectedIndex];
                PresetCompiler.Compile(preset, SpOptions);
            }
        }
    }
}
