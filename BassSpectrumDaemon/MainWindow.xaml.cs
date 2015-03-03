using BassSpectrumDaemon.Classes;
using System;
using System.IO;
using System.Threading;
using System.Windows;

namespace BassSpectrumDaemon
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {
        private AudioSpectrum _spectrum;
        private System.Windows.Forms.NotifyIcon _trayicon;
        private bool _loaded;

        public MainWindow()
        {
            InitializeComponent();
            _spectrum = new AudioSpectrum(this.LelvelIdic);
            _trayicon = new System.Windows.Forms.NotifyIcon();
            Stream iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/BassSpectrumDaemon;component/icon.ico")).Stream;
            _trayicon.Icon = new System.Drawing.Icon(iconStream);
            _trayicon.Visible = true;
            _trayicon.MouseDoubleClick += _trayicon_MouseDoubleClick;
            _trayicon.Text = "BassPlayer Audio Spectrum Daemon";
            CbSerialPort.SelectedIndex = 0;
            CbAudioDevices.SelectedIndex = 0;
        }

        private void _trayicon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (this.Visibility == System.Windows.Visibility.Collapsed)
            {
                this.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                this.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CbAudioDevices.ItemsSource = _spectrum.Devices;
            CbSerialPort.ItemsSource = SerialPortProvider.Ports;
            _loaded = true;
        }

        private void BtnRedetect_Click(object sender, RoutedEventArgs e)
        {
            CbSerialPort.ItemsSource = null;
            CbSerialPort.ItemsSource = SerialPortProvider.Ports;
        }

        private void CbSerialOutput_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                _spectrum.Serial = SerialPortProvider.ConfigurePort(CbSerialPort.SelectedItem.ToString());
            }
            catch (Exception ex)
            {
                BassEngine.Helpers.ErrorDialog(ex, "Serial port error");
            }
        }

        private void CbSerialOutput_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_spectrum.Serial != null)
            {
                if (_spectrum.Serial.IsOpen) _spectrum.Serial.Close();
                _spectrum.Serial.Dispose();
                _spectrum.Serial = null;
            }
        }

        private void CbMonitoring_Checked(object sender, RoutedEventArgs e)
        {
            if (CbAudioDevices.SelectedItem == null)
            {
                MessageBox.Show("Please select device first");
                CbMonitoring.IsChecked = false;
                return;
            }
                _spectrum.DeviceName = CbAudioDevices.SelectedItem.ToString();
                _spectrum.IsEnabled = true;
        }

        private void CbMonitoring_Unchecked(object sender, RoutedEventArgs e)
        {
            _spectrum.IsEnabled = false;
            this.LelvelIdic.Level = 0;
        }

        private bool DoExit()
        {
            var q = MessageBox.Show("Exit application?", "Exit", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (q == MessageBoxResult.Yes) return true;
            return false;
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            if (DoExit())
            {
                Dispose();
                this.Close();
            }
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !DoExit();
            if (!e.Cancel) Dispose();
        }

        protected virtual void Dispose(bool native)
        {
            if (_spectrum != null)
            {
                _spectrum.Dispose();
                _spectrum = null;
            }
            if (_trayicon != null)
            {
                _trayicon.Dispose();
                _trayicon = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void ModeSelect(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            if (RbSpectrum.IsChecked == true) _spectrum.DisplayType = Messages.Spectrum;
            else if (RbLevels.IsChecked == true) _spectrum.DisplayType = Messages.Level;
        }

        private void BtnConfigDate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var port = SerialPortProvider.ConfigurePort(CbSerialPort.SelectedItem.ToString());
                port.Write(SerialPortProvider.TimePacket, 0, SerialPortProvider.TimePacket.Length);
                Thread.Sleep(1000);
                port.Close();
                port = null;
            }
            catch (Exception ex)
            {
                BassEngine.Helpers.ErrorDialog(ex, "Serial port error");
            }
        }
    }
}
