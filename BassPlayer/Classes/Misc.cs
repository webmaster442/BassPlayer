using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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

    [ValueConversion(typeof(int), typeof(bool?))]
    internal class EnableConverter: IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int val = System.Convert.ToInt32(value);
            if (val == 0) return true;
            else return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
