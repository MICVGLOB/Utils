using Modbus.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiltsVisualizerLight.Utils {
    public class LineDecoder {
        public LineDecoder() {
        }
        List<byte> SourceData;
        List<byte> sensorData;

        public void UpdateData(List<byte> data) {
            SourceData = data;
        }

        public bool IsDataValid() {
            if(SourceData.Count != 38 + 12)
                return false;
            if((SourceData[6] & 0x80) != 0)
                return false;
            sensorData = SourceData.GetRange(12, 38);
            var crc = ModbusHelper.CRC_Calc16(sensorData.Take(36).ToList());
            var dataCrc = (((UInt16)sensorData[36]) << 8) + (UInt16)sensorData[37];
            if(crc != dataCrc)
                return false;
            return true;
        }
        public int GetId() {
            return 256 * SourceData[4] + SourceData[5];
        }

        public double GetGravityAngle() {
            return AngleToView(BytesToIntValue(sensorData[12], sensorData[13]), multiplyer: 100.0);
        }
        public double GetDirectionAngle() {
            return AngleToView(BytesToIntValue(sensorData[14], sensorData[15]));
        }
        public int GetTemperature() {
            var value = BytesToIntValue(sensorData[10], sensorData[11]);
            if(value < 32000)
                return value;
            return (int)value - 65536;
        }
        public DateTime GetCreatedTime() {
            return new DateTime(2000 + sensorData[8], sensorData[9], sensorData[6], sensorData[4], sensorData[5], 0);
        }
        public static UInt16 AngleFromView(double angle, double multiplyer = 10.0) {
            if(angle >= 0)
                return (UInt16)Math.Round(multiplyer * angle);
            var tempAngle = 65536.0 + multiplyer * angle;
            return (UInt16)Math.Round(tempAngle);
        }
        public static double AngleToView(UInt16 angle, double multiplyer = 10.0) {
            if(angle < 32000)
                return (double)angle / multiplyer;
            var delta = (double)angle - 65536.0;
            return delta / multiplyer;
        }

        static UInt16 BytesToIntValue(byte high, byte low) {
            return (UInt16)((((ushort)high) << 8) + low);
        }
    }
}
