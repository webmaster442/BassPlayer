using System;
using System.Windows;
using System.Windows.Controls;

namespace FFConverter
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
            this.Title = string.Format("Bass Audio Converter - {0} file(s)", _files.Length - 1);
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

        private void BtnSaveCmd_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
            sfd.Filter = "Cmd files | *.cmd";
            sfd.FilterIndex = 0;
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _currentpreset.CommandLine = PresetCompiler.CompileUiToString(_currentpreset, SpOptions);
                BatCompiler.CreateBatFile(_currentpreset, _files, sfd.FileName, TbOutputFolder.Text);
            }
        }
    }
}
