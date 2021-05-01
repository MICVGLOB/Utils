using Controls.Core;
using Modbus.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StmFlashLoader.LoaderCore {
    public abstract class LoaderUnitBase {
        protected int index;
        protected int stagesCount;
        protected int transmitTimeout;
        protected int receiveTimeout;
        protected SerialPortErrorMode errorMode;
        protected SerialPortProgressType progressType;
        protected int serial;
        protected string id;

        public int Index { get { return index; } }
        public int StagesCount { get { return stagesCount; } }
        public string Id { get { return id; } }

        public LoaderUnitBase(int serial, int stagesCount, int index, int transmitTimeout, int receiveTimeout,
            SerialPortErrorMode errorMode, SerialPortProgressType progressType, string id) {
            this.id = id;
            this.serial = serial;
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

    public class LoaderCommandUnit : LoaderUnitBase {
        static byte transmitHeader = 0xe4;
        static byte receiveHeader = 0xe5;

        AnswerFormat answerFormat;
        CommandIds commandId;
        int quantity;
        List<byte> data;
        int newSerial;

        public LoaderCommandUnit(CommandIds commandId, int serial, List<byte> data, int stagesCount = 1,
            int index = 1, int transmitTimeout = 30, int receiveTimeout = 1000,
            SerialPortErrorMode errorMode = SerialPortErrorMode.ErrorOnTimeout, SerialPortProgressType progressType = SerialPortProgressType.Normal, string stringId = "")
            : base(serial, stagesCount, index, transmitTimeout, receiveTimeout, errorMode, progressType, stringId) {
            this.commandId = commandId;
            this.quantity = data.Count;
            this.data = data;
            this.answerFormat = Commands.Info[commandId].AnswerFormat;
            if(commandId == CommandIds.SetNewSerial)
                newSerial = Utils.LConverters.ListToSerial(data);
        }
        public CommandIds GetCommandId() {
            return commandId;
        }
        public override TransmitUnit CreateTransmitUnit() {
            return new TransmitUnit(CreateTransmitData(), answerFormat == AnswerFormat.Compact ? 10 : 46, (x) => IsRecievedDataValid(x), index, stagesCount,
                transmitTimeout, receiveTimeout, errorMode, progressType);
        }
        List<byte> CreateTransmitData() {
            var serial = (UInt16)(this.serial);
            var quantity = (UInt16)(this.quantity);
            var result = new List<byte>();
            result.Add(transmitHeader);
            result.Add(0);
            result.Add(0);
            result.Add((byte)((serial) >> 8));
            result.Add((byte)(serial));
            result.Add(Commands.Info[commandId].ByteId);
            if(Commands.Info[commandId].DataLength.HasValue
                && quantity != (UInt16)Commands.Info[commandId].DataLength.Value)
                throw new Exception("Incorrect transmit data");
            if(Commands.Info[commandId].DataLength.HasValue) {
                result.Add((byte)((quantity) >> 8));
                result.Add((byte)(quantity));
            }
            data.ForEach(x => result.Add(x));
            var crc = ModbusHelper.CRC_Calc16(result);
            result.Add((byte)crc);
            result.Add((byte)(crc >> 8));
            return result;
        }
        bool IsRecievedDataValid(List<byte> receivedData) {
            if(receivedData.Count != (answerFormat == AnswerFormat.Compact ? 10 : 46))
                return false;
            var serial = (UInt16)(commandId == CommandIds.SetNewSerial ? newSerial : this.serial);
            var crc = ModbusHelper.CRC_Calc16(receivedData.Take(receivedData.Count - 2).ToList());
            return ((byte)(crc >> 8) == receivedData[receivedData.Count - 1])
                && ((byte)(crc) == receivedData[receivedData.Count - 2])
                && (receivedData[0] == receiveHeader)
                && (receivedData[1] == 0)
                && (receivedData[2] == 0)
                && (receivedData[3] == (byte)(serial >> 8))
                && (receivedData[4] == (byte)serial)
                && (receivedData[5] == Commands.Info[commandId].ByteId);
        }
        public List<byte> GetReceivedData(List<byte> receivedData) {
            if(answerFormat == AnswerFormat.Compact)
                return receivedData.GetRange(6, 2).ToList();
            return receivedData.GetRange(6, 38).ToList();
        }
    }
}
