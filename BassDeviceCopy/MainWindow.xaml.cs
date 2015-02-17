using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace BassDeviceCopy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string[] _files;
        private Progress<double> Indicator;
        private byte[] _buffer;
        private string _targetdir;
        private CancellationTokenSource _cts;
        private bool _error;

        public MainWindow()
        {
            InitializeComponent();
            _files = Environment.GetCommandLineArgs();
            if (_files.Length < 2)
            {
                MessageBox.Show("No files selected.\r\nThis Program can be used from BassPlayer.\r\nApplicaton Will now Exit", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                this.Close();
            }
            _buffer = new byte[131072]; //128KiB
            _error = false;
            Title = string.Format("Copy To Device - {0} files", _files.Length - 1);
            Indicator = new Progress<double>(ReportProgress);
        }

        private void ReportProgress(double obj)
        {
            PbCurrent.Value = obj;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (BtnStart.IsEnabled == false)
            {
                var q = MessageBox.Show("Copy in progress. Abort?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (q == MessageBoxResult.Yes)
                {
                    BtnCancel_Click(null, null);
                    e.Cancel = false;
                }
                else e.Cancel = true;
            }
        }

        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fb = new System.Windows.Forms.FolderBrowserDialog();
            fb.Description = "Select Copy target directory";
            fb.ShowNewFolderButton = true;
            if (fb.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TbFile.Text = fb.SelectedPath;
                _targetdir = fb.SelectedPath;
            }
        }

        private async void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            PbOveral.Maximum = _files.Length - 1;
            PbOveral.Value = 0;
            _cts = new CancellationTokenSource();
            BtnStart.IsEnabled = false;
            BtnBrowse.IsEnabled = false;
            await CopyFiles(_cts.Token, Indicator, (bool)RbOwerWrite.IsChecked);
            if (!_error)
            {
                MessageBox.Show("File copy complete", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            BtnStart.IsEnabled = true;
            BtnBrowse.IsEnabled = true;
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (_cts != null)
            {
                _cts.Cancel();
                MessageBox.Show("Copy Canceled", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
            else this.Close();
        }

        private Task CopyFiles(CancellationToken ct, IProgress<double> progress, bool owerwrite)
        {
            return Task.Run(() =>
                {
                    for (int i = 1; i < _files.Length; i++)
                    {
                        try
                        {
                            string targetname = Path.GetFileName(_files[i]);
                            string targetpath = Path.Combine(_targetdir, targetname);

                            if (File.Exists(targetpath) && !owerwrite) continue;

                            int read = 0;
                            using (var source = File.OpenRead(_files[i]))
                            {
                                using (var destination = File.Create(targetpath))
                                {
                                    do
                                    {
                                        read = source.Read(_buffer, 0, _buffer.Length);
                                        destination.Write(_buffer, 0, read);
                                        if (progress != null)
                                        {
                                            double percent = ((double)source.Position / (double)source.Length) * 100.00d;
                                            progress.Report(percent);
                                        }
                                        ct.ThrowIfCancellationRequested();
                                    }
                                    while (read > 0);
                                }
                            }
                            Dispatcher.Invoke(() =>
                            {
                                PbOveral.Value += 1;
                                TaskBar.ProgressValue = PbOveral.Value / (_files.Length - 1);
                            });
                        }
                        catch (Exception ex)
                        {
                            Dispatcher.Invoke(() =>
                            {
                                string message = string.Format("An Error occured:\r\n{0}", ex.Message);
                                MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                _error = true;
                            });
                        }
                    }
                }, ct);
        }
    }
}
