using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StmFlashLoader.LoaderCore {
    public class McuConfig {
        public UInt32 FlashBaseAddress { get { return 0x08000000; } }

        public McuConfig(string displayName, int pageSize, int idsPageIndex,
            int firmwarePageIndex, int serialModbusAddress, int defaultModbusAddress, List<int> sectorSizes = null, int modbusFlashAddressOffset = 0) {
            DisplayName = displayName;
            PageSize = pageSize;
            SerialModbusAddress = serialModbusAddress;
            DefaultModbusAddress = defaultModbusAddress;
            this.firmwarePageIndex = firmwarePageIndex;
            if(sectorSizes == null) {
                ModbusFlashAddress = FlashBaseAddress + (UInt32)idsPageIndex * (UInt32)pageSize + (UInt32)modbusFlashAddressOffset;
                FirmwareFlashAddress = FlashBaseAddress + (UInt32)firmwarePageIndex * (UInt32)pageSize;
            } else {
                SectorSizes = sectorSizes;
                ModbusFlashAddress = GetAddress(idsPageIndex) + (UInt32)modbusFlashAddressOffset;
                FirmwareFlashAddress = GetAddress(firmwarePageIndex);
            }
            SerialFlashAddress = ModbusFlashAddress + 4;
        }
        public string DisplayName { get; private set; }
        public int PageSize { get; private set; }
        public UInt32 ModbusFlashAddress { get; private set; }
        public UInt32 SerialFlashAddress { get; private set; }
        public UInt32 FirmwareFlashAddress { get; private set; }
        public int SerialModbusAddress { get; private set; }
        public List<int> SectorSizes { get; private set; }
        public int DefaultModbusAddress { get; private set; }

        int firmwarePageIndex;

        public int GetErasingRegionsCount(int firmwareSize) {
            if(SectorSizes == null)
                return (firmwareSize / PageSize) + 1;
            var result = 0;
            var sectorIndex = firmwarePageIndex;
            do {
                result++;
                firmwareSize -= SectorSizes[sectorIndex++];
            }
            while(firmwareSize > 0);
            return result;
        }

        UInt32 GetAddress(int sectorNumber) {
            if(SectorSizes == null)
                return 0;
            UInt32 result = FlashBaseAddress;
            for(int i = 0; i < sectorNumber; i++) {
                result += (UInt32)SectorSizes[i];
            }
            return result;
        }
    }
}