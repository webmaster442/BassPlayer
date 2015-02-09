using AudioConv.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace AudioConv
{
    /// <summary>
    /// Interaction logic for ProgressWindow.xaml
    /// </summary>
    public partial class ProgressWindow : Window
    {
        private OutputNameGenerator _namegen;
        private ObservableCollection<ProcessInfo> _processinfo;
        private Process[] _processes;
        private DispatcherTimer _timer;

        public ProgressWindow(bool usemulticpu, string pattern, DateTime time, int counter)
        {
            InitializeComponent();
            _namegen = new OutputNameGenerator(counter, pattern, time, usemulticpu);
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += _timer_Tick;
            _timer.IsEnabled = false;
            _processinfo = new ObservableCollection<ProcessInfo>();
            LBProcessStatus.ItemsSource = _processinfo;
        }

        private bool IsRunning(Process process)
        {
            try
            {
                return (process != null && process.HasExited == false);
            }
            catch { return false; }
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            bool terminate = true;
            for (int i = 0; i<_processes.Length; i++)
            {
                terminate = !IsRunning(_processes[i]);
                _processinfo[i].Running = terminate;
            }
            if (terminate)
            {
                _timer.IsEnabled = false;
                AbortProcesses();
                this.Close();
            }
        }

        private void TaskKill(int pid)
        {
            Process p = new Process();
            p.StartInfo.FileName = "taskkill.exe";
            p.StartInfo.Arguments = string.Format("/PID {0} /T /F", pid);
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.Start();
        }

        private void AbortProcesses()
        {
            if (_processes != null)
            {
                for (int i = 0; i < _processes.Length; i++)
                {
                    if (_processes[i] == null) continue;
                    try
                    {
                        TaskKill(_processes[i].Id);
                    }
                    catch (InvalidOperationException) { }
                    _processes[i] = null;
                }
            }
        }

        public void GenerateAndRunCmds(IEnumerable<string> files, IAudioConv converter, string outdir)
        {
            _namegen.Generate(files, outdir);
            int i = 0;
            string[] tempnames = new string[_namegen.Cores];
            StreamWriter[] tempfiles = new StreamWriter[_namegen.Cores];
            _processes = new Process[_namegen.Cores];

            for (i = 0; i < _namegen.Cores; i++)
            {
                tempnames[i] = System.IO.Path.GetTempFileName() + ".cmd";
                tempfiles[i] = File.CreateText(tempnames[i]);
            }

            string enginedir = System.AppDomain.CurrentDomain.BaseDirectory;
            if (IntPtr.Size == 8) enginedir = Path.Combine(enginedir, @"Engine\x64");
            else enginedir = Path.Combine(enginedir, @"Engine\x86");

            for (i = 0; i < tempfiles.Length; i++)
            {
                foreach (var item in _namegen[i])
                {
                    string line = converter.GetCommandLine();
                    line = line.Replace("[input]", item.Key);
                    line = line.Replace("[output]", System.IO.Path.Combine(outdir, item.Value));
                    line = enginedir + "\\" + line;
                    tempfiles[i].WriteLine(line);
                }
                //self delete after run
                tempfiles[i].WriteLine("DEL \"%~f0\"");
            }

            _processinfo.Clear();

            for (i = 0; i < _namegen.Cores; i++)
            {
                tempfiles[i].Close();
                _processes[i] = new Process();
                _processes[i].StartInfo.FileName = "cmd.exe";
                _processes[i].StartInfo.Arguments = "/c" + tempnames[i];
                _processes[i].StartInfo.UseShellExecute = false;
                _processes[i].StartInfo.CreateNoWindow = true;
                _processes[i].StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                _processinfo.Add(new ProcessInfo
                {
                    ID = i,
                    Running = true
                });
                
                _processes[i].Start();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            AbortProcesses();
            this.DialogResult = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _timer.IsEnabled = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            AbortProcesses();
            this.DialogResult = false;
        }
    }

    internal class ProcessInfo
    {
        public int ID { get; set; }
        public bool Running { get; set; }
    }
}
