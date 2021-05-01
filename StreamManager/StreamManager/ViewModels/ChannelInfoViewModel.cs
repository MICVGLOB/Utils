using Modbus.Core;
using StreamManager.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Mvvm.ViewModels {
    public class ChannelInfoViewModel : StreamViewModelBase {
        protected override string SourceId { get { return "ChannelInfo"; } }

        public ChannelInfoViewModel() {
            ReadCommand = new Core.DelegateCommand(Read, () => true);
        }

        #region Properties

        int channel = 1;
        long mondayVolume = 0;
        long tuesdayVolume = 0;
        long wednesdayVolume = 0;
        long thursdayVolume = 0;
        long fridayVolume = 0;
        long saturdayVolume = 0;
        long sundayVolume = 0;
        int currentFreezeInterval = 0;
        int totalFreezeInterval = 0;
        int levelSerial = 0;
        double levelAngle = 0;
        double levelValue = 0;
        int velocitySerial = 0;
        double velocityAngle = 0;
        double velocityValue = 0;
        int cellNumber = 0;
        int recordsCount = 0;
        int lastChannelRegValue = 0;
        int channelState = 0;
        long totalVolume = 0;
        double currentFlow = 0;
        int sensorsTemperature = 0;

        public long MondayVolume { get { return mondayVolume; } set { SetPropertyValue("MondayVolume", ref mondayVolume, value); } }
        public long TuesdayVolume { get { return tuesdayVolume; } set { SetPropertyValue("TuesdayVolume", ref tuesdayVolume, value); } }
        public long WednesdayVolume { get { return wednesdayVolume; } set { SetPropertyValue("WednesdayVolume", ref wednesdayVolume, value); } }
        public long ThursdayVolume { get { return thursdayVolume; } set { SetPropertyValue("ThursdayVolume", ref thursdayVolume, value); } }
        public long FridayVolume { get { return fridayVolume; } set { SetPropertyValue("FridayVolume", ref fridayVolume, value); } }
        public long SaturdayVolume { get { return saturdayVolume; } set { SetPropertyValue("SaturdayVolume", ref saturdayVolume, value); } }
        public long SundayVolume { get { return sundayVolume; } set { SetPropertyValue("SundayVolume", ref sundayVolume, value); } }
        public int CurrentFreezeInterval { get { return currentFreezeInterval; } set { SetPropertyValue("CurrentFreezeInterval", ref currentFreezeInterval, value); } }
        public int TotalFreezeInterval { get { return totalFreezeInterval; } set { SetPropertyValue("TotalFreezeInterval", ref totalFreezeInterval, value); } }

        public int LevelSerial { get { return levelSerial; } set { SetPropertyValue("LevelSerial", ref levelSerial, value); } }
        public double LevelAngle { get { return levelAngle; } set { SetPropertyValue("LevelAngle", ref levelAngle, value); } }
        public double LevelValue { get { return levelValue; } set { SetPropertyValue("LevelValue", ref levelValue, value); } }
        public int VelocitySerial { get { return velocitySerial; } set { SetPropertyValue("VelocitySerial", ref velocitySerial, value); } }
        public double VelocityAngle { get { return velocityAngle; } set { SetPropertyValue("VelocityAngle", ref velocityAngle, value); } }
        public double VelocityValue { get { return velocityValue; } set { SetPropertyValue("VelocityValue", ref velocityValue, value); } }
        public int CellNumber { get { return cellNumber; } set { SetPropertyValue("CellNumber", ref cellNumber, value); } }
        public int RecordsCount { get { return recordsCount; } set { SetPropertyValue("RecordsCount", ref recordsCount, value); } }
        public int LastChannelRegValue { get { return lastChannelRegValue; } set { SetPropertyValue("LastChannelRegValue", ref lastChannelRegValue, value); } }
        public int ChannelState { get { return channelState; } set { SetPropertyValue("ChannelState", ref channelState, value); } }
        public long TotalVolume { get { return totalVolume; } set { SetPropertyValue("TotalVolume", ref totalVolume, value); } }
        public double CurrentFlow { get { return currentFlow; } set { SetPropertyValue("CurrentFlow", ref currentFlow, value); } }
        public int SensorsTemperature { get { return sensorsTemperature; } set { SetPropertyValue("SensorsTemperature", ref sensorsTemperature, value); } }
        public int Channel { get { return channel; } set { SetPropertyValue("Channel", ref channel, value); } }

        #endregion

        public ICommand ReadCommand { get; private set; }

        void Read() {
            Scenarious.CreateScenario("ReadData", (byte)MainViewModel.Address);
            Scenarious.AddWriteUnit(0x00E9, new List<UInt16>() { 0x0FED });
            Scenarious.AddReadUnit((UInt16)(0x0000 + 32 * (channel - 1)), 9, unitId: "ReadStep1");
            Scenarious.AddReadUnit((UInt16)(0x0009 + 32 * (channel - 1)), 10, unitId: "ReadStep2");
            Scenarious.AddReadUnit((UInt16)(0x0013 + 32 * (channel - 1)), 10, unitId: "ReadStep3");
            Scenarious.AddReadUnit((UInt16)(0x001D + 32 * (channel - 1)), 3, unitId: "ReadStep4");
            Scenarious.AddWriteUnit(0x00E9, new List<UInt16>() { 0 });
            ExecuteNext();
        }

        protected override Dictionary<string, Action<List<UInt16>>> CreateReadActions() {
            var result = new Dictionary<string, Action<List<UInt16>>>();
            result.Add("ReadStep1", data => {
                TotalVolume = Converters.ShortToLongValue(data[0], data[1]);
                CurrentFlow = Converters.ShortToLongValue(data[3], data[4]) / 10.0;
                ChannelState = data[5];
                SensorsTemperature = Converters.TemperatureToView(data[6]);
                MondayVolume = Converters.DailyConverter(data[7], data[8]);
            });
            result.Add("ReadStep2", data => {
                TuesdayVolume = Converters.DailyConverter(data[0], data[1]);
                WednesdayVolume = Converters.DailyConverter(data[2], data[3]);
                ThursdayVolume = Converters.DailyConverter(data[4], data[5]);
                FridayVolume = Converters.DailyConverter(data[6], data[7]);
                SaturdayVolume = Converters.DailyConverter(data[8], data[9]);
            });
            result.Add("ReadStep3", data => {
                SundayVolume = Converters.DailyConverter(data[0], data[1]);
                LevelSerial = data[2];
                VelocitySerial = data[3];
                LevelValue = Converters.ShortToLongValue(data[4], data[5]) / 10.0;
                VelocityValue = Converters.VelocityToView(data[6]);
                LevelAngle = Converters.AngleToView(data[7]);
                VelocityAngle = Converters.AngleToView(data[8]);
                TotalFreezeInterval = data[9];
            });
            result.Add("ReadStep4", data => {
                CellNumber = data[0] >> 8;
                RecordsCount = data[0] & 0x00FF;
                CurrentFreezeInterval = data[1];
                LastChannelRegValue = data[2];
            });
            return result;
        }
    }
}
