using AudioConv.Classes;
using System.Windows.Controls;

namespace AudioConv.Converters
{
    /// <summary>
    /// Interaction logic for WavConv.xaml
    /// </summary>
    public partial class WavConv : UserControl, IAudioConv
    {
        public WavConv()
        {
            SampleRate = SampleRates.NoChange;
            Channels = Classes.Channels.NoChange;
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

        public string Extension
        {
            get { return "wav"; }
        }

        public string GetCommandLine()
        {
            return string.Format("ffmpeg.exe -i \"[input]\" -vn {0} {1} \"[output].{2}\"", ConvHelpers.GetSampleRate(SampleRate),
                                                                                           ConvHelpers.GetChannels(Channels),
                                                                                           Extension);
        }
    }
}
