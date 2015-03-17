using BassEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Media.Imaging;

namespace BassPlayer.SongSources
{
    /// <summary>
    /// Album art storage class
    /// </summary>
    [Serializable]
    public class AlbumArt
    {
        /// <summary>
        /// Album name
        /// </summary>
        [OptionalField]
        public string Name;
        /// <summary>
        /// Cover as a byte array
        /// </summary>
        [OptionalField]
        public byte[] Data;

        /// <summary>
        /// Gets the cover as a bitmapsource
        /// </summary>
        public BitmapSource Cover
        {
            get
            {
                using (var ms = new MemoryStream(Data))
                {
                    BitmapImage ret = new BitmapImage();
                    ret.BeginInit();
                    ret.CacheOption = BitmapCacheOption.OnLoad;
                    ret.StreamSource = ms;
                    ret.EndInit();
                    return ret;
                }
            }
        }

        /// <summary>
        /// Creates an albumart instance
        /// </summary>
        /// <param name="name">name of the album</param>
        /// <param name="img">Album cover image</param>
        /// <returns>An instance of AlbumArt</returns>
        public static AlbumArt Create(string name, BitmapSource img)
        {
            AlbumArt ret = new AlbumArt();
            ret.Name = name;
            using (var ms = new MemoryStream())
            {
                JpegBitmapEncoder jpg = new JpegBitmapEncoder();
                jpg.QualityLevel = 80;
                jpg.Frames.Add(BitmapFrame.Create(img));
                jpg.Save(ms);
                ret.Data = ms.ToArray();
            }
            return ret;
        }

        public static AlbumArt Create(KeyValuePair<string, BitmapSource> keypair)
        {
            AlbumArt ret = new AlbumArt();
            ret.Name = keypair.Key;
            using (var ms = new MemoryStream())
            {
                JpegBitmapEncoder jpg = new JpegBitmapEncoder();
                jpg.QualityLevel = 80;
                jpg.Frames.Add(BitmapFrame.Create(keypair.Value));
                jpg.Save(ms);
                ret.Data = ms.ToArray();
            }
            return ret;
        }

        public AlbumArt()
        {
            Name = "";
            Data = new byte[0];
        }
    }

    internal class AlbumArtStorage: Dictionary<string, BitmapSource>
    {
        private string _file;
        private bool test;

        public AlbumArtStorage(string file): base()
        {
            _file = file;
            if (File.Exists(_file)) Load();
        }

        public void Save()
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                using (var stream = File.Create(_file))
                {
                    AlbumArt[] array = new AlbumArt[this.Keys.Count];
                    int i = 0;
                    foreach (var pair in this)
                    {
                        array[i] = AlbumArt.Create(pair);
                        i++;
                    }
                    bf.Serialize(stream, array);
                }
            }
            catch (Exception ex)
            {
                Helpers.ErrorDialog(ex, "Cover storage save error");
            }
        }

        private void Load()
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                using (var stream = File.OpenRead(_file))
                {
                    AlbumArt[] array = (AlbumArt[])bf.Deserialize(stream);
                    this.Clear();
                    foreach (var item in array)
                    {
                        this.Add(item.Name, item.Cover);
                    }
                }
            }
            catch (Exception ex)
            {
                Helpers.ErrorDialog(ex, "Cover storage load error");
            }
        }

        public BitmapSource GetCover(TrackData data)
        {
            try
            {
                if (this.ContainsKey(data.Album)) return this[data.Album];
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
                        this[data.Album] = ret;
                        return ret;
                    }
                    else
                    {
                        this[data.Album] = new BitmapImage(new Uri("/BassPlayer;component/Images/audio_file-100.png", UriKind.Relative));
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
