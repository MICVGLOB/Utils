using Modbus.Core;
using Mvvm.Core;
using SensorsManager.Reports;
using SensorsManager.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Mvvm.ViewModels {
    public abstract class SensorViewModelBase : ObservableObject {
        protected ReportFactory factory;
        public SensorViewModelBase() {
            ReadCommand = new DelegateCommand(Read, CanReadWrite);
            WriteCommand = new DelegateCommand(Write, CanReadWrite);
            SaveCommand = new DelegateCommand(Save, CanSave);
            factory = new ReportFactory();
            ReadCoefficientsCommand = new DelegateCommand(ReadCoefficients, () => CanReadCoefficients());
        }
        MainViewModel mainViewModel;
        bool enableGSensorCoefficientsChanged = true;
        int hParam = 0;
        double angle = 0;
        string recordDate;
        int zeroX = 0;
        int scaleX = 0;
        int zeroY = 0;
        int scaleY = 0;
        Visibility fileWarningVisibility = Visibility.Collapsed;
        protected bool isCoefficientFileExist = false;
        protected bool isProgress;
        bool createWriteReport;

        public MainViewModel MainViewModel { get { return mainViewModel; } set { SetPropertyValue("MainViewModel", ref mainViewModel, value, x => OnMainViewModelChanged(x)); } }
        public bool EnableGSensorCoefficientsChanged { get { return enableGSensorCoefficientsChanged; } set { SetPropertyValue("EnableGSensorCoefficientsChanged", ref enableGSensorCoefficientsChanged, value); } }
        public int HParam { get { return hParam; } set { SetPropertyValue("HParam", ref hParam, value); } }
        public double Angle { get { return angle; } set { SetPropertyValue("Angle", ref angle, value); } }
        public string RecordDate { get { return recordDate; } set { SetPropertyValue("RecordDate", ref recordDate, value); } }
        public int ZeroX { get { return zeroX; } set { SetPropertyValue("ZeroX", ref zeroX, value); } }
        public int ScaleX { get { return scaleX; } set { SetPropertyValue("ScaleX", ref scaleX, value); } }
        public int ZeroY { get { return zeroY; } set { SetPropertyValue("ZeroY", ref zeroY, value); } }
        public int ScaleY { get { return scaleY; } set { SetPropertyValue("ScaleY", ref scaleY, value); } }
        public Visibility FileWarningVisibility { get { return fileWarningVisibility; } set { SetPropertyValue("FileWarningVisibility", ref fileWarningVisibility, value); } }


        public ICommand ReadCommand { get; private set; }
        public ICommand WriteCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }

        public ICommand ReadCoefficientsCommand { get; private set; }

        public void Read() {
            if(MainViewModel.UseBridge)
                Scenarious.CreateScenario("ReadData", (byte)MainViewModel.BridgeAddress, null, (byte)MainViewModel.Address,
                    () => AddVerificationUnits());
            else
                Scenarious.CreateScenario("ReadData", (byte)MainViewModel.Address, primaryAction: () => AddVerificationUnits());
            AddReadUnits();
            isProgress = true;
            Start();
        }
        public void Write() {
            if(!FolderHelper.IsReadOnly) {
                string path = GetWriteFilePath();
                if(File.Exists(path)) {
                    var result = ShowWarningMessage("Файл с сохраненными данными уже существует и будет перезаписан. Продолжить?");
                    if(result == MessageBoxResult.No || result == MessageBoxResult.Cancel)
                        return;
                }
                createWriteReport = true;
            }
            if(MainViewModel.UseBridge) {
                Scenarious.CreateScenario("WriteData", (byte)MainViewModel.BridgeAddress, (UInt16)MainViewModel.Serial,
                    (byte)MainViewModel.Address, () => AddVerificationUnits());
                var timeout = MainViewModel.SensorType == SensorType.IUG5_WIRELESS ? 30 : 0;
                List<UInt16> data = new List<UInt16>() { (UInt16)MainViewModel.Address, Constants.ModbusWriteFunction, GetBridgeRWStepPosition(1) }
                .Concat(ValuesFromView("Write_Step1")).ToList();
                Scenarious.AddWriteUnit(Constants.BridgeTaskAddress, data, unitId: "Write_Step1", transmitTimeout: timeout);
                data = new List<UInt16>() { (UInt16)MainViewModel.Address, Constants.ModbusWriteFunction, GetBridgeRWStepPosition(2) }
                .Concat(ValuesFromView("Write_Step2")).ToList();
                Scenarious.AddWriteUnit(Constants.BridgeTaskAddress, data, unitId: "Write_Step2", transmitTimeout: timeout);
                data = new List<UInt16>() { (UInt16)MainViewModel.Address, Constants.ModbusWriteFunction, GetBridgeRWStepPosition(3) }
                .Concat(ValuesFromView("Write_Step3")).ToList();
                Scenarious.AddWriteUnit(Constants.BridgeTaskAddress, data, unitId: "Write_Step3", transmitTimeout: timeout);
                AddReadUnits();
            } else {
                Scenarious.CreateScenario("WriteData", (byte)MainViewModel.Address, (UInt16)MainViewModel.Serial, primaryAction: () => AddVerificationUnits());
                Scenarious.AddWriteUnit(MainViewModel.SensorType == SensorType.IUG5_WIRELESS ? Constants.IUG5_StartRWFlashAddress : Constants.IUG1_2_StartRWFlashAddress, ValuesFromView("Write_Step1"));
                if(MainViewModel.SensorType != SensorType.IUG5_WIRELESS) {
                    Scenarious.AddWriteUnit((UInt16)(Constants.IUG1_2_StartRWFlashAddress + 0x05), ValuesFromView("Write_Step2"));
                    Scenarious.AddWriteUnit((UInt16)(Constants.IUG1_2_StartRWFlashAddress + 2 * 0x05), ValuesFromView("Write_Step3"));
                }
                AddReadUnits();
            }
            isProgress = true;
            Start();
        }

        void AddReadUnits() {
            if(MainViewModel.UseBridge) {
                Scenarious.AddWriteUnit(Constants.BridgeTaskAddress, new List<UInt16>() { (UInt16)MainViewModel.Address, Constants.ModbusReadFunction, 0x1E01 });
                Scenarious.AddReadUnit(Constants.BridgeReadAddress, 1, transmitTimeout: 1000, unitId: "Read_Serial");
                Scenarious.AddWriteUnit(Constants.BridgeTaskAddress, new List<UInt16>() { (UInt16)MainViewModel.Address, Constants.ModbusReadFunction, GetBridgeRWStepPosition(1) });
                Scenarious.AddReadUnit(Constants.BridgeReadAddress, 5, transmitTimeout: 1000, unitId: "Read_Step1");
                Scenarious.AddWriteUnit(Constants.BridgeTaskAddress, new List<UInt16>() { (UInt16)MainViewModel.Address, Constants.ModbusReadFunction, GetBridgeRWStepPosition(2) });
                Scenarious.AddReadUnit(Constants.BridgeReadAddress, 5, transmitTimeout: 1000, unitId: "Read_Step2");
                Scenarious.AddWriteUnit(Constants.BridgeTaskAddress, new List<UInt16>() { (UInt16)MainViewModel.Address, Constants.ModbusReadFunction, GetBridgeRWStepPosition(3) });
                Scenarious.AddReadUnit(Constants.BridgeReadAddress, 5, transmitTimeout: 1000, unitId: "Read_Step3");
                if(MainViewModel.SensorType == SensorType.IUG5_WIRELESS) {
                    Scenarious.AddWriteUnit(Constants.BridgeTaskAddress, new List<UInt16>() { (UInt16)MainViewModel.Address, Constants.ModbusReadFunction, GetBridgeRWStepPosition(4) });
                    Scenarious.AddReadUnit(Constants.BridgeReadAddress, 4, transmitTimeout: 1000, unitId: "Read_Step4");
                }
                Scenarious.AddWriteUnit(Constants.BridgeActivateTranslatorAddress, new List<ushort>() { 0 });
                Scenarious.AddWriteUnit(Constants.BridgeConfigurateAddress, new List<ushort>() { 0 });
            } else {
                Scenarious.AddReadUnit(0x1E, 1, unitId: "Read_Serial");
                Scenarious.AddReadUnit(MainViewModel.SensorType == SensorType.IUG5_WIRELESS ? Constants.IUG5_StartRWFlashAddress : Constants.IUG1_2_StartRWFlashAddress,
                    MainViewModel.SensorType == SensorType.IUG5_WIRELESS ? (UInt16)11 : (UInt16)5, unitId: "Read_Step1");
                if(MainViewModel.SensorType == SensorType.IUG5_WIRELESS)
                    Scenarious.AddReadUnit((UInt16)(Constants.IUG5_StartReadCoeffAddress), 4, unitId: "Read_Step4");
                else {
                    Scenarious.AddReadUnit((UInt16)(Constants.IUG1_2_StartRWFlashAddress + 0x05), 5, unitId: "Read_Step2");
                    Scenarious.AddReadUnit((UInt16)(Constants.IUG1_2_StartRWFlashAddress + 2 * 0x05), 5, unitId: "Read_Step3");
                }
            }
        }
        public void Save() {
            string path = GetReadFilePath();
            if(File.Exists(path)) {
                var result = ShowWarningMessage("Файл с сохраненными данными уже существует и будет перезаписан. Продолжить?");
                if(result == MessageBoxResult.No || result == MessageBoxResult.Cancel)
                    return;
            }
            string readProtocol = GetReadProtocol();
            using(StreamWriter sw = new StreamWriter(path)) {
                sw.WriteLine(readProtocol);
            }
            factory.ShowProtocol(path);
        }

        void AddVerificationUnits() {
            if(MainViewModel.UseBridge) {
                Scenarious.AddWriteUnit(Constants.BridgeTaskAddress, new List<UInt16>() { (UInt16)MainViewModel.Address, Constants.ModbusReadFunction,
                    (UInt16)(MainViewModel.SensorType == SensorType.IUG5_WIRELESS ? 0x2d01 : 0x1101)});
                Scenarious.AddReadUnit(Constants.BridgeReadAddress, 1, transmitTimeout: 1000, unitId: "ReadVerificationInfo");
            } else
                Scenarious.AddReadUnit((UInt16)(MainViewModel.SensorType == SensorType.IUG5_WIRELESS ? 0x2d : 0x11), 1, unitId: "ReadVerificationInfo");
        }

        protected abstract string GetReadProtocol();
        protected abstract string GetWriteProtocol();
        protected abstract void ValuesToView(string id, List<UInt16> data);
        protected abstract List<UInt16> ValuesFromView(string id);
        protected abstract string GetUniqueId();
        protected abstract int GetIndex();
        protected abstract bool IsSensorVerified(UInt16 verifiedValue);
        protected abstract string GetReadFilePath();
        protected abstract string GetWriteFilePath();

        public void ReadCoefficients() {
            int zeroX, scaleX, zeroY, scaleY;
            factory.GetCalibrationCoefficients(FilePathHelper.Calibration(MainViewModel.Serial), out zeroX, out scaleX, out zeroY, out scaleY);
            ZeroX = zeroX;
            ScaleX = scaleX;
            ZeroY = zeroY;
            ScaleY = scaleY;
        }
        public bool CanReadWrite() {
            return !isProgress;
        }
        public bool CanSave() {
            return !(FolderHelper.IsReadOnly || isProgress);
        }
        public bool CanReadCoefficients() {
            return EnableGSensorCoefficientsChanged && isCoefficientFileExist && MainViewModel != null;
        }
        void OnMainViewModelChanged(MainViewModel newValue) {
            if(newValue == null)
                return;
            newValue.PropertyChanged += MainViewModel_PropertyChanged;
            UpdateEnableGSensorCoefficientsChanged();
        }
        void Start() {
            MainViewModel.TransmitUnit = Scenarious.GetNextUnit().CreateTransmitUnit();
        }
        void MainViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if(e.PropertyName == "SensorType") {
                UpdateEnableGSensorCoefficientsChanged();
                UpdateIsCoefficientFileExist();
            }
            if(((e.PropertyName == "SelectedPageIndex") && MainViewModel.SelectedPageIndex == GetIndex())
                || e.PropertyName == "Serial") {
                UpdateIsCoefficientFileExist();
            }
            if(e.PropertyName == "ReceiveUnit" && MainViewModel.SourceId == GetUniqueId())
                ReceiveUnitProgress();
        }
        void ReceiveUnitProgress() {
            if(MainViewModel.IsTerminateOccured()) {
                isProgress = false;
                createWriteReport = false;
                CommandManager.InvalidateRequerySuggested();
                return;
            }
            var unit = MainViewModel.ReceiveUnit;
            var id = Scenarious.GetCurrentUnit().Id;
            if(id == "ReadVerificationInfo" && !IsSensorVerified(((ModbusReadUnit)Scenarious.GetCurrentUnit()).GetReceivedData(unit.Data).First())) {
                MainViewModel.IsCancelSerialPortOperation = true;
                isProgress = false;
                createWriteReport = false;
                CommandManager.InvalidateRequerySuggested();
                MessageBox.Show("Тип датчика не соответствует выбранной вкладке. Операция прервана!", "Внимание!",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if(id == "Read_Serial")
                MainViewModel.Serial = ((ModbusReadUnit)Scenarious.GetCurrentUnit()).GetReceivedData(unit.Data).First();
            if(id == "Read_Step1" || id == "Read_Step2" || id == "Read_Step3" || id == "Read_Step4")
                ValuesToView(id, ((ModbusReadUnit)Scenarious.GetCurrentUnit()).GetReceivedData(unit.Data));
            if(!Scenarious.IsLastUnit())
                Start();
            else {
                isProgress = false;
                CommandManager.InvalidateRequerySuggested();
                if(createWriteReport) {
                    createWriteReport = false;
                    string writeProtocol = GetWriteProtocol();
                    using(StreamWriter sw = new StreamWriter(GetWriteFilePath())) {
                        sw.WriteLine(writeProtocol);
                    }
                    factory.ShowProtocol(GetWriteFilePath());
                }
            }
        }
        MessageBoxResult ShowWarningMessage(string message) {
            return MessageBox.Show(message, "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        }
        void UpdateEnableGSensorCoefficientsChanged() {
            EnableGSensorCoefficientsChanged = !(MainViewModel.SensorType == SensorType.IUG5_WIRELESS);
        }
        void UpdateIsCoefficientFileExist() {
            if(MainViewModel == null)
                return;
            var path = FilePathHelper.Calibration(MainViewModel.Serial);
            isCoefficientFileExist = File.Exists(path);
            FileWarningVisibility = ((MainViewModel.SensorType != SensorType.IUG5_WIRELESS) && !isCoefficientFileExist)
                ? Visibility.Visible : Visibility.Collapsed;
        }
        UInt16 GetBridgeRWStepPosition(int stepNumber) {
            UInt16 regsQuantity = 0x05;
            UInt16 offset = (UInt16)(0x05 * (stepNumber - 1));
            if(MainViewModel.SensorType == SensorType.IUG5_WIRELESS) {
                if(stepNumber == 4)
                    return (UInt16)(((Constants.IUG5_StartReadCoeffAddress) << 8) + 4);
                else
                    return (UInt16)(((Constants.IUG5_StartRWFlashAddress + offset) << 8) + regsQuantity);
            }
            return (UInt16)(((Constants.IUG1_2_StartRWFlashAddress + offset) << 8) + regsQuantity);
        }
    }
}
