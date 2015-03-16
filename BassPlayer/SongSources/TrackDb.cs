using BassEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Xml.Serialization;

namespace BassPlayer.SongSources
{
    class TrackDb: List<TrackData>
    {

        public enum QueryType
        {
            Album, Artist, Year, Genre
        }

        private string _file;

        public TrackDb() : base()
        {
            string profile = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            _file = Path.Combine(profile, "BassPlayerLib.xml");
            if (File.Exists(_file)) Load();
        }

        private void Load()
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(TrackData[]));
                using (var f = File.OpenRead(_file))
                {
                    var array = (TrackData[])xs.Deserialize(f);
                    this.AddRange(array);
                }
            }
            catch (Exception ex)
            {
                Helpers.ErrorDialog(ex, "Database Load Error");
            }
        }

        public void Save()
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(TrackData[]));
                using (var f = File.Create(_file))
                {
                    xs.Serialize(f, this.ToArray());
                }
            }
            catch (Exception ex)
            {
                Helpers.ErrorDialog(ex, "Database Save Error");
            }
        }

        public string[] Artists
        {
            get
            {
                return (from i in this.AsParallel() 
                        orderby i.Artist ascending
                        select i.Artist).Distinct().ToArray();
            }
        }

        public string[] Albums
        {
            get
            {
                return (from i in this.AsParallel()
                        orderby i.Album ascending
                        select i.Album).Distinct().ToArray();
            }
        }

        public uint[] Years
        {
            get
            {
                return (from i in this.AsParallel()
                        orderby i.Year descending
                        select i.Year).Distinct().ToArray();
            }
        }

        public string[] Genres
        {
            get
            {
                return (from i in this.AsParallel()
                        orderby i.Genre ascending
                        select i.Genre).Distinct().ToArray();
            }
        }

        public TrackData[] Query(QueryType type, object parameter)
        {
            switch (type)
            {
                case QueryType.Album:
                    return (from track in this.AsParallel()
                            where track.Album == (string)parameter
                            orderby track.Disc, track.Track, track.Artist, track.Title
                            select track).ToArray();
                case QueryType.Artist:
                    return (from track in this.AsParallel()
                            where track.Artist == (string)parameter
                            orderby track.Title
                            select track).ToArray();
                case QueryType.Genre:
                    return (from track in this.AsParallel()
                            where track.Genre == (string)parameter
                            orderby track.Artist, track.Title
                            select track).ToArray();
                case QueryType.Year:
                    return (from track in this.AsParallel()
                            where track.Year == (uint)parameter
                            orderby track.Artist, track.Title
                            select track).ToArray();
                default:
                    return null;
            }
        }

        public Task ProcessFiles(IEnumerable<string> files)
        {
            return Task.Run(() =>
            {
                List<TrackData> data = new List<TrackData>(files.Count());
                foreach (var file in files)
                {
                    data.Add(TrackData.CreateFromFile(file));
                }
                Dispatcher.CurrentDispatcher.Invoke(() =>
                {
                    this.AddRange(data);
                });
            });
        }
    }
}
