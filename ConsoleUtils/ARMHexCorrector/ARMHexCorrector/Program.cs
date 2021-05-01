using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace ARMHexCorrector {
    class Program {
        static void Main(string[] args) {
            if(args.Count() < 4)
                throw new Exception("Incorrect sequence of input parameters");
            string inputFileName = args[0];
            string outputFileName = args[1];
            UInt32 targetCRC = Convert.ToUInt32(args[2], 16);
            var data = File.ReadAllBytes(inputFileName).ToList();
            while (data.Count() < Convert.ToUInt32(args[3], 16) - 4) {
                data.Add(0xff);
            }
            data.AddRange(CRC.CreatePatch(CRC.CalculateCRC32(data), targetCRC));
            File.WriteAllBytes(outputFileName, data.ToArray());
            Console.WriteLine("File sucessfully created!");
        }
    }
}
