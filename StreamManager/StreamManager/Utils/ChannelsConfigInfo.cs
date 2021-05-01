using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace StreamManager.Utils {
    public static class ChannelsConfigInfo {
        static ObservableCollection<ChannelInfoData> source = CreateChannelsInfo();
        static Dictionary<int, string> profileTypesSource = CreateProfileTypes();
        static Dictionary<string, int> profileCodesSource = CreateProfileCodes();
        public static ObservableCollection<ChannelInfoData> Source { get { return source; } }

        public static Dictionary<int, string> ProfileTypes { get { return profileTypesSource; } }
        public static Dictionary<string, int> ProfileCodes { get { return profileCodesSource; } }

        public static ChannelInfoData FindInfo(string profileType) {
            return Source.Single(y => y.ProfileType == profileType);
        }
        public static int GetCoeffsCount(string profileType) {
            return FindInfo(profileType).CoeffsCount;
        }
        static ObservableCollection<ChannelInfoData> CreateChannelsInfo() {
            var source = new ObservableCollection<ChannelInfoData>() {
            new ChannelInfoData("Rectangle", "Прямоугольник", new ChannelCoeffInfo[] {
                new ChannelCoeffInfo("Ширина", minValue : 0, maxValue : 8000, precision : 0, unit: "мм"),
            }),
            new ChannelInfoData("Circle", "Окружность", new ChannelCoeffInfo[] {
                new ChannelCoeffInfo("Диаметр", minValue : 0, maxValue : 8000, precision : 0, unit: "мм", viewMultiplier: 2),
            }),
            new ChannelInfoData("Trapeze", "Трапеция", new ChannelCoeffInfo[] {
                new ChannelCoeffInfo("Меньшее осн.", minValue : 0, maxValue : 15000, precision : 0, unit: "мм"),
                new ChannelCoeffInfo("Большее осн.", minValue : 0, maxValue : 25000, precision : 0, unit: "мм"),
                new ChannelCoeffInfo("Выс. лотка", minValue : 0, maxValue : 15000, precision : 0, unit: "мм"),
            }),
            new ChannelInfoData("Flask", "Колба", new ChannelCoeffInfo[] {
                new ChannelCoeffInfo("Диаметр", minValue : 0, maxValue : 8000, precision : 0, unit: "мм", viewMultiplier: 2),
            }),
            new ChannelInfoData("Table", "Таблица", new ChannelCoeffInfo[] { }),
            new ChannelInfoData("StepChannel", "Ступенчатый", new ChannelCoeffInfo[] {
                new ChannelCoeffInfo("Узкая часть", minValue : 0, maxValue : 8000, precision : 0, unit: "мм"),
                new ChannelCoeffInfo("Широкая часть", minValue : 0, maxValue : 8000, precision : 0, unit: "мм"),
                new ChannelCoeffInfo("Высота ступени", minValue : 0, maxValue : 8000, precision : 0, unit: "мм"),
            }),
            new ChannelInfoData("UChannel", "U-канал", new ChannelCoeffInfo[] {
                new ChannelCoeffInfo("Коэфф. R", minValue : 0, maxValue : 8000, precision : 0, unit: "мм"),
                new ChannelCoeffInfo("Коэфф. L1", minValue : 0, maxValue : 8000, precision : 0, unit: "мм"),
                new ChannelCoeffInfo("Коэфф. L2", minValue : 0, maxValue : 8000, precision : 0, unit: "мм"),
                new ChannelCoeffInfo("Коэфф. H1", minValue : 0, maxValue : 8000, precision : 0, unit: "мм"),
                new ChannelCoeffInfo("Коэфф. H2", minValue : 0, maxValue : 8000, precision : 0, unit: "мм"),
            }),
            new ChannelInfoData("Arch", "Свод", new ChannelCoeffInfo[] {
                new ChannelCoeffInfo("Диаметр", minValue : 0, maxValue : 8000, precision : 0, unit: "мм"),
                new ChannelCoeffInfo("Выс. ила", minValue : 0, maxValue : 8000, precision : 0, unit: "мм"),
            }),
            };
            return source;
        }

        static Dictionary<int, string> CreateProfileTypes() {
            return new Dictionary<int, string>() {
                { 0, "Rectangle" }, { 1, "Circle" },
                { 2, "Trapeze" }, { 3, "Flask" },
                { 4, "Table" }, { 5, "StepChannel" },
                { 6, "UChannel" }, {7, "Arch" } };
        }
        static Dictionary<string, int> CreateProfileCodes() {
            return new Dictionary<string, int>() {
                { "Rectangle", 0 }, { "Circle", 1 },
                { "Trapeze", 2 }, { "Flask", 3 },
                { "Table", 4 }, { "StepChannel", 5 },
                { "UChannel", 6 }, {"Arch", 7 } };
        }
    }

    public class ChannelInfoData {
        public ChannelInfoData(string profileType, string name, ChannelCoeffInfo[] coeffsInfo) {
            ProfileType = profileType;
            Name = name;
            CoeffsInfo = new List<ChannelCoeffInfo>();
            CoeffsInfo.Add(new ChannelCoeffInfo("Коррекция", minValue: 0, maxValue: 2, precision: 3, unit: string.Empty));
            CoeffsInfo.Add(new ChannelCoeffInfo("S ила", minValue: 0, maxValue: 10000, precision: 0, unit: "кв.см"));
            CoeffsInfo.Add(new ChannelCoeffInfo("Выс. ПСП", minValue: 0, maxValue: 15000, precision: 0, unit: "мм"));
            foreach(var info in coeffsInfo)
                CoeffsInfo.Add(info);
        }
        public string ProfileType { get; private set; }
        public string Name { get; private set; }

        public int CoeffsCount { get { return CoeffsInfo == null ? 0 : CoeffsInfo.Count; } }

        public List<ChannelCoeffInfo> CoeffsInfo { get; private set; }
    }
    public class ChannelCoeffInfo {
        public ChannelCoeffInfo(string header, double minValue, double maxValue, int precision, string unit, int viewMultiplier = 1) {
            Unit = unit;
            Header = header;
            Name = string.IsNullOrEmpty(unit) ? header : string.Format("{0}, {1}", header, unit);
            MinValue = minValue;
            MaxValue = maxValue;
            Precision = precision;
            ViewMultiplier = viewMultiplier;
        }
        public string Unit { get; private set; }
        public string Name { get; private set; }
        public string Header { get; private set; }
        public double MinValue { get; private set; }
        public double MaxValue { get; private set; }
        public int Precision { get; private set; }
        public int ViewMultiplier { get; private set; }
        public double ViewRatio { get { return ViewMultiplier / Math.Pow(10, Precision); } }
    }
    public class ChannelData {
        public ChannelData(string profileType, List<double> data) {
            ProfileType = profileType;
            Data = data;
        }
        public string ProfileType { get; private set; }
        public List<double> Data { get; private set; }
    }
}
