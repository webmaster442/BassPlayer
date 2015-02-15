using BassPlayer.Properties;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace BassPlayer.Classes
{
    [Serializable]
    public class RecentItem
    {
        public string Title { get; set; }
        public DateTime LastPlayed { get; set; }
        public int PlayCount { get; set; }
        public string FilePath { get; set; }
    }


    internal class RecentPlays: ObservableCollection<RecentItem>
    {
        public RecentPlays() : base()
        {
            DeSerialize();
        }

        private void DeSerialize()
        {
            if (string.IsNullOrEmpty(Settings.Default.RecentXML)) return;
            using (StringReader sr = new StringReader(Settings.Default.RecentXML))
            {
                this.Clear();
                XmlSerializer xs = new XmlSerializer(typeof(RecentItem[]));
                RecentItem[] loaded = (RecentItem[])xs.Deserialize(sr);
                this.AddRange(loaded);
            }
        }

        public void Save()
        {
            using (StringWriter sw = new StringWriter())
            {
                XmlSerializer xs = new XmlSerializer(typeof(RecentItem[]));
                xs.Serialize(sw, this.ToArray());
                Settings.Default.RecentXML = sw.ToString();
            }
        }

        public void Add(PlayListEntry ple)
        {
        }
    }
}
