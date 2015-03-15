using FFConverter.Properties;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FFConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PresetManager _presets;
        private Preset _currentpreset;
        private readonly string[] _titles;
        private readonly string[] _descriptions;
        private ObservableCollection<string> _filelist;
        private bool _loaded;


        public MainWindow()
        {
            InitializeComponent();
            _presets = new PresetManager();
            LbPresets.ItemsSource = _presets;
            LbPresets.SelectedIndex = 0;
            _currentpreset = _presets[0];
            _filelist = new ObservableCollection<string>();
            _titles = new string[] { "Presets", "Input Files", "Preset Options", "Output Options", "Run" };
            _descriptions = new string[] 
            {
                "Welcome to FFConverter. Select a conversion preset from the list. FFConverter uses the GPL ffmpeg converter.\r\nYou can download a windows build of FFMPEG by clicking on the FFMPEG logo",
                "Please select the files that you wish to convert.",
                "Here you can tweak the selected preset options, if the preset has options.",
                "Here you can specify the output folder and override the output file extension.\r\nWARNING! Overriding the output extension may cause conversion & playback problems",
                "You can save the job as a CMD file, that can be runed later, or you can run the conversion process now."
            };
            LbFiles.ItemsSource = _filelist;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _loaded = true;
            TbPageDescription.Text = _descriptions[TcPages.SelectedIndex];
            TbPageHeader.Text = _titles[TcPages.SelectedIndex];
            TbFFMpeg.Text = Settings.Default.FFmpegPath;
            var files = Environment.GetCommandLineArgs();
            for (int i = 1; i < files.Length; i++)
            {
                _filelist.Add(files[i]);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Settings.Default.FFmpegPath = TbFFMpeg.Text;
            Settings.Default.Save();
            e.Cancel = false;
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
            if (!File.Exists(Settings.Default.FFmpegPath))
            {
                var question =MessageBox.Show("FFMPEG can't be found at the specified path. The Created cmd file will do nothing.\r\nDo you want to continue?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (question == MessageBoxResult.No) return;
            }
            System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
            sfd.Filter = "Cmd files | *.cmd";
            sfd.FilterIndex = 0;
            _currentpreset.Extension = TbExtension.Text;
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _currentpreset.CommandLine = PresetCompiler.CompileUiToString(_currentpreset, SpOptions);
                BatCompiler.CreateBatFile(_currentpreset, _filelist.ToArray(), sfd.FileName, TbOutputFolder.Text);
                MessageBox.Show("CMD file created succesfully.", "Infrormation", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnRun_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(Settings.Default.FFmpegPath))
            {
                MessageBox.Show("FFMPEG can't be found. Please set FFMPEG path fist at the output options!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Dispatcher.Invoke(() => { TcPages.SelectedIndex = 3; });
                return;
            }
            string filename = Path.GetTempFileName() + ".cmd";
            _currentpreset.Extension = TbExtension.Text;
            _currentpreset.CommandLine = PresetCompiler.CompileUiToString(_currentpreset, SpOptions);
            BatCompiler.CreateBatFile(_currentpreset, _filelist.ToArray(), filename, TbOutputFolder.Text);
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.Arguments = string.Format("/c \"{0}\"", filename);
            p.StartInfo.UseShellExecute = false;
            p.Start();
        }

        private void Image_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Process.Start("http://ffmpeg.zeranoe.com/builds/");
        }

        private void BtnFFBrowse_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "ffmpeg.exe | ffmpeg.exe";
            ofd.FilterIndex = 0;
            ofd.FileName = TbFFMpeg.Text;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TbFFMpeg.Text = ofd.FileName;
                Settings.Default.FFmpegPath = TbFFMpeg.Text;
            }
        }

        private void AddFiles_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach (var file in ofd.FileNames)
                {
                    _filelist.Add(file);
                }
            }
        }

        private void RemSelected_Click(object sender, RoutedEventArgs e)
        {
            if (LbFiles.SelectedIndex > -1)
            {
                while (LbFiles.SelectedItems.Count > 0)
                {
                    _filelist.Remove(LbFiles.SelectedItems[0].ToString());
                }
            }
        }

        private void RemAll_Click(object sender, RoutedEventArgs e)
        {
            _filelist.Clear();
        }


    }
}
