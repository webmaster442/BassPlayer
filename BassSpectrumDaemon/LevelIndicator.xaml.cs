using System.Windows.Controls;
using Un4seen.Bass;

namespace BassSpectrumDaemon
{
    /// <summary>
    /// Interaction logic for LevelIndicator.xaml
    /// </summary>
    public partial class LevelIndicator : UserControl
    {
        public LevelIndicator()
        {
            InitializeComponent();
            PbR.Maximum = ushort.MaxValue;
            PbL.Maximum = ushort.MaxValue;
        }

        public int Level
        {
            get { return 0; }
            set
            {
                int l = Utils.LowWord32(value);
                int r = Utils.HighWord32(value);
                PbL.Value = l;
                PbR.Value = r;
            }
        }
    }
}
