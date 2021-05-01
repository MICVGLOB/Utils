using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StmFlashLoader.Utils {
    public static class LConverters {
        public static List<byte> UInt32ToList(UInt32 value) {
            var result = new List<byte>();
            result.Add((byte)(value >> 24));
            result.Add((byte)(value >> 16));
            result.Add((byte)(value >> 8));
            result.Add((byte)value);
            return result;
        }
        public static int ListToSerial(List<byte> data) {
            int result = (int)data[0] << 24;
            result += (int)data[1] << 16;
            result += (int)data[2] << 8;
            result += (int)data[3];
            return result;
        }
        public static List<byte> SerialToList(int value) {
            var result = new List<byte>();
            result.Add(0);
            result.Add(0);
            result.Add((byte)((value) >> 8));
            result.Add((byte)(value));
            return result;
        }
        public static List<byte> Int16ToList(int value) {
            var result = new List<byte>();
            result.Add((byte)((value) >> 8));
            result.Add((byte)(value));
            return result;
        }
        public static List<byte> ZeroUint32ToList() {
            return new List<byte>() { 0, 0, 0, 0 };
        }
        public static string LoaderVersionToView(List<byte> data) {
            byte high = data[0];
            byte low = data[1];
            return ((((UInt16)high << 8) + low) / 10).ToString("F1").Replace(",", ".");
        }
        public static string McuIdToView(List<byte> data) {
            return string.Format("0x{0}", BitConverter.ToString(data.ToArray(), 2, 12).Replace("-", ""));
        }
        public static int ModbusAddressToView(List<byte> data) {
            return data[23];
        }
        public static int BaudrateToView(List<byte> data) {
            byte high = data[24];
            byte low = data[25];
            return ((UInt16)high << 8) + low;
        }
        public static int PageSizeToView(List<byte> data) {
            byte high = data[26];
            byte low = data[27];
            return ((UInt16)high << 8) + low;
        }
        public static string BaseAddressToView(List<byte> data) {
            return string.Format("0x{0}", BitConverter.ToString(data.ToArray(), 32, 4).Replace("-", ""));
        }
    }
}
