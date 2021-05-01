using Controls.Core;
using Modbus.Core;
using Mvvm.Core;
using StmFlashLoader.LoaderCore;
using StmFlashLoader.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Mvvm.ViewModels {
    public partial class MainViewModel : ObservableObject {
        MessageHelper MessageHelper { get; set; }
        int flashFragmentIndex = 0;

        public MainViewModel() {
            InitFieldsAndProperties();
            MessageHelper = new MessageHelper(s => AppendTextCommand.Execute(s));
            Address = McuInfoCreator.Info[0].DefaultModbusAddress;
        }
        void SelectFile() {
            var path = FileHelper.SelectFile();
            if(!string.IsNullOrEmpty(path))
                FilePath = path;
        }
        void LoadFlash() {
            if(IfUsbReconnect() || !TryReadingFile())
                return;
            flashFragmentIndex = 0;
            bool isLastFragment = false;
            LoaderScenario.Create("UpdateFlashScenario", serial);
            LoaderScenario.AddCommandUnit(CommandIds.SetProgramParameters,
                (LConverters.Int16ToList(McuConfig.PageSize).
                Concat(LConverters.ZeroUint32ToList()).
                Concat(LConverters.UInt32ToList(McuConfig.FirmwareFlashAddress)).
                Concat(LConverters.Int16ToList(McuConfig.GetErasingRegionsCount(fileSize))).ToList()),
                receiveTimeout: 300, errorMode: SerialPortErrorMode.InfoOnTimeout,
                progressType: SerialPortProgressType.Repeated, unitId: "BeginFlashUpdate");
            LoaderScenario.AddCommandUnit(CommandIds.EraseSomePages, new List<byte>() { 0, 0 },
                receiveTimeout: 3000, unitId: "EraseSomePages");
            if(Baudrate != SerialPortControl.DefaultBaudrate && AllowBaudrateSelector)
                LoaderScenario.AddCommandUnit(CommandIds.SetNewBaudrate, new List<byte>() { (byte)(Baudrate >> 8), (byte)Baudrate },
                    receiveTimeout: 3000, unitId: "SetNewBaudrate");
            do {
                var firmwareFragmentContent = CreateFlashFragment(ref isLastFragment);
                var bytesCount = firmwareFragmentContent.Count + 2;
                var data = new List<byte>() { (byte)(bytesCount >> 8), (byte)(bytesCount),
                (byte)(flashFragmentIndex >> 8), (byte)(flashFragmentIndex)}.Concat(firmwareFragmentContent).ToList();
                LoaderScenario.AddCommandUnit(CommandIds.WriteFlashFragment, data, receiveTimeout: 3000,
                    unitId: string.Format("UpdateFragment_{0}", flashFragmentIndex));
                flashFragmentIndex++;
            } while(!isLastFragment);
            LoaderScenario.AddCommandUnit(CommandIds.SetSuccessfulFlag, new List<byte>() { 0, 0 },
                unitId: "SetSuccessfulFlag");
            BeginProgress(clearText: false);
            MessageHelper.SendSearchMessage();
            if(WaitUsbConnect)
                MessageHelper.SendConnectUsbMessage();
            else
                ExecuteNext();
        }
        void ReadSerial() {
            Scenarious.CreateScenario("ReadSerialScenario", (byte)Address);
            Scenarious.AddReadUnit((UInt16)(McuConfig.SerialModbusAddress), 1, unitId: "ReadSerial");
            BeginProgress(isModbusScenario: true);
            ExecuteNext();
        }
        void ChangeAddress() {
            if(IfUsbReconnect())
                return;
            LoaderScenario.Create("ChangeAddressScenario", serial);
            LoaderScenario.AddCommandUnit(CommandIds.SetNewAddress,
                (LConverters.Int16ToList(NewAddress).
                Concat(LConverters.ZeroUint32ToList()).
                Concat(LConverters.UInt32ToList(McuConfig.ModbusFlashAddress)).ToList()),
                receiveTimeout: 300, errorMode: SerialPortErrorMode.InfoOnTimeout,
                progressType: SerialPortProgressType.Repeated, unitId: "ChangeAddress");
            BeginProgress();
            MessageHelper.SendSearchMessage();
            if(WaitUsbConnect)
                MessageHelper.SendConnectUsbMessage();
            else
                ExecuteNext();
        }
        void ChangeSerial() {
            if(IfUsbReconnect())
                return;
            LoaderScenario.Create("ChangeSerialScenario", serial);
            LoaderScenario.AddCommandUnit(CommandIds.SetNewSerial,
                (LConverters.SerialToList(NewSerial).
                Concat(LConverters.ZeroUint32ToList()).
                Concat(LConverters.UInt32ToList(McuConfig.SerialFlashAddress)).ToList()),
                receiveTimeout: 2000, errorMode: SerialPortErrorMode.InfoOnTimeout,
                progressType: SerialPortProgressType.Repeated, unitId: "ChangeSerial");
            BeginProgress();
            MessageHelper.SendSearchMessage();
            if(WaitUsbConnect)
                MessageHelper.SendConnectUsbMessage();
            else
                ExecuteNext();
        }
        void OnReceiveUnitChanged(ReceiveUnit unit) {
            if(unit == null)
                return;
            if(unit.Result == SerialPortControlMode.TerminateByUser || unit.Result == SerialPortControlMode.Error) {
                EndProgress();
                if(!isModbusScenario)
                    MessageHelper.SendTerminateMessage();
                return;
            }
            if(unit.IsTimeoutOccures && (GetCurrentId() == "ChangeSerial" || GetCurrentId() == "ChangeAddress" || GetCurrentId() == "BeginFlashUpdate")) {
                TransmitUnit = LoaderScenario.GetCurrentUnit().CreateTransmitUnit();
                return;
            }
            if(GetCurrentId() != "ChangeSerial" && GetCurrentId() != "ChangeAddress")
                MessageHelper.SendExecuteMessage(isModbusScenario);

            switch(GetCurrentId()) {
                case "ReadSerial":
                    var data = GetModbusData(unit);
                    Serial = data[0];
                    break;
                case "ChangeSerial":
                    Serial = NewSerial;
                    SendIdUpdateText(unit);
                    break;
                case "ChangeAddress":
                    Address = NewAddress;
                    SendIdUpdateText(unit);
                    break;
                case "SetNewBaudrate":
                    SerialPortBaudrate = Baudrate;
                    MessageHelper.SendUpdateProgressMesssage();
                    break;
                case "BeginFlashUpdate":
                    MessageHelper.SendUpdateFirmwareMessage(GetLoaderData(unit), McuConfig.GetErasingRegionsCount(fileSize), 
                        AllowBaudrateSelector);
                    break;
                case "EraseSomePages":
                    if(Baudrate == SerialPortControl.DefaultBaudrate || !AllowBaudrateSelector)
                        MessageHelper.SendUpdateProgressMesssage();
                    break;
            }
            if(!IsLastUnit()) {
                ExecuteNext();
                MessageHelper.SendPreMessage(isModbusScenario);
            } else
                EndProgress(GetCurrentId() == "ChangeSerial" || GetCurrentId() == "ChangeAddress");
        }
        bool CanSelectFile() {
            return !InProgress;
        }
        bool CanLoadFlash() {
            return HasPorts && !string.IsNullOrEmpty(FilePath) && !InProgress;
        }
        bool CanReadSerial() {
            return HasPorts && !InProgress;
        }
        bool CanChangeId() {
            return HasPorts && !InProgress;
        }

        void OnMcuConfigChanged() {
            Address = McuConfig.DefaultModbusAddress;
            AllowBaudrateSelector = !McuConfig.DisplayName.Contains("Control-01");
        }
        List<byte> CreateFlashFragment(ref bool isLastFragment) {
            if((fileSize - flashFragmentIndex * McuConfig.PageSize) > McuConfig.PageSize)
                return firmware.GetRange(flashFragmentIndex * McuConfig.PageSize, McuConfig.PageSize).ToList();
            isLastFragment = true;
            var tail = fileSize - flashFragmentIndex * McuConfig.PageSize;
            return firmware.GetRange(flashFragmentIndex * McuConfig.PageSize, tail);
        }
        void OnSerialPortsChanged() {
            if(!(InProgress && WaitUsbConnect))
                return;
            var port = SerialPortDetector.GetName(portKey: "stmicroelectronics");
            if(!string.IsNullOrEmpty(port)) {
                SelectedPort = port;
                ExecuteNext();
            }
        }
        void OnWaitUsbConnectChanged() {
            if(WaitUsbConnect)
                return;
            if(InProgress)
                MessageHelper.SendTerminateMessage();
            EndProgress();
        }
        bool IfUsbReconnect() {
            if(!WaitUsbConnect || string.IsNullOrEmpty(SerialPortDetector.GetName(portKey: "stmicroelectronics")))
                return false;
            ClearTextCommand.Execute(null);
            MessageHelper.SendReconnectUsbMessage();
            return true;
        }
        bool IsLastUnit() {
            return isModbusScenario ? Scenarious.IsLastUnit() : LoaderScenario.IsLastUnit();
        }
        void ExecuteNext() {
            TransmitUnit = isModbusScenario ? Scenarious.GetNextUnit().CreateTransmitUnit()
                : LoaderScenario.GetNextUnit().CreateTransmitUnit();
        }
        string GetCurrentId() {
            return isModbusScenario ? Scenarious.GetCurrentUnit().Id
                : LoaderScenario.GetCurrentUnit().Id;
        }
        List<UInt16> GetModbusData(ReceiveUnit unit) {
            var readUnit = Scenarious.GetCurrentUnit() as ModbusReadUnit;
            return readUnit == null ? null : readUnit.GetReceivedData(unit.Data);
        }
        List<byte> GetLoaderData(ReceiveUnit unit) {
            var readUnit = LoaderScenario.GetCurrentUnit() as LoaderCommandUnit;
            return readUnit == null ? null : readUnit.GetReceivedData(unit.Data);
        }
        void BeginProgress(bool isModbusScenario = false, bool clearText = true) {
            if(clearText)
                ClearTextCommand.Execute(null);
            this.isModbusScenario = isModbusScenario;
            InProgress = true;
            SetDefaultBaudrate();
        }
        void EndProgress(bool normalStateCancel = false) {
            InProgress = false;
            if(normalStateCancel)
                IsNormalStateCancelSerialPortOperation = true;
            else
                IsCancelSerialPortOperation = false;
            CommandManager.InvalidateRequerySuggested();
            SetDefaultBaudrate();
        }
        void SetDefaultBaudrate() {
            SerialPortBaudrate = SerialPortControl.DefaultBaudrate;
        }
        void SendIdUpdateText(ReceiveUnit unit) {
            MessageHelper.SendCreateIdUpdateMessage(GetLoaderData(unit), LoaderScenario.GetCommandId());
        }
        bool TryReadingFile() {
            ClearTextCommand.Execute(null);
            MessageHelper.SendBeginReadingFileMessage();
            bool state = false;
            var firmware = FileHelper.GetContent(FilePath, ref state);
            if(!state) {
                MessageHelper.SendFailReadFileMessage();
                return false;
            }
            this.firmware = firmware.ToList();
            fileSize = firmware.Count();
            MessageHelper.SendSuccessfulReadFileMessage(fileSize);
            return true;
        }
    }
}
