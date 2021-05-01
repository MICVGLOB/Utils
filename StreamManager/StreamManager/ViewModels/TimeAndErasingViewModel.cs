using Controls.Core;
using Modbus.Core;
using Mvvm.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Mvvm.ViewModels {
    public class TimeAndErasingViewModel : StreamViewModelBase {
        protected override string SourceId { get { return "TimeAndErasing"; } }

        public TimeAndErasingViewModel() {
            displayTimeTimer = new DispatcherTimer();
            displayTimeTimer.Interval = TimeSpan.FromMilliseconds(1000);
            displayTimeTimer.Tick += OnDisplayTimeTimerTick;
            displayTimeTimer.Start();
            ReadTimeCommand = new DelegateCommand(ReadTime, CanReadWriteTime);
            WriteTimeCommand = new DelegateCommand(WriteTime, CanReadWriteTime);
            EraseVolumeCommand = new DelegateCommand(EraseVolume, CanEraseInfo);
            EraseWeekInfoCommand = new DelegateCommand(EraseWeekInfo, CanEraseInfo);
            EraseServiceIntervalCommand = new DelegateCommand(EraseServiceInterval, () => true);
            SelectedDate = DateTime.Now;
        }

        void OnDisplayTimeTimerTick(object sender, EventArgs e) {
            if(!AllowSystemTime)
                return;
            CurrentDateTime = DateTime.Now;
        }

        DispatcherTimer displayTimeTimer;

        #region Properties

        int hour = 0;
        int minute = 0;
        int timeType = 0;
        bool allowSystemTime = true;
        DateTime currentDateTime;
        DateTime selectedDate;

        int channel = 1;
        bool allowErase;

        public int Hour { get { return hour; } set { SetPropertyValue("Hour", ref hour, value); } }
        public int Minute { get { return minute; } set { SetPropertyValue("Minute", ref minute, value); } }
        public int TimeType { get { return timeType; } set { SetPropertyValue("TimeType", ref timeType, value); } }
        public bool AllowSystemTime { get { return allowSystemTime; } set { SetPropertyValue("AllowSystemTime", ref allowSystemTime, value); } }
        public DateTime CurrentDateTime { get { return currentDateTime; } set { SetPropertyValue("CurrentDateTime", ref currentDateTime, value); } }
        public DateTime SelectedDate { get { return selectedDate; } set { SetPropertyValue("SelectedDate", ref selectedDate, value); } }

        public int Channel { get { return channel; } set { SetPropertyValue("Channel", ref channel, value); } }
        public bool AllowErase { get { return allowErase; } set { SetPropertyValue("AllowErase", ref allowErase, value); } }

        #endregion

        public ICommand ReadTimeCommand { get; private set; }
        public ICommand WriteTimeCommand { get; private set; }
        public ICommand EraseVolumeCommand { get; private set; }
        public ICommand EraseWeekInfoCommand { get; private set; }
        public ICommand EraseServiceIntervalCommand { get; private set; }

        void ReadTime() {
            Scenarious.CreateScenario("ReadDateTimeScenario", (byte)MainViewModel.Address);
            Scenarious.AddReadUnit(0x00A1, 3, unitId: "ReadDateTime");
            ExecuteNext();
        }
        void WriteTime() {
            Scenarious.CreateScenario("WriteDateTimeScenario", (byte)MainViewModel.Address);
            Scenarious.AddWriteUnit(0x00E9, new List<UInt16>() { 0x0FED });
            Scenarious.AddWriteUnit(0x00A1, EncodeDateTime(), unitId: "WriteDateTime");
            Scenarious.AddWriteUnit(0x00E9, new List<UInt16>() { 0 });
            ExecuteNext();
        }
        void EraseVolume() {
            EraseData(0x00B3);
        }
        void EraseWeekInfo() {
            EraseData(0x00B4);
        }
        void EraseServiceInterval() {
            EraseData(0x00B3, true);
        }

        void EraseData(UInt16 reg, bool isResourceCounterErase = false) {
            var result = ShowWarningMessage("Вы уверены, что хотите стереть данные?");
            if(result == MessageBoxResult.No || result == MessageBoxResult.Cancel)
                return;
            Scenarious.CreateScenario("EraseDataScenario", (byte)MainViewModel.Address);
            Scenarious.AddWriteUnit(0x00E9, new List<UInt16>() { 0x0FED });
            Scenarious.AddWriteUnit(0x00AA, new List<UInt16>() { 2 });
            var value = isResourceCounterErase ? (UInt16)(1 << 5) : (UInt16)(1 << (channel - 1));
            Scenarious.AddWriteUnit(reg, new List<UInt16>() { value }, transmitTimeout: 30, receiveTimeout: 3000);
            Scenarious.AddWriteUnit(0x00AA, new List<UInt16>() { 0 });
            Scenarious.AddWriteUnit(0x00E9, new List<UInt16>() { 0x0FED });
            ExecuteNext();
            AllowErase = false;
        }
        bool CanReadWriteTime() {
            return true;
        }
        bool CanEraseInfo() {
            return AllowErase;
        }

        protected override Dictionary<string, Action<List<UInt16>>> CreateReadActions() {
            var result = new Dictionary<string, Action<List<UInt16>>>();
            result.Add("ReadDateTime", data => {
                DecodeDateTime(data);
            });
            return result;
        }
        void DecodeDateTime(List<UInt16> data) {
            Hour = ((data[0] & 0x7FFF) >> 8);
            Minute = (data[0] & 0x00FF);
            var day = ((data[1] & 0x7FFF) >> 8);
            var year = ((data[2] & 0x7FFF) >> 8) + 2000;
            var month = (data[2] & 0x00FF);
            SelectedDate = new DateTime(year, month, day);
        }
        List<UInt16> EncodeDateTime() {
            List<UInt16> data = new List<UInt16>();
            if(AllowSystemTime) {
                Hour = CurrentDateTime.Hour;
                Minute = CurrentDateTime.Minute;
                SelectedDate = CurrentDateTime;
            }
            var dateInfo = EncodeDate(SelectedDate);
            data.Add((UInt16)(((UInt16)Hour << 8) + Minute + GetTimeTypeLabel()));
            data.Add(dateInfo.Item1);
            data.Add(dateInfo.Item2);
            return data;
        }
        Tuple<UInt16, UInt16> EncodeDate(DateTime dateTime) {
            var timeTypeLabel = GetTimeTypeLabel();
            var dayOfWeekIndex = EncodeToDayOfWeek(dateTime.DayOfWeek);
            return new Tuple<ushort, ushort>((UInt16)(((UInt16)dateTime.Day << 8) + dayOfWeekIndex + timeTypeLabel),
                (UInt16)(((UInt16)(dateTime.Year - 2000) << 8) + dateTime.Month + timeTypeLabel));
        }
        int EncodeToDayOfWeek(DayOfWeek dayOfWeek) {
            var days = new Dictionary<DayOfWeek, int>() {
                { DayOfWeek.Monday, 1 }, { DayOfWeek.Tuesday, 2 },
                { DayOfWeek.Wednesday, 3}, { DayOfWeek.Thursday, 4 }, { DayOfWeek.Friday, 5 },
                { DayOfWeek.Saturday, 6}, { DayOfWeek.Sunday, 7} };
            return days[dayOfWeek];
        }
        int GetTimeTypeLabel() {
            return ((UInt16)(((UInt16)timeType) << 15));
        }
    }
}
