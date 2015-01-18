using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Un4seen.Bass.AddOn.Tags;

namespace BassPlayer.Classes
{
    [Serializable]
    public class PlayListEntry
    {

        public string Title { get; set; }
        public string Artist { get; set; }
        public double Time { get; set; }
        public string File { get; set; }


        public string ArtistTitle
        {
            get
            {
                if (string.IsNullOrEmpty(Artist) && string.IsNullOrEmpty(Title))
                {
                    if (File.StartsWith("http://")) return File;
                    else return Path.GetFileName(File);
                }
                return string.Format("{0} - {1}", Artist, Title);
            }
        }

        public string TimeString
        {
            get
            {
                TimeSpan ts = TimeSpan.FromSeconds(Time);
                return ts.ToShortTime();
            }
        }

        public static PlayListEntry FromFile(string filename)
        {
            PlayListEntry entry = new PlayListEntry();
            if (filename.StartsWith("http://"))
            {
                entry.File = filename;
                return entry;
            }
            var tags = BassTags.BASS_TAG_GetFromFile(filename);
            entry.Artist = tags.artist;
            entry.Title = tags.title;
            entry.Time = tags.duration;
            entry.File = filename;
            return entry;
        }
    }
}
