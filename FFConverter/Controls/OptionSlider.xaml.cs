using System;
using System.Collections.Generic;
using System.Windows.Controls;

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
                        break;
                }
            }
        }
    }
}
