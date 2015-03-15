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
        private string _unit;

        public OptionSlider()
        {
            InitializeComponent();
            _unit = "";
        }

        public string InputPattern
        {
            get;
            set;
        }

        public string GeneratePattern()
        {
            return string.Format("{0}{1}", SValue.Value, _unit);
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
                    case "step":
                        SValue.TickFrequency = Convert.ToDouble(token.Value);
                        SValue.IsSnapToTickEnabled = true;
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
                    case "unit":
                        _unit = token.Value;
                        break;
                }
            }
        }
    }
}
