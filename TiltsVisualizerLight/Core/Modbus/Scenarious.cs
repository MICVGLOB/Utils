using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiltsVisualizerLight.Controls;

namespace Modbus.Core {
    public static class Scenarious {
        const int lockRegsStartAddress = 10;
        const int bridgeSelectorRegAddress = 0xaa;
        const int bridgeStartRegToCommunicate = 0xab;
        const int bridgeUnlockRegNumber = 0xe9;

        static int StagesCount;
        static int internalIndex;
        static string scenarioId;
        static List<ModbusUnitBase> modbusUnits;
        static int indexOfCurrentUnit;
        static byte modbusAddress;

        public static List<ModbusUnitBase> ModbusUnits { get { return modbusUnits; } }

        public static void CreateScenario(string id, byte address, UInt16? serial = null, UInt16? sensorAddress = null,
            Action primaryAction = null) {
            modbusUnits = new List<ModbusUnitBase>();
            modbusAddress = address;
            scenarioId = id;
            StagesCount = 0;
            internalIndex = 1;
            indexOfCurrentUnit = -1;
            if(sensorAddress != null) {
                AddWriteUnit(bridgeUnlockRegNumber, new List<UInt16>() { 0x0FED });
                AddWriteUnit(bridgeSelectorRegAddress, new List<UInt16>() { 1 });
            }
            if(primaryAction != null)
                primaryAction.Invoke();
            if(serial != null)
                AddUnlockUnits(serial.Value, sensorAddress);
        }
        public static ModbusUnitBase GetCurrentUnit() {
            return (indexOfCurrentUnit < ModbusUnits.Count && indexOfCurrentUnit >= 0) ? ModbusUnits[indexOfCurrentUnit] : null;
        }
        public static ModbusUnitBase GetNextUnit() {
            indexOfCurrentUnit++;
            return (indexOfCurrentUnit < ModbusUnits.Count) ? ModbusUnits[indexOfCurrentUnit] : null;
        }
        public static ModbusUnitBase GetPreviousUnit() {
            indexOfCurrentUnit--;
            return (indexOfCurrentUnit >= 0) ? ModbusUnits[indexOfCurrentUnit] : null;
        }
        public static bool IsLastUnit(ModbusUnitBase unit = null) {
            return (unit == null) ? (indexOfCurrentUnit == ModbusUnits.Count - 1)
                : (ModbusUnits.IndexOf(unit) == ModbusUnits.Count - 1);
        }
        public static void AddReadUnit(UInt16 startAddress, UInt16 quantity, int transmitTimeout = 30, int receiveTimeout = 3000,
            SerialPortErrorMode errorMode = SerialPortErrorMode.ErrorOnTimeout, SerialPortProgressType progressType = SerialPortProgressType.Normal, string unitId = null) {
            modbusUnits.Add(new ModbusReadUnit(unitId ?? scenarioId, modbusAddress, startAddress, quantity, StagesCount, internalIndex++, transmitTimeout, receiveTimeout, errorMode, progressType));
            UpdateStagesCount();
        }
        public static void AddReadReportUnit(UInt16 startAddress, UInt16 quantity, int transmitTimeout = 30, int receiveTimeout = 3000,
            SerialPortErrorMode errorMode = SerialPortErrorMode.ErrorOnTimeout, SerialPortProgressType progressType = SerialPortProgressType.Normal, string unitId = null) {
            modbusUnits.Add(new ModbusReadReportUnit(unitId ?? scenarioId, modbusAddress, startAddress, quantity, StagesCount, internalIndex++, transmitTimeout, receiveTimeout, errorMode, progressType));
            UpdateStagesCount();
        }
        public static void AddWriteUnit(UInt16 startAddress, List<UInt16> data, int transmitTimeout = 30, int receiveTimeout = 3000,
            SerialPortErrorMode errorMode = SerialPortErrorMode.ErrorOnTimeout, SerialPortProgressType progressType = SerialPortProgressType.Normal, string unitId = null) {
            modbusUnits.Add(new ModbusWriteUnit(unitId ?? scenarioId, modbusAddress, startAddress, data, StagesCount, internalIndex++, transmitTimeout, receiveTimeout, errorMode, progressType));
            UpdateStagesCount();
        }
        static void AddUnlockUnits(UInt16 serial, UInt16? sensorAddress) {
            AddUnlockUnit(modbusAddress, 0, () => (UInt16)((UInt16)(serial + 8533) ^ 0x1fa7), sensorAddress);
            AddUnlockUnit(modbusAddress, 1, () => (UInt16)((UInt16)(57963 - serial) ^ 0x8643), sensorAddress);
            AddUnlockUnit(modbusAddress, 2, () => (UInt16)((UInt16)(34527 - 2 * serial) ^ 0x6511), sensorAddress);
            AddUnlockUnit(modbusAddress, 3, () => (UInt16)((UInt16)(15889 + 3 * serial) ^ 0x2375), sensorAddress);
        }
        static void UpdateStagesCount() {
            StagesCount++;
            modbusUnits.ForEach(x => x.UpdateStagesCount(StagesCount));
        }
        static void AddUnlockUnit(byte deviceAddress, int index, Func<UInt16> regValueGetter, UInt16? sensorAddress) {
            var address = (UInt16)(lockRegsStartAddress + index);
            if(sensorAddress.HasValue)
                AddWriteUnit(bridgeStartRegToCommunicate, new List<UInt16>() { (UInt16)sensorAddress, 16, (UInt16)((address << 8) + 1), regValueGetter() });
            else
                AddWriteUnit(address, new List<UInt16>() { regValueGetter() });
        }
    }
}
