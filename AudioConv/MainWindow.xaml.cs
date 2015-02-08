using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace AudioConv
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _supported;
        private ObservableCollection<string> _files;

        public MainWindow()
        {
            InitializeComponent();
            _supported = "*.wav;*.mp3;*.mp4;*.m4a;*.m4b;*.flac;*.wv;*.wma;*.ogg;*.ac3";
            _files = new ObservableCollection<string>();
            LbInputFiles.ItemsSource = _files;
        }

        #region Menu

        private void FileExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void InputAddDir_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fd = new System.Windows.Forms.FolderBrowserDialog();
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string[] filters = _supported.Split(';');
                List<string> files = new List<string>(30);
                foreach (var filter in filters)
                {
                    files.AddRange(Directory.GetFiles(fd.SelectedPath, filter));
                }
                files.Sort();
                foreach (var file in files) _files.Add(file);
            }

        }

        private void InputAddFiles_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = "Audio Files | " + _supported;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach (var file in ofd.FileNames)
                {
                    _files.Add(file);
                }
            }
        }

        #endregion

        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fd = new System.Windows.Forms.FolderBrowserDialog();
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TbOutDir.Text = fd.SelectedPath;
            }
        }
    }
}
