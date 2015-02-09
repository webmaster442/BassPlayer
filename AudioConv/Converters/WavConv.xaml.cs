using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AudioConv.Classes;

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
