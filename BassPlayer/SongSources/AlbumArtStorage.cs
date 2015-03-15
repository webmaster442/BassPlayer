using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BassPlayer.SongSources
{
    [Serializable]
    public class AlbumArt
    {
        public string Name { get; set; }
        public byte[] Data { get; set; }
    }

    internal class AlbumArtStorage
    {
        private Dictionary<string, byte[]> _storage;

        public AlbumArtStorage()
        {
            _storage = new Dictionary<string, byte[]>();
        }

        private void AddItem(string Key, BitmapSource src)
        {
            using (var ms = new MemoryStream())
            {
                var encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(src));
                encoder.QualityLevel = 100;
                encoder.Save(ms);
                _storage.Add(Key, ms.ToArray());
            }
        }

        private BitmapSource GetItem(string Key)
        {
            using (var ms = new MemoryStream(_storage[Key]))
            {
                BitmapImage ret = new BitmapImage();
                ret.BeginInit();
                ret.CacheOption = BitmapCacheOption.OnLoad;
                ret.StreamSource = ms;
                ret.EndInit();
                return ret;
            }
        }

        public BitmapSource this[string key]
        {
            get { return GetItem(key); }
            set { AddItem(key, value); }
        }

    }
}
