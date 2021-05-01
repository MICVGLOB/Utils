using Controls.Core;
using Modbus.Core;
using Mvvm.Core;
using SensorsManager.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Mvvm.ViewModels {
    public class AngleCalibrationViewModel : ObservableObject {
        public AngleCalibrationViewModel() {
            CalculateCommand = new DelegateCommand(Calculate, () => CanCalculate());
            CalculateAndCreateDocumentCommand = new DelegateCommand(CalculateAndCreateDocument, () => CanCalculateAndCreateDocument());
        }

        public ICommand CalculateCommand { get; private set; }
        public ICommand CalculateAndCreateDocumentCommand { get; private set; }

        bool isContinuesReading;
        int currentX;
        int minX;
        int maxX;
        int currentY;
        int minY;
        int maxY;
        int zeroX;
        int scaleX;
        int zeroY;
        int scaleY;
        MainViewModel mainViewModel;

        public bool IsContinuesReading { get { return isContinuesReading; } set { SetPropertyValue("IsContinuesReading", ref isContinuesReading, value, x => OnIsContinuesReadingChanged(x)); } }
        public int CurrentX { get { return currentX; } set { SetPropertyValue("CurrentX", ref currentX, value); } }
        public int MinX { get { return minX; } set { SetPropertyValue("MinX", ref minX, value); } }
        public int MaxX { get { return maxX; } set { SetPropertyValue("MaxX", ref maxX, value); } }

        public int CurrentY { get { return currentY; } set { SetPropertyValue("CurrentY", ref currentY, value); } }
        public int MinY { get { return minY; } set { SetPropertyValue("MinY", ref minY, value); } }
        public int MaxY { get { return maxY; } set { SetPropertyValue("MaxY", ref maxY, value); } }

        public int ZeroX { get { return zeroX; } set { SetPropertyValue("ZeroX", ref zeroX, value); } }
        public int ScaleX { get { return scaleX; } set { SetPropertyValue("ScaleX", ref scaleX, value); } }
        public int ZeroY { get { return zeroY; } set { SetPropertyValue("ZeroY", ref zeroY, value); } }
        public int ScaleY { get { return scaleY; } set { SetPropertyValue("ScaleY", ref scaleY, value); } }
        public MainViewModel MainViewModel { get { return mainViewModel; } set { SetPropertyValue("MainViewModel", ref mainViewModel, value, x => OnMainViewModelChanged(x)); } }

        void Calculate() {
            double minX = -(65536.0 - MinX);
            double minY = -(65536.0 - MinY);
            if(MainViewModel.SensorType == SensorType.IUG1_3_STANDART) {
                double a1 = 10000.0 * (2.43 - 2.0) / 0.8;
                double a2 = 10000.0 * 2.43 / 0.8 / 32768.0;
                ZeroX = (int)Math.Round(a1 * (minX - MaxX) / (minX + MaxX));
                ScaleX = (int)Math.Round(a2 * (MaxX - minX) / 2);
                ZeroY = (int)Math.Round(a1 * (minY - MaxY) / (minY + MaxY));
                ScaleY = (int)Math.Round(a2 * (MaxY - minY) / 2);
            }
            if(MainViewModel.SensorType == SensorType.IUG3_RATIOMETRIC) {
                double b = 3.814697 / 100000.0;
                ZeroX = (int)Math.Round(20000.0 * MaxX / (MaxX - minX));
                ScaleX = (int)Math.Round(20000.0 / (b * (MaxX - minX)));
                ZeroY = (int)Math.Round(20000.0 * MaxY / (MaxY - minY));
                ScaleY = (int)Math.Round(20000.0 / (b * (MaxY - minY)));
            }
            if(MainViewModel.SensorType == SensorType.IUG5_WIRELESS) {
                double b = 1 / (2 * 0.2 * 32768.0);
                ZeroX = (int)Math.Round(20000.0 * MaxX / (MaxX - minX));
                ScaleX = (int)Math.Round(20000.0 / (b * (MaxX - minX)));
                ZeroY = (int)Math.Round(20000.0 * MaxY / (MaxY - minY));
                ScaleY = (int)Math.Round(20000.0 / (b * (MaxY - minY)));
            }
        }
        void CalculateAndCreateDocument() {
            string path = FilePathHelper.Calibration(MainViewModel.Serial);
            if(File.Exists(path)) {
                var result = System.Windows.MessageBox.Show("Файл калибровки уже существует и будет перезаписан. Продолжить?", "Внимание!",
                 System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Warning);
                if(result == System.Windows.MessageBoxResult.No || result == System.Windows.MessageBoxResult.Cancel)
                    return;
            }
            Calculate();
            string calibrationProtocol = MainViewModel.ReportFactory.CreateCalibrationProtocol(DateTime.Now, MainViewModel.OperatorName, MainViewModel.Serial, MainViewModel.Address, MainViewModel.SensorType,
                MinX, MaxX, MinY, MaxY, ZeroX, ScaleX, ZeroY, ScaleY);
            using(StreamWriter sw = new StreamWriter(path)) {
                sw.WriteLine(calibrationProtocol);
            }
            MainViewModel.ReportFactory.ShowProtocol(path);
        }

        bool CanCalculate() {
            return MainViewModel != null && MinX != 0 && MaxX != 0 && MinY != 0 && MaxY != 0;
        }
        bool CanCalculateAndCreateDocument() {
            return CanCalculate() && !FolderHelper.IsReadOnly;
        }
        void OnIsContinuesReadingChanged(bool newValue) {
            if(newValue) {
                Scenarious.CreateScenario("ReadData", (byte)MainViewModel.Address);
                Scenarious.AddReadUnit(0x08, 2, 30, 1000, SerialPortErrorMode.ErrorOnTimeout, SerialPortProgressType.Repeated);
                MainViewModel.TransmitUnit = Scenarious.GetNextUnit().CreateTransmitUnit();
                MainViewModel.RepeatTimer.Start();
                MinX = 0;
                MaxX = 0;
                MinY = 0;
                MaxY = 0;
            } else
                MainViewModel.IsCancelSerialPortOperation = true;
            CommandManager.InvalidateRequerySuggested();
        }

        void RepeatTimer_Tick(object sender, EventArgs e) {
            MainViewModel.TransmitUnit = Scenarious.GetCurrentUnit().CreateTransmitUnit();
        }
        void MainViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if(e.PropertyName == "SelectedPageIndex")
                IsContinuesReading = false;
            if(e.PropertyName == "ReceiveUnit" && MainViewModel.SourceId == "Calibration")
                ReceiveUnitProgress();
        }
        void ReceiveUnitProgress() {
            if(MainViewModel.IsTerminateOccured()) {
                IsContinuesReading = false;
                return;
            }
            var unit = MainViewModel.ReceiveUnit;
            switch(Scenarious.GetCurrentUnit().Id) {
                case "ReadData":
                    var regValues = ((ModbusReadUnit)Scenarious.GetCurrentUnit()).GetReceivedData(unit.Data);
                    CurrentX = regValues[0];
                    CurrentY = regValues[1];
                    MinX = SensorsManager.Utils.Converters.ValueToMinValue(regValues[0], MinX);
                    MaxX = SensorsManager.Utils.Converters.ValueToMaxValue(regValues[0], MaxX);
                    MinY = SensorsManager.Utils.Converters.ValueToMinValue(regValues[1], MinY);
                    MaxY = SensorsManager.Utils.Converters.ValueToMaxValue(regValues[1], MaxY);
                    break;
            }
        }
        void OnMainViewModelChanged(MainViewModel newValue) {
            if(newValue == null)
                return;
            newValue.PropertyChanged += MainViewModel_PropertyChanged;
            newValue.RepeatTimer.Tick += RepeatTimer_Tick;
        }
    }
}

