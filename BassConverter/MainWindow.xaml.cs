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
        private Preset _currentpreset;

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
            LbPresets.SelectedIndex = 0;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TcPages.SelectedIndex == 1)
            {
                _currentpreset = _presets[LbPresets.SelectedIndex];
                PresetCompiler.CompileToUi(_currentpreset, SpOptions);
            }
        }

        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TbOutputFolder.Text = fbd.SelectedPath;
            }
        }
    }
}
