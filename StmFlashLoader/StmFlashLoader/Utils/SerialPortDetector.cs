using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text;

namespace StmFlashLoader.Utils {
    public static class SerialPortDetector {
        public static string GetName(string portKey) {
            var result = string.Empty;
            using(var finder = new ManagementObjectSearcher("SELECT * FROM WIN32_SerialPort")) {
                string[] serialNames = SerialPort.GetPortNames();
                var ports = finder.Get().Cast<ManagementBaseObject>();
                var portPairs = (from n in serialNames
                                     join p in ports on n equals p["DeviceID"].ToString()
                                     select new Tuple<string, string>(n, p["Caption"].ToString()));
                var port = portPairs.SingleOrDefault(x => x.Item2.ToLower().Contains(portKey.ToLower()));
                if(port != null)
                    result = port.Item1;
            }
            return result;
        }
    }
}
