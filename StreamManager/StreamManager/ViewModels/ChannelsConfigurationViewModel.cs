using Modbus.Core;
using Mvvm.Core;
using Core;
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
    public class ChannelsConfigurationViewModel : StreamViewModelBase {
        protected override string SourceId { get { return "ChannelsConfiguration"; } }

        public ChannelsConfigurationViewModel() {
            ReadCommand = new DelegateCommand(Read, CanReadWrite);
            WriteCommand = new DelegateCommand(Write, CanReadWrite);
            CreateDocumentCommand = new DelegateCommand(ReadAndCreateDocument, CanCreateDocument);
            GetChannelsCountCommand = new DelegateCommand(GetChannelsCountAndSerial, () => true);
        }

        #region Properties

        int channelsCount = 0;
        ChannelData channel1Data;
        ChannelData channel2Data;
        ChannelData channel3Data;
        ChannelData channel4Data;
        ChannelData channel5Data;

        int version;
        int serial;
        bool allowCreateProtocol;

        List<string> profileTypes;
        List<UInt16> coefficients;

        public int ChannelsCount { get { return channelsCount; } set { SetPropertyValue("ChannelsCount", ref channelsCount, value); } }
        public ChannelData Channel1Data { get { return channel1Data; } set { SetPropertyValue("Channel1Data", ref channel1Data, value); } }
        public ChannelData Channel2Data { get { return channel2Data; } set { SetPropertyValue("Channel2Data", ref channel2Data, value); } }
        public ChannelData Channel3Data { get { return channel3Data; } set { SetPropertyValue("Channel3Data", ref channel3Data, value); } }
        public ChannelData Channel4Data { get { return channel4Data; } set { SetPropertyValue("Channel4Data", ref channel4Data, value); } }
        public ChannelData Channel5Data { get { return channel5Data; } set { SetPropertyValue("Channel5Data", ref channel5Data, value); } }
        public int Serial { get { return serial; } set { SetPropertyValue("Serial", ref serial, value); } }
        public int Version { get { return version; } set { SetPropertyValue("Version", ref version, value); } }
        #endregion

        public ICommand ReadCommand { get; private set; }
        public ICommand WriteCommand { get; private set; }
        public ICommand CreateDocumentCommand { get; private set; }
        public ICommand GetChannelsCountCommand { get; private set; }

        void GetChannelsCountAndSerial() {
            Scenarious.CreateScenario("GetChannelsCountScenario", (byte)MainViewModel.Address);
            Scenarious.AddReadUnit(0x00A4, 1, unitId: "GetChannelsCount");
            Scenarious.AddReadUnit(0x00FB, 2, unitId: "GetSerial");
            ExecuteNext();
        }
        void Read() {
            Scenarious.CreateScenario("ReadChannelsConfigurationScenario", (byte)MainViewModel.Address);
            Scenarious.AddReadUnit(0x00B8, 1, unitId: "ReadProfileValues");
            Scenarious.AddReadUnit(0x00B9, 10, unitId: "ReadConfigStep1");
            Scenarious.AddReadUnit(0x00C3, 10, unitId: "ReadConfigStep2");
            Scenarious.AddReadUnit(0x00CD, 10, unitId: "ReadConfigStep3");
            ExecuteNext();
            allowCreateProtocol = false;
        }
        void Write() {
            EncodeChannelsData();
            Scenarious.CreateScenario("WriteChannelsConfigurationScenario", (byte)MainViewModel.Address);
            Scenarious.AddWriteUnit(0x00E9, new List<UInt16>() { 0x0FED });
            Scenarious.AddWriteUnit(0x00B8, new List<UInt16>() { EncodeProfileTypes() });
            Scenarious.AddWriteUnit(0x00B9, coefficients.Take(10).ToList());
            Scenarious.AddWriteUnit(0x00C3, coefficients.Skip(10).Take(10).ToList());
            Scenarious.AddWriteUnit(0x00CD, coefficients.Skip(20).ToList());
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
        bool CanReadWrite() {
            return ChannelsCount != 0;
        }
        bool CanCreateDocument() {
            return ChannelsCount != 0;
        }

        protected override Dictionary<string, Action<List<UInt16>>> CreateReadActions() {
            var result = new Dictionary<string, Action<List<UInt16>>>();
            result.Add("GetChannelsCount", data => ChannelsCount = data[0]);
            result.Add("GetSerial", data => {
                Serial = data[0];
                Version = data[1];
                RaiseCanExecuteCommand();
            });
            result.Add("ReadProfileValues", data => DecodeProfileTypes(data[0]));
            result.Add("ReadConfigStep1", data => {
                coefficients = new List<ushort>();
                ConcatCoeffs(data);
            });
            result.Add("ReadConfigStep2", data => ConcatCoeffs(data));
            result.Add("ReadConfigStep3", data => {
                ConcatCoeffs(data);
                DecodeChannelsData();
                CreateProtocolIfNeeded();
            });
            return result;
        }
        void CreateDocument() {
            var path = GetDocumentPath();
            using(StreamWriter sw = new StreamWriter(path)) {
                sw.WriteLine(ReportFactory.CreateChannelsConfigProtocol(DateTime.Now, MainViewModel.OperatorName, Serial, MainViewModel.Address,
                             Version, ChannelsCount, new ChannelData[] { Channel1Data, Channel2Data, Channel3Data, Channel4Data, Channel5Data }));
            }
            ReportFactory.ShowProtocol(path);
        }

        void DecodeChannelsData() {
            List<List<UInt16>> channelsData = new List<List<UInt16>>();
            Utils.Iterate(ChannelsCount, x => channelsData.Add(new List<UInt16>()));

            int initialIndex = 15;
            Utils.Iterate(ChannelsCount, j => {
                var i = initialIndex + 3 * j;
                channelsData[j].Add(coefficients[i + 1]);
                channelsData[j].Add(coefficients[i + 2]);
                channelsData[j].Add(coefficients[i]);
            });

            initialIndex = 0;
            Utils.Iterate(ChannelsCount, j => {
                int i = initialIndex + 3 * j;
                int remainCount = ChannelsConfigInfo.GetCoeffsCount(profileTypes[j]) - 3;
                Utils.Iterate(remainCount, k => channelsData[j].Add(coefficients[i + k]));

                if(j < 2 && profileTypes[j] == "UChannel") {
                    channelsData[j].RemoveAt(channelsData[j].Count - 1);
                    channelsData[j].RemoveAt(channelsData[j].Count - 1);
                    channelsData[j].Add(coefficients[9 + 3 * j]);
                    channelsData[j].Add(coefficients[9 + 3 * j + 1]);
                }
            });

            Utils.Iterate(ChannelsCount, i => {
                var channelconfig = ChannelsConfigInfo.FindInfo(profileTypes[i]);
                var doubleCoeffs = new List<double>();
                Utils.Iterate(channelconfig.CoeffsCount, j => doubleCoeffs.Add(channelsData[i][j] * channelconfig.CoeffsInfo[j].ViewRatio));
                SetProperty(string.Format("Channel{0}Data", i + 1), new ChannelData(profileTypes[i], doubleCoeffs));
            });
        }

        void EncodeChannelsData() {
            coefficients = new List<UInt16>();
            Utils.Iterate(30, x => coefficients.Add(0));

            int initialIndex = 15;
            Utils.Iterate(ChannelsCount, i => {
                var channelData = GetChannelData(i);
                var channelConfig = ChannelsConfigInfo.FindInfo(channelData.ProfileType);

                coefficients[initialIndex + 3 * i] = (UInt16)(channelData.Data[2] * Math.Pow(10, channelConfig.CoeffsInfo[2].Precision));
                coefficients[initialIndex + 3 * i + 1] = (UInt16)(channelData.Data[0] * Math.Pow(10, channelConfig.CoeffsInfo[0].Precision));
                coefficients[initialIndex + 3 * i + 2] = (UInt16)(channelData.Data[1] * Math.Pow(10, channelConfig.CoeffsInfo[1].Precision));
            });

            initialIndex = 0;
            Utils.Iterate(ChannelsCount, i => {
                var channelData = GetChannelData(i);
                var channelConfig = ChannelsConfigInfo.FindInfo(channelData.ProfileType);
                int remainCount = Math.Min(3, channelConfig.CoeffsCount - 3);

                Utils.Iterate(remainCount, j =>
                coefficients[3 * i + j] = (UInt16)(channelData.Data[3 + j] / channelConfig.CoeffsInfo[3 + j].ViewRatio));
                if(i < 2 && channelData.ProfileType == "UChannel") {
                    coefficients[3 * i + 9] = (UInt16)(channelData.Data[6] * Math.Pow(10, channelConfig.CoeffsInfo[6].Precision));
                    coefficients[3 * i + 9 + 1] = (UInt16)(channelData.Data[7] * Math.Pow(10, channelConfig.CoeffsInfo[7].Precision));
                }
            });
        }

        void ConcatCoeffs(List<UInt16> newCoeffs) {
            coefficients = coefficients.Concat(newCoeffs).ToList();
        }
        protected override void OnSelectedPageIndexChanged(int newPageIndex) {
            ChannelsCount = 0;
            RaiseCanExecuteCommand();
        }
        void CreateProtocolIfNeeded() {
            if(allowCreateProtocol) {
                CreateDocument();
                allowCreateProtocol = false;
            }
        }

        void DecodeProfileTypes(UInt16 profiles) {
            profileTypes = new List<string>();
            Utils.Iterate(ChannelsCount, x => {
                var code = (UInt16)(profiles >> (3 * x)) & 0x0007;
                profileTypes.Add(ChannelsConfigInfo.ProfileTypes[code]);
            });
        }
        UInt16 EncodeProfileTypes() {
            UInt16 result = 0;
            Utils.Iterate(ChannelsCount, x => {
                var code = ChannelsConfigInfo.ProfileCodes[GetChannelData(x).ProfileType];
                result = (UInt16)(result + (code << (3 * x)));
            });
            return result;
        }
        ChannelData GetChannelData(int index) {
            return (ChannelData)GetProperty(string.Format("Channel{0}Data", index + 1));
        }
        string GetDocumentPath() {
            return FilePathHelper.WriteChannelsConfig(Serial);
        }
    }
}
