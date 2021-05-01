using Controls.Core;
using Modbus.Core;
using Mvvm.Core;
using StreamManager.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Threading;

namespace Mvvm.ViewModels {
    public class RegistersManagerViewModel : StreamViewModelBase {
        protected override string SourceId { get { return "RegistersManager"; } }

        public RegistersManagerViewModel() {
            StartRegNumber = 0;
            Quantity = 1;
            RegValues = new List<UInt16>();
            ReadCommand = new DelegateCommand(ReadData, CanReadWrite);
            WriteCommand = new DelegateCommand(WriteData, CanReadWrite);
            ReadPageCommand = new DelegateCommand<VerifiedValue<UInt16>>(ReadPageData, CanReadPageData);
            OverviewReadCommand = new DelegateCommand(OverviewReadData, CanReadWrite);
        }

        #region Properties

        int startRegNumber;
        int quantity;
        List<UInt16> regValues;
        ICommand appendTextCommand;
        bool continuesReading;
        bool useBridge = false;
        bool divideBy5Regs;
        string unblockVariant = UnblockVariants[0];
        bool isBittUnblockRequired = false;
        VerifiedValue<UInt16> address;
        VerifiedValue<UInt16> serial;
        bool inProgress;


        public int StartRegNumber { get { return startRegNumber; } set { SetPropertyValue("StartRegNumber", ref startRegNumber, value); } }
        public int Quantity { get { return quantity; } set { SetPropertyValue("Quantity", ref quantity, value); } }
        public List<UInt16> RegValues { get { return regValues; } set { SetPropertyValue("RegValues", ref regValues, value); } }
        public ICommand AppendTextCommand { get { return appendTextCommand; } set { SetPropertyValue("AppendTextCommand", ref appendTextCommand, value); } }
        public bool ContinuesReading { get { return continuesReading; } set { SetPropertyValue("ContinuesReading", ref continuesReading, value, x => OnContinuesReadingChanged(x)); } }
        public VerifiedValue<UInt16> Address { get { return address; } set { SetPropertyValue("Address", ref address, value); } }
        public VerifiedValue<UInt16> Serial { get { return serial; } set { SetPropertyValue("Serial", ref serial, value); } }
        public bool UseBridge { get { return useBridge; } set { SetPropertyValue("UseBridge", ref useBridge, value, x => OnUseBridgeChanged(x)); } }
        public bool DivideBy5Regs { get { return divideBy5Regs; } set { SetPropertyValue("DivideBy5Regs", ref divideBy5Regs, value, x => OnDivideBy5RegsChanged()); } }
        public bool IsBittUnblockRequired { get { return isBittUnblockRequired; } set { SetPropertyValue("IsBittUnblockRequired", ref isBittUnblockRequired, value); } }
        public string UnblockVariant { get { return unblockVariant; } set { SetPropertyValue("UnblockVariant", ref unblockVariant, value, x => OnUnblockVariantChanged()); } }
        public bool InProgress { get { return inProgress; } set { SetPropertyValue("InProgress", ref inProgress, value); } }

        #endregion

        public ICommand ReadCommand { get; private set; }
        public ICommand WriteCommand { get; private set; }
        public ICommand ReadPageCommand { get; private set; }
        public ICommand OverviewReadCommand { get; private set; }

        void ReadData() {
            Scenarious.CreateScenario("ReadAllDataScenario", (byte)(UseBridge ? MainViewModel.Address : Address.Value), null, UseBridge ? (UInt16?)Address.Value : (UInt16?)null);
            AddReadUnits(UseBridge, DivideBy5Regs);
            ExecuteNext();
        }
        bool atomOperationComplete = false;

        void OnContinuesReadingChanged(bool? newValue) {
            if(!MainViewModel.HasPorts)
                return;
            if(newValue == true) {
                ReadData();
                MainViewModel.RepeatTimer.Start();
                atomOperationComplete = false;
                InProgress = true;
            } else {
                if(!(UseBridge || DivideBy5Regs)) {
                    MainViewModel.IsCancelSerialPortOperation = true;
                } else if(!UseBridge && DivideBy5Regs) {
                    MainViewModel.RepeatTimer.Stop();
                    if(atomOperationComplete) {
                        InProgress = false;
                    } else {
                        MainViewModel.IsCancelSerialPortOperation = true;
                    }
                } else if(UseBridge) {
                    MainViewModel.RepeatTimer.Stop();
                    //MainViewModel.IsCancelSerialPortOperation = true;
                    Dispatcher.CurrentDispatcher.BeginInvoke((Action)(() => {
                        if(Scenarious.IsLastUnit()) {
                            Scenarious.CreateScenario("FinalizeScenario", (byte)MainViewModel.Address);
                            AddFinalizeUnits();
                            ExecuteNext();
                        } else
                            AddFinalizeUnits();
                    }));
                }
                RaiseCanExecuteCommand();
            }
        }
        protected override void OnRepeatTimerTick(object sender, EventArgs e) {
            if(!(UseBridge || DivideBy5Regs))
                MainViewModel.TransmitUnit = Scenarious.GetCurrentUnit().CreateTransmitUnit();
            else
                ReadData();
        }
        void AddReadUnits(bool useBridge, bool divideBy5Regs) {
            if(!(useBridge || divideBy5Regs)) {
                Scenarious.AddReadUnit((UInt16)StartRegNumber, (UInt16)Quantity, 30, 1000,
                SerialPortErrorMode.ErrorOnTimeout, ContinuesReading ? SerialPortProgressType.Repeated : SerialPortProgressType.Normal, unitId: "ReadAllData");

                return;
            }
            int readStep = 0;
            int regsCounter = Quantity;
            do {
                var regsQuantity = regsCounter < 5 ? regsCounter : 5;
                var stepStartRegNumber = StartRegNumber + 5 * readStep;
                if(useBridge) {
                    Scenarious.AddWriteUnit(Constants.BridgeTaskAddress, new List<UInt16>() { (UInt16)Address.Value,
                        Constants.ModbusReadFunction, GetBridgeRWStepPosition((byte)stepStartRegNumber, (byte)regsQuantity) });
                    Scenarious.AddReadUnit(Constants.BridgeReadAddress, (UInt16)regsQuantity, transmitTimeout: 1000,
                        unitId: string.Format("Read_Step{0}", readStep + 1));
                } else
                    Scenarious.AddReadUnit((UInt16)stepStartRegNumber, (UInt16)regsQuantity, unitId: string.Format("Read_Step{0}", readStep + 1));
                readStep++;
                regsCounter -= 5;
            } while(regsCounter > 0);

            readSteps = readStep;
            if((!ContinuesReading && UseBridge) || IsBittUnblockRequired)
                AddFinalizeUnits(false);
        }

        void WriteData() {
            var isSensorUnblockRequired = UnblockVariant == UnblockVariants[2];

            if(IsBittUnblockRequired) {
                Scenarious.CreateScenario("WriteAllDataScenario", (byte)Address.Value);
                if(isBittUnblockRequired)
                    Scenarious.AddWriteUnit(0x00E9, new List<UInt16>() { 0x0FED });
                AddWriteAllDataUnit();
                if(isBittUnblockRequired)
                    Scenarious.AddWriteUnit(0x00E9, new List<UInt16>() { 0 });
            } else {
                Scenarious.CreateScenario("WriteAllDataScenario", (byte)(UseBridge ? MainViewModel.Address : Address.Value),
                isSensorUnblockRequired ? Serial.Value : (UInt16?)null,
                UseBridge ? (UInt16?)Address.Value : (UInt16?)null);
                AddWriteUnits(UseBridge, DivideBy5Regs);
                if(UseBridge)
                    AddFinalizeUnits(false);
            }

            ExecuteNext();
        }
        void AddWriteUnits(bool useBridge, bool divideBy5Regs) {
            if(!useBridge && !divideBy5Regs) {
                AddWriteAllDataUnit();
                return;
            }
            int writeStep = 0;
            int regsCounter = Quantity;
            do {
                var regsQuantity = regsCounter < 5 ? regsCounter : 5;
                var stepStartRegNumber = StartRegNumber + 5 * writeStep;
                var dataHeader = useBridge ? new List<UInt16>() { (UInt16)Address.Value, Constants.ModbusWriteFunction,
                        GetBridgeRWStepPosition((byte)stepStartRegNumber, (byte)regsQuantity) } : new List<UInt16>();
                Scenarious.AddWriteUnit(useBridge ? Constants.BridgeTaskAddress : (UInt16)stepStartRegNumber,
                    dataHeader.Concat(RegValues.GetRange(5 * writeStep, regsQuantity)).ToList(),
                    unitId: string.Format("Write_Step{0}", writeStep + 1));
                writeStep++;
                regsCounter -= 5;
            } while(regsCounter > 0);
        }
        void AddWriteAllDataUnit() {
            Scenarious.AddWriteUnit((UInt16)StartRegNumber, RegValues, unitId: "WriteAllData");
        }
        UInt16 GetBridgeRWStepPosition(byte startAddress, byte quantity) {
            return (UInt16)((((UInt16)startAddress) << 8) + (UInt16)quantity);
        }
        void ReadPageData(VerifiedValue<UInt16> parameter) {
            ReadReport(parameter);
        }
        void OverviewReadData() {
            ReadReport(null, true);
        }
        bool CanReadPageData(VerifiedValue<UInt16> parameter) {
            return parameter != null && parameter.IsValid && MainViewModel != null && MainViewModel.HasPorts && !InProgress;
        }
        bool CanReadWrite() {
            return MainViewModel != null && MainViewModel.HasPorts && !InProgress;
        }
        void OnUseBridgeChanged(bool newState) {
            DivideBy5Regs = newState;
            UpdateRepeatTimerInterval();
        }
        void OnDivideBy5RegsChanged() {
            UpdateRepeatTimerInterval();
        }
        void UpdateRepeatTimerInterval() {
            if(UseBridge)
                MainViewModel.RepeatTimer.Interval = TimeSpan.FromMilliseconds(7000);
            else
                MainViewModel.RepeatTimer.Interval = DivideBy5Regs ? Constants.BridgeConnectRepeatInterval : Constants.DirectConnectRepeatInterval;
        }
        void OnUnblockVariantChanged() {
            IsBittUnblockRequired = UnblockVariant == UnblockVariants[1];
        }

        List<UInt16> readResult;
        int readSteps = 1;

        void ApplyRegValues(List<UInt16> data, int stepIndex) {
            if(stepIndex == readSteps)
                RegValues = data;
        }

        protected override void ReceiveUnitProgress() {
            if(MainViewModel.ReceiveUnit.Result == SerialPortControlMode.TerminateByUser
                || MainViewModel.ReceiveUnit.Result == SerialPortControlMode.Error
                || MainViewModel.ReceiveUnit.IsTimeoutOccures) {
                MainViewModel.RepeatTimer.Stop();
                ContinuesReading = false;
                InProgress = false;
                MainViewModel.IsCancelSerialPortOperation = false;
                RaiseCanExecuteCommand();
                return;
            }
            base.ReceiveUnitProgress();
            if(Scenarious.IsLastUnit() && !ContinuesReading) {
                RaiseCanExecuteCommand();
                InProgress = false;
            }
            if(Scenarious.IsLastUnit()) {
                atomOperationComplete = true;
            }
        }
        protected override Dictionary<string, Action<List<UInt16>>> CreateReadActions() {
            var result = new Dictionary<string, Action<List<UInt16>>>();
            result.Add("ReadPageData", data => {
                List<byte> byteData = new List<byte>();
                for(int i = 0; i < 33; i++) {
                    byteData.Add((byte)(data[i] >> 8));
                    byteData.Add((byte)data[i]);
                }
                AddText(StringsHelper.RecordToLookUpStringConverter(byteData) + Environment.NewLine);
            });
            result.Add("ReadAllData", data => {
                RegValues = data;
            });
            result.Add("Read_Step1", data => {
                readResult = new List<UInt16>();
                readResult = readResult.Concat(data).ToList();
                ApplyRegValues(readResult, 1);
            });
            result.Add("Read_Step2", data => {
                readResult = readResult.Concat(data).ToList();
                ApplyRegValues(readResult, 2);
            });
            result.Add("Read_Step3", data => {
                readResult = readResult.Concat(data).ToList();
                ApplyRegValues(readResult, 3);
            });
            result.Add("Read_Step4", data => {
                readResult = readResult.Concat(data).ToList();
                ApplyRegValues(readResult, 4);
            });
            return result;
        }
        void ReadReport(VerifiedValue<UInt16> parameter, bool overview = false) {
            Scenarious.CreateScenario("ReadPageDataScenario", (byte)Address.Value);
            Scenarious.AddWriteUnit(253, new List<UInt16>() { (UInt16)(overview ? 104 : (parameter.Value + 1)) });
            for(int i = 0; i < 23; i++)
                Scenarious.AddReadUnit(255, 33, 200, unitId: "ReadPageData");
            AddText("-------------Страница отчета ------------" + Environment.NewLine + Environment.NewLine);
            ExecuteNext();
        }
        protected override void ExecuteNext() {
            base.ExecuteNext();
            InProgress = true;
        }
        void AddText(string text) {
            if(AppendTextCommand != null)
                AppendTextCommand.Execute(text);
        }
        void AddFinalizeUnits(bool forceTerminated = true) {
            Scenarious.AddWriteUnit(Constants.BridgeActivateTranslatorAddress, new List<ushort>() { 0 });
            Scenarious.AddWriteUnit(Constants.BridgeConfigurateAddress, new List<ushort>() { 0 }, unitId: forceTerminated ? "LastWriteUnit" : string.Empty);
        }
        public static List<string> UnblockVariants = new List<string>() { "<не нужно>", "БИТТ", "Датчик" };
    }
}
