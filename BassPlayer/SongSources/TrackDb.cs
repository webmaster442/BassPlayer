using BassEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Xml.Serialization;

namespace BassPlayer.SongSources
{
    /// <summary>
    /// Simple XML NOSQL database storage
    /// </summary>
    class TrackDb: List<TrackData>
    {
        /// <summary>
        /// Query subtype
        /// </summary>
        public enum QueryType
        {
            Album, Artist, Year, Genre, All, Search
        }

        private string _file;

        private bool _needsave;

        public TrackDb() : base()
        {
            string profile = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            _file = Path.Combine(profile, "BassPlayerLib.xml");
            if (File.Exists(_file)) Load();
            _needsave = false;
        }

        /// <summary>
        /// Loads the database
        /// </summary>
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

        /// <summary>
        /// Save database
        /// </summary>
        public void Save()
        {
            if (!_needsave) return;
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

        /// <summary>
        /// Gets the artists in the db
        /// </summary>
        public string[] Artists
        {
            get
            {
                return (from i in this.AsParallel() 
                        orderby i.Artist ascending
                        select i.Artist).Distinct().ToArray();
            }
        }

        /// <summary>
        /// Gets the albumns in the db
        /// </summary>
        public string[] Albums
        {
            get
            {
                return (from i in this.AsParallel()
                        orderby i.Album ascending
                        select i.Album).Distinct().ToArray();
            }
        }

        /// <summary>
        /// Gets the years in the db
        /// </summary>
        public string[] Years
        {
            get
            {
                return (from i in this.AsParallel()
                        orderby i.Year descending
                        select i.Year).Distinct().Select(x => x.ToString()).ToArray();
            }
        }

        /// <summary>
        /// Gets the genres in the db
        /// </summary>
        public string[] Genres
        {
            get
            {
                return (from i in this.AsParallel()
                        orderby i.Genre ascending
                        select i.Genre).Distinct().ToArray();
            }
        }

        /// <summary>
        /// Returns specific tracks that mach the query in the db
        /// </summary>
        /// <param name="type">Query type</param>
        /// <param name="parameter">Query parameters</param>
        /// <returns>an array of tracks that match the citeria</returns>
        public PlayListEntry[] Query(QueryType type, string parameter)
        {
            if (string.IsNullOrEmpty(parameter)) return null;
            switch (type)
            {
                case QueryType.Album:
                    return (from track in this.AsParallel()
                            where track.Album == (string)parameter
                            orderby track.Disc, track.Track, track.Artist, track.Title
                            select (PlayListEntry)track).ToArray();
                case QueryType.Artist:
                    return (from track in this.AsParallel()
                            where track.Artist == (string)parameter
                            orderby track.Title
                            select (PlayListEntry)track).ToArray();
                case QueryType.Genre:
                    return (from track in this.AsParallel()
                            where track.Genre == (string)parameter
                            orderby track.Artist, track.Title
                            select (PlayListEntry)track).ToArray();
                case QueryType.Year:
                    return (from track in this.AsParallel()
                            where track.Year == Convert.ToUInt32(parameter)
                            orderby track.Artist, track.Title
                            select (PlayListEntry)track).ToArray();
                case QueryType.All:
                    return (from track in this.AsParallel()
                            orderby track.Artist ascending, track.Title ascending
                            select (PlayListEntry)track).ToArray();
                case QueryType.Search:
                    return (from track in this.AsParallel()
                            where track.Title.Contains(parameter) ||
                                  track.Artist.Contains(parameter) ||
                                  track.Album.Contains(parameter) ||
                                  track.Genre.Contains(parameter)
                            orderby track.Artist, track.Album, track.Title, track.Genre
                            select (PlayListEntry)track).ToArray();
                default:
                    return null;
            }
        }

        /// <summary>
        /// Deletes items from the database
        /// </summary>
        /// <param name="entrys">PlaylistEntry to remove</param>
        public void DeleteItems(IEnumerable<PlayListEntry> entrys)
        {
            var query = (from i in this
                         from j in entrys
                         where i.File == j.FileName
                         select i).ToArray();
            foreach (var item in query) this.Remove(item);
            _needsave = true;
        }

        /// <summary>
        /// Adds files to the DB
        /// </summary>
        /// <param name="files">Files to process</param>
        /// <returns>A processing task</returns>
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
                    _needsave = true;
                });
            });
        }

        /// <summary>
        /// Backup Database
        /// </summary>
        public void DoBackup()
        {
            System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
            sfd.FileName = string.Format("BassPlayerLibBackup-{0}-{1}-{2}.xml.gz", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    using (var file = File.Create(sfd.FileName))
                    {
                        using (var compress = new GZipStream(file, CompressionMode.Compress))
                        {
                            XmlSerializer xs = new XmlSerializer(typeof(TrackData[]));
                            xs.Serialize(compress, this.ToArray());
                            MessageBox.Show("Backup complete", "Backup", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Helpers.ErrorDialog(ex, "Backup failed");
                }
            }
        }

        /// <summary>
        /// Restores database
        /// </summary>
        public void DoResore()
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "XML Backups|*.xml.gz";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    using (var file = File.OpenRead(ofd.FileName))
                    {
                        using (var compress = new GZipStream(file, CompressionMode.Decompress))
                        {
                            this.Clear();
                            XmlSerializer xs = new XmlSerializer(typeof(TrackData[]));
                            var array = (TrackData[])xs.Deserialize(compress);
                            this.AddRange(array);
                            this.Save();
                            MessageBox.Show("Restore complete", "Backup", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Helpers.ErrorDialog(ex, "Restore failed");
                }
            }
        }
    }
}
