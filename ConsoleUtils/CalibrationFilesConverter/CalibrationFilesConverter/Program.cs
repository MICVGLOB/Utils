using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Word = Microsoft.Office.Interop.Word;

namespace CalibrationFilesConverter {
    class Program {
        static void Main(string[] args) {
            var converter = new Converter();
            converter.CreateReports();
            Console.WriteLine("Files(s) successfully converted!");
        }
    }

    partial class Converter {
        public void CreateReports() {
            GetDocReports().ForEach(rep => {
                int serial = 0;
                string protocolString = string.Empty;
                try {
                    protocolString = GetProtocolContent(rep, out serial);
                } catch(Exception) {
                    Console.WriteLine("Error: String with parameters not found! One or all .txt files is not a calibration report!");
                    Environment.Exit(-1);
                }
                var path = Path.Combine(Directory.GetCurrentDirectory(), string.Format("IUG_CAL_SN{0}.txt", serial));
                using(StreamWriter sw = new StreamWriter(path)) {
                    sw.WriteLine(protocolString);
                }
            });
        }
        List<string> GetDocReports() {
            var reports = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.doc", SearchOption.TopDirectoryOnly).Where(x => !x.Contains("~$")).ToList();
            if(!reports.Any()) {
                Console.WriteLine("Error: Reports (.doc files) not found!");
                Environment.Exit(-1);
            }
            return reports;
        }
        string GetProtocolContent(string path, out int serial) {
            var report = ParseDocReport(path);
            var serialNumberString = report.First(x => x.Contains("Серийный номер датчика:")).Replace("\t", "").Replace(" ", "");
            var xcoeffString = report.First(x => x.Contains("Ось Х") && x.Contains("По нулю:")).Replace("\t", "").Replace(" ", "");
            var ycoeffString = report.First(x => x.Contains("Ось Y") && x.Contains("По нулю:")).Replace("\t", "").Replace(" ", "");
            var modbusAddressString = report.First(x => x.Contains("Адрес MODBUS:")).Replace("\t", "").Replace(" ", "");
            var operatorString = report.First(x => x.Contains("Оператор:")).Replace("\t", "").Replace(" ", "");
            var dateString = report.First(x => x.Contains("Дата калибровки:")).Replace("\t", "").Replace(" ", "");
            var timeString = report.First(x => x.Contains("Время калибровки:")).Replace("\t", "").Replace(" ", "");
            var xAdcString = report.First(x => (x.Contains("Ось X") || x.Contains("Ось Х")) && x.Contains("Минимум:")).Replace("\t", "").Replace(" ", "");
            var yAdcString = report.First(x => x.Contains("Ось Y") && x.Contains("Минимум:")).Replace("\t", "").Replace(" ", "");

            serial = int.Parse((new Regex(@":\d{2,6}")).Match(serialNumberString).Value.Replace(":", ""));
            int zeroX = int.Parse((new Regex(@"ю:\d{4,5}")).Match(xcoeffString).Value.Replace("ю:", ""));
            int zeroY = int.Parse((new Regex(@"ю:\d{4,5}")).Match(ycoeffString).Value.Replace("ю:", ""));
            int scaleX = int.Parse((new Regex(@"е:\d{4,5}")).Match(xcoeffString).Value.Replace("е:", ""));
            int scaleY = int.Parse((new Regex(@"е:\d{4,5}")).Match(ycoeffString).Value.Replace("е:", ""));
            int modbusAddress = int.Parse((new Regex(@"S:\d{2,3}")).Match(modbusAddressString).Value.Replace("S:", ""));
            string operatorName = operatorString.Replace("Оператор:", "");
            string date = dateString.Replace("Датакалибровки:", "");
            string time = timeString.Replace("Времякалибровки:", "");

            int minX = int.Parse((new Regex(@"Минимум:\d{4,5}")).Match(xAdcString).Value.Replace("Минимум:", ""));
            int minY = int.Parse((new Regex(@"Минимум:\d{4,5}")).Match(yAdcString).Value.Replace("Минимум:", ""));
            int maxX = int.Parse((new Regex(@"Максимум:\d{4,5}")).Match(xAdcString).Value.Replace("Максимум:", ""));
            int maxY = int.Parse((new Regex(@"Максимум:\d{4,5}")).Match(yAdcString).Value.Replace("Максимум:", ""));
            return string.Format(calibrationProtocolTemplate, date, time, operatorName, serial, modbusAddress,
                "IUG-1, IUG-3(стандартный)", minX, maxX, minY, maxY, zeroX, scaleX, zeroY, scaleY);
        }

        string[] ParseDocReport(object fileName) {
            string[] report = null;
            try {
                object varFileName = fileName;
                object varFalseValue = false;
                object varTrueValue = true;
                object varMissing = Type.Missing;

                Word.Application wordApplication = new Word.Application();
                Word.Document document = wordApplication.Documents.Open(ref varFileName, ref varMissing,
                       ref varFalseValue,
                       ref varMissing, ref varMissing, ref varMissing, ref varMissing,
                       ref varMissing, ref varMissing, ref varMissing,
                       ref varMissing, ref varMissing, ref varMissing, ref varMissing,
                       ref varMissing, ref varMissing);
                document.Activate();
                report = document.Range().Text.Split('\r');
                document.Close();
                wordApplication.Quit();

                if(document != null)
                    Marshal.ReleaseComObject(document);
                if(wordApplication != null)
                    Marshal.ReleaseComObject(wordApplication);
                document = null;
                wordApplication = null;
                GC.Collect();
            } catch(Exception e) {
                Console.WriteLine("Error: " + e.Message);
                Environment.Exit(-1);
            }
            return report;
        }
    }
}
