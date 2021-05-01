using Controls.Core;
using Modbus.Core;
using Mvvm.Core;
using StreamCorrector.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Threading;

namespace Mvvm.ViewModels {
    public partial class MainViewModel : ObservableObject {
        UpdateInfo updateData;

        UInt16 Group0Serial;
        UInt16 Group1Serial;
        byte Group0Address;
        byte Group1Address;
        CurrentScenario CurrentScenario;

        public MainViewModel() {
            updateData = SerializeUpdateInfo.Data;
            InitFieldsAndProperties();
        }

        void Update() {
            InProgress = true;
            UpdateStatusString = "Корректировка регистров...";
            UpdateStatus = UpdateStatus.Progress;
            CreateBridgeUpdateScenario();
            SendNextUnit();
        }

        void CreateBridgeUpdateScenario() {
            Scenarious.CreateScenario("BridgeUpdateScenario", GetBridgeAddress());
            if(updateData.BridgeSerial.HasValue)
                Scenarious.AddReadUnit(0x00FB, 1, unitId: "ReadBridgeSerial");
            Scenarious.AddWriteUnit(0x00E9, new List<UInt16>() { 0x0FED });
            BridgeRegs.ForEach(x => Scenarious.AddWriteUnit((UInt16)(x.RegAddress), GetRegData(x),
                unitId: string.Format("WriteBridgeReg{0}", BridgeRegs.IndexOf(x))));
            BridgeRegs.ForEach(x => Scenarious.AddReadUnit((UInt16)(x.RegAddress), 1,
                unitId: string.Format("ReadBridgeReg{0}", BridgeRegs.IndexOf(x))));
            if(!(Group0Regs.Any() || Group1Regs.Any())) {
                Scenarious.AddWriteUnit(0x00E9, new List<UInt16>() { 0 }, unitId: "ResetUpdate");
                return;
            }
            Scenarious.AddWriteUnit(0x00AA, new List<UInt16>() { 1 });
            Scenarious.AddWriteUnit(BridgeTaskAddress, new List<UInt16>() { Group0Address, ModbusReadFunction, 0x1e01 });
            Scenarious.AddReadUnit(BridgeReadAddress, 1, transmitTimeout: 1000, unitId: "ReadGroup0Serial");
            if(Group1Regs.Any()) {
                Scenarious.AddWriteUnit(BridgeTaskAddress, new List<UInt16>() { Group1Address, ModbusReadFunction, 0x1e01 });
                Scenarious.AddReadUnit(BridgeReadAddress, 1, transmitTimeout: 1000, unitId: "ReadGroup1Serial");
            }
            CurrentScenario = CurrentScenario.BridgeUpdate;
        }
        void CreateGroup0UpdateScenario() {
            Scenarious.CreateScenario("Group0UpdateScenario", GetBridgeAddress(), Group0Serial, Group0Address);
            Group0Regs.ForEach(x => Scenarious.AddWriteUnit(BridgeTaskAddress,
                new List<UInt16>() { Group0Address, ModbusWriteFunction, (UInt16)(((UInt16)x.RegAddress << 8) + 1), GetRegData(x).First() }));
            Group0Regs.ForEach(x => {
                Scenarious.AddWriteUnit(BridgeTaskAddress, new List<UInt16>() { Group0Address, ModbusReadFunction, (UInt16)(((UInt16)x.RegAddress << 8) + 1) });
                Scenarious.AddReadUnit(BridgeReadAddress, 1, transmitTimeout: 1000, unitId: string.Format("ReadGroup0Reg{0}", Group0Regs.IndexOf(x)));
            });
            if(!Group1Regs.Any())
                CloseCommunication();
            CurrentScenario = CurrentScenario.Group0Update;

        }
        void CreateGroup1UpdateScenario() {
            Scenarious.CreateScenario("Group1UpdateScenario", GetBridgeAddress(), Group1Serial, Group1Address);
            Group1Regs.ForEach(x => Scenarious.AddWriteUnit(BridgeTaskAddress,
                new List<UInt16>() { Group1Address, ModbusWriteFunction, (UInt16)(((UInt16)x.RegAddress << 8) + 1), GetRegData(x).First() }));
            Group1Regs.ForEach(x => {
                Scenarious.AddWriteUnit(BridgeTaskAddress, new List<UInt16>() { Group1Address, ModbusReadFunction, (UInt16)(((UInt16)x.RegAddress << 8) + 1) });
                Scenarious.AddReadUnit(BridgeReadAddress, 1, transmitTimeout: 1000, unitId: string.Format("ReadGroup1Reg{0}", Group1Regs.IndexOf(x)));
            });
            CloseCommunication();
            CurrentScenario = CurrentScenario.Group1Update;
        }

        void OnReceiveUnitChanged(ReceiveUnit unit) {
            if(unit != null && (unit.Result == SerialPortControlMode.TerminateByUser || unit.Result == SerialPortControlMode.Error || unit.IsTimeoutOccures)) {
                IsCancelSerialPortOperation = false;
                InProgress = false;
                if(UpdateStatus != UpdateStatus.Error) {
                    UpdateStatusString = string.Empty;
                    UpdateStatus = UpdateStatus.Progress;
                }
                CommandManager.InvalidateRequerySuggested();
                return;
            }
            var id = Scenarious.GetCurrentUnit().Id;
            if(id == "ReadBridgeSerial" && !CheckBridgeSerial(GetRegFromReceivedData(unit))) {
                TerminateUpdate("Серийный номер БИТТ не совпадает с разрешенным!", UpdateStatus.Error);
                return;
            }
            if(id == "ReadGroup0Serial")
                Group0Serial = GetRegFromReceivedData(unit);
            if(id == "ReadGroup1Serial")
                Group1Serial = GetRegFromReceivedData(unit);
            if(!(CorrectionValid(unit, id, BridgeRegs, "ReadBridgeReg")
                && CorrectionValid(unit, id, Group0Regs, "ReadGroup0Reg")
                && CorrectionValid(unit, id, Group1Regs, "ReadGroup1Reg")))
                return;
            if(!Scenarious.IsLastUnit())
                SendNextUnit();
            else if(CurrentScenario == CurrentScenario.BridgeUpdate && Group0Regs.Any())
                RunScenarioAsync(() => {
                    CreateGroup0UpdateScenario();
                    SendNextUnit();
                });
            else if(CurrentScenario == CurrentScenario.Group0Update && Group1Regs.Any())
                RunScenarioAsync(() => {
                    CreateGroup1UpdateScenario();
                    SendNextUnit();
                });
            else {
                UpdateStatusString = "Коррекция выполнена успешно!";
                UpdateStatus = UpdateStatus.OK;
                InProgress = false;
                CommandManager.InvalidateRequerySuggested();
            }
        }

        bool CorrectionValid(ReceiveUnit unit, string id, List<RegInfo> collection, string header) {
            if(id.Contains(header) && !CheckReg(int.Parse(id.Replace(header, "")), GetRegFromReceivedData(unit),
                collection)) {
                TerminateUpdate(string.Format("Ошибка обновления регистра {0}!", id.Replace("Read", "")), UpdateStatus.Error);
                return false;
            }
            return true;
        }
        bool CheckBridgeSerial(UInt16 receivedSerial) {
            return !updateData.BridgeSerial.HasValue ? false : receivedSerial == updateData.BridgeSerial.Value;
        }
        bool CheckReg(int regIndex, UInt16 receivedReg, List<RegInfo> collection) {
            var expectedReg = GetRegData(collection.ElementAt(regIndex)).First();
            return receivedReg == expectedReg;
        }
        List<UInt16> GetRegData(RegInfo info) {
            double inputValue = AskRegs.Contains(info) ? (double)Helpers.GetPropertyValue(string.Format("InputValueBindable_Reg{0}", AskRegs.IndexOf(info)), this) : info.InputValue;
            var realValue = (int)Math.Round(info.InputValueMultplier * inputValue);
            var result = new List<UInt16>();
            if(realValue >= 0)
                result.Add((UInt16)realValue);
            else {
                var convertedValue = 65536 + realValue;
                result.Add((UInt16)convertedValue);
            }
            return result;
        }
        UInt16 GetRegFromReceivedData(ReceiveUnit unit) {
            return (((ModbusReadUnit)Scenarious.GetCurrentUnit()).GetReceivedData(unit.Data)).First();
        }
        void SendNextUnit() {
            TransmitUnit = Scenarious.GetNextUnit().CreateTransmitUnit();
        }
        void CloseCommunication() {
            Scenarious.AddWriteUnit(0x00AA, new List<UInt16>() { 0 });
            Scenarious.AddWriteUnit(0x00E9, new List<UInt16>() { 0 });
        }
        byte GetBridgeAddress() {
            return updateData.AskTheBridgeAddress ? (byte)ModbusBridgeAddressBindable : (byte)updateData.ModbusBridgeAddress;
        }
        void RunScenarioAsync(Action action) {
            Dispatcher.CurrentDispatcher.BeginInvoke(action);
        }
        void TerminateUpdate(string message, UpdateStatus updateStatus) {
            UpdateStatusString = message;
            UpdateStatus = updateStatus;
            IsCancelSerialPortOperation = true;
            InProgress = false;
            CommandManager.InvalidateRequerySuggested();
        }
    }
    public enum CurrentScenario {
        BridgeUpdate,
        Group0Update,
        Group1Update
    }
}
