using BassPlayer.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace BassPlayer.Classes
{
    /// <summary>
    /// Media type identification for engine
    /// </summary>
    internal enum MediaType
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

    internal static class Extensisons
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
    internal static class Helpers
    {
        public static void ErrorDialog(Exception ex, string description = null)
        {
            if (description != null)
            {
                MessageBox.Show(string.Format("{0}\r\nDetails:{1}", description, ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        public static WebClient CreateClient()
        {
            WebClient client = new WebClient();
            if (Properties.Settings.Default.ProxyEnabled)
            {
                WebProxy proxy = new WebProxy(Settings.Default.ProxyAddress, Convert.ToInt32(Settings.Default.ProxyPort));
                proxy.Credentials = new NetworkCredential(Settings.Default.ProxyUser, Settings.Default.ProxyPassword);
                client.Proxy = proxy;
            }
            return client;
        }
    }

    /// <summary>
    /// Gets file name of a path
    /// </summary>
    [ValueConversion(typeof(string), typeof(string))]
    internal class FileNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return null;
            string name = value.ToString();
            return Path.GetFileName(name);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

    /// <summary>
    /// seconds to time string converter
    /// </summary>
    [ValueConversion(typeof(double), typeof(string))]
    internal class TimeConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return null;
            double val = System.Convert.ToDouble(value);
            TimeSpan ts = TimeSpan.FromSeconds(val);
            return ts.ToShortTime();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// double formated as %
    /// </summary>
    [ValueConversion(typeof(double), typeof(string))]
    internal class PercentConveter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return null;
            double val = System.Convert.ToDouble(value);
            return val.ToString("P");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// Menu enabler based on tab index
    /// </summary>
    [ValueConversion(typeof(int), typeof(bool?))]
    internal class EnableConverter: IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int val = System.Convert.ToInt32(value);
            int par = System.Convert.ToInt32(parameter);
            return val == par;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// A text shortener converter
    /// </summary>
    [ValueConversion(typeof(string), typeof(string))]
    internal class TextShorter: IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string v = System.Convert.ToString(value);
            if (v.Length < 80) return v;
            int len = v.Length - 80;
            return v.Substring(0, len) + " ...";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

    [ValueConversion(typeof(DateTime), typeof(string))]
    internal class LastPlayedConv: IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return "";
            DateTime date = System.Convert.ToDateTime(value);
            var diff = DateTime.Now - date;
            if (diff.TotalDays > 1) return string.Format("{0:0.0} days ago", diff.TotalDays);
            else if (diff.TotalHours > 1) return string.Format("{0:0.0} hr. ago", diff.TotalHours);
            else if (diff.TotalMinutes > 1) return string.Format("{0:0.0} min. ago", diff.TotalMinutes);
            else if (diff.TotalSeconds > 1) return string.Format("{0:0.0} sec. ago", diff.TotalSeconds);
            else return "Just now";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return DateTime.Now;
        }
    }
}
