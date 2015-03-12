using System;
using System.ComponentModel;
using System.IO;
using Un4seen.Bass.AddOn.Tags;

namespace BassPlayer.SongSources
{
    [Serializable]
    public class PlayListEntry : INotifyPropertyChanged
    {

        private string _title, _artist, _filename;
        private double _time;

        /// <summary>
        /// Title String
        /// </summary>
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                UpdateProperty("Title");
            }
        }

        /// <summary>
        /// Artist
        /// </summary>
        public string Artist
        {
            get { return _artist; }
            set
            {
                _artist = value;
                UpdateProperty("Artist");
            }
        }

        /// <summary>
        /// Time
        /// </summary>
        public double Time
        {
            get { return _time; }
            set
            {
                _time = value;
                UpdateProperty("Time");
            }
        }

        /// <summary>
        /// File Name
        /// </summary>
        public string FileName
        {
            get { return _filename; }
            set
            {
                _filename = value;
                UpdateProperty("FileName");
            }
        }

        /// <summary>
        /// Artist & Title
        /// </summary>
        public string ArtistTitle
        {
            get
            {
                if (FileName == null) return null;
                if (string.IsNullOrEmpty(Artist) && string.IsNullOrEmpty(Title))
                {
                    if (FileName.StartsWith("http://")) return FileName;
                    else return Path.GetFileName(FileName);
                }
                return string.Format("{0} - {1}", Artist, Title);
            }
        }

        /// <summary>
        /// Creates a PlayList entry from a filename
        /// </summary>
        /// <param name="filename">Filename</param>
        /// <returns>PlaylistEntry</returns>
        public static PlayListEntry FromFile(string filename)
        {
            PlayListEntry entry = new PlayListEntry();
            if (filename.StartsWith("http://") || filename.StartsWith("https://"))
            {
                entry.FileName = filename;
                return entry;
            }
            entry.FileName = filename;

            try
            {
                TagLib.File file = TagLib.File.Create(filename);
                entry.Artist = file.Tag.FirstPerformer;
                entry.Title = file.Tag.Title;
                entry.Time = file.Properties.Duration.TotalSeconds;
            }
            catch (Exception) { }
            /*try
            {
                var tags = BassTags.BASS_TAG_GetFromFile(filename);
                entry.Artist = tags.artist;
                entry.Title = tags.title;
                entry.Time = tags.duration;
            }
            catch (Exception) { }
            */
            return entry;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void UpdateProperty(string Name)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(Name));
        }

        public static implicit operator RecentItem(PlayListEntry ple)
        {
            RecentItem ret = new RecentItem();
            ret.FilePath = ple.FileName;
            ret.Artist = ple.Artist;
            ret.Title = ple.Title;
            ret.Time = ple.Time;
            ret.PlayCount = 1;
            return ret;
        }
    }
}
