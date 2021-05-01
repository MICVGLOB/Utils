using Controls.Core;
using Mvvm.Core;
using StreamManager;
using StreamManager.Reports;
using StreamManager.Utils;
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

        #region Properties

        int selectedPageIndex = 0;
        int address = 0;
        int serial = 0;
        TransmitUnit transmitUnit;
        ReceiveUnit receieveUnit;
        string sourceId;
        bool isCancelSerialPortOperation;
        bool hasPorts;
        List<byte> receiveTerminalData;
        bool terminalMode;

        string operatorName = OperatorNames[0];

        public ReportFactory ReportFactory;
        public DispatcherTimer RepeatTimer { get { return repeatTimer; } }
        public int SelectedPageIndex { get { return selectedPageIndex; } set { SetPropertyValue("SelectedPageIndex", ref selectedPageIndex, value, x => OnSelectedPageIndexChanged(x)); } }
        public int Address { get { return address; } set { SetPropertyValue("Address", ref address, value); } }
        public int Serial { get { return serial; } set { SetPropertyValue("Serial", ref serial, value); } }

        public TransmitUnit TransmitUnit { get { return transmitUnit; } set { SetPropertyValue("TransmitUnit", ref transmitUnit, value); } }
        public ReceiveUnit ReceiveUnit { get { return receieveUnit; } set { SetPropertyValue("ReceiveUnit", ref receieveUnit, value); } }
        public string SourceId { get { return sourceId; } set { SetPropertyValue("SourceId", ref sourceId, value); } }
        public bool IsCancelSerialPortOperation { get { return isCancelSerialPortOperation; } set { SetPropertyValue("IsCancelSerialPortOperation", ref isCancelSerialPortOperation, value); } }
        public bool HasPorts { get { return hasPorts; } set { SetPropertyValue("HasPorts", ref hasPorts, value); } }

        public string OperatorName { get { return operatorName; } set { SetPropertyValue("OperatorName", ref operatorName, value); } }

        public List<byte> ReceiveTerminalData { get { return receiveTerminalData; } set { SetPropertyValue("ReceiveTerminalData", ref receiveTerminalData, value, x => OnReceiveTerminalDataChanged(x)); } }
        public bool TerminalMode { get { return terminalMode; } set { SetPropertyValue("TerminalMode", ref terminalMode, value); } }

        #endregion

        protected virtual void OnReceiveTerminalDataChanged(List<byte> newData) {
        }


        public bool IsTerminateOccured() {
            var unit = ReceiveUnit;
            if((unit != null) && (unit.Result == SerialPortControlMode.TerminateByUser || unit.Result == SerialPortControlMode.Error || unit.IsTimeoutOccures)) {
                IsCancelSerialPortOperation = false;
                return true;
            }
            return false;
        }
        void OnSelectedPageIndexChanged(int newIndex) {
            TerminalMode = newIndex == 7;
            switch(newIndex) {
                case 0:
                    SourceId = "CommonConfiguration";
                    break;
                case 1:
                    SourceId = "ChannelsConfiguration";
                    break;
                case 2:
                    SourceId = "TimeAndErasing";
                    break;
                case 3:
                    SourceId = "CurrentOutput";
                    break;
                case 4:
                    SourceId = "MudCalculator";
                    break;
                case 5:
                    SourceId = "ChannelInfo";
                    break;
                case 6:
                    SourceId = "RegistersManager";
                    break;
                case 7:
                    SourceId = "Terminal";
                    break;
            }
        }
    }
}
