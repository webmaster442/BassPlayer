using System;
using System.IO;
using Un4seen.Bass.AddOn.Tags;

namespace BassPlayer.Classes
{
    [Serializable]
    public class PlayListEntry
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public double Time { get; set; }
        public string FileName { get; set; }


        public string ArtistTitle
        {
            get
            {
                if (string.IsNullOrEmpty(Artist) && string.IsNullOrEmpty(Title))
                {
                    if (FileName.StartsWith("http://")) return FileName;
                    else return Path.GetFileName(FileName);
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
            if (filename.StartsWith("http://") || filename.StartsWith("https://"))
            {
                entry.FileName = filename;
                return entry;
            }
            entry.FileName = filename;
            try
            {
                var tags = BassTags.BASS_TAG_GetFromFile(filename);
                entry.Artist = tags.artist;
                entry.Title = tags.title;
                entry.Time = tags.duration;
            }
            catch (Exception) { }
            return entry;
        }
    }
}
