using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace FFConverter.Controls
{
    /// <summary>
    /// Interaction logic for OptionCombo.xaml
    /// </summary>
    public partial class OptionCombo : UserControl, IPresetControl
    {
        private string[] _values;

        public OptionCombo()
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
            if (_values == null) return "";
            string retval = _values[CbOptions.SelectedIndex];
            if (retval == "*") return "";
            return retval;
        }

        public void SetupFromTokens(Dictionary<string, string> Tokens)
        {
            foreach (var token in Tokens)
            {
                switch (token.Key)
                {
                    case "texts":
                        string[] items = token.Value.Split(';');
                        foreach (var item in items) CbOptions.Items.Add(item);
                        break;
                    case "values":
                        _values = token.Value.Split(';');
                        break;
                    case "default":
                        int index = Convert.ToInt32(token.Value);
                        if (index < CbOptions.Items.Count && index > 0) CbOptions.SelectedIndex = index;
                        break;
                }
            }
        }
    }
}
