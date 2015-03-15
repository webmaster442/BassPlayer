using FFConverter.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace FFConverter
{
    /// <summary>
    /// Preset compilation stuff
    /// </summary>
    internal static class PresetCompiler
    {
        private const string pattern = @"\{(.*?)\}\s";
        private const string attrs = "\\w+\\=\".+\"";

        private static Dictionary<string, string> GetAtttrs(string[] parts)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            for (int i = 1; i < parts.Length; i++ )
            {
                if (Regex.IsMatch(parts[i], attrs))
                {
                    string[] r = parts[i].Replace("\"", "").Split('=');
                    dict.Add(r[0], r[1]);
                }
            }
            return dict;
        }

        /// <summary>
        /// Creates a UI from a given preset
        /// </summary>
        /// <param name="p">The preset</param>
        /// <param name="target">Render target stackpanel</param>
        public static void CompileToUi(Preset p, StackPanel target)
        {
            target.Children.Clear();
            int matches = 0;
            string[] tags = Regex.Split(p.CommandLine, pattern);
            string[] parts;
            Dictionary<string, string> parameters;
            foreach (var t in tags)
            {
                if (string.IsNullOrEmpty(t)) continue;
                parts = t.Split(' ');
                switch (parts[0])
                {
                    case "slider":
                        parameters = GetAtttrs(parts);
                        OptionSlider opt = new OptionSlider();
                        opt.InputPattern = "{" + t + "}";
                        opt.SetupFromTokens(parameters);
                        target.Children.Add(opt);
                        matches++;
                        break;
                }
            }
            if (matches < 1)
            {
                TextBlock t = new TextBlock();
                t.Text = "Current Preset has no options";
                target.Children.Add(t);
            }
        }

        /// <summary>
        /// Compiles Ui options to a command string
        /// </summary>
        /// <param name="p">Preset</param>
        /// <param name="target">Source stackapanel</param>
        /// <returns>command string</returns>
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
