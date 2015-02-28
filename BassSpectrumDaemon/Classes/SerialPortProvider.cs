using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassSpectrumDaemon.Classes
{
    internal static class SerialPortProvider
    {
        public static SerialPort ConfigurePort(string name)
        {
            SerialPort _port = new SerialPort(name);
            _port.BaudRate = 115200;
            _port.StopBits = StopBits.One;
            _port.Parity = Parity.None;
            _port.DataBits = 8;
            _port.DtrEnable = true;
            _port.Open();
            return _port;
        }

        public static string[] Ports
        {
            get { return SerialPort.GetPortNames(); }
        }
    }
}
