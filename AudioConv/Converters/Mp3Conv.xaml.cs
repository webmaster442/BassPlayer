using AudioConv.Classes;
using System.Windows.Controls;

namespace AudioConv.Converters
{
    /// <summary>
    /// Interaction logic for Mp3Conv.xaml
    /// </summary>
    public partial class Mp3Conv : UserControl, IAudioConv
    {
        public Mp3Conv()
        {
            SampleRate = SampleRates.NoChange;
            Channels = Classes.Channels.NoChange;
            BitRate = 192;
            Quality = 0;
            ConstantRate = true;
            InitializeComponent();
        }

        public SampleRates SampleRate
        {
            get;
            set;
        }

        public Channels Channels
        {
            get;
            set;
        }

        public int BitRate
        {
            get;
            set;
        }

        public int Quality
        {
            get;
            set;
        }

        public bool ConstantRate
        {
            get;
            set;
        }

        public string Extension
        {
            get { return "mp3"; }
        }

        public string GetCommandLine()
        {
            string rate = string.Format("{0}k");
            if (ConstantRate)
            {
                return string.Format("ffmpeg.exe -i \"[input]\" -vn {0} {1} -codec:a libmp3lame -b:a {2}  \"[output].{3}\"",
                                     ConvHelpers.GetSampleRate(SampleRate),
                                     ConvHelpers.GetChannels(Channels),
                                     rate,
                                     Extension);
            }
            else
            {
                return string.Format("ffmpeg.exe -i \"[input]\" -vn {0} {1} -codec:a libmp3lame -q:a {2}  \"[output].{3}\"",
                                     ConvHelpers.GetSampleRate(SampleRate),
                                     ConvHelpers.GetChannels(Channels),
                                     Quality.ToString(),
                                     Extension);
            }
        }
    }
}
