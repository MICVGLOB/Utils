using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modbus.Core {
    public static class StringsHelper {
        public static string RecordToLookUpStringConverter(List<byte> data) {
            //Debug.WriteLine(data[1]);


            StringBuilder sb = new StringBuilder();
            int startIndex = 0;
            sb.Append("Зап. " + BytesToUInt32DataString(data, ref startIndex, 2) + ":: ");
            sb.Append(BytesToUInt32DataString(data, ref startIndex, 1) + ":");
            sb.Append(BytesToUInt32DataString(data, ref startIndex, 1) + " ");
            sb.Append(BytesToUInt32DataString(data, ref startIndex, 1) + ".");
            sb.Append(BytesToUInt32DataString(data, ref startIndex, 1, false, (x) => (byte)(x >> 4)) + ".");
            sb.Append((2000 + UInt16.Parse(BytesToUInt32DataString(data, ref startIndex, 1))) + " ");
            sb.Append(BytesToUInt32DataString(data, ref startIndex, 2) + " ");
            for(int i = 0; i < 4; i++) {
                sb.Append(string.Format("/{0}к./ ", i + 1));
                sb.Append(BytesToUInt32DataString(data, ref startIndex, 4) + "куб.м ");
                sb.Append(BytesToUInt32DataString(data, ref startIndex, 2) + "мм ");
                sb.Append(BytesToUInt32DataString(data, ref startIndex, 2, checkNegative: true) + "м/c ");
                sb.Append(BytesToUInt32DataString(data, ref startIndex, 1, checkNegative: true) + "гр.С ");
                sb.Append(BytesToUInt32DataString(data, ref startIndex, 2) + "мин ");
                sb.Append(string.Format("Целост {0}к.(ур)", i + 1) + BytesToUInt32DataString(data, ref startIndex, 1) + " ");
                sb.Append(string.Format("Cтатус {0}к.", i + 1) + BytesToUInt32DataString(data, ref startIndex, 1) + " ");
                sb.Append(Environment.NewLine);
            }
            sb.Append("Целост 1к.(ск)" + BytesToUInt32DataString(data, ref startIndex, 1) + " ");
            sb.Append("Целост 2к.(ск)" + BytesToUInt32DataString(data, ref startIndex, 1) + " ");
            sb.Append("Целост 3к.(ск)" + BytesToUInt32DataString(data, ref startIndex, 1) + " ");
            sb.Append("Каналов " + BytesToUInt32DataString(data, ref startIndex, 1) + " ");
            sb.Append("CRC8 " + BytesToUInt32DataString(data, ref startIndex, 1) + " ");
            sb.Append(Environment.NewLine);
            return sb.ToString();
        }

        public static string InclinometerRecordToLookUpStringConverter(List<byte> data) {
            if(data.Count < 17)
                return string.Empty;
            StringBuilder sb = new StringBuilder();
            int startIndex = 0;
            sb.Append(BytesToUInt32DataString(data, ref startIndex, 1) + " ");
            sb.Append(BytesToUInt32DataString(data, ref startIndex, 1) + " ");
            sb.Append(BytesToUInt32DataString(data, ref startIndex, 1) + " ");
            sb.Append(BytesToUInt32DataString(data, ref startIndex, 1) + " ");
            BytesToUInt32DataString(data, ref startIndex, 2);

            double temperCode = 256 * (int)data[4] + data[5];
            sb.Append("T= " + (((temperCode - 1852)/-9.05) + 25).ToString("F1") + " deg; ");

            sb.Append("X= " + BytesToUInt32DataString(data, ref startIndex, 3, checkNegative: true) + "; ");
            sb.Append("Y= " + BytesToUInt32DataString(data, ref startIndex, 3, checkNegative: true) + "; ");
            sb.Append("Z= " + BytesToUInt32DataString(data, ref startIndex, 3, checkNegative: true) + "; ");
            sb.Append("Angle*10= " + BytesToUInt32DataString(data, ref startIndex, 2, checkNegative: true) + "");
            sb.Append(Environment.NewLine);
            return sb.ToString();
        }

        static string BytesToUInt32DataString(List<byte> data, ref int startIndex, int length, bool checkNegative = false, Func<byte, byte> getByte = null) {
            UInt32 value = 0;
            switch(length) {
                case 1: value = (getByte != null) ? (UInt32)getByte(data[startIndex]) : (UInt32)data[startIndex]; break;
                case 2: value = ((UInt32)data[startIndex] << 8) + (UInt32)data[startIndex + 1]; break;
                case 3: value = ((UInt32)data[startIndex] << 16) + ((UInt32)data[startIndex + 1] << 8) + (UInt32)data[startIndex + 2]; break;
                case 4: value = ((UInt32)data[startIndex] << 24) + ((UInt32)data[startIndex + 1] << 16) + ((UInt32)data[startIndex + 2] << 8) + (UInt32)data[startIndex + 3]; break;
            }
            startIndex += length;
            if(checkNegative) {
                UInt64 fullValue = ((UInt64)1) << (8 * length);
                UInt64 halfValue = fullValue / 2;
                if(value > halfValue) {
                    value = (UInt32)(fullValue - (UInt64)value);
                    return string.Format("-{0}", value.ToString());
                }
            }
            return value.ToString();
        }
    }

    public static class ModbusHelper {
        public static UInt16 CRC_Calc16(List<byte> source) {
            UInt16 crc = 0xffff;
            source.ForEach(s => {
                crc ^= (UInt16)s;
                for(int j = 0; j < 8; j++) {
                    if((crc & 0x0001) == 1) {
                        crc >>= 1;
                        crc ^= 0xa001;
                    } else
                        crc >>= 1;
                }
            });
            return (UInt16)(((UInt16)(crc >> 8)) + ((UInt16)(crc << 8)));
        }
    }
}
