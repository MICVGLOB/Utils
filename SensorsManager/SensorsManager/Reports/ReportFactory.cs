using Mvvm.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SensorsManager.Reports {
    public partial class ReportFactory {

        public string CreateCalibrationProtocol(DateTime dateTime, string operatorName, int serialNumber, int modbusAddress,
            SensorType sensorType, int minX, int maxX, int minY, int maxY, int zeroX, int scaleX, int zeroY, int scaleY) {
            var date = string.Format("{0:D2}:{1:D2}:{2}", dateTime.Day, dateTime.Month, dateTime.Year);
            var time = string.Format("{0:D2}:{1:D2}:{2:D2}", dateTime.Hour, dateTime.Minute, dateTime.Second);
            return string.Format(calibrationProtocolTemplate, date, time, operatorName, serialNumber, modbusAddress,
                GetSensorTypeString(sensorType), minX, maxX, minY, maxY, zeroX, scaleX, zeroY, scaleY);
        }
        public string CreateReadLevelCoeffProtocol(DateTime dateTime, string operatorName, int serialNumber, int modbusAddress,
            SensorType sensorType, double lLev, double r, double delta, double coeff, int h, double angle, int overLevel, double aL, double bL,
            int zeroX, int scaleX, int zeroY, int scaleY, string recordDate) {

            var date = string.Format("{0:D2}:{1:D2}:{2}", dateTime.Day, dateTime.Month, dateTime.Year);
            var time = string.Format("{0:D2}:{1:D2}:{2:D2}", dateTime.Hour, dateTime.Minute, dateTime.Second);
            return string.Format(levelCoeffProtocolTemplate, date, time, operatorName, serialNumber, modbusAddress,
                GetSensorTypeString(sensorType), lLev, r, delta, coeff, h, angle, overLevel, aL, bL, zeroX, scaleX, zeroY, scaleY, recordDate, "КОЭФФИЦИЕНТЫ ИЗМЕРИТЕЛЯ УРОВНЯ", "чтения");
        }
        public string CreateWriteLevelCoeffProtocol(DateTime dateTime, string operatorName, int serialNumber, int modbusAddress,
            SensorType sensorType, double lLev, double r, double delta, double coeff, int h, double angle, int overLevel, double aL, double bL,
            int zeroX, int scaleX, int zeroY, int scaleY, string recordDate) {

            var date = string.Format("{0:D2}:{1:D2}:{2}", dateTime.Day, dateTime.Month, dateTime.Year);
            var time = string.Format("{0:D2}:{1:D2}:{2:D2}", dateTime.Hour, dateTime.Minute, dateTime.Second);
            return string.Format(levelCoeffProtocolTemplate, date, time, operatorName, serialNumber, modbusAddress,
                GetSensorTypeString(sensorType), lLev, r, delta, coeff, h, angle, overLevel, aL, bL, zeroX, scaleX, zeroY, scaleY, recordDate, "ПРОТОКОЛ ЗАПИСИ В ИЗМЕРИТЕЛЬ УРОВНЯ", "записи");
        }
        public string CreateReadVelocityCoeffProtocol(DateTime dateTime, string operatorName, int serialNumber, int modbusAddress,
            SensorType sensorType, double lVel, double m, double width, double g, int h, double angle, int RLiq, double RMet, double bs,
            int zeroX, int scaleX, int zeroY, int scaleY, string recordDate) {

            var date = string.Format("{0:D2}:{1:D2}:{2}", dateTime.Day, dateTime.Month, dateTime.Year);
            var time = string.Format("{0:D2}:{1:D2}:{2:D2}", dateTime.Hour, dateTime.Minute, dateTime.Second);
            return string.Format(velocityCoeffProtocolTemplate, date, time, operatorName, serialNumber, modbusAddress,
                GetSensorTypeString(sensorType), lVel, m, width, g, h, angle, RLiq, RMet, bs, zeroX, scaleX, zeroY, scaleY, recordDate, "КОЭФФИЦИЕНТЫ ИЗМЕРИТЕЛЯ СКОРОСТИ", "чтения");
        }
        public string CreateWriteVelocityCoeffProtocol(DateTime dateTime, string operatorName, int serialNumber, int modbusAddress,
            SensorType sensorType, double lVel, double m, double width, double g, int h, double angle, int RLiq, double RMet, double bs,
            int zeroX, int scaleX, int zeroY, int scaleY, string recordDate) {

            var date = string.Format("{0:D2}:{1:D2}:{2}", dateTime.Day, dateTime.Month, dateTime.Year);
            var time = string.Format("{0:D2}:{1:D2}:{2:D2}", dateTime.Hour, dateTime.Minute, dateTime.Second);
            return string.Format(velocityCoeffProtocolTemplate, date, time, operatorName, serialNumber, modbusAddress,
                GetSensorTypeString(sensorType), lVel, m, width, g, h, angle, RLiq, RMet, bs, zeroX, scaleX, zeroY, scaleY, recordDate, "ПРОТОКОЛ ЗАПИСИ В ИЗМЕРИТЕЛЬ СКОРОСТИ", "записи");
        }
        public void ShowProtocol(string path) {
            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
            info.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;
            info.FileName = path;
            System.Diagnostics.Process.Start(info);
        }
        public void GetCalibrationCoefficients(string path, out int zeroX, out int scaleX, out int zeroY, out int scaleY) {
            var report = File.ReadAllLines(path);
            var xcoeffString = report.First(x => x.Contains("Ось Х") && x.Contains("По нулю:")).Replace("\t", "").Replace(" ", "");
            var ycoeffString = report.First(x => x.Contains("Ось Y") && x.Contains("По нулю:")).Replace("\t", "").Replace(" ", "");

            zeroX = int.Parse((new Regex(@"ю:\d{4,5}")).Match(xcoeffString).Value.Replace("ю:", ""));
            zeroY = int.Parse((new Regex(@"ю:\d{4,5}")).Match(ycoeffString).Value.Replace("ю:", ""));
            scaleX = int.Parse((new Regex(@"е:\d{4,5}")).Match(xcoeffString).Value.Replace("е:", ""));
            scaleY = int.Parse((new Regex(@"е:\d{4,5}")).Match(ycoeffString).Value.Replace("е:", ""));
        }
        string GetSensorTypeString(SensorType type) {
            switch(type) {
                case SensorType.IUG1_3_STANDART:
                    return "IUG-1, IUG-3 (стандартный)";
                case SensorType.IUG3_RATIOMETRIC:
                    return "IUG-3 (логометрический)";
                case SensorType.IUG5_WIRELESS:
                    return "IUG-5 (беспроводной)";
            }
            return string.Empty;
        }
    }
}
