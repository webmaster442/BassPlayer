using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using FFConverter.Controls;

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
            string[] parts;
            foreach (var t in tags)
            {
                if (!Regex.IsMatch(t, pattern)) continue;
                parts = t.Replace("{", "").Replace("}", "").Split(' ');
                switch (parts[0])
                {
                }
                matches++;
            }
            if (matches == 2)
            {
                TextBlock t = new TextBlock();
                t.Text = "Current Preset has no options";
                target.Children.Add(t);
            }
        }

        public static string CompileUiToString(Preset p, StackPanel target)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(p.CommandLine);
            var items = target.Children.OfType<IPresetControl>();
            foreach (var item in items)
            {
                sb.Replace(item.InputPattern, item.GeneratePattern());
            }
            return sb.ToString();
        }
    }
}
