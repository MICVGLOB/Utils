using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace StmFlashLoader.LoaderCore {
    public static class McuInfoCreator {
        static ObservableCollection<McuConfig> mcuInfo = CreateMcuInfo();

        public static ObservableCollection<McuConfig> Info { get { return mcuInfo; } }

        static ObservableCollection<McuConfig> CreateMcuInfo() {
            var result = new ObservableCollection<McuConfig>();
            result.Add(new McuConfig("STM32F051C8 (IUG-5)", pageSize:1024, idsPageIndex:17, firmwarePageIndex:20, serialModbusAddress:30, defaultModbusAddress:10));
            result.Add(new McuConfig("STM32F103CB (GPRS-02)", pageSize: 1024, idsPageIndex: 32, firmwarePageIndex: 34, serialModbusAddress: 0, defaultModbusAddress: 60));
            result.Add(new McuConfig("STM32F207ZG (Control-01)", pageSize: 2048, idsPageIndex: 2, 
                firmwarePageIndex: 3, serialModbusAddress:0, defaultModbusAddress: 60,
                sectorSizes: new List<int>() { 16384, 16384, 16384, 16384, 65536, 131072,
                    131072, 131072, 131072, 131072, 131072, 131072 }, modbusFlashAddressOffset: 0x7F0));
            result.Add(new McuConfig("STM32F405RG (BITT-01.4)", pageSize: 2048, idsPageIndex: 2,
                firmwarePageIndex: 4, serialModbusAddress: 251, defaultModbusAddress: 50,
                sectorSizes: new List<int>() { 16384, 16384, 16384, 16384, 65536, 131072,
                    131072, 131072, 131072, 131072, 131072, 131072 }, modbusFlashAddressOffset: 0));
            return result;
        }
    }
}