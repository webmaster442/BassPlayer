using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassEngine
{
    public class ProxyConfig
    {
        public string URL { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Passwd { get; set; }
        public bool Enabled { get; set; }
        public bool RequiresAuth { get; set; }
    }
}
