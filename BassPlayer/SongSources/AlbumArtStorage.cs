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
    /// <summary>
    /// Album art storage class
    /// </summary>
    [Serializable]
    public class AlbumArt: IEquatable<AlbumArt>
    {
        /// <summary>
        /// Album name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Cover as a byte array
        /// </summary>
        public byte[] Data { get; set; }

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

        public override string ToString()
        {
            return Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ Data.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            // STEP 1: Check for null
            if (obj == null) return false;
            if (this.GetType() != obj.GetType()) return false;
            return Equals((AlbumArt)obj);
        }

        public bool Equals(AlbumArt other)
        {
            return this.Name == other.Name && this.Data == other.Data;
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

        public bool ContainsAlbum(string albumname)
        {
            return (from i in _albumdata where i.Name == albumname select i).Count() > 0;
        }

        public void SyncToDb(Dictionary<string, TrackData> SyncData)
        {
            foreach (var item in SyncData)
            {
            }
        }
    }
}
