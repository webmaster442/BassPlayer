using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFConverter
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
            string ret = cmdline.Replace("{input}", "\"" + input + "\"");
            string of =  Path.GetFileName(Path.ChangeExtension(input, ext));
            string outfile = Path.Combine(outfolder, of);
            ret = ret.Replace("{output}", "\"" + outfile + "\"");
            return ret;
        }

        public static void CreateBatFile(Preset p, string[] files, string filename, string outdir)
        {
            string ffmpeg = GetFFMpegPath();

            string drive = Path.GetPathRoot(ffmpeg).Replace("\\", "");
            int counter = 0;

            using (var file = File.CreateText(filename))
            {
                file.WriteLine(drive);
                file.WriteLine("cd \"{0}\"", ffmpeg);
                foreach (var f in files)
                {
                    if (counter == 0)
                    {
                        counter++;
                        continue;
                    }
                    string line = MapInputOutput(p.CommandLine, f, outdir, p.Extension);
                    file.WriteLine(line);
                    counter++;
                }
            }
        }
    }
}
