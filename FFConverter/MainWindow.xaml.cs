using System;
using System.Diagnostics;
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
        private readonly string[] _titles;
        private readonly string[] _descriptions;
        private bool _loaded;


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
            _currentpreset = _presets[0];
            _titles = new string[] { "Presets", "Preset Options", "Output Options", "Run" };
            _descriptions = new string[] 
            {
                "Welcome to FFConverter. Select a conversion preset from the list. FFConverter uses the GPL ffmpeg converter.\r\nYou can download a windows build of FFMPEG by clicking on the FFMPEG logo",
                "Here you can tweak the selected preset options, if the preset has options.",
                "Here you can specify the output folder and override the output file extension.\r\nWARNING! Overriding the output extension may cause conversion & playback problems",
                "You can save the job as a CMD file, that can be runed later, or you can run the conversion process now."
            };
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_loaded) return;
            if (TcPages.SelectedIndex == 1)
            {
                _currentpreset = _presets[LbPresets.SelectedIndex];
                TbExtension.Text = _currentpreset.Extension;
                PresetCompiler.CompileToUi(_currentpreset, SpOptions);
            }
            TbPageDescription.Text = _descriptions[TcPages.SelectedIndex];
            TbPageHeader.Text = _titles[TcPages.SelectedIndex];
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
            _currentpreset.Extension = TbExtension.Text;
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _currentpreset.CommandLine = PresetCompiler.CompileUiToString(_currentpreset, SpOptions);
                BatCompiler.CreateBatFile(_currentpreset, _files, sfd.FileName, TbOutputFolder.Text);
                MessageBox.Show("CMD file created succesfully.", "Infrormation", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _loaded = true;
            TbPageDescription.Text = _descriptions[TcPages.SelectedIndex];
            TbPageHeader.Text = _titles[TcPages.SelectedIndex];
        }

        private void Image_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Process.Start("http://ffmpeg.zeranoe.com/builds/");
        }
    }
}
