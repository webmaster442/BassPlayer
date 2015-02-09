using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AudioConv.Classes
{
    internal class CmdGenerator
    {
        private OutputNameGenerator _namegen;

        public CmdGenerator(bool usemulticpu, string pattern, DateTime time, int counter)
        {
            _namegen = new OutputNameGenerator(counter, pattern, time, usemulticpu);
        }

        public void GenerateAndRunCmds(IEnumerable<string> files, IAudioConv converter, string outdir)
        {
            _namegen.Generate(files, outdir);
            int i = 0;
            string[] tempnames = new string[_namegen.Cores];
            StreamWriter[] tempfiles = new StreamWriter[_namegen.Cores];

            for (i = 0; i < _namegen.Cores; i++)
            {
                tempnames[i] = Path.GetTempFileName() + ".cmd";
                tempfiles[i] = File.CreateText(tempnames[i]);
            }

            for (i = 0; i < tempfiles.Length; i++)
            {
                foreach (var item in _namegen[i])
                {
                    string line = converter.GetCommandLine();
                    line = line.Replace("[input]", item.Key);
                    line = line.Replace("[output]", Path.Combine(outdir, item.Value));
                    tempfiles[i].WriteLine(line);
                }
                //self delete after run
                tempfiles[i].WriteLine("DEL \"%~f0\"");
            }

            for (i = 0; i < _namegen.Cores; i++)
            {
                tempfiles[i].Close();
            }

        }
    }
}
