using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Text.RegularExpressions;

namespace FFConverter
{
    internal static class PresetCompiler
    {
        private const string pattern = @"\{.*\}";

        public static void CompileToUi(Preset p, StackPanel target)
        {
            target.Children.Clear();
            int matches = 0;
            string[] tags = p.CommandLine.Split(' ');
            foreach (var t in tags)
            {
                if (!Regex.IsMatch(t, pattern)) continue;
                matches++;
            }
            if (matches == 2)
            {
                TextBlock t = new TextBlock();
                t.Text = "Current Preset has no options";
                target.Children.Add(t);
            }
        }
    }
}
