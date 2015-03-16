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
    }

    internal class AlbumArtStorage
    {
        private List<AlbumArt> _albumdata;

        public AlbumArtStorage()
        {
            _albumdata = new List<AlbumArt>();
        }

        public void Add(AlbumArt art)
        {
            if (_albumdata.Contains(art))
            {
                var index = _albumdata.IndexOf(art);
                _albumdata[index] = art;
            }
        }

    }
}
