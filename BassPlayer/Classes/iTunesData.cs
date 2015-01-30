using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using System.Web;

namespace BassPlayer.Classes
{
    /// <summary>
    /// Represents tags in iTunes XML
    /// </summary>
    [Serializable]
    internal class iTunesSong
    {
        public string Album { get; set; }       // Name
        public string Artist { get; set; }      // Album
        public string Genre { get; set; }       // Genre
        public string Kind { get; set; }        // Kind
        public string Location { get; set; }    // Location
        public string Name { get; set; }        // Name
        public Int64 TotalTime { get; set; }    // TotalTime
        public int DiscNumber { get; set; }     // Disc number
        public int TrackNumber { get; set; }    // TrackNumber
        public bool Compilation { get; set; }   // Compilation
        public string AlbumArtist { get; set; } // Album Artist

    }

    /// <summary>
    /// iTunes Data Acces class based on http://hugeonion.com/2008/06/14/linq-to-itunes/
    /// </summary>
    internal class iTunesData
    {
        private IEnumerable<iTunesSong> _db;

        public iTunesData()
        {
            string profile = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            string file = Path.Combine(profile, "iTunes\\iTunes Music Library.xml");

            if (File.Exists(file))
            {
                isLoaded = true;
                _db = LoadSongsFromITunes(file);
            }
        }

        public bool isLoaded
        {
            get;
            private set;
        }

        private IEnumerable<iTunesSong> LoadSongsFromITunes(string filename)
        {
            var rawsongs = from song in XDocument.Load(filename).Descendants("plist").Elements("dict").Elements("dict").Elements("dict")
                           select new XElement("song",
                               from key in song.Descendants("key")
                               select new XElement(((string)key).Replace(" ", ""),
                                   (string)(XElement)key.NextNode));

            var songs = from s in rawsongs
                        select new iTunesSong()
                            {
                                Album = s.Element("Album").ToString(string.Empty),
                                Artist = s.Element("Artist").ToString(string.Empty),
                                Genre = s.Element("Genre").ToString(string.Empty),
                                Kind = s.Element("Kind").ToString(string.Empty),
                                Location = s.Element("Location").ToString(string.Empty),
                                Name = s.Element("Name").ToString(string.Empty),
                                TotalTime = s.Element("TotalTime").ToInt64(0),
                                TrackNumber = s.Element("TrackNumber").ToInt(0),
                                Compilation = s.Element("Compilation").ToBool(),
                                AlbumArtist = s.Element("AlbumArtist").ToString(string.Empty),
                                DiscNumber = s.Element("DiscNumber").ToInt(0)
                            };
            return songs;
        }

        public string[] Artists
        {
            get
            {
                if (!isLoaded) return null;
                var query = (from i in _db group i by i.AlbumArtist into g select g).Select(i => i.Key).OrderBy(i => i);
                return query.ToArray();
            }
        }

        public string[] Albums
        {
            get
            {
                if (!isLoaded) return null;
                var query = (from i in _db where i.Compilation == false group i by i.Album into g select g).Select(i => i.Key);
                return query.ToArray();
            }
        }

        public string[] Compilations
        {
            get
            {
                if (!isLoaded) return null;
                var query = (from i in _db where i.Compilation == true group i by i.Album into g select g).Select(i => i.Key);
                return query.ToArray();
            }
        }

        public string[] Genres
        {
            get
            {
                if (!isLoaded) return null;
                var q = (from i in _db select i.Genre).Distinct();
                return q.ToArray();
            }
        }

        public IEnumerable<PlayListEntry> Filter(string filterstring)
        {
            string[] parts = filterstring.Split('/');
            if (parts.Length < 2) return null;
            switch (parts[0])
            {
                case "Artists":
                    return (from i in _db orderby i.Name ascending
                            where i.Artist == parts[1]
                            select new PlayListEntry 
                            {
                                Artist = i.Artist,
                                Title = i.Name,
                                Time = i.TotalTime / 1000,
                                FileName = FileFromUrl(i.Location)
                            }).ToArray();
                case "Albums":
                    return (from i in _db orderby i.DiscNumber, i.TrackNumber ascending
                            where i.Album == parts[1]
                            select new PlayListEntry
                            {
                                Artist = i.Artist,
                                Title = i.Name,
                                Time = i.TotalTime / 1000,
                                FileName = FileFromUrl(i.Location)
                            }).ToArray();
                case "Genres":
                    return (from i in _db
                            orderby i.Artist, i.Name ascending
                            where i.Genre == parts[1]
                            select new PlayListEntry
                            {
                                Artist = i.Artist,
                                Title = i.Name,
                                Time = i.TotalTime / 1000,
                                FileName = FileFromUrl(i.Location)
                            }).ToArray();
                default:
                    return null;
            }
        }

        private string FileFromUrl(string p)
        {
            string decoded = HttpUtility.UrlDecode(p);
            return decoded.Replace("file://localhost/", "");
        }
    }

    internal static class XExtensions
    {
        public static int ToInt(this XElement xe, int emptyValue)
        {
            return xe == null ? emptyValue : int.Parse(xe.Value);
        }

        public static Int64 ToInt64(this XElement xe, Int64 emptyValue)
        {
            return xe == null ? emptyValue : Int64.Parse(xe.Value);
        }

        public static string ToString(this XElement xe, string emptyValue)
        {
            return xe == null ? emptyValue : xe.Value;
        }

        public static bool ToBool(this XElement xe)
        {
            return xe != null;
        }
    }
}
