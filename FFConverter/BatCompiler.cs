using System;
using System.IO;

namespace FFConverter
{
    /// <summary>
    /// Creates CMD files that can be run
    /// </summary>
    internal static class BatCompiler
    {
        /// <summary>
        /// Replaces {input} and {output} tags in a command line to valid filenames
        /// </summary>
        /// <param name="cmdline">Command line</param>
        /// <param name="input">input file path</param>
        /// <param name="outfolder">output folder path</param>
        /// <param name="ext">Output extension</param>
        /// <returns>A command line string with apropiate input & output files</returns>
        private static string MapInputOutput(string cmdline, string input, string outfolder, string ext)
        {
            string ret = cmdline.Replace("{input}", "\"" + input + "\"");
            string of =  Path.GetFileName(Path.ChangeExtension(input, ext));
            string outfile = Path.Combine(outfolder, of);
            ret = ret.Replace("{output}", "\"" + outfile + "\"");
            return ret;
        }

        /// <summary>
        /// Creates a bat/cmd file
        /// </summary>
        /// <param name="p">a preset</param>
        /// <param name="files">an array of filenames</param>
        /// <param name="filename">Output filename</param>
        /// <param name="outdir">Output directory</param>
        /// <param name="deleteself">delete cmd file after execution</param>
        public static void CreateBatFile(Preset p, string[] files, string filename, string outdir, bool deleteself = false)
        {
            string ffmpeg = Path.GetDirectoryName(FFConverter.Properties.Settings.Default.FFmpegPath);

            string drive = Path.GetPathRoot(ffmpeg).Replace("\\", "");
            int counter = 0;

            using (var file = File.CreateText(filename))
            {
                file.WriteLine(drive);
                file.WriteLine("cd \"{0}\"", ffmpeg);
                foreach (var f in files)
                {
                    string line = MapInputOutput(p.CommandLine, f, outdir, p.Extension);
                    file.WriteLine(line);
                    counter++;
                }
                if (deleteself)
                {
                    file.WriteLine("PAUSE");
                    file.WriteLine("DEL \"%~f0\"");
                }
            }
        }
    }
}
