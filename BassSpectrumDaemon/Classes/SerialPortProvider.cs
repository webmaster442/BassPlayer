using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassSpectrumDaemon.Classes
{
    internal enum Messages: byte
    {
        Spectrum = 0x02,
        SpectrumInverse = 0x85, 
        Time = 0x57,
        Level = 0x49,
    }

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

        public static byte[] TimePacket
        {
            get
            {
                byte[] data = new byte[34];
                data[0] = (byte)Messages.Time;
                data[1] = (byte)(DateTime.Now.Year - 2000);
                data[2] = (byte)DateTime.Now.Month;
                data[3] = (byte)DateTime.Now.Day;
                data[4] = (byte)DateTime.Now.Hour;
                data[5] = (byte)DateTime.Now.Minute;
                data[6] = (byte)DateTime.Now.Second;
                data[33] = 255;
                return data;
            }
        }
    }
}
