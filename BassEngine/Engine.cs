using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace BassEngine
{
    /// <summary>
    /// Media type identification for engine
    /// </summary>
    public enum MediaType
    {
        /// <summary>
        /// Normal file
        /// </summary>
        File,
        /// <summary>
        /// Network stream
        /// </summary>
        Stream,
        /// <summary>
        /// CD Track
        /// </summary>
        CD
    }

    public static class Extensisons
    {
        /// <summary>
        /// Converts a Timespan to a nice formated string
        /// </summary>
        /// <param name="ts">timespan to format</param>
        /// <returns>returns timespan in the folllowing format: hh:mm:ss</returns>
        public static string ToShortTime(this TimeSpan ts)
        {
            return string.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
        }

        /// <summary>
        /// Converts an IEnumerable to an Observable collection
        /// </summary>
        /// <typeparam name="T">Type of elements</typeparam>
        /// <param name="coll">an IEnumerable collection</param>
        /// <returns>The elements in an ObservableCollection</returns>
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> coll)
        {
            var c = new ObservableCollection<T>();
            foreach (var e in coll) c.Add(e);
            return c;
        }

        /// <summary>
        /// Apends elements from an IEnumerable collection to an observable collection
        /// </summary>
        /// <typeparam name="T">Type of elements</typeparam>
        /// <param name="collection">The ObserbableCollection to apend to</param>
        /// <param name="elements">an IEnumerable collection</param>
        public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> elements)
        {
            if (elements == null) return;
            foreach (var e in elements) collection.Add(e);
        }
    }

    /// <summary>
    /// Various helper functions
    /// </summary>
    public static class Helpers
    {
        public static void ErrorDialog(Exception ex, string description = null)
        {
            Dispatcher.CurrentDispatcher.Invoke(() =>
                {
                    if (description != null)
                    {
                        MessageBox.Show(string.Format("{0}\r\nDetails:{1}", description, ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
        }

        public static string Arguments(string[] args)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var arg in args)
            {
                sb.AppendFormat("\"{0}\" ", arg);
            }
            return sb.ToString();
        }

        public static WebClient CreateWebClient(ProxyConfig proxy)
        {
            WebClient client = new WebClient();
            if (proxy.Enabled)
            {
                WebProxy p = new WebProxy(proxy.URL, proxy.Port);
                if (proxy.RequiresAuth)
                {
                    p.Credentials = new NetworkCredential(proxy.Username, proxy.Passwd);
                }
                client.Proxy = p;
            }
            return client;
        }
    }
}
