using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Cd;
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
        private MediaType _filetype;
        private DOWNLOADPROC _streamrip;
        private FileStream _writer;
        private byte[] _data;

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
            BassCd.LoadMe(enginedir);
            Bass.BASS_PluginLoad(enginedir + "\\bass_aac.dll");
            Bass.BASS_PluginLoad(enginedir + "\\bass_ac3.dll");
            Bass.BASS_PluginLoad(enginedir + "\\bass_alac.dll");
            Bass.BASS_PluginLoad(enginedir + "\\bassflac.dll");
            Bass.BASS_PluginLoad(enginedir + "\\basswma.dll");
            Bass.BASS_PluginLoad(enginedir + "\\basswv.dll");
            _data = new byte[4096];
            _streamrip = new DOWNLOADPROC(DownloadStream);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_initialized) Bass.BASS_Free();
            BassCd.FreeMe();
            BassMix.FreeMe();
            Bass.BASS_PluginFree(0);
            Bass.FreeMe();
        }

        /// <summary>
        /// Free used resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
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
        /// Generates a File name from url
        /// </summary>
        /// <param name="url">Stream url</param>
        private string GenerateRippName(string url, string ext)
        {
            string name = url.Replace("/", "-");
            name = name.Replace(":", "_");
            string timestamp = string.Format("{0}{1}{2}{3}{4}{6}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            return string.Format("{0}-{1}.{2}", name, timestamp, ext);
        }

        /// <summary>
        /// Gets or sets the file or url to be played
        /// </summary>
        public string FileName
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
                    _filetype = MediaType.Stream;
                }
                else if (_file.StartsWith("cd://"))
                {
                    string[] info = _file.Replace("cd://", "").Split('/');
                    _source = BassCd.BASS_CD_StreamCreate(Convert.ToInt32(info[0]), Convert.ToInt32(info[1]), flags);
                    _filetype = MediaType.CD;
                }
                else
                {
                    _source = Bass.BASS_StreamCreateFile(_file, 0, 0, flags);
                    _filetype = MediaType.File;
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

        public bool StreamRipEnabled { get; set; }
        public string StreamRipFolder { get; set; }

        /// <summary>
        /// Audio Stream ripper callback
        /// </summary>
        private void DownloadStream(IntPtr buffer, int length, IntPtr user)
        {
            if (!StreamRipEnabled) return;
            if (_writer == null)
            {
                // create the file
                _writer = System.IO.File.OpenWrite("output.mp3");
            }
            if (buffer == IntPtr.Zero)
            {
                // finished downloading
                _writer.Flush();
                _writer.Close();
            }
            else
            {
                // increase the data buffer as needed 
                if (_data == null || _data.Length < length) _data = new byte[length];
                // copy from managed to unmanaged memory
                Marshal.Copy(buffer, _data, 0, length);
                // write to file
                _writer.Write(_data, 0, length);
            }
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
        /// Returns the current media type
        /// </summary>
        public MediaType MediaType
        {
            get { return _filetype; }
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
                if (_filetype == Classes.MediaType.Stream)
                {
                    if (Length == 0) return;
                }
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

        /// <summary>
        /// List tracks on a CD drive
        /// </summary>
        /// <param name="drive">CD drive path</param>
        /// <returns>An array of playlist entry's</returns>
        public static PlayListEntry[] GetCdInfo(string drive)
        {
            List<PlayListEntry> list = new List<PlayListEntry>();
          
            int drivecount = BassCd.BASS_CD_GetDriveCount(true);
            int driveindex = 0;
            for (int i = 0; i < drivecount; i++)
            {
                var info = BassCd.BASS_CD_GetInfo(i);
                if (info.DriveLetter == drive[0])
                {
                    driveindex = i;
                    break;
                }
            }

            if (BassCd.BASS_CD_IsReady(driveindex))
            {
                for (int i = 0; i < BassCd.BASS_CD_GetTracks(driveindex); i++)
                {
                    PlayListEntry entry = new PlayListEntry();
                    entry.FileName = string.Format("cd://{0}/{1}", driveindex, i);
                    entry.Artist = "CD";
                    entry.Title = string.Format("Track #{0:00}", i);
                    entry.Time = BassCd.BASS_CD_GetTrackLengthSeconds(driveindex, i);
                    list.Add(entry);
                }
            }
            BassCd.BASS_CD_Release(driveindex);
            return list.ToArray();
        }
    }
}
