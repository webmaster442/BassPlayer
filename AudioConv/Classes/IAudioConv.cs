using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace AudioConv.Classes
{

    /// <summary>
    /// Sample rates Enumeration
    /// </summary>
    public enum SampleRates
    {
        NoChange = -1,
        Rate44100 = 44100,
        Rate48000 = 48000
    }

    /// <summary>
    /// Channels Enumeration
    /// </summary>
    public enum Channels
    {
        NoChange = -1,
        Ch1 = 1,
        Ch2 = 2,
        Ch4 = 4,
        Ch6 = 6,
        Ch8 = 8
    }

    /// <summary>
    /// Audio converter interface
    /// </summary>
    public interface IAudioConv
    {
        /// <summary>
        /// Sample rate
        /// </summary>
        SampleRates SampleRate { get; set; }

        /// <summary>
        /// Channels
        /// </summary>
        Channels Channels { get; set; }

        /// <summary>
        /// codec defautlt extension
        /// </summary>
        string Extension { get; }

        /// <summary>
        /// Audio converter command line
        /// </summary>
        /// <returns>a command line string that can be processed by cmd.exe</returns>
        string GetCommandLine();
    }

    /// <summary>
    /// Conversion helper functions
    /// </summary>
    public static class ConvHelpers
    {
        /// <summary>
        /// Converts sample rate to command string
        /// </summary>
        /// <param name="rate">Sample rate</param>
        /// <returns>sample rate as string</returns>
        public static string GetSampleRate(SampleRates rate)
        {
            if (rate == SampleRates.NoChange) return "";
            else
            {
                int value = (int)rate;
                return string.Format("-ar {0}", value);
            }
        }

        /// <summary>
        /// Converts channels to command string
        /// </summary>
        /// <param name="ch">channels</param>
        /// <returns><channels as string/returns>
        public static string GetChannels(Channels ch)
        {
            if (ch == Channels.NoChange) return "";
            else
            {
                int value = (int)ch;
                return string.Format("-ac {0}", value);
            }
        }
    }
}
