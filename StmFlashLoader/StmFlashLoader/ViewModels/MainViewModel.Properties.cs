using Controls.Core;
using Mvvm.Core;
using StmFlashLoader.LoaderCore;
using StmFlashLoader.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Mvvm.ViewModels {
    public partial class MainViewModel : ObservableObject {
        bool hasPorts;
        bool inProgress;
        bool isCancelSerialPortOperation;
        bool isNormalStateCancelSerialPortOperation;
        ICommand appendTextCommand;
        ICommand clearTextCommand;

        int address;
        int serial;
        int newAddress;
        int newSerial;
        McuConfig mcuConfig;
        int baudrate;
        List<string> serialPorts;
        string selectedPort;
        bool waitUsbConnect;
        bool allowBaudrateSelector = true;

        int serialPortBaudrate = SerialPortControl.DefaultBaudrate;
        bool isModbusScenario;

        TransmitUnit transmitUnit;
        ReceiveUnit receieveUnit;

        string filePath;
        List<byte> firmware;
        int fileSize;

        public ICommand SelectFileCommand { get; private set; }
        public ICommand LoadFlashCommand { get; private set; }
        public ICommand ReadSerialCommand { get; private set; }
        public ICommand ChangeAddressCommand { get; private set; }
        public ICommand ChangeSerialCommand { get; private set; }

        public bool HasPorts { get { return hasPorts; } set { SetPropertyValue("HasPorts", ref hasPorts, value); } }
        public bool InProgress { get { return inProgress; } set { SetPropertyValue("InProgress", ref inProgress, value); } }
        public bool IsCancelSerialPortOperation { get { return isCancelSerialPortOperation; } set { SetPropertyValue("IsCancelSerialPortOperation", ref isCancelSerialPortOperation, value); } }
        public bool IsNormalStateCancelSerialPortOperation { get { return isNormalStateCancelSerialPortOperation; } set { SetPropertyValue("IsNormalStateCancelSerialPortOperation", ref isNormalStateCancelSerialPortOperation, value); } }
        public bool AllowBaudrateSelector { get { return allowBaudrateSelector; } set { SetPropertyValue("AllowBaudrateSelector", ref allowBaudrateSelector, value); } }

        public ICommand AppendTextCommand { get { return appendTextCommand; } set { SetPropertyValue("AppendTextCommand", ref appendTextCommand, value); } }
        public ICommand ClearTextCommand { get { return clearTextCommand; } set { SetPropertyValue("ClearTextCommand", ref clearTextCommand, value); } }

        public int Address { get { return address; } set { SetPropertyValue("Address", ref address, value); } }
        public int Serial { get { return serial; } set { SetPropertyValue("Serial", ref serial, value); } }
        public int NewAddress { get { return newAddress; } set { SetPropertyValue("NewAddress", ref newAddress, value); } }
        public int NewSerial { get { return newSerial; } set { SetPropertyValue("SewSerial", ref newSerial, value); } }

        public McuConfig McuConfig { get { return mcuConfig; } set { SetPropertyValue("McuConfig", ref mcuConfig, value, x => OnMcuConfigChanged()); } }
        public int Baudrate { get { return baudrate; } set { SetPropertyValue("Baudrate", ref baudrate, value); } }
        public int SerialPortBaudrate { get { return serialPortBaudrate; } set { SetPropertyValue("SerialPortBaudrate", ref serialPortBaudrate, value); } }

        public TransmitUnit TransmitUnit { get { return transmitUnit; } set { SetPropertyValue("TransmitUnit", ref transmitUnit, value); } }
        public ReceiveUnit ReceiveUnit { get { return receieveUnit; } set { SetPropertyValue("ReceiveUnit", ref receieveUnit, value, x => OnReceiveUnitChanged(x)); } }

        public string FilePath { get { return filePath; } set { SetPropertyValue("FilePath", ref filePath, value); } }

        public List<string> SerialPorts { get { return serialPorts; } set { SetPropertyValue("SerialPorts", ref serialPorts, value, x=> OnSerialPortsChanged()); } }
        public string SelectedPort { get { return selectedPort; } set { SetPropertyValue("SelectedPort", ref selectedPort, value); } }
        public bool WaitUsbConnect { get { return waitUsbConnect; } set { SetPropertyValue("WaitUsbConnect", ref waitUsbConnect, value, x => OnWaitUsbConnectChanged()); } }


        void InitFieldsAndProperties() {
            SelectFileCommand = new DelegateCommand(SelectFile, () => CanSelectFile());
            LoadFlashCommand = new DelegateCommand(LoadFlash, () => CanLoadFlash());
            ReadSerialCommand = new DelegateCommand(ReadSerial, () => CanReadSerial());
            ChangeAddressCommand = new DelegateCommand(ChangeAddress, () => CanChangeId());
            ChangeSerialCommand = new DelegateCommand(ChangeSerial, () => CanChangeId());

            FilePath = FileHelper.GetFilePath();
        }
    }
}
