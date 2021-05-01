using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StreamManager.Utils {
    public static class Constants {

        public static string CurrentConfigFolderName { get { return "CURRENT CONFIG Coeff"; } }
        public static string ChannelsConfigFolderName { get { return "CHANNELS CONFIG Coeff"; } }
        public static string CommonConfigFolderName { get { return "COMMON CONFIG Coeff"; } }

        public static TimeSpan DirectConnectRepeatInterval { get { return TimeSpan.FromMilliseconds(500); } }
        public static TimeSpan BridgeConnectRepeatInterval { get { return TimeSpan.FromMilliseconds(3000); } }

        public static UInt16 ModbusReadFunction { get { return 3; } }
        public static UInt16 ModbusWriteFunction { get { return 16; } }

        public static UInt16 BridgeTaskAddress { get { return 0xab; } }
        public static UInt16 BridgeReadAddress { get { return 0xae; } }
        public static UInt16 BridgeActivateTranslatorAddress { get { return 0xaa; } }
        public static UInt16 BridgeConfigurateAddress { get { return 0xe9; } }
    }

    public static class Info {
        static List<ChannelInfo> channelsInfo = new List<ChannelInfo>() {
            new ChannelInfo(1, "Первый канал"),
            new ChannelInfo(2, "Второй канал"),
            new ChannelInfo(3, "Третий канал"),
            new ChannelInfo(4, "Четвертый канал"),
            new ChannelInfo(5, "Пятый канал")};

        static List<DayOfWeekInfo> daysInfo = new List<DayOfWeekInfo>() {
            new DayOfWeekInfo(1, "Понедельник"),
            new DayOfWeekInfo(2, "Вторник"),
            new DayOfWeekInfo(3, "Среда"),
            new DayOfWeekInfo(4, "Четверг"),
            new DayOfWeekInfo(5, "Пятница"),
            new DayOfWeekInfo(6, "Суббота"),
            new DayOfWeekInfo(7, "Воскресенье")};

        static List<TimeTypeInfo> timeTypesInfo = new List<TimeTypeInfo>() {
            new TimeTypeInfo(0, "Зимнее"),
            new TimeTypeInfo(1, "Летнее") };

        static List<ChannelInfo> channelsCounterInfo = new List<ChannelInfo>() {
            new ChannelInfo(1, "Один канал"),
            new ChannelInfo(2, "Два канала"),
            new ChannelInfo(3, "Три канала"),
            new ChannelInfo(4, "Четыре канала"),
            new ChannelInfo(5, "Пять каналов")};

        public static List<ChannelInfo> ChannelsInfo { get { return channelsInfo; } }
        public static List<DayOfWeekInfo> DaysOfWeekInfo { get { return daysInfo; } }
        public static List<TimeTypeInfo> TimeTypesInfo { get { return timeTypesInfo; } }
        public static List<ChannelInfo> ChannelsCounterInfo { get { return channelsCounterInfo; } }
    }

    public class ChannelInfo {
        public ChannelInfo(int number, string channelName) {
            Number = number;
            ChannelName = channelName;
        }
        public int Number { get; private set; }
        public string ChannelName { get; private set; }
    }
    public class DayOfWeekInfo {
        public DayOfWeekInfo(int index, string dayName) {
            Index = index;
            DayName = dayName;
        }
        public int Index { get; private set; }
        public string DayName { get; private set; }
    }
    public class TimeTypeInfo {
        public TimeTypeInfo(int index, string timeTypeName) {
            Index = index;
            TimeTypeName = timeTypeName;
        }
        public int Index { get; private set; }
        public string TimeTypeName { get; private set; }
    }
}
