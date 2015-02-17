using BassPlayer.Properties;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace BassPlayer.Classes
{
    [Serializable]
    public class RecentItem : INotifyPropertyChanged
    {
        private string _title, _filepath;
        private int _playcount;
        private DateTime _lastplayed;
        private string _time;

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                UpdateProperty("Title");
            }
        }

        public DateTime LastPlayed
        {
            get { return _lastplayed; }
            set
            {
                _lastplayed = value;
                UpdateProperty("LastPlayed");
            }
        }
        public int PlayCount
        {
            get { return _playcount; }
            set
            {
                _playcount = value;
                UpdateProperty("PlayCount");
            }
        }
        public string FilePath
        {
            get { return _filepath; }
            set
            {
                _filepath = value;
                UpdateProperty("FilePath");
            }
        }

        public string Time
        {
            get { return _time; }
            set
            {
                _time = value;
                UpdateProperty("Time");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void UpdateProperty(string Name)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(Name));
        }

    }


    internal class RecentPlays: ObservableCollection<RecentItem>
    {
        private string _file;

        public RecentPlays() : base()
        {
            string profile = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            _file = Path.Combine(profile, "BassPlayerHistory.xml");
            DeSerialize();
        }

        private void DeSerialize()
        {
            if (!File.Exists(_file)) return;
            try
            {
                using (var stream = File.OpenRead(_file))
                {
                    this.Clear();
                    XmlSerializer xs = new XmlSerializer(typeof(RecentItem[]));
                    RecentItem[] loaded = (RecentItem[])xs.Deserialize(stream);
                    this.AddRange(loaded);
                }
            }
            catch (IOException ex)
            {
                Helpers.ErrorDialog(ex, "Recent items load error. The file is corrupted");
            }
        }

        /// <summary>
        /// Saves recent items
        /// </summary>
        public void Save()
        {
            try
            {
                using (var sw = File.Create(_file))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(RecentItem[]));
                    xs.Serialize(sw, this.ToArray());
                }
            }
            catch (IOException ex)
            {
                Helpers.ErrorDialog(ex, "Recent items save error");
            }
        }

        private RecentItem OldestLeastPlayed()
        {
            return (from i in this orderby i.LastPlayed ascending, i.PlayCount ascending select i).FirstOrDefault();
        }

        private RecentItem Contains(PlayListEntry entry)
        {
            return (from i in this where i.FilePath == entry.FileName select i).FirstOrDefault();
        }

        /// <summary>
        /// Adds a PlaylistEntry to the collection
        /// </summary>
        /// <param name="ple">A PlaylistEntry to add</param>
        public void Add(PlayListEntry ple)
        {
            if (this.Count > Settings.Default.RecentCount)
            {
                this.Remove(OldestLeastPlayed());
            }
            
            var match = Contains(ple);
            
            if (match != null)
            {
                match.LastPlayed = DateTime.Now;
                match.PlayCount += 1;
                int index = this.IndexOf(match);
                this[index] = match;
                return;
            }

            var item = new RecentItem
            {
                FilePath = ple.FileName,
                Title = ple.ArtistTitle,
                Time = ple.TimeString,
                LastPlayed = DateTime.Now,
                PlayCount = 1
            };

            base.Add(item);
        }

        /// <summary>
        /// Updates an item based on it's index
        /// </summary>
        /// <param name="index">index</param>
        public void UpdateItemAtIndex(int index)
        {
            if (index > this.Count || index < 0) return;
            this[index].PlayCount += 1;
            this[index].LastPlayed = DateTime.Now;
        }
    }
}
