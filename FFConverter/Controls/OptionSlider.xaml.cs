using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Linq;

namespace FFConverter.Controls
{
    /// <summary>
    /// Interaction logic for OptionSlider.xaml
    /// </summary>
    public partial class OptionSlider : UserControl, IPresetControl
    {
        public OptionSlider()
        {
            InitializeComponent();
        }

        public string InputPattern
        {
            get;
            set;
        }

        public string GeneratePattern()
        {
            return SValue.Value.ToString();
        }

        public void SetupFromTokens(Dictionary<string, string> Tokens)
        {
            foreach (var token in Tokens)
            {
                switch (token.Key)
                {
                    case "min":
                        SValue.Minimum = Convert.ToDouble(token.Value);
                        break;
                    case "max":
                        SValue.Maximum = Convert.ToDouble(token.Value);
                        break;
                    case "val":
                        SValue.Value = Convert.ToDouble(token.Value);
                        break;
                    case "text":
                        TbDescription.Text = token.Value;
                        break;
                    case "stops":
                        var list = (from i in token.Value.Split(';') select Convert.ToDouble(i)).ToArray();
                        var minval = list.Min();
                        var maxval = list.Max();
                        if (SValue.Minimum > minval) SValue.Minimum = minval;
                        if (SValue.Maximum < maxval) SValue.Maximum = maxval;
                        SValue.IsSnapToTickEnabled = true;
                        SValue.Ticks = new System.Windows.Media.DoubleCollection(list);
                        break;
                }
            }
        }
    }
}
