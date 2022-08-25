using Modbus.Core;
using Mvvm.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using TiltsVisualizerLight.Base;
using TiltsVisualizerLight.Controls;
using TiltsVisualizerLight.Reports;
using TiltsVisualizerLight.Utils;

namespace TiltsVisualizerLight.ViewModels {
    public class UniversalConfigurationViewModel : ObservableObject {
        public UniversalConfigurationViewModel() {
            ReadIdsCommand = new DelegateCommand(ReadIdentifiers, () => !OperationInProgress);
            WriteIdsCommand = new DelegateCommand(WriteIdentifiers, () => !OperationInProgress);

            WriteDateTimeCommand = new DelegateCommand(WriteDateTime, () => !OperationInProgress);
            ReadDateTimeCommand = new DelegateCommand(ReadDateTime, () => !AllowZigBee && !OperationInProgress);

            CalcCoeffCommand = new DelegateCommand(CalculateCoefficients, CanCalculateCoefficients);
            CreateReportCommand = new DelegateCommand(CreateReport, CanCreateReport);

            LoadCoeffFromFileCommand = new DelegateCommand(LoadCoefficientsFromFile, CanLoadCoefficientsFromFile);
            ReadCoeffCommand = new DelegateCommand(ReadCoefficients, () => !OperationInProgress);
            WriteCoeffCommand = new DelegateCommand(WriteCoefficients, () => !OperationInProgress);

            ReadConfigCommand = new DelegateCommand(ReadCommonConfiguration, () => !OperationInProgress);
            WriteConfigCommand = new DelegateCommand(WriteCommonConfiguration, () => !OperationInProgress);

            ReadLineCommand = new DelegateCommand<VerifiedValue<UInt16>>(ReadLineData, CanReadLineData);

            ReadNextLineCommand = new DelegateCommand(ReadNextLineData, CanReadNextLineData);

            SensorDateTime = DateTime.FromBinary(0);

            SensorModes = new List<SensorModeInfo>() { new SensorModeInfo(0, "без буфера ОЗУ"), new SensorModeInfo(1, "с буфером ОЗУ"), new SensorModeInfo(2, "калибровка"), new SensorModeInfo(3, "по углам превышения") };
            SelectedSensorMode = SensorModes[2];
            OnSensorSerialChanged();
            TerminalMessageLength = MainViewModel.SensorInfoMessageLength;
            Baudrate = FileHelper.GetSerialPortConfig().ModbusBaudrate;
        }

        #region Properties
        MainViewModel mainViewModel;

        int sensorAddress;
        int sensorActualAddress;
        int sensorSerial;
        int zigBeeId16;
        int zigBeeId64;

        DateTime systemDateTime;
        DateTime sensorDateTime;

        int currentX;
        int minX;
        int maxX;
        int currentY;
        int minY;
        int maxY;
        int currentZ;
        int minZ;
        int maxZ;

        int zeroX;
        int scaleX;
        int zeroY;
        int scaleY;
        int zeroZ;
        int scaleZ;

        public List<SensorModeInfo> sensorModes;
        public SensorModeInfo selectedSensorMode;
        public int connectionInterval;
        public int measurementInterval;
        public int heartbeatInterval;
        public int filterValue;
        public double tiltOffsetAngle;
        public double noticeAngle;
        public double unsafeAngle;
        public int ramBufferSize;
        public string softVersion;

        TransmitUnit transmitUnit;
        ReceiveUnit receieveUnit;

        List<byte> receivedTerminalData;
        List<byte> transmittedTerminalData;

        bool isCancelSerialPortOperation;
        bool isCoefficientFileExist;

        bool operationInProgress;
        bool allowContinuesReading;

        bool allowZigBee;

        bool isTerminalMode;
        int terminalMessageLength;
        int baudrate;

        ICommand appendTextCommand;
        public ICommand ReadLineCommand { get; private set; }
        public ICommand ReadNextLineCommand { get; private set; }

        public int SensorAddress { get { return sensorAddress; } set { SetPropertyValue("SensorAddress", ref sensorAddress, value); } }
        public int SensorActualAddress { get { return sensorActualAddress; } set { SetPropertyValue("SensorActualAddress", ref sensorActualAddress, value); } }

        public int SensorSerial { get { return sensorSerial; } set { SetPropertyValue("SensorSerial", ref sensorSerial, value, x => OnSensorSerialChanged()); } }
        public int ZigBeeId16 { get { return zigBeeId16; } set { SetPropertyValue("ZigBeeId16", ref zigBeeId16, value); } }
        public int ZigBeeId64 { get { return zigBeeId64; } set { SetPropertyValue("ZigBeeId64", ref zigBeeId64, value); } }

        public DateTime SystemDateTime { get { return systemDateTime; } set { SetPropertyValue("SystemDateTime", ref systemDateTime, value); } }
        public DateTime SensorDateTime { get { return sensorDateTime; } set { SetPropertyValue("sensorDateTime", ref sensorDateTime, value); } }


        public int CurrentX { get { return currentX; } set { SetPropertyValue("CurrentX", ref currentX, value); } }
        public int MinX { get { return minX; } set { SetPropertyValue("MinX", ref minX, value); } }
        public int MaxX { get { return maxX; } set { SetPropertyValue("MaxX", ref maxX, value); } }

        public int CurrentY { get { return currentY; } set { SetPropertyValue("CurrentY", ref currentY, value); } }
        public int MinY { get { return minY; } set { SetPropertyValue("MinY", ref minY, value); } }
        public int MaxY { get { return maxY; } set { SetPropertyValue("MaxY", ref maxY, value); } }

        public int CurrentZ { get { return currentZ; } set { SetPropertyValue("CurrentZ", ref currentZ, value); } }
        public int MinZ { get { return minZ; } set { SetPropertyValue("MinZ", ref minZ, value); } }
        public int MaxZ { get { return maxZ; } set { SetPropertyValue("MaxZ", ref maxZ, value); } }

        public int ZeroX { get { return zeroX; } set { SetPropertyValue("ZeroX", ref zeroX, value); } }
        public int ScaleX { get { return scaleX; } set { SetPropertyValue("ScaleX", ref scaleX, value); } }
        public int ZeroY { get { return zeroY; } set { SetPropertyValue("ZeroY", ref zeroY, value); } }
        public int ScaleY { get { return scaleY; } set { SetPropertyValue("ScaleY", ref scaleY, value); } }
        public int ZeroZ { get { return zeroZ; } set { SetPropertyValue("ZeroZ", ref zeroZ, value); } }
        public int ScaleZ { get { return scaleZ; } set { SetPropertyValue("ScaleZ", ref scaleZ, value); } }

        public bool OperationInProgress { get { return operationInProgress; } set { SetPropertyValue("OperationInProgress", ref operationInProgress, value); } }

        public bool AllowZigBee { get { return allowZigBee; } set { SetPropertyValue("AllowZigBee", ref allowZigBee, value, x => OnAllowZigBeeChanged()); } }

        public bool AllowContinuesReading { get { return allowContinuesReading; } set { SetPropertyValue("AllowContinuesReading", ref allowContinuesReading, value, x => OnAllowContinuesReadingChanged()); } }
        public MainViewModel MainViewModel { get { return mainViewModel; } set { SetPropertyValue("MainViewModel", ref mainViewModel, value, x => OnMainViewModelChanged()); } }

        public List<SensorModeInfo> SensorModes { get { return sensorModes; } set { SetPropertyValue("SensorModes", ref sensorModes, value); } }
        public SensorModeInfo SelectedSensorMode { get { return selectedSensorMode; } set { SetPropertyValue("SelectedSensorMode", ref selectedSensorMode, value); } }

        public int ConnectionInterval { get { return connectionInterval; } set { SetPropertyValue("ConnectionInterval", ref connectionInterval, value); } }
        public int MeasurementInterval { get { return measurementInterval; } set { SetPropertyValue("MeasurementInterval", ref measurementInterval, value); } }
        public int HeartbeatInterval { get { return heartbeatInterval; } set { SetPropertyValue("HeartbeatInterval", ref heartbeatInterval, value); } }
        public int FilterValue { get { return filterValue; } set { SetPropertyValue("FilterValue", ref filterValue, value); } }
        public double TiltOffsetAngle { get { return tiltOffsetAngle; } set { SetPropertyValue("TiltOffsetAngle", ref tiltOffsetAngle, value); } }
        public double NoticeAngle { get { return noticeAngle; } set { SetPropertyValue("NoticeAngle", ref noticeAngle, value); } }
        public double UnsafeAngle { get { return unsafeAngle; } set { SetPropertyValue("UnsafeAngle", ref unsafeAngle, value); } }
        public int RamBufferSize { get { return ramBufferSize; } set { SetPropertyValue("RamBufferSize", ref ramBufferSize, value); } }
        public string SoftVersion { get { return softVersion; } set { SetPropertyValue("SoftVersion", ref softVersion, value); } }

        public TransmitUnit TransmitUnit { get { return transmitUnit; } set { SetPropertyValue("TransmitUnit", ref transmitUnit, value); } }
        public ReceiveUnit ReceiveUnit { get { return receieveUnit; } set { SetPropertyValue("ReceiveUnit", ref receieveUnit, value, x => OnReceiveUnitChanged()); } }

        public List<byte> ReceivedTerminalData { get { return receivedTerminalData; } set { SetPropertyValue("ReceivedTerminalData", ref receivedTerminalData, value, x => OnReceivedTerminalDataChanged()); } }
        public List<byte> TransmittedTerminalData { get { return transmittedTerminalData; } set { SetPropertyValue("TransmittedTerminalData", ref transmittedTerminalData, value); } }
        public bool IsTerminalMode { get { return isTerminalMode; } set { SetPropertyValue("IsTerminalMode", ref isTerminalMode, value); } }
        public int TerminalMessageLength { get { return terminalMessageLength; } set { SetPropertyValue("TerminalMessageLength", ref terminalMessageLength, value); } }
        public int Baudrate { get { return baudrate; } set { SetPropertyValue("Baudrate", ref baudrate, value); } }

        public bool IsCancelSerialPortOperation { get { return isCancelSerialPortOperation; } set { SetPropertyValue("IsCancelSerialPortOperation", ref isCancelSerialPortOperation, value); } }

        public ICommand AppendTextCommand { get { return appendTextCommand; } set { SetPropertyValue("AppendTextCommand", ref appendTextCommand, value); } }

        Dictionary<string, Action<List<UInt16>>> readDataActions;
        Dictionary<string, Action<List<UInt16>>> ReadDataActions {
            get {
                if(readDataActions == null)
                    readDataActions = CreateReadActions();
                return readDataActions;
            }
        }
        Dictionary<string, Action<List<ushort>>> CreateReadActions() {
            var result = new Dictionary<string, Action<List<UInt16>>>();
            result.Add("ReadDateTime", data => {
                DecodeDateTime(data);
            });
            result.Add("ReadIdentifiers", data => {
                DecodeIdentifiers(data);
            });
            result.Add("ReadCoefficients", data => {
                DecodeCoefficients(data);
            });
            result.Add("ReadCommonConfiguration", data => {
                DecodeCommonConfiguration(data);
            });
            result.Add("ReadRawData", data => {
                DecodeRawData(data);
            });
            result.Add("ReadSoftVersion", data => {
                DecodeSoftVersion(data);
            });
            result.Add("ReadNextLineData", data => {
                DecodeLineData(data);
            });
            return result;
        }
        protected virtual Action<string> WriteDataProcessing {
            get {
                return id => {
                    if(Scenarious.IsLastUnit() && id == "LastWriteUnit") {
                        IsCancelSerialPortOperation = true;
                        CommandManager.InvalidateRequerySuggested();
                    }
                };
            }
        }
        #endregion

        #region Commands
        public ICommand ReadIdsCommand { get; private set; }
        public ICommand WriteIdsCommand { get; private set; }

        public ICommand WriteDateTimeCommand { get; private set; }
        public ICommand ReadDateTimeCommand { get; private set; }

        public ICommand CalcCoeffCommand { get; private set; }
        public ICommand CreateReportCommand { get; private set; }
        public ICommand LoadCoeffFromFileCommand { get; private set; }
        public ICommand ReadCoeffCommand { get; private set; }
        public ICommand WriteCoeffCommand { get; private set; }

        public ICommand ReadConfigCommand { get; private set; }
        public ICommand WriteConfigCommand { get; private set; }
        #endregion


        void WriteIdentifiers() {
            Scenarious.CreateScenario("WriteIdentifiersScenario", (byte)SensorActualAddress);
            Scenarious.AddWriteUnit(0x0018, new List<UInt16>() { 0x0FED });
            Scenarious.AddWriteUnit(0x0020, EncodeIdentifiers(), unitId: "WriteIdentifiers");
            Scenarious.AddWriteUnit(0x0018, new List<UInt16>() { 0 });
            ExecuteNext();
        }
        void ReadIdentifiers() {
            Scenarious.CreateScenario("ReadIdentifiersScenario", (byte)SensorActualAddress);
            Scenarious.AddReadUnit(0x0020, 8, unitId: "ReadIdentifiers");
            ExecuteNext();
        }
        void WriteDateTime() {
            Scenarious.CreateScenario("WriteDateTimeScenario", (byte)SensorActualAddress);
            if(!AllowZigBee)
                Scenarious.AddWriteUnit(0x0018, new List<UInt16>() { 0x0FED });
            Scenarious.AddWriteUnit(0x0019, EncodeDateTime(), unitId: "WriteDateTime");
            if(!AllowZigBee)
                Scenarious.AddWriteUnit(0x0018, new List<UInt16>() { 0 });
            ExecuteNext();
            if(AllowZigBee)
                AddText("Запрос на УСТАНОВКУ ВРЕМЕНИ координатора отправлен!" + Environment.NewLine);
        }
        void ReadDateTime() {
            Scenarious.CreateScenario("ReadDateTimeScenario", (byte)SensorActualAddress);
            Scenarious.AddReadUnit(0x0019, 3, unitId: "ReadDateTime");
            ExecuteNext();
        }
        bool CanCalculateCoefficients() {
            return MainViewModel != null && MinX != 0 && MaxX != 0 && MinY != 0 && MaxY != 0 && MinZ != 0 && MaxZ != 0;
        }
        bool CanCreateReport() {
            return MainViewModel != null && ZeroX > 5000 && ScaleX > 5000 && ZeroY > 5000 && ScaleY > 5000 && ZeroZ > 5000 && ScaleZ > 5000;
        }
        void WriteCoefficients() {
            Scenarious.CreateScenario("WriteCoefficientsScenario", (byte)SensorActualAddress);
            Scenarious.AddWriteUnit(0x0018, new List<UInt16>() { 0x0FED });
            Scenarious.AddWriteUnit(0x0028, EncodeCoefficients(), unitId: "WriteCoefficients");
            Scenarious.AddWriteUnit(0x0018, new List<UInt16>() { 0 });
            ExecuteNext();
        }
        void ReadCoefficients() {
            Scenarious.CreateScenario("ReadCoefficientsScenario", (byte)SensorActualAddress);
            Scenarious.AddReadUnit(0x0028, 6, unitId: "ReadCoefficients");
            ExecuteNext();
        }
        void WriteCommonConfiguration() {
            Scenarious.CreateScenario("WriteCommonConfigurationScenario", (byte)SensorActualAddress);
            if(!AllowZigBee)
                Scenarious.AddWriteUnit(0x0018, new List<UInt16>() { 0x0FED });
            Scenarious.AddWriteUnit(0x002E, EncodeCommonConfiguration(), unitId: "WriteCommonConfiguration");
            if(!AllowZigBee)
                Scenarious.AddWriteUnit(0x0018, new List<UInt16>() { 0 });
            ExecuteNext();
            if(AllowZigBee)
                AddText("Запрос на ЗАПИСЬ конфигурации датчика отправлен!" + Environment.NewLine);
        }
        void ReadCommonConfiguration() {
            Scenarious.CreateScenario("ReadCommonConfigurationScenario", (byte)SensorActualAddress);
            Scenarious.AddReadUnit(0x002E, 9, unitId: "ReadCommonConfiguration");
            if(!AllowZigBee)
                Scenarious.AddReadUnit(0x0017, 1, unitId: "ReadSoftVersion");
            ExecuteNext();
            if(AllowZigBee)
                AddText("Запрос на ЧТЕНИЕ конфигурации датчика отправлен!" + Environment.NewLine);
        }

        public void LoadCoefficientsFromFile() {
            int zeroX, scaleX, zeroY, scaleY, zeroZ, scaleZ;
            MainViewModel.reportFactory.GetCalibrationCoefficients(FilePathHelper.Calibration(SensorSerial), out zeroX, out scaleX, out zeroY, out scaleY, out zeroZ, out scaleZ);
            ZeroX = zeroX;
            ScaleX = scaleX;
            ZeroY = zeroY;
            ScaleY = scaleY;
            ZeroZ = zeroZ;
            ScaleZ = scaleZ;
        }
        public bool CanLoadCoefficientsFromFile() {
            return isCoefficientFileExist && MainViewModel != null;
        }
        void OnSensorSerialChanged() {
            var path = FilePathHelper.Calibration(SensorSerial);
            isCoefficientFileExist = File.Exists(path);
        }
        void OnAllowContinuesReadingChanged() {
            if(AllowContinuesReading) {
                Scenarious.CreateScenario("ReadRawData", (byte)SensorActualAddress);
                Scenarious.AddReadUnit(0x08, 6, 30, 1000, SerialPortErrorMode.ErrorOnTimeout, SerialPortProgressType.Repeated);
                TransmitUnit = Scenarious.GetNextUnit().CreateTransmitUnit();
                MainViewModel.RestartRepeatTimer();
                MinX = 0;
                MaxX = 0;
                MinY = 0;
                MaxY = 0;
                MinZ = 0;
                MaxZ = 0;
                ZeroX = 5000;
                ScaleX = 5000;
                ZeroY = 5000;
                ScaleY = 5000;
                ZeroZ = 5000;
                ScaleZ = 5000;
            } else { 
                MainViewModel.StopRepeatTimer();
                IsCancelSerialPortOperation = true;
            }
            CommandManager.InvalidateRequerySuggested();
        }
        void OnRepeatTimerTick(object sender, EventArgs e) {
            TransmitUnit = Scenarious.GetCurrentUnit().CreateTransmitUnit();
        }
        void CalculateCoefficients() {
            ZeroX = CalcZeroCoeff(MinX, MaxX);
            ScaleX = CalcScaleCoeff(MinX, MaxX);
            ZeroY = CalcZeroCoeff(MinY, MaxY);
            ScaleY = CalcScaleCoeff(MinY, MaxY);
            ZeroZ = CalcZeroCoeff(MinZ, MaxZ);
            ScaleZ = CalcScaleCoeff(MinZ, MaxZ);
        }
        void CreateReport() {
            string path = FilePathHelper.Calibration(SensorSerial);
            if(File.Exists(path)) {
                var result = System.Windows.MessageBox.Show("Файл калибровки уже существует и будет перезаписан. Продолжить?", "Внимание!",
                 System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Warning);
                if(result == System.Windows.MessageBoxResult.No || result == System.Windows.MessageBoxResult.Cancel)
                    return;
            }
            string calibrationReport = MainViewModel.reportFactory.CreateCalibrationProtocol(DateTime.Now, "", SensorSerial, SensorActualAddress, MinX, MaxX, MinY, MaxY, ZeroX, 
                ScaleX, ZeroY, ScaleY, MinZ, MaxZ, ZeroZ, ScaleZ);
            using(StreamWriter sw = new StreamWriter(path)) {
                sw.WriteLine(calibrationReport);
            }
            MainViewModel.reportFactory.ShowProtocol(path);
        }
        void ReadLineData(VerifiedValue<UInt16> parameter) {
            Scenarious.CreateScenario("ReadLineDataScenario", (byte)SensorActualAddress);
            Scenarious.AddWriteUnit(0x0041, new List<UInt16>() { (UInt16)(parameter.Value + 1) });
            Scenarious.AddReadReportUnit(0x0040, 1, unitId: "ReadNextLineData");
            ExecuteNext();
        }
        bool CanReadLineData(VerifiedValue<UInt16> parameter) {
            return parameter != null && parameter.IsValid && MainViewModel != null && !AllowZigBee;
        }
        void ReadNextLineData() {
            Scenarious.CreateScenario("ReadNextLineDataScenario", (byte)SensorActualAddress);
            Scenarious.AddReadReportUnit(0x0040, 1, unitId: "ReadNextLineData");
            ExecuteNext();
        }
        bool CanReadNextLineData() {
            return MainViewModel != null && !AllowZigBee;
        }
        void OnReceiveUnitChanged() {
            if(IsTerminateOccured()) {
                AllowContinuesReading = false;
                return;
            }
            var id = Scenarious.GetCurrentUnit().Id;
            List<UInt16> data = GetReceivedData(ReceiveUnit);
            if(data != null && ReadDataActions.ContainsKey(id))
                ReadDataActions[id].Invoke(data);
            if(!Scenarious.IsLastUnit())
                ExecuteNext();
            WriteDataProcessing.Invoke(id);
        }

        public bool IsTerminateOccured() {
            var unit = ReceiveUnit;
            if((unit != null) && (unit.Result == SerialPortControlMode.TerminateByUser || unit.Result == SerialPortControlMode.Error || unit.IsTimeoutOccures)) {
                MainViewModel.StopRepeatTimer();
                IsCancelSerialPortOperation = false;
                return true;
            }
            return false;
        }
        void OnMainViewModelChanged() {
            if(MainViewModel != null) {
                MainViewModel.PropertyChanged += OnMainViewModelPropertyChanged;
                MainViewModel.RepeatTimer.Tick += OnRepeatTimerTick;
            }
        }
        void OnMainViewModelPropertyChanged(object sender, PropertyChangedEventArgs e) {
            if(e.PropertyName == "SystemDateTime")
                SystemDateTime = ((MainViewModel)sender).SystemDateTime;
            if(e.PropertyName == "SelectedPageIndex") {
                AllowContinuesReading = false;
                MainViewModel.StopRepeatTimer();
            }
        }
        void ExecuteNext() {
            var transmitUnit = Scenarious.GetNextUnit().CreateTransmitUnit();
            if(!AllowZigBee) {
                TransmitUnit = transmitUnit;
                return;
            }
            TransmittedTerminalData = AddZigBeeHeaderAndFooter(transmitUnit.Data);
        }
        void OnAllowZigBeeChanged() {
            MainViewModel.StopRepeatTimer();
            IsTerminalMode = AllowZigBee;
            var config = FileHelper.GetSerialPortConfig();
            Baudrate = AllowZigBee ? config.TerminalBaudrate : config.ModbusBaudrate;
        }

        void OnReceivedTerminalDataChanged() {
            if(ReceivedTerminalData == null || !ReceivedTerminalData.Any())
                return;
            DecodeLineDataFromZigBee(ReceivedTerminalData);
            Dispatcher.CurrentDispatcher.BeginInvoke((Action)(() => {
                ReceivedTerminalData.Clear();
            }));
        }

        void DecodeLineDataFromZigBee(List<byte> data) {
            var sb = new StringBuilder();
            var isModbusAnswer = (data[6] & 0x80) != 0;
            sb.Append(GetZigBeeHeaderInfo(data.Take(8).ToList(), isModbusAnswer));
            var payloadData = data.GetRange(12, data[11]);
            if(isModbusAnswer) {
                for(int i = 0; i < data[11]; i++)
                    sb.Append(data[i].ToString() + " ");
            } else
                sb.Append(StringsHelper.LineToLookUpStringConverter(payloadData));
            AddText(sb.ToString() + Environment.NewLine);
            if(isModbusAnswer) {
                bool parseOK;
                bool parseReadAnswer = false;
                TryParseModbusOverZigBee(payloadData, data[7], out parseOK, ref parseReadAnswer);
                if(!parseOK)
                    return;
                AddText((parseReadAnswer ? "Конфигурация измерителя прочитана, поля данных обновлены!" : "Новая конфигурация измерителя записана успешно!") + Environment.NewLine);
            }
        }

        string GetZigBeeHeaderInfo(List<byte> data, bool isModbusAnswer = false) {
            var sb = new StringBuilder();
            UInt32 utcTime = (((UInt32)data[0]) << 24) + (((UInt32)data[1]) << 16) + (((UInt32)data[2]) << 8) + data[3];
            sb.Append("Время " + (utcTime == 0xffffffff ? "не опр." : utcTime.ToString("X8")));
            sb.Append(" ");
            var addr = (((UInt16)data[4]) << 8) + data[5];
            sb.Append("Адр.16=" + addr.ToString() + " ");
            if(isModbusAnswer) {
                sb.Append("Возможно, это ответ Modbus! ");
            } else
                sb.Append("Строка=" + data[7] + " ");
            sb.Append("Полезн. данн. ");
            return sb.ToString();
        }

        void TryParseModbusOverZigBee(List<byte> data, int dataSize, out bool parseOK, ref bool parseReadAnswer) {
            parseOK = false;
            if(dataSize < 6 || dataSize > 37)
                return;
            var payloadData = data.Take(dataSize).ToList();
            var crc = ModbusHelper.CRC_Calc16(payloadData.Take(dataSize - 2).ToList());

            if(((byte)(crc >> 8) != payloadData[dataSize - 2]) || ((byte)crc != payloadData[dataSize - 1]))
                return;

            if(payloadData[1] == 16) {
                parseReadAnswer = false;
                parseOK = true;
            }
            if(payloadData[1] == 3) {
                if(dataSize != 23 || payloadData[2] != 18)
                    return;
                var fieldsData = payloadData.GetRange(3, payloadData[2]);
                SelectedSensorMode = SensorModes.FirstOrDefault(x => x.Id == fieldsData[1]);
                ConnectionInterval = (((UInt16)fieldsData[2]) << 8) + (UInt16)fieldsData[3];
                MeasurementInterval = (((UInt16)fieldsData[4]) << 8) + (UInt16)fieldsData[5];
                RamBufferSize = fieldsData[7];
                NoticeAngle = LineDecoder.AngleToView((UInt16)((((UInt16)fieldsData[8]) << 8) + (UInt16)fieldsData[9]));
                UnsafeAngle = LineDecoder.AngleToView((UInt16)((((UInt16)fieldsData[10]) << 8) + (UInt16)fieldsData[11]));
                HeartbeatInterval = (((UInt16)fieldsData[12]) << 8) + (UInt16)fieldsData[13];
                FilterValue = fieldsData[15];
                TiltOffsetAngle = LineDecoder.AngleToView((UInt16)((((UInt16)fieldsData[16]) << 8) + (UInt16)fieldsData[17]), multiplyer: 100);
                parseReadAnswer = true;
                parseOK = true;
            }
        }

        List<byte> AddZigBeeHeaderAndFooter(List<byte> source, int size = 44) {
            var content = new List<byte>() { 0xfb, 0xfb, 0xfb, (byte)source.Count }.Concat(source).ToList();
            int delta = size - content.Count;
            if(delta < 0)
                return new List<byte>();
            for(int i = 0; i < delta; i++)
                content.Add(0xff);
            return content;
        }
        List<UInt16> EncodeIdentifiers() {
            List<UInt16> data = new List<UInt16>();
            data.Add((UInt16)(SensorSerial >> 16));
            data.Add((UInt16)SensorSerial);
            data.Add((UInt16)SensorAddress);
            data.Add((UInt16)ZigBeeId16);
            data.Add(0);
            data.Add(0);
            data.Add((UInt16)(ZigBeeId64 >> 16));
            data.Add((UInt16)ZigBeeId64);
            return data;
        }
        void DecodeIdentifiers(List<UInt16> data) {
            SensorSerial = (int)(((UInt32)data[0] << 16) + data[1]);
            SensorAddress = data[2];
            ZigBeeId16 = data[3];
            ZigBeeId64 = (int)(((UInt32)data[6] << 16) + data[7]);
        }
        List<UInt16> EncodeDateTime() {
            List<UInt16> data = new List<UInt16>();
            data.Add((UInt16)(((UInt16)SystemDateTime.Hour << 8) + SystemDateTime.Minute));
            data.Add((UInt16)((UInt16)SystemDateTime.Day << 8));
            data.Add((UInt16)(((UInt16)(SystemDateTime.Year - 2000) << 8) + SystemDateTime.Month));
            return data;
        }
        void DecodeDateTime(List<UInt16> data) {
            int hour = data[0] >> 8;
            int minute = data[0] & 0x00FF;
            var day = data[1] >> 8;
            var year = (data[2] >> 8) + 2000;
            var month = data[2] & 0x00FF;
            SensorDateTime = new DateTime(year, month, day, hour, minute, 0);
        }
        List<UInt16> EncodeCoefficients() {
            List<UInt16> data = new List<UInt16>();
            data.Add((UInt16)ZeroX);
            data.Add((UInt16)ScaleX);
            data.Add((UInt16)ZeroY);
            data.Add((UInt16)ScaleY);
            data.Add((UInt16)ZeroZ);
            data.Add((UInt16)ScaleZ);
            return data;
        }
        void DecodeCoefficients(List<UInt16> data) {
            ZeroX = data[0];
            ScaleX = data[1];
            ZeroY = data[2];
            ScaleY = data[3];
            ZeroZ = data[4];
            ScaleZ = data[5];
        }
        List<UInt16> EncodeCommonConfiguration() {
            List<UInt16> data = new List<UInt16>();
            data.Add((UInt16)SelectedSensorMode.Id);
            data.Add((UInt16)ConnectionInterval);
            data.Add((UInt16)MeasurementInterval);
            data.Add((UInt16)RamBufferSize);
            data.Add(LineDecoder.AngleFromView(NoticeAngle));
            data.Add(LineDecoder.AngleFromView(UnsafeAngle));
            data.Add((UInt16)HeartbeatInterval);
            data.Add((UInt16)FilterValue);
            data.Add(LineDecoder.AngleFromView(TiltOffsetAngle, multiplyer: 100.0));
            return data;
        }
        void DecodeCommonConfiguration(List<UInt16> data) {
            SelectedSensorMode = SensorModes.FirstOrDefault(x => x.Id == data[0]);
            ConnectionInterval = data[1];
            MeasurementInterval = data[2];
            RamBufferSize = data[3];
            NoticeAngle = LineDecoder.AngleToView(data[4]);
            UnsafeAngle = LineDecoder.AngleToView(data[5]);
            HeartbeatInterval = data[6];
            FilterValue = data[7];
            TiltOffsetAngle = LineDecoder.AngleToView(data[8], multiplyer: 100);
        }
        void DecodeRawData(List<UInt16> data) {
            UInt32 x = (((UInt32)data[0]) << 16) + (UInt32)data[1];
            UInt32 y = (((UInt32)data[2]) << 16) + (UInt32)data[3];
            UInt32 z = (((UInt32)data[4]) << 16) + (UInt32)data[5];

            var intX = ConvertUintToIntDiv1024(x);
            var intY = ConvertUintToIntDiv1024(y);
            var intZ = ConvertUintToIntDiv1024(z);

            CurrentX = intX;
            MaxX = ValueToMaxValue(intX, MaxX);
            MinX = ValueToMinValue(intX, MinX);

            CurrentY = intY;
            MaxY = ValueToMaxValue(intY, MaxY);
            MinY = ValueToMinValue(intY, MinY);

            CurrentZ = intZ;
            MaxZ = ValueToMaxValue(intZ, MaxZ);
            MinZ = ValueToMinValue(intZ, MinZ);
        }

        void DecodeLineData(List<UInt16> data) {
            List<byte> byteData = new List<byte>();
            for(int i = 0; i < 19; i++) {
                byteData.Add((byte)(data[i] >> 8));
                byteData.Add((byte)data[i]);
            }
            AddText(StringsHelper.LineToLookUpStringConverter(byteData) + Environment.NewLine);
        }
        void AddText(string text) {
            if(AppendTextCommand != null)
                AppendTextCommand.Execute(text);
        }

        void DecodeSoftVersion(List<UInt16> data) {
            SoftVersion = (data[0] / 10.0).ToString("F1").Replace(",", ".");
        }
        int ConvertUintToIntDiv1024(UInt32 source) {
            if(source < 2147483648)
                return ((int)source) / 1024;
            else {
                UInt32 inverted = (~source) + 1;
                return ((int)inverted) / -1024;
            }
        }
        int ValueToMaxValue(int newValue, int oldValue) {
            if(newValue > 0 && newValue > oldValue)
                return newValue;
            return oldValue;
        }
        int ValueToMinValue(int newValue, int oldValue) {
            if(newValue < 0 && newValue < oldValue)
                return newValue;
            return oldValue;
        }

        int CalcZeroCoeff(int min, int max) {
            double result = 20000.0 * max / (max - min);
            return (int)Math.Round(result);
        }
        int CalcScaleCoeff(int min, int max) {
            double result = 2.0 * 4.0 * 256000.0 * 10000.0 / (max - min);
            return (int)Math.Round(result);
        }

        List<UInt16> GetReceivedData(ReceiveUnit unit) {
            var readUnit = Scenarious.GetCurrentUnit() as ModbusReadUnit;
            return readUnit == null ? null : readUnit.GetReceivedData(unit.Data);
        }
    }

    public class SensorModeInfo {
        public SensorModeInfo(int id, string name) {
            Id = id;
            Name = name;
        }
        public int Id { get; set; }
        public string Name { get; set; }
    }
}