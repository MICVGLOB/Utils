using Controls.Core;
using Controls;
using Modbus.Core;
using Mvvm.Core;
using Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace Mvvm.ViewModels {
    public class ModbusEditorViewModel : ObservableObject {
        public ModbusEditorViewModel() {
            StartRegNumber = 0;
            Quantity = 1;
            RegValues = new List<UInt16>();
            ReadCommand = new DelegateCommand(ReadData, () => true);
            WriteCommand = new DelegateCommand(WriteData, () => true);
            ReadPageCommand = new DelegateCommand<VerifiedValue<UInt16>>(ReadPageData, CanReadPageData);
            OverviewReadCommand = new DelegateCommand(OverviewReadData, () => true);
            repeatTimer = new DispatcherTimer();
            repeatTimer.Interval = TimeSpan.FromMilliseconds(500);
            repeatTimer.Tick += RepeatTimer_Tick;
        }

        public ICommand ReadCommand { get; private set; }
        public ICommand WriteCommand { get; private set; }
        public ICommand ReadPageCommand { get; private set; }
        public ICommand OverviewReadCommand { get; private set; }

        VerifiedValue<UInt16> address;
        VerifiedValue<UInt16> serial;
        bool unlockRequired;
        bool continuesReading;
        TransmitUnit transmitUnit;
        ReceiveUnit receieveUnit;
        int startRegNumber;
        int quantity;
        List<UInt16> regValues;
        ICommand appendTextCommand;
        DispatcherTimer repeatTimer;

        public VerifiedValue<UInt16> Address { get { return address; } set { SetPropertyValue("Address", ref address, value); } }
        public VerifiedValue<UInt16> Serial { get { return serial; } set { SetPropertyValue("Serial", ref serial, value); } }
        public bool UnlockRequired { get { return unlockRequired; } set { SetPropertyValue("UnlockRequired", ref unlockRequired, value); } }
        public bool ContinuesReading { get { return continuesReading; } set { SetPropertyValue("ContinuesReading", ref continuesReading, value, (x) => OnContinuesReadingChanged(x)); } }

        public TransmitUnit TransmitUnit { get { return transmitUnit; } set { SetPropertyValue("TransmitUnit", ref transmitUnit, value); } }
        public ReceiveUnit ReceiveUnit { get { return receieveUnit; } set { SetPropertyValue("ReceiveUnit", ref receieveUnit, value, (x) => OnReceiveUnitChanged(x)); } }

        public int StartRegNumber { get { return startRegNumber; } set { SetPropertyValue("StartRegNumber", ref startRegNumber, value); } }
        public int Quantity { get { return quantity; } set { SetPropertyValue("Quantity", ref quantity, value); } }
        public List<UInt16> RegValues { get { return regValues; } set { SetPropertyValue("RegValues", ref regValues, value); } }

        public ICommand AppendTextCommand { get { return appendTextCommand; } set { SetPropertyValue("AppendTextCommand", ref appendTextCommand, value); } }

        void ReadData() {
            Scenarious.CreateScenario("ReadData", (byte)Address.Value);
            Scenarious.AddReadUnit((UInt16)StartRegNumber, (UInt16)Quantity, 30, 1000,
                SerialPortErrorMode.ErrorOnTimeout, ContinuesReading ? SerialPortProgressType.Repeated : SerialPortProgressType.Normal);
            Start();
        }
        void OnContinuesReadingChanged(bool? newValue) {
            if(newValue == true) {
                ReadData();
                repeatTimer.Start();
            }
        }
        void RepeatTimer_Tick(object sender, EventArgs e) {
            TransmitUnit = Scenarious.GetCurrentUnit().CreateTransmitUnit();
        }
        void WriteData() {
            Scenarious.CreateScenario("WriteData", (byte)Address.Value, UnlockRequired ? Serial.Value : (UInt16?)null);
            Scenarious.AddWriteUnit((UInt16)StartRegNumber, RegValues);
            Start();
        }
        void ReadPageData(VerifiedValue<UInt16> parameter) {
            ReadReport(parameter);
        }
        void OverviewReadData() {
            ReadReport(null, true);
        }
        bool CanReadPageData(VerifiedValue<UInt16> parameter) {
            return parameter == null ? false : parameter.IsValid;
        }
        void OnReceiveUnitChanged(ReceiveUnit unit) {
            if(unit.Result == SerialPortControlMode.TerminateByUser || unit.Result == SerialPortControlMode.Error || unit.IsTimeoutOccures) {
                repeatTimer.Stop();
                ContinuesReading = false;
                UnlockRequired = false;
                return;
            }
            switch(Scenarious.GetCurrentUnit().Id) {
                case "WriteData":
                    if(!Scenarious.IsLastUnit())
                        Start();
                    else
                        UnlockRequired = false;
                    return;
                case "ReadData":
                    RegValues = ((ModbusReadUnit)Scenarious.GetCurrentUnit()).GetReceivedData(unit.Data);
                    return;
                case "ReadPageData":
                    if(Scenarious.GetCurrentUnit() is ModbusReadUnit) {
                        Debug.WriteLine(Scenarious.GetCurrentUnit().Id.ToString() + " " + Scenarious.GetCurrentUnit().Index.ToString());
                        //if(Scenarious.GetCurrentUnit().Index == 24) {

                        //}
                        var data = ((ModbusReadUnit)Scenarious.GetCurrentUnit()).GetReceivedData(unit.Data);
                        List<byte> byteData = new List<byte>();
                        for(int i = 0; i < 33; i++) {
                            byteData.Add((byte)(data[i] >> 8));
                            byteData.Add((byte)data[i]);
                        }
                        AddText(StringsHelper.RecordToLookUpStringConverter(byteData) + Environment.NewLine);
                    } else
                        AddText("-------------Страница отчета ------------" + Environment.NewLine + Environment.NewLine);
                    if(!Scenarious.IsLastUnit())
                        Start();
                    return;
            }
        }
        void ReadReport(VerifiedValue<UInt16> parameter, bool overview = false) {
            Scenarious.CreateScenario("ReadPageData", (byte)Address.Value);
            Scenarious.AddWriteUnit(253, new List<UInt16>() { (UInt16)(overview ? 104 : (parameter.Value + 1)) });
            for(int i = 0; i < 23; i++)
                Scenarious.AddReadUnit(255, 33, 200);
            Start();
        }
        void Start() {
            TransmitUnit = Scenarious.GetNextUnit().CreateTransmitUnit();
        }
        void AddText(string text) {
            if(AppendTextCommand != null)
                AppendTextCommand.Execute(text);
        }
    }
}
