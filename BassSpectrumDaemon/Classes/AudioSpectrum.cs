using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Windows.Threading;
using Un4seen.Bass;
using Un4seen.BassWasapi;

namespace BassSpectrumDaemon.Classes
{
    internal class AudioSpectrum: IDisposable
    {
        private bool _bass, _wasapi, _enabled;
        private string[] _devices;
        private WASAPIPROC _process;
        private DispatcherTimer _timer;
        private float[] _fftbuffer;
        private int _lastlevel;
        private int _hanctrl;

        private const int _LINES = 32;
        private byte[] _spectrumbuffer;
        private LevelIndicator _indicator;

        /// <summary>
        /// Ctor
        /// </summary>
        public AudioSpectrum(LevelIndicator indicator)
        {
            DisplayType = Messages.Spectrum;
            string enginedir = System.AppDomain.CurrentDomain.BaseDirectory;
            if (Utils.Is64Bit) enginedir = Path.Combine(enginedir, @"Engine\x64");
            else enginedir = Path.Combine(enginedir, @"Engine\x86");
            Bass.LoadMe(enginedir);
            BassWasapi.LoadMe(enginedir);
            List<string> devices = new List<string>();
            for (int i = 0; i < BassWasapi.BASS_WASAPI_GetDeviceCount(); i++)
            {
                var device = BassWasapi.BASS_WASAPI_GetDeviceInfo(i);
                if (device.IsEnabled && device.IsLoopback)
                {
                    devices.Add(string.Format("{0} - {1}", i, device.name));
                }
            }
            _devices = devices.ToArray();
            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATETHREADS, false);
            _bass = Bass.BASS_Init(0, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
            _process = new WASAPIPROC(Process);
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(25);
            _timer.IsEnabled = false;
            _timer.Tick += _timer_Tick;
            _fftbuffer = new float[1024];
            _spectrumbuffer = new byte[_LINES+2];
            _indicator = indicator;
        }

        /// <summary>
        /// Wasapi Callback
        /// </summary>
        private int Process(IntPtr buffer, int length, IntPtr user)
        {
            return length;
        }

        protected virtual void Dispose(bool native)
        {
            if (_bass) Bass.BASS_Free();
            if (_wasapi) BassWasapi.BASS_WASAPI_Free();
            BassWasapi.FreeMe();
            Bass.FreeMe();
        }

        /// <summary>
        /// Dispose function
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Retuns an array of available loopback devices
        /// </summary>
        public string[] Devices
        {
            get { return _devices; }
        }

        /// <summary>
        /// Serial port
        /// </summary>
        public SerialPort Serial { get; set; }

        /// <summary>
        /// Gets or sets the monitored device index
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// Display type
        /// </summary>
        public Messages DisplayType { get; set; }

        /// <summary>
        /// Gets or sets device monitoring
        /// </summary>
        public bool IsEnabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;
                if (_enabled)
                {
                    if (!_bass) throw new Exception("Bass Init Error");
                    if (!_wasapi)
                    {
                        var array = DeviceName.Split(' ');
                        int devindex = Convert.ToInt32(array[0]);
                        _wasapi = BassWasapi.BASS_WASAPI_Init(devindex, 0, 0,
                                                              BASSWASAPIInit.BASS_WASAPI_BUFFER,
                                                              1f, 0.05f,
                                                              _process, IntPtr.Zero);
                        if (!_wasapi) throw new Exception("WASAPI Init Error");
                        else BassWasapi.BASS_WASAPI_Start();
                        _timer.IsEnabled = true;
                    }
                }
                else
                {
                    BassWasapi.BASS_WASAPI_Stop(true);
                    _timer.IsEnabled = false;
                    if (_wasapi)
                    {
                        BassWasapi.BASS_WASAPI_Free();
                        _wasapi = false;
                    }
                }
            }
        }

        private void GetSpectrumData()
        {
            int x, y;
            int b0 = 0;
            for (x = 0; x < _LINES; x++)
            {
                float peak = 0;
                int b1 = (int)Math.Pow(2, x * 10.0 / (_LINES - 1));
                if (b1 > 1023) b1 = 1023;
                if (b1 <= b0) b1 = b0 + 1;
                for (; b0 < b1; b0++)
                {
                    if (peak < _fftbuffer[1 + b0]) peak = _fftbuffer[1 + b0];
                }
                y = (int)(Math.Sqrt(peak) * 3 * 254 - 4);
                if (y > 254) y = 254;
                if (y < 0) y = 0;
                _spectrumbuffer[x+1] = (byte)y;
            }
            _spectrumbuffer[33] = 255;
        }

        private byte Map(int x, int in_min, int in_max, int out_min, int out_max)
        {
            return (byte)((x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min);
        }

        private void GetLevelData(int level)
        {
            short l, r;
            l = Utils.LowWord(level);
            r = Utils.HighWord(level);
            _spectrumbuffer[1] = Map(l, 0, short.MaxValue, 0, 254);
            _spectrumbuffer[2] = Map(r, 0, short.MaxValue, 0, 254);
            _spectrumbuffer[33] = 255;
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            try
            {
                int ret = BassWasapi.BASS_WASAPI_GetData(_fftbuffer, (int)BASSData.BASS_DATA_FFT2048);
                int level = BassWasapi.BASS_WASAPI_GetLevel();

                switch (DisplayType)
                {
                    case Messages.Spectrum:
                        if (ret > 0) GetSpectrumData();
                        break;
                    case Messages.Level:
                        GetLevelData(level);
                        break;

                }
                if (Serial != null)
                {
                    _spectrumbuffer[0] = (byte)this.DisplayType;
                    Serial.Write(_spectrumbuffer, 0, _spectrumbuffer.Length);
                }

                if (level == _lastlevel && level != 0) _hanctrl++;
                _lastlevel = level;
                _indicator.Level = level;

                if (_hanctrl > 3)
                {
                    _hanctrl = 0;
                    BassWasapi.BASS_WASAPI_Free();
                    Bass.BASS_Free();
                    _bass = Bass.BASS_Init(0, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
                    _wasapi = false;
                }
            }
            catch (Exception ex)
            {
                BassEngine.Helpers.ErrorDialog(ex, "Spectrum error");
                _timer.IsEnabled = false;
                if (_bass) Bass.BASS_Free();
                if (_wasapi) BassWasapi.BASS_WASAPI_Free();
            }
        }
    }
}
