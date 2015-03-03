using System;
using System.IO;
using System.Windows.Data;

namespace BassEngine.Converters
{
    /// <summary>
    /// Gets file name of a path
    /// </summary>
    [ValueConversion(typeof(string), typeof(string))]
    public class FileNameConverter : IValueConverter
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
    public class TimeConverter : IValueConverter
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
    public class PercentConveter : IValueConverter
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
    public class EnableConverter : IValueConverter
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
    public class TextShorter : IValueConverter
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

    /// <summary>
    /// Converts last played date to a time relative to current time
    /// </summary>
    [ValueConversion(typeof(DateTime), typeof(string))]
    public class LastPlayedConv : IValueConverter
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

    /// <summary>
    /// Negates a bool? value during conversion 
    /// </summary>
    [ValueConversion(typeof(bool?), typeof(bool?))]
    public class NegateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return null;
            bool? val = (bool?)value;
            return !val;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return null;
            bool? val = (bool?)value;
            return !val;
        }
    }
}
