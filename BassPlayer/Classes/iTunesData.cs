using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;

namespace BassPlayer.Classes
{
    internal class iTunesSong
    {
        // Tag (in iTunes XML):
        public int Id { get; set; }             // TrackID
        public string Album { get; set; }       // Name
        public string Artist { get; set; }      // Album
        public int BitRate { get; set; }        // BitRate
        public string Comments { get; set; }    // Comments
        public string Composer { get; set; }    // Composer
        public string Genre { get; set; }       // Genre
        public string Kind { get; set; }        // Kind
        public string Location { get; set; }    // Location
        public string Name { get; set; }        // Name
        public int PlayCount { get; set; }      // PlayCount
        public int SampleRate { get; set; }     // SampleRate
        public Int64 Size { get; set; }         // Size
        public Int64 TotalTime { get; set; }    // TotalTime
        public int TrackNumber { get; set; }    // TrackNumber
    }

    internal class iTunesData
    {
        private IEnumerable<iTunesSong> _db;

        public iTunesData()
        {
            string profile = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            string file = Path.Combine(profile, "iTunes\\itunesdb.xml");
            _db = LoadSongsFromITunes(file);
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
                                Id = s.Element("TrackID").ToInt(0),
                                Album = s.Element("Album").ToString(string.Empty),
                                Artist = s.Element("Artist").ToString(string.Empty),
                                BitRate = s.Element("BitRate").ToInt(0),
                                Comments = s.Element("Comments").ToString(string.Empty),
                                Composer = s.Element("Composer").ToString(string.Empty),
                                Genre = s.Element("Genre").ToString(string.Empty),
                                Kind = s.Element("Kind").ToString(string.Empty),
                                Location = s.Element("Location").ToString(string.Empty),
                                Name = s.Element("Name").ToString(string.Empty),
                                PlayCount = s.Element("PlayCount").ToInt(0),
                                SampleRate = s.Element("SampleRate").ToInt(0),
                                Size = s.Element("Size").ToInt64(0),
                                TotalTime = s.Element("TotalTime").ToInt64(0),
                                TrackNumber = s.Element("TrackNumber").ToInt(0)
                            };
            return songs;
        }
    }

    private static class XExtensions
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
    }
}
