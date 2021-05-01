
using System;
using System.Collections.Generic;

namespace Stm32ULoader {
    /*-------------------------------------- */
    /*----------- Bootloader Core -----------*/
    /*-------------------------------------- */

    public class BootCoreType {
        public UInt16 BootloaderVersion;

        public UInt32 SerialNumber;
        public UInt32 NewSerialNumber;
        public UInt32 SerialNumberFlashAddress;

        public byte ModbusAddress;
        public byte NewModbusAddress;
        public UInt32 ModbusAddressFlashAddress;

        public UInt16 ModbusBitrate;
        public UInt16 NewModbusBitrate;
        public UInt16 FlashPageSize;
        public UInt32 FlashBaseAddress;
        public UInt32 FlashBaseAddressForMainProgram;
        public UInt16 FlashPageForErasing;
        public UInt16 FlashFragmentNumber;


        public byte[] TransmitData;
        public UInt16 TransmitDataLength;
        public byte[] ReceiveData;
        public UInt16 ReceiveDataLength;


        public UInt16 RLoaderVersion;
        public UInt32 RIDMcuHi;
        public UInt64 RIDMcuLo;
        public UInt16 RModbusAddress;
        public UInt16 RModbusBitrate;
        public UInt16 RFlashPageSize;
        public UInt32 RFlashBaseAddressForMainProgram;
        public UInt16 RFlashPageForErasing;

        public byte[] FlashArray;
        public Int32 FlashArrayLength;

        public struct CommandTemplate {
            public string HeaderString;
            public byte HeaderCommand;
            public UInt32 SerialNumber;
            public byte CommandId;
            public UInt16 UsefulDataLength;

            public CommandTemplate(string HeaderStringT, byte CommandIdT, UInt16 UsefulDataLengthT, byte HeaderCommandT = 0xe4, UInt32 SerialNumberT = 50) {
                HeaderString = HeaderStringT;
                HeaderCommand = HeaderCommandT;
                SerialNumber = SerialNumberT;
                CommandId = CommandIdT;
                UsefulDataLength = UsefulDataLengthT;
            }
        };

        public List<CommandTemplate> CommandList = new List<CommandTemplate>();

        private void CommandListInit() {
            CommandList.Add(new CommandTemplate("READ_LOADER_SETTING", 0x01, 2));
            CommandList.Add(new CommandTemplate("SET_NEW_BAUDRATE", 0x02, 2));
            CommandList.Add(new CommandTemplate("SET_PROGRAM_PARAMETERS", 0x03, 12));
            CommandList.Add(new CommandTemplate("SET_NEW_SERIAL", 0x04, 12));
            CommandList.Add(new CommandTemplate("SET_NEW_MODBUS_ADDRESS", 0x05, 10));
            CommandList.Add(new CommandTemplate("ERASE_SOME_PAGES", 0x06, 2));
            CommandList.Add(new CommandTemplate("WRITE_FLASH_FRAGMENT", 0x07, 2));
            CommandList.Add(new CommandTemplate("SET_SUCCESSFUL_FLAG", 0x08, 0x02));
        }

        public byte CommandIdFinder(string Header) {
            for(int i = 0; i < CommandList.Count; i++) {
                if(CommandList[i].HeaderString == Header) {
                    return CommandList[i].CommandId;
                }
            }
            return 0;
        }

        public string HeaderStringFinder(byte CCode) {
            for(int i = 0; i < CommandList.Count; i++) {
                if(CommandList[i].CommandId == CCode) {
                    return CommandList[i].HeaderString;
                }
            }
            return "";
        }

        public BootCoreType() {
            CommandListInit();
            BootloaderVersion = 0;
            SerialNumber = 0;
            NewSerialNumber = 0;
            SerialNumberFlashAddress = 0;
            ModbusAddress = 0;
            NewModbusAddress = 0;
            ModbusAddressFlashAddress = 0;
            ModbusBitrate = 0;
            NewModbusBitrate = 0;
            FlashPageSize = 0;
            FlashBaseAddress = 0;
            FlashBaseAddressForMainProgram = 0;
            FlashPageForErasing = 0;
            FlashFragmentNumber = 0;

            TransmitData = new byte[4096];
            TransmitDataLength = 0;
            ReceiveData = new byte[1024];
            ReceiveDataLength = 0;

            RLoaderVersion = 0;
            RIDMcuHi = 0;
            RIDMcuLo = 0;
            RModbusAddress = 0;
            RModbusBitrate = 0;
            RFlashPageSize = 0;
            RFlashBaseAddressForMainProgram = 0;
            RFlashPageForErasing = 0;

            FlashArray = null;
            FlashArrayLength = 0;
        }

        public bool LoaderMessageCreator(string Header) {
            int IndexTmp = 0xffff;
            for(int i = 0; i < CommandList.Count; i++) {
                if(CommandList[i].HeaderString == Header) {
                    IndexTmp = i;
                }
            }
            if(IndexTmp == 0xffff)
                return true;

            TransmitData[0] = CommandList[IndexTmp].HeaderCommand;
            TransmitData[1] = (byte)(SerialNumber >> 24);
            TransmitData[2] = (byte)(SerialNumber >> 16);
            TransmitData[3] = (byte)(SerialNumber >> 8);
            TransmitData[4] = (byte)(SerialNumber);
            TransmitData[5] = CommandList[IndexTmp].CommandId;
            TransmitData[6] = (byte)(CommandList[IndexTmp].UsefulDataLength >> 8);
            TransmitData[7] = (byte)(CommandList[IndexTmp].UsefulDataLength);
            TransmitData[8] = 0;
            TransmitData[9] = 0;
            UInt16 DataHeaderLength = 8;
            UInt16 DataLengthAddition = CommandList[IndexTmp].UsefulDataLength;
            byte[] digit = new byte[10];

            switch(CommandList[IndexTmp].HeaderString) {
                case "SET_NEW_BAUDRATE":
                    ConvertDigitToByteArray(NewModbusBitrate, ref digit);
                    Array.Copy(digit, 8 - sizeof(UInt16), TransmitData, 8, sizeof(UInt16));
                    break;
                case "SET_PROGRAM_PARAMETERS":
                    ConvertDigitToByteArray(FlashPageSize, ref digit);
                    Array.Copy(digit, 8 - sizeof(UInt16), TransmitData, 8, sizeof(UInt16));
                    ConvertDigitToByteArray(FlashBaseAddressForMainProgram, ref digit);
                    Array.Copy(digit, 8 - sizeof(UInt64), TransmitData, 8 + sizeof(UInt16), sizeof(UInt64));
                    ConvertDigitToByteArray(FlashPageForErasing, ref digit);
                    Array.Copy(digit, 8 - sizeof(UInt16), TransmitData, 8 + sizeof(UInt16) + sizeof(UInt64), sizeof(UInt16));
                    break;

                case "SET_NEW_SERIAL":
                    ConvertDigitToByteArray(NewSerialNumber, ref digit);
                    Array.Copy(digit, 8 - sizeof(UInt32), TransmitData, 8, sizeof(UInt32));
                    ConvertDigitToByteArray(SerialNumberFlashAddress, ref digit);
                    Array.Copy(digit, 8 - sizeof(UInt64), TransmitData, 8 + sizeof(UInt32), sizeof(UInt64));
                    break;

                case "SET_NEW_MODBUS_ADDRESS":
                    ConvertDigitToByteArray(NewModbusAddress, ref digit);
                    Array.Copy(digit, 8 - sizeof(UInt16), TransmitData, 8, sizeof(UInt16));
                    ConvertDigitToByteArray(ModbusAddressFlashAddress, ref digit);
                    Array.Copy(digit, 8 - sizeof(UInt64), TransmitData, 8 + sizeof(UInt16), sizeof(UInt64));
                    break;
                case "WRITE_FLASH_FRAGMENT":
                    // construct outside this function
                    break;
            }

            UInt16 tmp = CRC_Calc16_NoSwap(TransmitData, (UInt16)(DataHeaderLength + DataLengthAddition));
            TransmitData[DataHeaderLength + DataLengthAddition] = (byte)(tmp >> 8);
            TransmitData[DataHeaderLength + DataLengthAddition + 1] = (byte)(tmp);
            TransmitDataLength = (UInt16)(DataHeaderLength + DataLengthAddition + 2);
            return false;
        }

        public void LoaderFragmentMessageCreator() {
            LoaderMessageCreator("WRITE_FLASH_FRAGMENT");
            TransmitData[8] = (byte)(FlashFragmentNumber >> 8);
            ;
            TransmitData[9] = (byte)FlashFragmentNumber;

            if((FlashArrayLength - FlashFragmentNumber * FlashPageSize) < FlashPageSize) {
                Int32 delta = FlashArrayLength - FlashFragmentNumber * FlashPageSize;
                Array.Copy(FlashArray, (int)(FlashPageSize * FlashFragmentNumber), TransmitData, 10, delta);
                TransmitDataLength = (UInt16)(10 + delta);
                TransmitData[6] = (byte)(((UInt32)(2 + delta)) >> 8);
                TransmitData[7] = (byte)((UInt32)(2 + delta));
            } else {
                Array.Copy(FlashArray, (int)(FlashPageSize * FlashFragmentNumber), TransmitData, 10, FlashPageSize);
                TransmitDataLength = (UInt16)(10 + FlashPageSize);
                TransmitData[6] = (byte)((2 + FlashPageSize) >> 8);
                TransmitData[7] = (byte)(2 + FlashPageSize);
            }

            UInt16 tmp = CRC_Calc16_NoSwap(TransmitData, TransmitDataLength);
            TransmitData[TransmitDataLength] = (byte)(tmp >> 8);
            TransmitData[TransmitDataLength + 1] = (byte)(tmp);
            TransmitDataLength += 2;

            FlashFragmentNumber++;
        }

        public void ModbusReadMessageCreator(byte Addr, string CommTmp = "0300000001") // Read 
        {
            TransmitData[0] = Addr;
            int bytesCounter = 1;
            for(Int32 i = 0; i < CommTmp.Length; i += 2)
                TransmitData[bytesCounter++] = (byte)((Convert.ToInt16(CommTmp[i].ToString(), 16) * 16)
                    + Convert.ToInt16(CommTmp[i + 1].ToString(), 16));
            UInt16 tmp = CRC_Calc16(TransmitData, (UInt16)(CommTmp.Length / 2 + 1));
            TransmitData[CommTmp.Length / 2 + 1] = (byte)(tmp >> 8);
            TransmitData[CommTmp.Length / 2 + 2] = (byte)(tmp);
            TransmitDataLength = (UInt16)(CommTmp.Length / 2 + 3);
        }

        public bool ModbusOneRegReadVeryfier(byte Addr, ref UInt16 Rez) {
            Rez = 0;
            if(((CRC_Calc16(ReceiveData, (UInt16)(ReceiveDataLength - 2)) >> 8) != (UInt16)ReceiveData[(UInt16)(ReceiveDataLength - 2)])
                || ((CRC_Calc16(ReceiveData, (UInt16)(ReceiveDataLength - 2)) & 0x00ff) != ReceiveData[(UInt16)(ReceiveDataLength - 1)])) { return true; } // CRC Fail 
            if((ReceiveData[0] != Addr) || (ReceiveData[1] != 3) || (ReceiveData[2] != 2)) {
                return true;
            }
            Rez = (UInt16)((((UInt16)ReceiveData[3]) << 8) + (UInt16)ReceiveData[4]);
            return false;
        }

        public bool CompactAnswerVeryfier(byte CCode) {
            if(ReceiveData[0] != 0xe5)
                return true;
            if(((CRC_Calc16_NoSwap(ReceiveData, (UInt16)(ReceiveDataLength - 2)) >> 8) != (UInt16)ReceiveData[(UInt16)(ReceiveDataLength - 2)])
                || ((CRC_Calc16_NoSwap(ReceiveData, (UInt16)(ReceiveDataLength - 2)) & 0x00ff) != ReceiveData[(UInt16)(ReceiveDataLength - 1)])) { return true; } // CRC Fail 

            if(HeaderStringFinder(CCode) == "SET_NEW_SERIAL") {
                if((ConvertBinArrToUint32(ReceiveData, 1) != NewSerialNumber) || (ReceiveData[5] != CCode))
                    return true;
            } else {
                if((ConvertBinArrToUint32(ReceiveData, 1) != SerialNumber) || (ReceiveData[5] != CCode))
                    return true;
            }
            RLoaderVersion = ConvertBinArrToUint16(ReceiveData, 6);
            return false;
        }

        public bool ExpandedAnswerVeryfier(byte CCode) {
            if(CompactAnswerVeryfier(CCode))
                return true;
            RIDMcuHi = ConvertBinArrToUint32(ReceiveData, 8);
            RIDMcuLo = ConvertBinArrToUint64(ReceiveData, 8 + sizeof(UInt32));
            RModbusAddress = ConvertBinArrToUint16(ReceiveData, 28);
            RModbusBitrate = ConvertBinArrToUint16(ReceiveData, 28 + sizeof(UInt16));
            RFlashPageSize = ConvertBinArrToUint16(ReceiveData, 28 + 2 * sizeof(UInt16));
            RFlashBaseAddressForMainProgram = ConvertBinArrToUint32(ReceiveData, 28 + 3 * sizeof(UInt16) + 4);
            RFlashPageForErasing = ConvertBinArrToUint16(ReceiveData, 28 + 3 * sizeof(UInt16) + sizeof(UInt32) + 4);
            return false;
        }
        private void ConvertDigitToByteArray(UInt64 a, ref byte[] b) {
            for(uint i = 0; i < 8; i++) {
                b[i] = (byte)(a >> ((byte)(8 * (7 - i))));
            }
        }

        private UInt16 ConvertBinArrToUint16(byte[] b, UInt16 index) {
            return (UInt16)(((UInt16)b[index] << 8) + (UInt16)b[index + 1]);
        }

        private UInt32 ConvertBinArrToUint32(byte[] b, UInt16 index) {
            return (((UInt32)b[index] << 24) + ((UInt32)b[index + 1] << 16) + ((UInt32)b[index + 2] << 8) + (UInt32)b[index + 3]);
        }

        private UInt64 ConvertBinArrToUint64(byte[] b, UInt16 index) {
            return (((UInt64)b[index] << 56) + ((UInt64)b[index + 1] << 48) + ((UInt64)b[index + 2] << 40) + ((UInt64)b[index + 3] << 32) +
                ((UInt64)b[index + 4] << 24) + ((UInt64)b[index + 5] << 16) + ((UInt64)b[index + 6] << 8) + (UInt64)b[index + 7]);
        }

        private UInt16 CRC_Calc16(byte[] b, UInt16 N) {
            UInt16 crc = 0xffff;

            for(UInt16 i = 0; i < N; i++) {
                crc ^= (UInt16)b[i];

                for(UInt16 j = 0; j < 8; j++) {
                    if((crc & 0x0001) == 1) {
                        crc >>= 1;
                        crc ^= 0xa001;
                    } else {
                        crc >>= 1;
                    }
                }
            }
            UInt16 temp1 = (UInt16)(crc >> 8); // Перестановка байт
            UInt16 temp2 = (UInt16)(crc << 8);
            return (UInt16)(temp1 + temp2);
        }

        private UInt16 CRC_Calc16_NoSwap(byte[] b, UInt16 N) {
            UInt16 tmp = CRC_Calc16(b, N);
            UInt16 temp1 = (UInt16)(tmp >> 8); // Перестановка байт
            UInt16 temp2 = (UInt16)(tmp << 8);
            return (UInt16)(temp1 + temp2);
        }
    };
}