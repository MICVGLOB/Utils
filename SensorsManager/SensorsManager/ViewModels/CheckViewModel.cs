using Controls.Core;
using Modbus.Core;
using Mvvm.Core;
using SensorsManager.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Mvvm.ViewModels {
    public class CheckViewModel : ObservableObject {
        public CheckViewModel() {
            ReadCommand = new DelegateCommand(Read, () => CanReadWrite());
            WriteCommand = new DelegateCommand(Write, () => CanReadWrite());
        }

        MainViewModel mainViewModel;
        bool? noWriteCoeffError = null;
        bool? angleExceed90Error = null;
        bool? noLevelError = null;
        bool? calculationError = null;
        bool? negativeVelocityError = null;
        bool? overflowError = null;
        bool? pipeOutOfFlameError = null;
        bool? pipeTouchBottomError = null;
        bool? noWirelessError = null;
        bool noDecodeSelector = true;
        bool levelDecodeSelector = false;
        bool velocityDecodeSelector = false;
        DecodeErrorType decodeErrorType = DecodeErrorType.No;
        int startRegNumber;
        int quantity;
        int reg1Value = 0;
        int reg2Value = 0;
        int reg3Value = 0;
        int reg4Value = 0;
        int reg5Value = 0;
        double offsetAngle = 0;
        bool isContinuesReading = false;
        bool unlockRequired = false;
        bool enableInteraction = true;

        public MainViewModel MainViewModel { get { return mainViewModel; } set { SetPropertyValue("MainViewModel", ref mainViewModel, value, x => OnMainViewModelChanged(x)); } }
        public bool? NoWriteCoeffError { get { return noWriteCoeffError; } set { SetPropertyValue("NoWriteCoeffError", ref noWriteCoeffError, value); } }
        public bool? AngleExceed90Error { get { return angleExceed90Error; } set { SetPropertyValue("AngleExceed90Error", ref angleExceed90Error, value); } }
        public bool? NoLevelError { get { return noLevelError; } set { SetPropertyValue("NoLevelError", ref noLevelError, value); } }
        public bool? CalculationError { get { return calculationError; } set { SetPropertyValue("CalculationError", ref calculationError, value); } }
        public bool? NegativeVelocityError { get { return negativeVelocityError; } set { SetPropertyValue("NegativeVelocityError", ref negativeVelocityError, value); } }
        public bool? OverflowError { get { return overflowError; } set { SetPropertyValue("OverflowError", ref overflowError, value); } }
        public bool? PipeOutOfFlameError { get { return pipeOutOfFlameError; } set { SetPropertyValue("PipeOutOfFlameError", ref pipeOutOfFlameError, value); } }
        public bool? PipeTouchBottomError { get { return pipeTouchBottomError; } set { SetPropertyValue("PipeTouchBottomError", ref pipeTouchBottomError, value); } }
        public bool? NoWirelessError { get { return noWirelessError; } set { SetPropertyValue("NoWirelessError", ref noWirelessError, value); } }

        public bool NoDecodeSelector { get { return noDecodeSelector; } set { SetPropertyValue("NoDecodeSelector", ref noDecodeSelector, value, x => OnDecodeErrorTypeChanged(x, DecodeErrorType.No)); } }
        public bool LevelDecodeSelector { get { return levelDecodeSelector; } set { SetPropertyValue("LevelDecodeSelector", ref levelDecodeSelector, value, x => OnDecodeErrorTypeChanged(x, DecodeErrorType.Level)); } }
        public bool VelocityDecodeSelector { get { return velocityDecodeSelector; } set { SetPropertyValue("VelocityDecodeSelector", ref velocityDecodeSelector, value, x => OnDecodeErrorTypeChanged(x, DecodeErrorType.Velocity)); } }
        public DecodeErrorType DecodeErrorType { get { return decodeErrorType; } set { SetPropertyValue("DecodeErrorType", ref decodeErrorType, value); } }
        public int StartRegNumber { get { return startRegNumber; } set { SetPropertyValue("StartRegNumber", ref startRegNumber, value); } }
        public int Quantity { get { return quantity; } set { SetPropertyValue("Quantity", ref quantity, value); } }

        public int Reg1Value { get { return reg1Value; } set { SetPropertyValue("Reg1Value", ref reg1Value, value); } }
        public int Reg2Value { get { return reg2Value; } set { SetPropertyValue("Reg2Value", ref reg2Value, value); } }
        public int Reg3Value { get { return reg3Value; } set { SetPropertyValue("Reg3Value", ref reg3Value, value); } }
        public int Reg4Value { get { return reg4Value; } set { SetPropertyValue("Reg4Value", ref reg4Value, value); } }
        public int Reg5Value { get { return reg5Value; } set { SetPropertyValue("Reg5Value", ref reg5Value, value); } }
        public double OffsetAngle { get { return offsetAngle; } set { SetPropertyValue("OffsetAngle", ref offsetAngle, value); } }
        public bool IsContinuesReading { get { return isContinuesReading; } set { SetPropertyValue("IsContinuesReading", ref isContinuesReading, value, (x) => OnIsContinuesReadingChanged(x)); } }
        public bool EnableInteraction { get { return enableInteraction; } set { SetPropertyValue("EnableInteraction", ref enableInteraction, value); } }
        public bool UnlockRequired { get { return unlockRequired; } set { SetPropertyValue("UnlockRequired", ref unlockRequired, value); } }

        public ICommand ReadCommand { get; private set; }
        public ICommand WriteCommand { get; private set; }

        void OnIsContinuesReadingChanged(bool newValue) {
            EnableInteraction = !newValue;
            if(newValue) {
                Read();
                MainViewModel.RepeatTimer.Start();
            } else {
                if(MainViewModel.UseBridge) {
                    Scenarious.AddWriteUnit(Constants.BridgeActivateTranslatorAddress, new List<ushort>() { 0 });
                    Scenarious.AddWriteUnit(Constants.BridgeConfigurateAddress, new List<ushort>() { 0 }, unitId: "LastWriteUnit");
                } else {
                    MainViewModel.IsCancelSerialPortOperation = true;
                }
                CommandManager.InvalidateRequerySuggested();
            }
        }
        void RepeatTimer_Tick(object sender, EventArgs e) {
            MainViewModel.TransmitUnit = MainViewModel.UseBridge ? Scenarious.GetPreviousUnit().CreateTransmitUnit()
                : Scenarious.GetCurrentUnit().CreateTransmitUnit();
        }
        public void Read() {
            if(MainViewModel.UseBridge) {
                Scenarious.CreateScenario("ReadInfo", (byte)MainViewModel.BridgeAddress, null, (byte)MainViewModel.Address);
                Scenarious.AddWriteUnit(Constants.BridgeTaskAddress, new List<UInt16>() { (UInt16)MainViewModel.Address, Constants.ModbusReadFunction, (UInt16)(((UInt16)StartRegNumber << 8) + Quantity) });
                Scenarious.AddReadUnit(Constants.BridgeReadAddress, (UInt16)Quantity, transmitTimeout: 1000, unitId: "ReadData");
                if(!IsContinuesReading) {
                    Scenarious.AddWriteUnit(Constants.BridgeActivateTranslatorAddress, new List<ushort>() { 0 });
                    Scenarious.AddWriteUnit(Constants.BridgeConfigurateAddress, new List<ushort>() { 0 });
                }
            } else {
                Scenarious.CreateScenario("ReadInfo", (byte)MainViewModel.Address);
                Scenarious.AddReadUnit((UInt16)StartRegNumber, (UInt16)Quantity, 30, 1000,
                    SerialPortErrorMode.ErrorOnTimeout, IsContinuesReading ? SerialPortProgressType.Repeated : SerialPortProgressType.Normal, unitId: "ReadData");
            }
            ExecuteNext();
        }
        public void Write() {
            if(MainViewModel.UseBridge) {
                Scenarious.CreateScenario("WriteInfo", (byte)MainViewModel.BridgeAddress, UnlockRequired ? (UInt16)MainViewModel.Serial : (UInt16?)null, (byte)MainViewModel.Address);
                List<UInt16> data = new List<UInt16>() { (UInt16)MainViewModel.Address, Constants.ModbusWriteFunction, (UInt16)(((UInt16)StartRegNumber << 8) + Quantity) }
                .Concat(CreateRegValues()).ToList();
                Scenarious.AddWriteUnit(Constants.BridgeTaskAddress, data);
                Scenarious.AddWriteUnit(Constants.BridgeActivateTranslatorAddress, new List<ushort>() { 0 });
                Scenarious.AddWriteUnit(Constants.BridgeConfigurateAddress, new List<ushort>() { 0 });
            } else {
                Scenarious.CreateScenario("WriteInfo", (byte)MainViewModel.Address, UnlockRequired ? (UInt16)MainViewModel.Serial : (UInt16?)null);
                Scenarious.AddWriteUnit((UInt16)StartRegNumber, CreateRegValues());
            }
            UnlockRequired = false;
            ExecuteNext();
        }

        List<UInt16> CreateRegValues() {
            var result = new List<UInt16>();
            result.Add((UInt16)Reg1Value);
            if(Quantity > 1)
                result.Add((UInt16)Reg2Value);
            if(Quantity > 2)
                result.Add((UInt16)Reg3Value);
            if(Quantity > 3)
                result.Add((UInt16)Reg4Value);
            if(Quantity > 4)
                result.Add((UInt16)Reg5Value);
            return result;
        }

        public bool CanReadWrite() {
            return EnableInteraction;
        }

        void ExecuteNext() {
            MainViewModel.TransmitUnit = Scenarious.GetNextUnit().CreateTransmitUnit();
        }

        void OnMainViewModelChanged(MainViewModel newValue) {
            if(newValue == null)
                return;
            newValue.PropertyChanged += MainViewModel_PropertyChanged;
            newValue.RepeatTimer.Tick += RepeatTimer_Tick;
        }
        void MainViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if(e.PropertyName == "SelectedPageIndex")
                IsContinuesReading = false;
            if(e.PropertyName == "ReceiveUnit" && MainViewModel.SourceId == "Check")
                ReceiveUnitProgress();
        }
        void ReceiveUnitProgress() {
            if(MainViewModel.IsTerminateOccured()) {
                IsContinuesReading = false;
                return;
            }
            var unit = MainViewModel.ReceiveUnit;
            switch(Scenarious.GetCurrentUnit().Id) {
                case "ReadData":
                    var regValues = ((ModbusReadUnit)Scenarious.GetCurrentUnit()).GetReceivedData(unit.Data);
                    Reg1Value = regValues[0];
                    if(Quantity > 1)
                        Reg2Value = regValues[1];
                    if(Quantity > 2)
                        Reg3Value = regValues[2];
                    if(Quantity > 3)
                        Reg4Value = regValues[3];
                    if(Quantity > 4)
                        Reg5Value = regValues[4];
                    if(DecodeErrorType == DecodeErrorType.Level) {
                        SetLevelErrorFlags(regValues[2]);
                        OffsetAngle = SensorsManager.Utils.Converters.AngleToView(regValues[4]);
                    }
                    if(DecodeErrorType == DecodeErrorType.Velocity) {
                        SetVelocityErrorFlags(regValues[2]);
                        OffsetAngle = SensorsManager.Utils.Converters.AngleToView(regValues[4]);
                    }
                    break;
                case "WriteInfo":
                    break;
            }
            if(!Scenarious.IsLastUnit())
                ExecuteNext();
            if(Scenarious.IsLastUnit() && Scenarious.GetCurrentUnit().Id == "LastWriteUnit") {
                MainViewModel.IsCancelSerialPortOperation = true;
                CommandManager.InvalidateRequerySuggested();
            }
        }

        void SetLevelErrorFlags(UInt16 stateReg) {
            NoWriteCoeffError = (stateReg & 0x0001) != 0;
            AngleExceed90Error = (stateReg & 0x0002) != 0;
            CalculationError = (stateReg & 0x0008) != 0;
            OverflowError = (stateReg & 0x0010) != 0;
            if(MainViewModel.SensorType == SensorType.IUG5_WIRELESS)
                NoWirelessError = (stateReg & 0x0080) != 0;
        }
        void SetVelocityErrorFlags(UInt16 stateReg) {
            NoWriteCoeffError = (stateReg & 0x0001) != 0;
            AngleExceed90Error = (stateReg & 0x0002) != 0;
            NoLevelError = (stateReg & 0x0004) != 0;
            CalculationError = (stateReg & 0x0008) != 0;
            NegativeVelocityError = (stateReg & 0x0010) != 0;
            PipeOutOfFlameError = (stateReg & 0x0020) != 0;
            PipeTouchBottomError = (stateReg & 0x0040) != 0;
            if(MainViewModel.SensorType == SensorType.IUG5_WIRELESS)
                NoWirelessError = (stateReg & 0x0080) != 0;
        }

        void OnDecodeErrorTypeChanged(bool newValue, DecodeErrorType type) {
            if(!newValue)
                return;
            DecodeErrorType = type;
            NoWriteCoeffError = null;
            AngleExceed90Error = null;
            NoLevelError = null;
            CalculationError = null;
            NegativeVelocityError = null;
            OverflowError = null;
            PipeOutOfFlameError = null;
            PipeTouchBottomError = null;
            NoWirelessError = null;
            if(type != DecodeErrorType.No) {
                Quantity = 5;
                StartRegNumber = 0;
                Reg1Value = 0;
                Reg2Value = 0;
                Reg3Value = 0;
                Reg4Value = 0;
                Reg5Value = 0;
                OffsetAngle = 0;
            }
        }
    }
    public enum DecodeErrorType {
        No,
        Level,
        Velocity
    }
}
