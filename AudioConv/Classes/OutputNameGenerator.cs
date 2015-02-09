using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AudioConv.Classes
{
    internal class OutputNameGenerator
    {
        private Dictionary<string, string>[] _lists;
        private int _counter;
        private string _pattern;
        private DateTime _dtime;

        public OutputNameGenerator(int counterstart, string pattern, DateTime time, bool multicpu)
        {
            int coreCount = 1;
            if (multicpu)
            {
                coreCount = 0;
                foreach (var item in new System.Management.ManagementObjectSearcher("Select * from Win32_Processor").Get())
                {
                    coreCount += int.Parse(item["NumberOfCores"].ToString());
                }
            }

            _lists = new Dictionary<string, string>[coreCount];
            for (int i=0; i<coreCount; i++)
            {
                _lists[i] = new Dictionary<string, string>();
            }
            _counter = counterstart;
            _pattern = pattern;
            _dtime = time;
            Cores = coreCount;
        }

        public int Cores
        {
            private set;
            get;
        }

        private string GenFileName(string fname, int cntr, string outdir)
        {
            StringBuilder ret = new StringBuilder(_pattern);
            ret.Replace("[N]", fname);
            ret.Replace("[C]", cntr.ToString());
            ret.Replace("[Y]", _dtime.Year.ToString());
            ret.Replace("[M]", _dtime.Month.ToString());
            ret.Replace("[D]", _dtime.Day.ToString());
            ret.Replace("[h]", _dtime.Hour.ToString());
            ret.Replace("[m]", _dtime.Minute.ToString());
            ret.Replace("[s]", _dtime.Second.ToString());
            if (outdir == "[input]") outdir = Path.GetDirectoryName(fname);
            return Path.Combine(outdir, ret.ToString());
        }

        public void Generate(IEnumerable<string> items, string OutDir)
        {
            int core = 0;
            int counter = _counter;
            foreach (var item in items)
            {
                _lists[core].Add(item, GenFileName(item, counter, OutDir));
                core++;
                counter++;
                if (core > _lists.Length) core = 0;
            }
        }

        public Dictionary<string, string> this[int core]
        {
            get { return _lists[core]; }
        }

        public static DateTime DateTimeFromString(string s)
        {
            var parts = s.Split('-');
            if (parts.Length < 6) throw new FormatException("invalid date & time format");
            DateTime ret = new DateTime(Convert.ToInt32(parts[0]),
                                        Convert.ToInt32(parts[1]),
                                        Convert.ToInt32(parts[2]),
                                        Convert.ToInt32(parts[3]),
                                        Convert.ToInt32(parts[4]),
                                        Convert.ToInt32(parts[5]));
            return ret;
        }
    }
}
