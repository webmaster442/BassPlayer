using BassSpectrumDaemon.Classes;
using System;
using System.IO;
using System.Windows;

namespace BassSpectrumDaemon
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AudioSpectrum _spectrum;
        private System.Windows.Forms.NotifyIcon _trayicon;

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
        }

        private void BtnRedetect_Click(object sender, RoutedEventArgs e)
        {
            CbSerialPort.ItemsSource = null;
            CbSerialPort.ItemsSource = SerialPortProvider.Ports;
        }

        private void CbSerialOutput_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void CbMonitoring_Checked(object sender, RoutedEventArgs e)
        {
            if (CbMonitoring.IsChecked == true)
            {
                _spectrum.IsEnabled = true;
            }
            else
            {
                _spectrum.IsEnabled = false;
            }
        }

        private bool DoExit()
        {
            var q = MessageBox.Show("Exit application?", "Exit", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (q == MessageBoxResult.Yes) return true;
            return false;
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            if (DoExit()) this.Close();
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !DoExit();
        }
    }
}
