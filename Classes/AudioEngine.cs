using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Mix;
using Un4seen.Bass.AddOn.Tags;

namespace BassPlayer.Classes
{
    /// <summary>
    /// Audio Output Engine
    /// </summary>
    internal class AudioEngine : IDisposable
    {
        private bool _initialized;
        private string _file;
        private int _source, _mixer;
        private bool _netstream;
        private DOWNLOADPROC _streamrip;

        /// <summary>
        /// Ctor
        /// </summary>
        public AudioEngine()
        {
            string enginedir = System.AppDomain.CurrentDomain.BaseDirectory;
            if (Utils.Is64Bit) enginedir = Path.Combine(enginedir, @"Engine\x64");
            else enginedir = Path.Combine(enginedir, @"Engine\x86");
            Bass.LoadMe(enginedir);
            BassMix.LoadMe(enginedir);
            Bass.BASS_PluginLoad(enginedir + "\\bass_aac.dll");
            Bass.BASS_PluginLoad(enginedir + "\\bass_ac3.dll");
            Bass.BASS_PluginLoad(enginedir + "\\bass_alac.dll");
            Bass.BASS_PluginLoad(enginedir + "\\bassflac.dll");
            Bass.BASS_PluginLoad(enginedir + "\\basswma.dll");
            Bass.BASS_PluginLoad(enginedir + "\\basswv.dll");
            _streamrip = new DOWNLOADPROC(DownloadStream);
        }

        /// <summary>
        /// Free used resources
        /// </summary>
        public void Dispose()
        {
            if (_initialized) Bass.BASS_Free();
            Bass.BASS_PluginFree(0);
            Bass.FreeMe();
            BassMix.FreeMe();
        }

        /// <summary>
        /// Display Error message
        /// </summary>
        /// <param name="message"></param>
        private void Error(string message)
        {
            var error = Bass.BASS_ErrorGetCode();
            string text = string.Format("{0}\r\nBass Error code: {1}\r\nError Description: {2}", message, (int)error, error.ToString());
            MessageBox.Show(text, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Change output device
        /// </summary>
        /// <param name="name">string device</param>
        public void ChangeDevice(string name)
        {
            for (int i = 0; i < Bass.BASS_GetDeviceCount(); i++)
            {
                var device = Bass.BASS_GetDeviceInfo(i);
                if (device.name == name)
                {
                    if (_initialized)
                    {
                        Bass.BASS_Free();
                        _initialized = false;
                    }
                    _initialized = Bass.BASS_Init(i, 48000, BASSInit.BASS_DEVICE_FREQ, IntPtr.Zero);
                    if (!_initialized)
                    {
                        Error("Bass.dll init failed");
                        return;
                    }
                    Bass.BASS_Start();
                }
            }
        }

        /// <summary>
        /// Gets or sets the file or url to be played
        /// </summary>
        public string File
        {
            get { return _file; }
            set
            {
                _file = value;
                var flags = BASSFlag.BASS_SAMPLE_LOOP | BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT;
                if (_source != 0)
                {
                    Bass.BASS_StreamFree(_source);
                    _source = 0;
                }
                if (_mixer != 0)
                {
                    Bass.BASS_StreamFree(_mixer);
                    _mixer = 0;
                }
                var mixerflags = BASSFlag.BASS_MIXER_DOWNMIX | BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_MIXER_POSEX | BASSFlag.BASS_STREAM_AUTOFREE;
                if (_file.StartsWith("http://") || _file.StartsWith("https://"))
                {
                    _source = Bass.BASS_StreamCreateURL(_file, 0, flags, _streamrip, IntPtr.Zero);
                    _netstream = true;
                }
                else
                {
                    _source = Bass.BASS_StreamCreateFile(_file, 0, 0, flags);
                    _netstream = false;
                }
                if (_source == 0)
                {
                    Error("Load failed");
                    return;
                }
                var ch = Bass.BASS_ChannelGetInfo(_source);
                _mixer = BassMix.BASS_Mixer_StreamCreate(ch.freq, ch.chans, mixerflags);
                if (_mixer == 0)
                {
                    Error("Mixer stream create failed");
                    return;
                }
                if (!BassMix.BASS_Mixer_StreamAddChannel(_mixer, _source, BASSFlag.BASS_MIXER_DOWNMIX))
                {
                    Error("Mixer chanel adding failed");
                    return;
                }
            }
        }

        /// <summary>
        /// Audio Stream ripper callback
        /// </summary>
        private void DownloadStream(IntPtr buffer, int length, IntPtr user)
        {
            /*if (_fs == null)
            {
                // create the file
                _fs = File.OpenWrite("output.mp3");
            }
            if (buffer == IntPtr.Zero)
            {
                // finished downloading
                _fs.Flush();
                _fs.Close();
            }
            else
            {
                // increase the data buffer as needed 
                if (_data == null || _data.Length < length)
                    _data = new byte[length];
                // copy from managed to unmanaged memory
                Marshal.Copy(buffer, _data, 0, length);
                // write to file
                _fs.Write(_data, 0, length);
            }*/
        }

        /// <summary>
        /// Get Playing tags
        /// </summary>
        public string Tags
        {
            get
            {
                TAG_INFO tags = new TAG_INFO();
                BassTags.BASS_TAG_GetFromFile(_source, tags);
                return string.Format("{0} - {1}", tags.artist, tags.title);
            }
        }

        public ImageSource ImageTag
        {
            get
            {
                TAG_INFO tags = new TAG_INFO();
                BassTags.BASS_TAG_GetFromFile(_source, tags);
                if (tags.PictureCount > 0)
                {
                    var img = tags.PictureGetImage(0);
                    MemoryStream ms = new MemoryStream();
                    img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    bi.StreamSource = ms;
                    bi.EndInit();
                    return bi;
                }
                else return new BitmapImage(new Uri("/BassPlayer;component/Images/audio_file-100.png", UriKind.Relative));
            }
        }

        /// <summary>
        /// Gets the channel length
        /// </summary>
        public double Length
        {
            get 
            {
                //if (_netstream) return 0;
                var len = Bass.BASS_ChannelGetLength(_source);
                return Bass.BASS_ChannelBytes2Seconds(_source, len);
            }
        }

        /// <summary>
        /// Returns true, if currently playing a netstream
        /// </summary>
        public bool IsNetStream
        {
            get { return _netstream; }
        }

        /// <summary>
        /// Gets or sets volume
        /// </summary>
        public float Volume
        {
            get { return Bass.BASS_GetVolume(); }
            set { Bass.BASS_SetVolume(value); }
        }

        /// <summary>
        /// Gets or sets position
        /// </summary>
        public double Position
        {
            get
            {
                var pos = Bass.BASS_ChannelGetPosition(_source);
                return Bass.BASS_ChannelBytes2Seconds(_source, pos);
            }
            set
            {
                if (_netstream) return;
                var pos = Bass.BASS_ChannelSeconds2Bytes(_source, value);
                Bass.BASS_ChannelSetPosition(_source, pos);
            }
        }

        /// <summary>
        /// Play
        /// </summary>
        public void Play()
        {
            Bass.BASS_ChannelPlay(_mixer, false);
        }

        /// <summary>
        /// Pause
        /// </summary>
        public void Pause()
        {
            Bass.BASS_ChannelPause(_mixer);
        }

        /// <summary>
        /// Stop
        /// </summary>
        public void Stop()
        {
            Bass.BASS_ChannelStop(_mixer);
        }

        /// <summary>
        /// Gets the available output devices
        /// </summary>
        /// <returns>device names in an array</returns>
        public string[] GetDevices()
        {
            int devcount = Bass.BASS_GetDeviceCount();
            List<string> _devices = new List<string>(devcount);
            for (int i = 0; i < devcount; i++)
            {
                var device = Bass.BASS_GetDeviceInfo(i);
                if (device.IsEnabled) _devices.Add(device.name);
            }
            return _devices.ToArray();
        }
    }
}
