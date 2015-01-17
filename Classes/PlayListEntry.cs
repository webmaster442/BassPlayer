using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Un4seen.Bass.AddOn.Tags;

namespace BassPlayer.Classes
{
    [Serializable]
    internal class PlayListEntry
    {

        public string Title { get; set; }
        public string Artist { get; set; }
        public double Time { get; set; }
        public string File { get; set; }


        public string ArtistTitle
        {
            get { return string.Format("{0} - {1}", Artist, Title); }
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
            var tags = BassTags.BASS_TAG_GetFromFile(filename);
            entry.Artist = tags.artist;
            entry.Title = tags.title;
            entry.Time = tags.duration;
            entry.File = filename;
            return entry;
        }
    }
}
