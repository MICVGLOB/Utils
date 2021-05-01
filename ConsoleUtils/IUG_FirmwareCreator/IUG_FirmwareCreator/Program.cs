using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace IUG_FirmwareCreator {
    class Program {
        static void Main(string[] args) {
            var creator = new Creator();
            creator.CreateBaseFirmware();
            creator.CreateFirmware();
            Console.WriteLine("Firmware(s) successfully created!");
        }
    }

    class Creator {
        string coeffStringTemplate = ":0A0FC000";
        string sourceFile = "IUG-5A.hex";
        string lastString = ":00000001FF";

        List<string> baseFirmware;
        public void CreateBaseFirmware() {
            try {
                baseFirmware = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), sourceFile)).ToList();
            } catch(Exception e) {
                Console.WriteLine("Error: " + e.Message);
                Environment.Exit(-1);
            }
            baseFirmware.RemoveAt(baseFirmware.Capacity - 1);
        }

        public void CreateFirmware() {
            GetReports().ForEach(rep => {
                int serial = 0;
                UInt32 sum = 0;
                string coeffString = string.Empty;
                try {
                    coeffString = GetReportParameters(rep, out serial);
                } catch(Exception) {
                    Console.WriteLine("Error: String with parameters not found! One or all .txt files is not a calibration report!");
                    Environment.Exit(-1);
                }
                for(int i = 1; i < coeffString.Length - 1; i += 2)
                    sum += Convert.ToUInt16(coeffString.Substring(i, 2), 16);
                coeffString = string.Format("{0}{1}", coeffString, ((byte)(~sum + 1)).ToString("X2"));
                Console.WriteLine(string.Format("Creating firmware for sensor {0}...", serial));
                WriteFirmware(coeffString, string.Format("{0}_{1}.hex", sourceFile.Replace(".hex", ""), serial));
            });
        }

        List<string> GetReports() {
            var reports = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.txt", SearchOption.TopDirectoryOnly).ToList();
            if(!reports.Any()) {
                Console.WriteLine("Error: Reports (.txt files) not found!");
                Environment.Exit(-1);
            }
            return reports;
        }

        string GetReportParameters(string path, out int serial) {
            var report = File.ReadAllLines(path);
            var serialNumberString = report.First(x => x.Contains("Серийный номер датчика:")).Replace("\t", "").Replace(" ", "");
            var xcoeffString = report.First(x => x.Contains("Ось Х") && x.Contains("По нулю:")).Replace("\t", "").Replace(" ", "");
            var ycoeffString = report.First(x => x.Contains("Ось Y") && x.Contains("По нулю:")).Replace("\t", "").Replace(" ", "");

            serial = int.Parse((new Regex(@":\d{2,6}")).Match(serialNumberString).Value.Replace(":", ""));
            int currentXOffset = int.Parse((new Regex(@"ю:\d{4,5}")).Match(xcoeffString).Value.Replace("ю:", ""));
            int currentYOffset = int.Parse((new Regex(@"ю:\d{4,5}")).Match(ycoeffString).Value.Replace("ю:", ""));
            int currentXScale = int.Parse((new Regex(@"е:\d{4,5}")).Match(xcoeffString).Value.Replace("е:", ""));
            int currentYScale = int.Parse((new Regex(@"е:\d{4,5}")).Match(ycoeffString).Value.Replace("е:", ""));

            return string.Format("{0}{1}{2}{3}{4}{5}", coeffStringTemplate, currentXOffset.ToString("X4"), currentXScale.ToString("X4"),
                    currentYOffset.ToString("X4"), currentYScale.ToString("X4"), serial.ToString("X4"));
        }

        void WriteFirmware(string coeffString, string fileName) {
            try {
                using(var writer = new StreamWriter(Path.Combine(Directory.GetCurrentDirectory(), fileName), false)) {
                    baseFirmware.ForEach(x => writer.WriteLine(x));
                    writer.WriteLine(coeffString);
                    writer.WriteLine(lastString);
                }
            } catch (Exception e) {
                Console.WriteLine("Error: " + e.Message);
                Environment.Exit(-1);
            }
        }
    }
}
