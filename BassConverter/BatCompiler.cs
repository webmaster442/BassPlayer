using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassConverter
{
    internal static class BatCompiler
    {
        private static string GetFFMpegPath()
        {
            string enginedir = System.AppDomain.CurrentDomain.BaseDirectory;
            if (IntPtr.Size == 8) return Path.Combine(enginedir, @"Engine\x64");
            else return Path.Combine(enginedir, @"Engine\x86");
        }

        private static string MapInputOutput(string cmdline, string input, string outfolder, string ext)
        {
            string ret = cmdline.Replace("{input}", input);
            string outfile = Path.Combine(outfolder, Path.ChangeExtension(input, ext));
            ret = ret.Replace("{output}", outfile);
            return ret;
        }

        public static void CreateBatFile(Preset p, string[] files, string filename, string outdir)
        {
            string ffmpeg = GetFFMpegPath();

            using (var file = File.CreateText(filename))
            {
                foreach (var f in files)
                {
                    string line = string.Format("{0}\\{1}", ffmpeg, MapInputOutput(p.CommandLine, f, outdir, p.Extension));
                    file.WriteLine(line);
                }
            }
        }
    }
}
