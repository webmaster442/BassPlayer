using System;
using System.Collections.Generic;
using System.IO;

namespace AudioConv.Classes
{
    internal static class CmdGenerator
    {
        private static List<string>[] _filelists;

        private static void Dispatch(IEnumerable<string> files, int cpucount)
        {
            _filelists = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            int i;

            _filelists = new List<string>[cpucount];
            for (i = 0; i < cpucount; i++) _filelists[i] = new List<string>(10);

            i = 0;
            foreach (var file in files)
            {
                _filelists[i].Add(file);
                i++;
                if (i > cpucount) i = 0;
            }
        }

        public static string OutputDirectory { get; set; }

        public static void GenerateAndRun(bool MultiCpu, IAudioConv Converter, IEnumerable<string> files)
        {
            int coreCount = 0;
            foreach (var item in new System.Management.ManagementObjectSearcher("Select * from Win32_Processor").Get())
            {
                coreCount += int.Parse(item["NumberOfCores"].ToString());
            }

            if (MultiCpu) Dispatch(files, coreCount);
            else Dispatch(files, 1);

            TextWriter[] _cmdfiles = new TextWriter[_filelists.Length];
            string[] _cmdpaths = new string[_filelists.Length];
            for (int i=0; i < _cmdfiles.Length; i++)
            {
                _cmdpaths[i] = Path.GetTempFileName() + ".cmd";
                _cmdfiles[i] = File.CreateText(_cmdpaths[i]);
            }

            for (int i=0; i<_filelists.Length; i++)
            {

            }
        }
    }
}
