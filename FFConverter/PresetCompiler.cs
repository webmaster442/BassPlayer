using FFConverter.Controls;
using System;
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

        private static Dictionary<string, string> GetAtttrs(string[] parts)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            for (int i = 0; i < parts.Length; i++ )
            {
                if (!parts[i].EndsWith("=")) continue;
                if (i + 1 < parts.Length)
                {
                    dict.Add(parts[i].Replace("=", ""), parts[i + 1]);
                }
            }
            return dict;
        }

        public static string[] SplitArguments(string commandLine)
        {
            var parmChars = commandLine.ToCharArray();
            var inSingleQuote = false;
            var inDoubleQuote = false;
            for (var index = 0; index < parmChars.Length; index++)
            {
                if (parmChars[index] == '"' && !inSingleQuote)
                {
                    inDoubleQuote = !inDoubleQuote;
                    parmChars[index] = '\n';
                }
                if (parmChars[index] == '\'' && !inDoubleQuote)
                {
                    inSingleQuote = !inSingleQuote;
                    parmChars[index] = '\n';
                }
                if (!inSingleQuote && !inDoubleQuote && parmChars[index] == ' ')
                    parmChars[index] = '\n';
            }
            return (new string(parmChars)).Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
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
                parts = SplitArguments(t);
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
                    case "combo":
                        parameters = GetAtttrs(parts);
                        OptionCombo combo = new OptionCombo();
                        combo.InputPattern = "{" + t + "}";
                        combo.SetupFromTokens(parameters);
                        target.Children.Add(combo);
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
