using BassEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Windows.Media.Imaging;

namespace BassPlayer.SongSources
{
    internal class ZipArtStorage
    {
        private Dictionary<string, BitmapSource> _storage;
        private string _filename;

        public ZipArtStorage(string filename)
        {
            _storage = new Dictionary<string,BitmapSource>();
            _filename = filename;
            if (File.Exists(_filename)) Load();
        }

        private void Load()
        {
            try
            {
                using (var zip = ZipFile.OpenRead(_filename))
                {
                    foreach (var entry in zip.Entries)
                    {
                        using (var stream = entry.Open())
                        {
                            BitmapImage ret = new BitmapImage();
                            ret.BeginInit();
                            ret.CacheOption = BitmapCacheOption.OnLoad;
                            ret.StreamSource = stream;
                            ret.EndInit();
                            _storage.Add(entry.Name, ret);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Helpers.ErrorDialog(ex, "Cover storage load error");
            }
        }

        public void Save()
        {
            try
            {
                using (var zip = ZipFile.Open(_filename + ".new", ZipArchiveMode.Create))
                {
                    foreach (var item in _storage)
                    {
                        var entry = zip.CreateEntry(item.Key, CompressionLevel.Fastest);
                        using (var stream = entry.Open())
                        {
                            JpegBitmapEncoder jpg = new JpegBitmapEncoder();
                            jpg.QualityLevel = 80;
                            jpg.Frames.Add(BitmapFrame.Create(item.Value));
                            jpg.Save(stream);
                        }
                    }
                }
                if (File.Exists(_filename)) File.Delete(_filename);
                File.Move(_filename + ".new", _filename);
            }
            catch (Exception ex)
            {
                Helpers.ErrorDialog(ex, "Cover storage save error");
            }
        }

        private static string MD5Hash(string input)
        {
            var md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public BitmapSource GetCover(TrackData data)
        {
            try
            {
                if (_storage.ContainsKey(data.Album)) return _storage[data.Album];
                else
                {
                    TagLib.File tags = TagLib.File.Create(data.File);
                    if (tags.Tag.Pictures.Length > 0)
                    {
                        var picture = tags.Tag.Pictures[0].Data;
                        MemoryStream ms = new MemoryStream(picture.Data);
                        BitmapImage ret = new BitmapImage();
                        ret.BeginInit();
                        ret.StreamSource = ms;
                        ret.DecodePixelWidth = 200;
                        ret.CacheOption = BitmapCacheOption.OnLoad;
                        ret.EndInit();
                        ms.Close();
                        _storage[data.Album] = ret;
                        return ret;
                    }
                    else
                    {
                        _storage[data.Album] = new BitmapImage(new Uri("/BassPlayer;component/Images/audio_file-100.png", UriKind.Relative));
                        return new BitmapImage(new Uri("/BassPlayer;component/Images/audio_file-100.png", UriKind.Relative));
                    }
                }
            }
            catch (Exception ex)
            {
                Helpers.ErrorDialog(ex, "Can't get cover for " + data.ToString());
                return new BitmapImage(new Uri("/BassPlayer;component/Images/audio_file-100.png", UriKind.Relative));
            }
        }
    }
}
