using Controls.Core;
using Core;
using Modbus.Core;
using Mvvm.Core;
using StreamManager.Reports;
using StreamManager.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Mvvm.ViewModels {
    public class CommonConfigurationViewModel : StreamViewModelBase {
        protected override string SourceId { get { return "CommonConfiguration"; } }

        public CommonConfigurationViewModel() {
            ReadCommand = new DelegateCommand(Read, CanReadWrite);
            WriteCommand = new DelegateCommand(Write, CanReadWrite);
            CreateDocumentCommand = new DelegateCommand(ReadAndCreateDocument, CanReadAndCreateDocument);
            TestIndicatorCommand = new DelegateCommand(TestIndicator, () => true);
        }

        #region Properties

        int channelsCounter = 1;
        int level1Address;
        int velocity1Address;
        int level2Address;
        int velocity2Address;
        int level3Address;
        int velocity3Address;
        int level4Address;
        int velocity4Address;
        int level5Address;
        int velocity5Address;

        double uPower;
        int noWorkInterval;
        int serial;
        int reportInterval;
        string version;
        double maxVelocity;
        bool? protectionActive;
        double velocityLimit;
        int brightness;
        bool allowCartridgeConnection;
        bool disablePCConnection;
        bool allowIndicatiorOff;
        bool allowWirelessSensors;
        bool allowCreateProtocol;
        List<int> levelAddresses;
        List<int> velocityAddresses;

        public int ChannelsCounter { get { return channelsCounter; } set { SetPropertyValue("ChannelsCounter", ref channelsCounter, value); } }
        public int Level1Address { get { return level1Address; } set { SetPropertyValue("Level1Address", ref level1Address, value); } }
        public int Velocity1Address { get { return velocity1Address; } set { SetPropertyValue("Velocity1Address", ref velocity1Address, value); } }
        public int Level2Address { get { return level2Address; } set { SetPropertyValue("Level2Address", ref level2Address, value); } }
        public int Velocity2Address { get { return velocity2Address; } set { SetPropertyValue("Velocity2Address", ref velocity2Address, value); } }
        public int Level3Address { get { return level3Address; } set { SetPropertyValue("Level3Address", ref level3Address, value); } }
        public int Velocity3Address { get { return velocity3Address; } set { SetPropertyValue("Velocity3Address", ref velocity3Address, value); } }
        public int Level4Address { get { return level4Address; } set { SetPropertyValue("Level4Address", ref level4Address, value); } }
        public int Velocity4Address { get { return velocity4Address; } set { SetPropertyValue("Velocity4Address", ref velocity4Address, value); } }
        public int Level5Address { get { return level5Address; } set { SetPropertyValue("Level5Address", ref level5Address, value); } }
        public int Velocity5Address { get { return velocity5Address; } set { SetPropertyValue("Velocity5Address", ref velocity5Address, value); } }

        public double UPower { get { return uPower; } set { SetPropertyValue("UPower", ref uPower, value); } }
        public int NoWorkInterval { get { return noWorkInterval; } set { SetPropertyValue("NoWorkInterval", ref noWorkInterval, value); } }
        public int Serial { get { return serial; } set { SetPropertyValue("Serial", ref serial, value); } }
        public int ReportInterval { get { return reportInterval; } set { SetPropertyValue("ReportInterval", ref reportInterval, value); } }
        public string Version { get { return version; } set { SetPropertyValue("Version", ref version, value); } }
        public double MaxVelocity { get { return maxVelocity; } set { SetPropertyValue("MaxVelocity", ref maxVelocity, value); } }
        public bool? ProtectionActive { get { return protectionActive; } set { SetPropertyValue("ProtectionActive", ref protectionActive, value); } }
        public double VelocityLimit { get { return velocityLimit; } set { SetPropertyValue("VelocityLimit", ref velocityLimit, value); } }

        public bool AllowCartridgeConnection { get { return allowCartridgeConnection; } set { SetPropertyValue("AllowCartridgeConnection", ref allowCartridgeConnection, value); } }
        public bool DisablePCConnection { get { return disablePCConnection; } set { SetPropertyValue("DisablePCConnection", ref disablePCConnection, value); } }
        public bool AllowIndicatiorOff { get { return allowIndicatiorOff; } set { SetPropertyValue("AllowIndicatiorOff", ref allowIndicatiorOff, value); } }
        public bool AllowWirelessSensors { get { return allowWirelessSensors; } set { SetPropertyValue("AllowWirelessSensors", ref allowWirelessSensors, value); } }
        public int Brightness { get { return brightness; } set { SetPropertyValue("Brightness", ref brightness, value); } }

        #endregion

        public ICommand ReadCommand { get; private set; }
        public ICommand WriteCommand { get; private set; }
        public ICommand CreateDocumentCommand { get; private set; }
        public ICommand TestIndicatorCommand { get; private set; }

        void Read() {
            Scenarious.CreateScenario("ReadDataScenario", (byte)MainViewModel.Address);
            Scenarious.AddReadUnit(0x00A4, 6, unitId: "ReadStep1");
            Scenarious.AddReadUnit(0x00F9, 4, unitId: "ReadStep2");
            Scenarious.AddReadUnit(0x00D7, 3, unitId: "ReadStep3");
            Scenarious.AddReadUnit(0x00DF, 4, unitId: "ReadStep4");
            Scenarious.AddReadUnit(0x00B5, 1, unitId: "ReadStep5");
            ExecuteNext();
            allowCreateProtocol = false;
        }
        void Write() {
            Scenarious.CreateScenario("WriteDataScenario", (byte)MainViewModel.Address);
            Scenarious.AddWriteUnit(0x00E9, new List<UInt16>() { 0x0FED });
            Scenarious.AddWriteUnit(0x00A4, EncodeChannelsAddress());
            Scenarious.AddWriteUnit(0x00B5, new List<UInt16>() { (UInt16)(AllowIndicatiorOff ? 0 : 1), 0, GetConfigTime()});
            Scenarious.AddWriteUnit(0x00D7, new List<UInt16>() {
                (UInt16)ReportInterval,
                (UInt16)((UInt16)(AllowCartridgeConnection ? 0x0000 : 0x0001) + (UInt16)(DisablePCConnection ? 0x0002 : 0x0000)),
                (UInt16)Brightness
            });
            Scenarious.AddWriteUnit(0x00DF, new List<UInt16>() {
                (UInt16)(NoWorkInterval/10),
                (UInt16)(MaxVelocity * 10),
                (UInt16)(VelocityLimit * 10),
                (UInt16)(AllowWirelessSensors ? 1 : 0)
            });
            Scenarious.AddWriteUnit(0x00E9, new List<UInt16>() { 0 });
            ExecuteNext();
        }
        void ReadAndCreateDocument() {
            if(File.Exists(GetDocumentPath())) {
                var result = ShowWarningMessage("Файл с сохраненными данными уже существует и будет перезаписан. Продолжить?");
                if(result == MessageBoxResult.No || result == MessageBoxResult.Cancel)
                    return;
            }
            Read();
            allowCreateProtocol = true;
        }

        void TestIndicator() {
            Scenarious.CreateScenario("WriteDateTimeScenario", (byte)MainViewModel.Address);
            Scenarious.AddWriteUnit(0x00E9, new List<UInt16>() { 0x0FED });
            Scenarious.AddWriteUnit(0x00AA, new List<UInt16>() { 4 });
            ExecuteNext();
        }

        bool CanReadWrite() {
            return true;
        }
        bool CanReadAndCreateDocument() {
            return !FolderHelper.IsReadOnly;
        }
        UInt16 GetConfigTime() {
            var month = (UInt16)DateTime.Now.Month;
            var year = (UInt16)(DateTime.Now.Year - 2000);
            return (UInt16)((month << 8) + year);
        }
        protected override Dictionary<string, Action<List<UInt16>>> CreateReadActions() {
            var result = new Dictionary<string, Action<List<UInt16>>>();
            result.Add("ReadStep1", data => {
                DecodeChannelsAddress(data);
            });
            result.Add("ReadStep2", data => {
                UPower = data[0] / 10.0;
                ProtectionActive = data[1] == 1;
                Serial = data[2];
                Version = StreamManager.Utils.Converters.FirmwareVersionToView(data[3]);
            });
            result.Add("ReadStep3", data => {
                ReportInterval = data[0];
                AllowCartridgeConnection = (data[1] & 0x0001) == 0;
                DisablePCConnection = (data[1] & 0x0002) != 0;
                Brightness = data[2];
            });
            result.Add("ReadStep4", data => {
                NoWorkInterval = StreamManager.Utils.Converters.NoWorkIntervalToView(data[0]);
                MaxVelocity = StreamManager.Utils.Converters.VelocityLimitToView(data[1]);
                VelocityLimit = StreamManager.Utils.Converters.VelocityLimitToView(data[2]);
                AllowWirelessSensors = data[3] == 1;
            });
            result.Add("ReadStep5", data => {
                AllowIndicatiorOff = (data[0] & 0x0001) == 0;
                if(allowCreateProtocol) {
                    CreateDocument();
                    allowCreateProtocol = false;
                }
            });
            return result;
        }
        string GetDocumentPath() {
            return FilePathHelper.WriteCommonConfig(Serial);
        }
        void CreateDocument() {
            var path = GetDocumentPath();
            using(StreamWriter sw = new StreamWriter(path)) {
                sw.WriteLine(ReportFactory.CreateCommonConfigProtocol(DateTime.Now, MainViewModel.OperatorName, Serial, MainViewModel.Address,
                             Version, ChannelsCounter, levelAddresses.ToArray(), velocityAddresses.ToArray(),
                             AllowIndicatiorOff, Brightness, ReportInterval, AllowCartridgeConnection, UPower, (bool)ProtectionActive,
                             NoWorkInterval, MaxVelocity, VelocityLimit));
            }
            ReportFactory.ShowProtocol(path);
        }

        void DecodeChannelsAddress(List<UInt16> data) {
            ChannelsCounter = data[0];
            levelAddresses = new List<int>();
            velocityAddresses = new List<int>();
            Utils.Iterate(ChannelsCounter, i => {
                var address1 = data[i + 1] >> 8;
                var address2 = data[i + 1] & 0x00FF;
                SetProperty(string.Format("Level{0}Address", i + 1), address1);
                SetProperty(string.Format("Velocity{0}Address", i + 1), address2);
                levelAddresses.Add(address1);
                velocityAddresses.Add(address2);
            });
        }
        List<UInt16> EncodeChannelsAddress() {
            var result = new List<UInt16>();
            result.Add((UInt16)ChannelsCounter);
            Utils.Iterate(ChannelsCounter, i => {
                var address1 = (UInt16)(int)GetProperty(string.Format("Level{0}Address", i + 1));
                var address2 = (UInt16)(int)GetProperty(string.Format("Velocity{0}Address", i + 1));
                result.Add((UInt16)((address1 << 8) + (UInt16)address2));
            });
            return result;
        }
    }
}
