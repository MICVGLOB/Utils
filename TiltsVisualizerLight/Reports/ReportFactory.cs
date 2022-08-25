using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TiltsVisualizerLight.Base;

namespace TiltsVisualizerLight.Reports {
    public partial class ReportFactory {

        static ReportFactory instance;
        public static ReportFactory Create() {
            if(instance == null)
                instance = new ReportFactory();
            return instance;
        }

        protected ReportFactory() { }

        List<ReportInfoBase> reportTypes;
        public List<ReportInfoBase> ReportTypes {
            get {
                if(reportTypes == null)
                    reportTypes = CreateReportTypes();
                return reportTypes;
            }
        }
        List<ReportInfoBase> CreateReportTypes() {
            var result = new List<ReportInfoBase>();
            result.Add(new UnsafeAllReportInfo("unsafeAll", "Все опасные уклоны", unsafeAllReportTemplate));
            result.Add(new NoticeAndUnsafeAllReportInfo("noticeAndUnsafeAll", "Все предупреждения и опасные уклоны", noticeAndUnsafeAllReportTemplate));
            result.Add(new UnsafeSensorReportInfo("unsafeSensor", "Опасные уклоны выбранного датчика", unsafeSensorReportTemplate));
            result.Add(new NoticeAndUnsafeSensorReportInfo("noticeAndUnsafeSensor", "Предупреждения и опасные уклоны выбранного датчика", noticeAndUnsafeSensorReportTemplate));
            result.Add(new AllEventsSensorReportInfo("allEventsSensor", "События выбранного датчика", allEventsSensorReportTemplate));
            result.Add(new NoSignalAllReportInfo("noSignalAll", "Все потери связи", noSignalAllReportTemplate));
            result.Add(new NoSignalSensorReportInfo("noSignalSensor", "Потери связи выбранного датчика", noSignalSensorReportTemplate));
            result.Add(new StatesAllReportInfo("statesAll", "Список зарегистрированных датчиков", statesAllReportTemplate));
            return result;
        }

        public string CreateInstantReportString(TiltInfo tilt, string hexId) {
            var dateTimeFormat = "HH:mm:ss";
            var signedTemperature = string.Format("{0}{1}", tilt.Temperature > 0 ? "+" : "", tilt.Temperature.ToString());
            return string.Format("Id={0}, Наклон={1:F2}°, Направление={2:F2}°,  t={3}°C, Время создания = {4}, Время получения данных = {5}", hexId,
                tilt.Gravity, tilt.Direction, signedTemperature, tilt.CreatedTime.ToString(dateTimeFormat), tilt.ReceivedTime.ToString(dateTimeFormat));
        }
        public string CreateCalibrationProtocol(DateTime dateTime, string operatorName, int serialNumber, int modbusAddress, int minX, int maxX, int minY, int maxY, 
            int zeroX, int scaleX, int zeroY, int scaleY, int minZ, int maxZ, int zeroZ, int scaleZ) {
            var date = string.Format("{0:D2}:{1:D2}:{2}", dateTime.Day, dateTime.Month, dateTime.Year);
            var time = string.Format("{0:D2}:{1:D2}:{2:D2}", dateTime.Hour, dateTime.Minute, dateTime.Second);
            return string.Format(calibrationProtocolTemplate, date, time, operatorName, serialNumber, modbusAddress,
                "ZigBee Wireless", minX, maxX, minY, maxY, zeroX, scaleX, zeroY, scaleY, minZ, maxZ, zeroZ, scaleZ);
        }
        public void GetCalibrationCoefficients(string path, out int zeroX, out int scaleX, out int zeroY, out int scaleY, out int zeroZ, out int scaleZ) {
            var report = File.ReadAllLines(path);
            var xcoeffString = report.First(x => x.Contains("Ось Х") && x.Contains("По нулю:")).Replace("\t", "").Replace(" ", "");
            var ycoeffString = report.First(x => x.Contains("Ось Y") && x.Contains("По нулю:")).Replace("\t", "").Replace(" ", "");
            var zcoeffString = report.First(x => x.Contains("Ось Z") && x.Contains("По нулю:")).Replace("\t", "").Replace(" ", "");

            zeroX = int.Parse(new Regex(@"ю:\d{4,5}").Match(xcoeffString).Value.Replace("ю:", ""));
            zeroY = int.Parse(new Regex(@"ю:\d{4,5}").Match(ycoeffString).Value.Replace("ю:", ""));
            zeroZ = int.Parse(new Regex(@"ю:\d{4,5}").Match(zcoeffString).Value.Replace("ю:", ""));
            scaleX = int.Parse(new Regex(@"е:\d{4,5}").Match(xcoeffString).Value.Replace("е:", ""));
            scaleY = int.Parse(new Regex(@"е:\d{4,5}").Match(ycoeffString).Value.Replace("е:", ""));
            scaleZ = int.Parse(new Regex(@"е:\d{4,5}").Match(zcoeffString).Value.Replace("е:", ""));
        }
        public void ShowProtocol(string path) {
            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
            info.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;
            info.FileName = path;
            System.Diagnostics.Process.Start(info);
        }
    }
}
