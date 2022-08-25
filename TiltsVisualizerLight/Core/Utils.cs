using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core {
    public static class Utils {
        public static void Iterate(int count, Action<int> action) {
            Enumerable.Range(0, count).ToList().ForEach(x => action.Invoke(x));
        }
    }
    public class SerialConfig {
        public string Name { get; set; }
        public int TerminalBaudrate { get; set; }
        public int ModbusBaudrate { get; set; }
        public System.IO.Ports.StopBits StopBits { get; set; }
        public System.IO.Ports.Parity Parity { get; set; }
    }
}
