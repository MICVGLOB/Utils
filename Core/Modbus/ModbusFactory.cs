using Controls.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modbus.Core {
    public abstract class ModbusUnitBase {
        protected int index;
        protected int stagesCount;
        protected int transmitTimeout;
        protected int receiveTimeout;
        protected SerialPortErrorMode errorMode;
        protected SerialPortProgressType progressType;
        protected byte deviceAddress;
        protected string id;

        public int Index { get { return index; } }
        public int StagesCount { get { return stagesCount; } }
        public string Id { get { return id; } }

        public ModbusUnitBase(string id, byte deviceAddress, int stagesCount, int index, int transmitTimeout, int receiveTimeout,
            SerialPortErrorMode errorMode, SerialPortProgressType progressType) {
            this.id = id;
            this.deviceAddress = deviceAddress;
            this.stagesCount = stagesCount;
            this.index = index;
            this.transmitTimeout = transmitTimeout;
            this.receiveTimeout = receiveTimeout;
            this.errorMode = errorMode;
            this.progressType = progressType;
        }
        public abstract TransmitUnit CreateTransmitUnit();
        public void UpdateStagesCount(int count) {
            stagesCount = count;
        }
    }

    public class ModbusWriteUnit : ModbusUnitBase {
        static byte writeFunction = 16;
        UInt16 startAddress;
        UInt16 quantity;
        List<UInt16> data;

        public ModbusWriteUnit(string id, byte deviceAddress, UInt16 startAddress, List<UInt16> data, int stagesCount = 1,
            int index = 1, int transmitTimeout = 30, int receiveTimeout = 1000,
            SerialPortErrorMode errorMode = SerialPortErrorMode.ErrorOnTimeout, SerialPortProgressType progressType = SerialPortProgressType.Normal)
            : base(id, deviceAddress, stagesCount, index, transmitTimeout, receiveTimeout, errorMode, progressType) {
            this.startAddress = startAddress;
            this.quantity = (UInt16)data.Count;
            this.data = data;
        }

        public override TransmitUnit CreateTransmitUnit() {
            return new TransmitUnit(CreateTransmitData(), 8, (x) => IsRecievedDataValid(x), index, stagesCount,
                transmitTimeout, receiveTimeout, errorMode, progressType);
        }
        List<byte> CreateTransmitData() {
            var result = new List<byte>();
            result.Add(deviceAddress);
            result.Add(writeFunction);
            result.Add((byte)(startAddress >> 8));
            result.Add((byte)startAddress);
            result.Add((byte)(quantity >> 8));
            result.Add((byte)quantity);
            result.Add((byte)(2 * quantity));
            data.ForEach(x => {
                result.Add((byte)(x >> 8));
                result.Add((byte)x);
            });
            var crc = ModbusHelper.CRC_Calc16(result);
            result.Add((byte)(crc >> 8));
            result.Add((byte)crc);
            return result;
        }
        bool IsRecievedDataValid(List<byte> receivedData) {
            if(receivedData.Count != 8)
                return false;
            var crc = ModbusHelper.CRC_Calc16(receivedData.Take(receivedData.Count - 2).ToList());
            return ((byte)(crc >> 8) == receivedData[receivedData.Count - 2])
                && ((byte)(crc) == receivedData[receivedData.Count - 1])
                && (receivedData[0] == deviceAddress)
                && (receivedData[1] == writeFunction)
                && (receivedData[2] == (byte)(startAddress >> 8))
                && (receivedData[3] == (byte)(startAddress))
                && (receivedData[4] == (byte)(quantity >> 8))
                && (receivedData[5] == (byte)(quantity));
        }
    }

    public class ModbusReadUnit : ModbusUnitBase {
        static byte readFunction = 3;
        UInt16 quantity;
        UInt16 startAddress;

        public ModbusReadUnit(string id, byte deviceAddress, UInt16 startAddress, UInt16 quantity, int stagesCount = 1, int index = 1, 
            int transmitTimeout = 30, int receiveTimeout = 1000, SerialPortErrorMode errorMode = SerialPortErrorMode.ErrorOnTimeout, 
            SerialPortProgressType progressType = SerialPortProgressType.Normal)
            : base(id, deviceAddress, stagesCount, index, transmitTimeout, receiveTimeout, errorMode, progressType) {
            this.startAddress = startAddress;
            this.quantity = quantity;
        }
        public override TransmitUnit CreateTransmitUnit() {
            return new TransmitUnit(CreateTransmitData(), 2*quantity + 5, (x) => IsRecievedDataValid(x), index, stagesCount,
                transmitTimeout, receiveTimeout, errorMode, progressType);
        }
        public List<UInt16> GetReceivedData(List<byte> receivedData) {
            var outputData = new List<UInt16>();
            for(int i = 3; i < receivedData.Count - 2; i += 2) {
                outputData.Add((UInt16)((((UInt16)receivedData[i]) << 8) + (UInt16)receivedData[i + 1]));
            }
            return outputData;
        }
        List<byte> CreateTransmitData() {
            var result = new List<byte>();
            result.Add(deviceAddress);
            result.Add(readFunction);
            result.Add((byte)(startAddress >> 8));
            result.Add((byte)startAddress);
            result.Add((byte)(quantity >> 8));
            result.Add((byte)quantity);
            result.Add((byte)(2 * quantity));
            var crc = ModbusHelper.CRC_Calc16(result);
            result.Add((byte)(crc >> 8));
            result.Add((byte)crc);
            return result;
        }
        bool IsRecievedDataValid(List<byte> receivedData) {
            if(receivedData.Count < 7 || (receivedData.Count - 5) != 2 * quantity)
                return false;
            var crc = ModbusHelper.CRC_Calc16(receivedData.Take(receivedData.Count - 2).ToList());
            return ((byte)(crc >> 8) == receivedData[receivedData.Count - 2])
                && ((byte)(crc) == receivedData[receivedData.Count - 1])
                && (receivedData[0] == deviceAddress)
                && (receivedData[1] == readFunction)
                && (receivedData[2] == (byte)(2 * quantity));
        }
    }
}
