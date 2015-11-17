using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace BassPlayer.SongSources
{
    /// <summary>
    /// Represents tags in iTunes XML
    /// </summary>
    [Serializable]
    internal class iTunesSong
    {
        public int Id { get; set; }             // ID
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
        public bool Podcast { get; set; }       // Podcast
        public string AlbumArtist { get; set; } // Album Artist
        public int Year { get; set; }           // Year

    }

    /// <summary>
    /// iTunes Data Acces class based on http://hugeonion.com/2008/06/14/linq-to-itunes/
    /// </summary>
    internal class iTunesData
    {
        private IEnumerable<iTunesSong> _db;
        private XDocument _xml;

        public iTunesData()
        {
            string profile = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            string file = Path.Combine(profile, "iTunes\\iTunes Music Library.xml");

            if (File.Exists(file))
            {
                isLoaded = true;
                _xml = XDocument.Load(file);
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
            var rawsongs = from song in _xml.Descendants("plist").Elements("dict").Elements("dict").Elements("dict").AsParallel()
                           select new XElement("song",
                               from key in song.Descendants("key")
                               select new XElement(((string)key).Replace(" ", ""),
                                   (string)(XElement)key.NextNode));

            var songs = from s in rawsongs.AsParallel()
                        select new iTunesSong()
                            {
                                Id = s.Element("TrackID").ToInt(0),
                                Album = s.Element("Album").ToString(string.Empty),
                                Artist = s.Element("Artist").ToString(string.Empty),
                                Genre = s.Element("Genre").ToString(string.Empty),
                                Kind = s.Element("Kind").ToString(string.Empty),
                                Location = s.Element("Location").ToString(string.Empty),
                                Name = s.Element("Name").ToString(string.Empty),
                                TotalTime = s.Element("TotalTime").ToInt64(0),
                                TrackNumber = s.Element("TrackNumber").ToInt(0),
                                Compilation = s.Element("Compilation").ToBool(),
                                Podcast = s.Element("Podcast").ToBool(),
                                AlbumArtist = s.Element("AlbumArtist").ToString(string.Empty),
                                DiscNumber = s.Element("DiscNumber").ToInt(0),
                                Year = s.Element("Year").ToInt(0)
                            };

            return songs;
        }

        public string[] Artists
        {
            get
            {
                if (!isLoaded) return null;
                var query = (from i in _db.AsParallel() orderby i.AlbumArtist select i.AlbumArtist).Distinct();
                return query.ToArray();
            }
        }

        public string[] Albums
        {
            get
            {
                if (!isLoaded) return null;
                var query = (from i in _db.AsParallel() where i.Compilation == false && i.Podcast == false orderby i.Album ascending select i.Album).Distinct();
                return query.ToArray();
            }
        }

        public string[] Podcasts
        {
            get
            {
                if (!isLoaded) return null;
                var query = (from i in _db.AsParallel() where i.Podcast == true orderby i.Album ascending select i.Album).Distinct();
                return query.ToArray();
            }
        }

        public string[] Compilations
        {
            get
            {
                if (!isLoaded) return null;
                var query = (from i in _db.AsParallel() where i.Compilation == true && i.Podcast == false orderby i.Album ascending select i.Album).Distinct();
                return query.ToArray();
            }
        }

        public string[] Genres
        {
            get
            {
                if (!isLoaded) return null;
                var q = (from i in _db where !string.IsNullOrEmpty(i.Genre) orderby i.Genre ascending select i.Genre).Distinct();
                return q.ToArray();
            }
        }

        public string[] Playlists
        {
            get
            {
                if (!isLoaded) return null;
                var q1 = from song in _xml.Descendants("key").AsParallel() where song.Value == "Playlist ID" select song.ElementsBeforeSelf("string").FirstOrDefault();
                var q2 = from q in q1 where q != null select q.Value;
                return (from item in q2 orderby item ascending select item).ToArray();
            }
        }

        public string[] Years
        {
            get
            {
                if (!isLoaded) return null;
                var q = (from i in _db orderby i.Year descending select i.Year).Distinct().Select(x => x.ToString());
                return q.ToArray();
            }
        }

        public IEnumerable<PlayListEntry> GetPlayList(string playlistname)
        {

            var q = from song in _xml.Descendants("key").AsParallel() 
                    where song.Value == "Playlist ID" && song.ElementsBeforeSelf("string").FirstOrDefault().Value == playlistname 
                    select song.Parent;
            var dict = q.FirstOrDefault().Descendants("array").Descendants("dict");

            var keys = from i in dict select i.Element("integer").ToInt(0);

            return (from d in _db 
                         join k in keys on d.Id equals k
                         where d.Id == k select new PlayListEntry
                         {
                                Artist = d.Artist,
                                Title = d.Name,
                                Time = d.TotalTime / 1000,
                                FileName = FileFromUrl(d.Location)
                         }).ToArray();
        }

        public IEnumerable<PlayListEntry> Filter(string filterstring)
        {
            if (!isLoaded) return null;
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
                case "Compilations":
                case "Podcasts":
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
                case "Songs":
                    return (from i in _db
                            orderby i.Artist, i.Album, i.DiscNumber, i.TrackNumber ascending
                            select new PlayListEntry
                            {
                                Artist = i.Artist,
                                Title = i.Name,
                                Time = i.TotalTime / 1000,
                                FileName = FileFromUrl(i.Location)
                            }).ToArray();
                case "Years":
                    return (from i in _db
                            orderby i.Artist, i.Name ascending
                            where i.Year == Convert.ToInt32(parts[1])
                            select new PlayListEntry
                            {
                                Artist = i.Artist,
                                Title = i.Name,
                                Time = i.TotalTime / 1000,
                                FileName = FileFromUrl(i.Location)
                            }).ToArray();
                case "Playlists":
                    return GetPlayList(parts[1]);
                default:
                    return null;
            }
        }

        public IEnumerable<PlayListEntry> Search(string text)
        {
            text = text.ToLower();
            return (from i in _db
                    where i.Name.ToLower().Contains(text) || 
                    i.Artist.ToLower().Contains(text) || 
                    i.Album.ToLower().Contains(text) ||
                    i.AlbumArtist.ToLower().Contains(text)
                    orderby i.Artist, i.Album ascending
                    select new PlayListEntry
                    {
                        Artist = i.Artist,
                        Title = i.Name,
                        Time = i.TotalTime / 1000,
                        FileName = FileFromUrl(i.Location)
                    }).ToArray();
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

        public static DateTime ToDateTime(this XElement xe)
        {
            return Convert.ToDateTime(xe.Value);
        }
    }
}
