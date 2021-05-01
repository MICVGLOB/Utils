using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StreamManager.Utils {
    public static class Converters {
        public static int TemperatureToView(UInt16 temperature) {
            if(temperature < 32000)
                return (int)temperature;
            var delta = (int)temperature - 65536;
            return delta;
        }
        public static double VelocityToView(UInt16 velocity) {
            if(velocity < 32000)
                return (double)velocity / 1000.0;
            var delta = (double)velocity - 65536.0;
            return delta / 1000.0;
        }
        public static double AngleToView(UInt16 angle) {
            if(angle < 32000)
                return (double)angle / 10.0;
            var delta = (double)angle - 65536.0;
            return delta / 10.0;
        }
        public static double VelocityLimitToView(UInt16 limit) {
            return limit / 10.0;
        }
        public static long LongValueToView(UInt16 high, UInt16 low) {
            return (((UInt32)high) << 16) + (UInt32)low;
        }
        public static string FirmwareVersionToView(int firmwareVersion) {
            if(firmwareVersion < 100)
                return ((double)firmwareVersion / 10).ToString("F1").Replace(",", ".");
            return ((double)firmwareVersion / 100).ToString("F2").Replace(",", ".");
        }
        public static Int32 DailyConverter(UInt16 high, UInt16 low) {
            var value = ShortToLongValue(high, low);
            return value == 0xFFFFFFFF ? -1 : (Int32)value;
        }
        public static UInt32 ShortToLongValue(UInt16 high, UInt16 low) {
            return (((UInt32)high) << 16) + low;
        }
        public static int NoWorkIntervalToView(UInt16 interval) {
            return interval * 10;
        }
    }
}
