using System;
using System.ComponentModel;

namespace BassPlayer.SongSources
{
    [Serializable]
    public class TrackData : INotifyPropertyChanged
    {
        private string _title, _artist, _album, _file, _genre;
        private uint _year, _playcount, _disc, _track;
        private double _time;

        public string File
        {
            get { return _file; }
            set
            {
                _file = value;
                UpdateProperty("File");
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                UpdateProperty("Title");
            }
        }

        public string Genre
        {
            get { return _genre; }
            set
            {
                _genre = value;
                UpdateProperty("Genre");
            }
        }

        public string Artist
        {
            get { return _artist; }
            set
            {
                _artist = value;
                UpdateProperty("Artist");
            }
        }

        public string Album
        {
            get { return _album; }
            set
            {
                _album = value;
                UpdateProperty("Album");
            }
        }

        public uint Year
        {
            get { return _year; }
            set
            {
                _year = value;
                UpdateProperty("Year");
            }
        }

        public double Time
        {
            get { return _time; }
            set
            {
                _time = value;
                UpdateProperty("Time");
            }
        }

        public uint PlayCount
        {
            get { return _playcount; } 
            set
            {
                _playcount = value;
                UpdateProperty("PlayCount");
            }
        }

        public uint Disc
        {
            get { return _disc; }
            set
            {
                _disc = value;
                UpdateProperty("Disc");
            }
        }

        public uint Track
        {
            get { return _track; }
            set
            {
                _track = value;
                UpdateProperty("Track");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void UpdateProperty(string Name)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(Name));
        }

        public static TrackData CreateFromFile(string filename)
        {
            try
            {
                TagLib.File file = TagLib.File.Create(filename);
                TrackData ret = new TrackData
                {
                    File = filename,
                    Title = file.Tag.Title,
                    Artist = file.Tag.FirstPerformer,
                    Album = file.Tag.Album,
                    Year = file.Tag.Year,
                    Genre = file.Tag.FirstGenre,
                    PlayCount = 0,
                    Time = file.Properties.Duration.TotalSeconds,
                    Disc = file.Tag.Disc,
                    Track = file.Tag.Track
                };
                return ret;
            }
            catch (Exception) { return null; }
        }

        public static implicit operator PlayListEntry(TrackData track)
        {
            if (track == null) return null;
            PlayListEntry ret = new PlayListEntry
            {
                Artist = track.Artist,
                Title = track.Title,
                Time = track.Time,
                FileName = track.File,
            };
            return ret;
        }

        public override string ToString()
        {
            return Artist + " - " + Title;
        }
    }
}
