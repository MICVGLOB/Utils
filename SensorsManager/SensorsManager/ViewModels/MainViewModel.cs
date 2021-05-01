using Controls.Core;
using Mvvm.Core;
using SensorsManager;
using SensorsManager.Reports;
using SensorsManager.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace Mvvm.ViewModels {
    public class MainViewModel : ObservableObject {
        public MainViewModel() {
            OnSelectedPageIndexChanged(0);
            ReportFactory = new ReportFactory();
            repeatTimer = new DispatcherTimer();
            repeatTimer.Interval = Constants.DirectConnectRepeatInterval;
        }

        DispatcherTimer repeatTimer;
        public static List<string> OperatorNames = new List<string>() { "Ершов", "Казьмин", "Трофимов" };
        int selectedPageIndex = 0;
        int address = 0;
        int serial = 0;
        int bridgeAddress = 0;
        bool useBridge = false;
        TransmitUnit transmitUnit;
        ReceiveUnit receieveUnit;
        string sourceId;
        bool isCancelSerialPortOperation;

        bool isIug1Sensor;
        bool isIug3Sensor;
        bool isIug5Sensor = true;
        SensorType sensorType = SensorType.IUG5_WIRELESS;

        string operatorName = OperatorNames[0];

        public ReportFactory ReportFactory;
        public DispatcherTimer RepeatTimer { get { return repeatTimer; } }
        public int SelectedPageIndex { get { return selectedPageIndex; } set { SetPropertyValue("SelectedPageIndex", ref selectedPageIndex, value, x => OnSelectedPageIndexChanged(x)); } }
        public int Address { get { return address; } set { SetPropertyValue("Address", ref address, value); } }
        public int Serial { get { return serial; } set { SetPropertyValue("Serial", ref serial, value); } }
        public int BridgeAddress { get { return bridgeAddress; } set { SetPropertyValue("BridgeAddress", ref bridgeAddress, value); } }
        public bool UseBridge { get { return useBridge; } set { SetPropertyValue("UseBridge", ref useBridge, value, x => OnUseBridgeChanged(x)); } }

        public TransmitUnit TransmitUnit { get { return transmitUnit; } set { SetPropertyValue("TransmitUnit", ref transmitUnit, value); } }
        public ReceiveUnit ReceiveUnit { get { return receieveUnit; } set { SetPropertyValue("ReceiveUnit", ref receieveUnit, value); } }
        public string SourceId { get { return sourceId; } set { SetPropertyValue("SourceId", ref sourceId, value); } }
        public bool IsCancelSerialPortOperation { get { return isCancelSerialPortOperation; } set { SetPropertyValue("IsCancelSerialPortOperation", ref isCancelSerialPortOperation, value); } }

        public bool IsIug1Sensor { get { return isIug1Sensor; } set { SetPropertyValue("IsIug1Sensor", ref isIug1Sensor, value, x => OnSensorTypeChanged(x, SensorType.IUG1_3_STANDART)); } }
        public bool IsIug3Sensor { get { return isIug3Sensor; } set { SetPropertyValue("IsIug3Sensor", ref isIug3Sensor, value, x => OnSensorTypeChanged(x, SensorType.IUG3_RATIOMETRIC)); } }
        public bool IsIug5Sensor { get { return isIug5Sensor; } set { SetPropertyValue("IsIug5Sensor", ref isIug5Sensor, value, x => OnSensorTypeChanged(x, SensorType.IUG5_WIRELESS)); } }
        public SensorType SensorType { get { return sensorType; } set { SetPropertyValue("SensorType", ref sensorType, value); } }

        public string OperatorName { get { return operatorName; } set { SetPropertyValue("OperatorName", ref operatorName, value); } }

        public bool IsTerminateOccured() {
            var unit = ReceiveUnit;
            if((unit != null) && (unit.Result == SerialPortControlMode.TerminateByUser || unit.Result == SerialPortControlMode.Error || unit.IsTimeoutOccures)) {
                repeatTimer.Stop();
                IsCancelSerialPortOperation = false;
                return true;
            }
            return false;
        }

        void OnSensorTypeChanged(bool newValue, SensorType type) {
            if(newValue)
                SensorType = type;
        }
        void OnSelectedPageIndexChanged(int newIndex) {
            switch(newIndex) {
                case 0:
                    SourceId = "Calibration";
                    break;
                case 1:
                    SourceId = "Check";
                    break;
                case 2:
                    SourceId = "Level";
                    break;
                case 3:
                    SourceId = "Velocity";
                    break;
            }
        }
        void OnUseBridgeChanged(bool newState) {
            repeatTimer.Interval = newState ? Constants.BridgeConnectRepeatInterval : Constants.DirectConnectRepeatInterval;
        }
    }
    public enum SensorType {
        IUG1_3_STANDART,
        IUG3_RATIOMETRIC,
        IUG5_WIRELESS
    }
}
