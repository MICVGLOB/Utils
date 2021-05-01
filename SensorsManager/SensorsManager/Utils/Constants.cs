using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SensorsManager.Utils {
    public static class Constants {
        public static string ReadLevelFolderName { get { return "READ Coeff Level"; } }
        public static string ReadVelocityFolderName { get { return "READ Coeff Velocity"; } }
        public static string WriteLevelFolderName { get { return "WRITE Coeff Level"; } }
        public static string WriteVelocityFolderName { get { return "WRITE Coeff Velocity"; } }
        public static string CalibrationFolderName { get { return "CALIBRATION Coeff"; } }

        public static TimeSpan DirectConnectRepeatInterval { get { return TimeSpan.FromMilliseconds(500); } }
        public static TimeSpan BridgeConnectRepeatInterval { get { return TimeSpan.FromMilliseconds(3000); } }

        public static UInt16 ModbusReadFunction { get { return 3; } }
        public static UInt16 ModbusWriteFunction { get { return 16; } }

        public static UInt16 IUG1_2_StartRWFlashAddress { get { return 0x0e; } }
        public static UInt16 IUG5_StartRWFlashAddress { get { return 0x2A; } }
        public static UInt16 IUG5_StartReadCoeffAddress { get { return 0x0E; } }

        public static UInt16 BridgeTaskAddress { get { return 0xab; } }
        public static UInt16 BridgeReadAddress { get { return 0xae; } }
        public static UInt16 BridgeActivateTranslatorAddress { get { return 0xaa; } }
        public static UInt16 BridgeConfigurateAddress { get { return 0xe9; } }


    }
}
