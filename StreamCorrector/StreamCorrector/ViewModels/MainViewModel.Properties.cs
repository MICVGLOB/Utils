using Controls.Core;
using Mvvm.Core;
using StreamCorrector.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Mvvm.ViewModels {
    public partial class MainViewModel {
        string correctorHeader;
        static string Version { get { return "1.00"; } }

        static UInt16 BridgeTaskAddress { get { return 0xab; } }
        static UInt16 BridgeReadAddress { get { return 0xae; } }
        static UInt16 ModbusReadFunction { get { return 3; } }
        static UInt16 ModbusWriteFunction { get { return 16; } }

        string correctorDescription;
        bool wantToAskBridgeAddress;
        int modbusBridgeAddressBindable;
        int modbusBridgeAddressBindableDefault;
        bool isCancelSerialPortOperation;

        string updateStatusString;
        UpdateStatus updateStatus;

        public ICommand UpdateCommand { get; private set; }
        bool hasPorts;
        bool inProgress;

        double inputValueBindable_Reg0;
        double inputValueBindable_Reg1;
        double inputValueBindable_Reg2;
        double inputValueBindable_Reg3;

        int inputValuePrecision_Reg0 = 1;
        int inputValuePrecision_Reg1 = 1;
        int inputValuePrecision_Reg2 = 1;
        int inputValuePrecision_Reg3 = 1;

        double inputValueMin_Reg0 = 0;
        double inputValueMin_Reg1 = 0;
        double inputValueMin_Reg2 = 0;
        double inputValueMin_Reg3 = 0;

        double inputValueMax_Reg0 = 1;
        double inputValueMax_Reg1 = 1;
        double inputValueMax_Reg2 = 1;
        double inputValueMax_Reg3 = 1;

        double inputValueDefault_Reg0 = 0.5;
        double inputValueDefault_Reg1 = 0.5;
        double inputValueDefault_Reg2 = 0.5;
        double inputValueDefault_Reg3 = 0.5;

        string description_Reg0 = "Регистр 0";
        string description_Reg1 = "Регистр 1";
        string description_Reg2 = "Регистр 2";
        string description_Reg3 = "Регистр 3";

        bool wantToAsk_Reg0;
        bool wantToAsk_Reg1;
        bool wantToAsk_Reg2;
        bool wantToAsk_Reg3;

        TransmitUnit transmitUnit;
        ReceiveUnit receieveUnit;

        public string CorrectorHeader { get { return correctorHeader; } set { SetPropertyValue("CorrectorHeader", ref correctorHeader, value); } }
        public string CorrectorDescription { get { return correctorDescription; } set { SetPropertyValue("CorrectorDescription", ref correctorDescription, value); } }
        public bool WantToAskBridgeAddress { get { return wantToAskBridgeAddress; } set { SetPropertyValue("WantToAskBridgeAddress", ref wantToAskBridgeAddress, value); } }
        public string UpdateStatusString { get { return updateStatusString; } set { SetPropertyValue("UpdateStatusString", ref updateStatusString, value); } }
        public UpdateStatus UpdateStatus { get { return updateStatus; } set { SetPropertyValue("UpdateStatus", ref updateStatus, value); } }

        public int ModbusBridgeAddressBindable { get { return modbusBridgeAddressBindable; } set { SetPropertyValue("ModbusBridgeAddressBindable", ref modbusBridgeAddressBindable, value); } }
        public int ModbusBridgeAddressBindableDefault { get { return modbusBridgeAddressBindableDefault; } set { SetPropertyValue("ModbusBridgeAddressBindableDefault", ref modbusBridgeAddressBindableDefault, value); } }

        public double InputValueBindable_Reg0 { get { return inputValueBindable_Reg0; } set { SetPropertyValue("InputValueBindable_Reg0", ref inputValueBindable_Reg0, value); } }
        public double InputValueBindable_Reg1 { get { return inputValueBindable_Reg1; } set { SetPropertyValue("InputValueBindable_Reg1", ref inputValueBindable_Reg1, value); } }
        public double InputValueBindable_Reg2 { get { return inputValueBindable_Reg2; } set { SetPropertyValue("InputValueBindable_Reg2", ref inputValueBindable_Reg2, value); } }
        public double InputValueBindable_Reg3 { get { return inputValueBindable_Reg3; } set { SetPropertyValue("InputValueBindable_Reg3", ref inputValueBindable_Reg3, value); } }

        public int InputValuePrecision_Reg0 { get { return inputValuePrecision_Reg0; } set { SetPropertyValue("InputValuePrecision_Reg0", ref inputValuePrecision_Reg0, value); } }
        public int InputValuePrecision_Reg1 { get { return inputValuePrecision_Reg1; } set { SetPropertyValue("InputValuePrecision_Reg1", ref inputValuePrecision_Reg1, value); } }
        public int InputValuePrecision_Reg2 { get { return inputValuePrecision_Reg2; } set { SetPropertyValue("InputValuePrecision_Reg2", ref inputValuePrecision_Reg2, value); } }
        public int InputValuePrecision_Reg3 { get { return inputValuePrecision_Reg3; } set { SetPropertyValue("InputValuePrecision_Reg3", ref inputValuePrecision_Reg3, value); } }

        public double InputValueMin_Reg0 { get { return inputValueMin_Reg0; } set { SetPropertyValue("InputValueMin_Reg0", ref inputValueMin_Reg0, value); } }
        public double InputValueMin_Reg1 { get { return inputValueMin_Reg1; } set { SetPropertyValue("InputValueMin_Reg1", ref inputValueMin_Reg1, value); } }
        public double InputValueMin_Reg2 { get { return inputValueMin_Reg2; } set { SetPropertyValue("InputValueMin_Reg2", ref inputValueMin_Reg2, value); } }
        public double InputValueMin_Reg3 { get { return inputValueMin_Reg3; } set { SetPropertyValue("InputValueMin_Reg3", ref inputValueMin_Reg3, value); } }

        public double InputValueMax_Reg0 { get { return inputValueMax_Reg0; } set { SetPropertyValue("InputValueMax_Reg0", ref inputValueMax_Reg0, value); } }
        public double InputValueMax_Reg1 { get { return inputValueMax_Reg1; } set { SetPropertyValue("InputValueMax_Reg1", ref inputValueMax_Reg1, value); } }
        public double InputValueMax_Reg2 { get { return inputValueMax_Reg2; } set { SetPropertyValue("InputValueMax_Reg2", ref inputValueMax_Reg2, value); } }
        public double InputValueMax_Reg3 { get { return inputValueMax_Reg3; } set { SetPropertyValue("InputValueMax_Reg3", ref inputValueMax_Reg3, value); } }

        public double InputValueDefault_Reg0 { get { return inputValueDefault_Reg0; } set { SetPropertyValue("InputValueDefault_Reg0", ref inputValueDefault_Reg0, value); } }
        public double InputValueDefault_Reg1 { get { return inputValueDefault_Reg1; } set { SetPropertyValue("InputValueDefault_Reg1", ref inputValueDefault_Reg1, value); } }
        public double InputValueDefault_Reg2 { get { return inputValueDefault_Reg2; } set { SetPropertyValue("InputValueDefault_Reg2", ref inputValueDefault_Reg2, value); } }
        public double InputValueDefault_Reg3 { get { return inputValueDefault_Reg3; } set { SetPropertyValue("InputValueDefault_Reg3", ref inputValueDefault_Reg3, value); } }

        public string Description_Reg0 { get { return description_Reg0; } set { SetPropertyValue("Description_Reg0", ref description_Reg0, value); } }
        public string Description_Reg1 { get { return description_Reg1; } set { SetPropertyValue("Description_Reg1", ref description_Reg1, value); } }
        public string Description_Reg2 { get { return description_Reg2; } set { SetPropertyValue("Description_Reg2", ref description_Reg2, value); } }
        public string Description_Reg3 { get { return description_Reg3; } set { SetPropertyValue("Description_Reg3", ref description_Reg3, value); } }

        public bool WantToAsk_Reg0 { get { return wantToAsk_Reg0; } set { SetPropertyValue("WantToAsk_Reg0", ref wantToAsk_Reg0, value); } }
        public bool WantToAsk_Reg1 { get { return wantToAsk_Reg1; } set { SetPropertyValue("WantToAsk_Reg1", ref wantToAsk_Reg1, value); } }
        public bool WantToAsk_Reg2 { get { return wantToAsk_Reg2; } set { SetPropertyValue("WantToAsk_Reg2", ref wantToAsk_Reg2, value); } }
        public bool WantToAsk_Reg3 { get { return wantToAsk_Reg3; } set { SetPropertyValue("WantToAsk_Reg3", ref wantToAsk_Reg3, value); } }

        public bool HasPorts { get { return hasPorts; } set { SetPropertyValue("HasPorts", ref hasPorts, value); } }

        public TransmitUnit TransmitUnit { get { return transmitUnit; } set { SetPropertyValue("TransmitUnit", ref transmitUnit, value); } }
        public ReceiveUnit ReceiveUnit { get { return receieveUnit; } set { SetPropertyValue("ReceiveUnit", ref receieveUnit, value, (x) => OnReceiveUnitChanged(x)); } }
        public bool IsCancelSerialPortOperation { get { return isCancelSerialPortOperation; } set { SetPropertyValue("IsCancelSerialPortOperation", ref isCancelSerialPortOperation, value); } }

        public bool InProgress { get { return inProgress; } set { SetPropertyValue("InProgress", ref inProgress, value); } }

        public List<RegInfo> AskRegs;
        public List<RegInfo> BridgeRegs;
        public List<RegInfo> Group0Regs;
        public List<RegInfo> Group1Regs;

        void InitFieldsAndProperties() {
            BridgeRegs = new List<RegInfo>();
            Group0Regs = new List<RegInfo>();
            Group1Regs = new List<RegInfo>();
            UpdateCommand = new DelegateCommand(Update, () => CanUpdate());
            CorrectorHeader = string.Format("{0} (v.{1}).", updateData.CorrectorHeader, MainViewModel.Version);
            CorrectorDescription = updateData.CorrectorDescription;
            if(updateData.AskTheBridgeAddress) {
                WantToAskBridgeAddress = true;
                ModbusBridgeAddressBindableDefault = updateData.ModbusBridgeAddress;
                ModbusBridgeAddressBindable = updateData.ModbusBridgeAddress;
            }
            AskRegs = updateData.RegsInfo.Where(x => x.AskTheUser).ToList();
            BridgeRegs = updateData.RegsInfo.Where(x => !x.IsSensorReg).ToList();
            var groups = updateData.RegsInfo.Where(x => x.IsSensorReg).GroupBy(x => x.ModbusSensorAddress);
            if(groups.Count() > 0) {
                Group0Regs = groups.ElementAt(0).ToList();
                Group0Address = (byte)Group0Regs.First().ModbusSensorAddress;
            }
            if(groups.Count() > 1) {
                Group1Regs = groups.ElementAt(1).ToList();
                Group1Address = (byte)Group1Regs.First().ModbusSensorAddress;
            }
            int askRegsCounter = 0;
            updateData.RegsInfo.ForEach(info => {
                if(askRegsCounter > 3)
                    return;
                if(info.AskTheUser) {
                    Helpers.SetPropertyValue(string.Format("InputValuePrecision_Reg{0}", askRegsCounter), info.InputValuePrecision, this);
                    Helpers.SetPropertyValue(string.Format("InputValueMin_Reg{0}", askRegsCounter), info.InputValueMin, this);
                    Helpers.SetPropertyValue(string.Format("InputValueMax_Reg{0}", askRegsCounter), info.InputValueMax, this);
                    Helpers.SetPropertyValue(string.Format("InputValueDefault_Reg{0}", askRegsCounter), info.InputValue, this);
                    Helpers.SetPropertyValue(string.Format("InputValueBindable_Reg{0}", askRegsCounter), info.InputValue, this);
                    Helpers.SetPropertyValue(string.Format("Description_Reg{0}", askRegsCounter), info.RegDescription, this);
                    Helpers.SetPropertyValue(string.Format("WantToAsk_Reg{0}", askRegsCounter), true, this);
                    askRegsCounter++;
                }
            });
        }
        bool CanUpdate() {
            return HasPorts && !inProgress;
        }
    }
    public enum UpdateStatus {
        Progress,
        Waiting,
        Error,
        OK
    }
}
