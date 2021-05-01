using Controls.Core;
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
    public class CurrentOutputViewModel : StreamViewModelBase {
        protected override string SourceId { get { return "CurrentOutput"; } }

        public CurrentOutputViewModel() {
            ReadCommand = new DelegateCommand(Read, CanReadWrite);
            WriteCommand = new DelegateCommand(Write, CanReadWrite);
            CreateDocumentCommand = new DelegateCommand(ReadAndCreateDocument, CanReadAndCreateDocument);
            ReadCalibrationValueCommand = new DelegateCommand(ReadCalibrationValue, CanReadWrite);
            WriteCalibrationValueCommand = new DelegateCommand(WriteCalibrationValue, CanReadWrite);
        }

        #region Properties

        int pwm_4mA = 0;
        int pwm_20mA = 0;
        int current_pwm = 0;
        long scale = 0;
        bool isFirstStream = true;
        bool isSecondStream;
        bool isThirdStream;
        bool isForthStream;
        bool isFifthStream;
        bool isAllStream;
        int firmware;

        int currentChannel = 1;
        bool allowCreateProtocol;

        public int Pwm_4mA { get { return pwm_4mA; } set { SetPropertyValue("Pwm_4mA", ref pwm_4mA, value); } }
        public int Pwm_20mA { get { return pwm_20mA; } set { SetPropertyValue("Pwm_20mA", ref pwm_20mA, value); } }
        public int Current_pwm { get { return current_pwm; } set { SetPropertyValue("Current_pwm", ref current_pwm, value); } }
        public long Scale { get { return scale; } set { SetPropertyValue("Scale", ref scale, value); } }
        public bool IsFirstStream { get { return isFirstStream; } set { SetPropertyValue("IsFirstStream", ref isFirstStream, value, x => OnStreamIndexChanged(x, 1)); } }
        public bool IsSecondStream { get { return isSecondStream; } set { SetPropertyValue("IsSecondStream", ref isSecondStream, value, x => OnStreamIndexChanged(x, 2)); } }
        public bool IsThirdStream { get { return isThirdStream; } set { SetPropertyValue("IsThirdStream", ref isThirdStream, value, x => OnStreamIndexChanged(x, 3)); } }
        public bool IsForthStream { get { return isForthStream; } set { SetPropertyValue("IsForthStream", ref isForthStream, value, x => OnStreamIndexChanged(x, 4)); } }
        public bool IsFifthStream { get { return isFifthStream; } set { SetPropertyValue("IsFifthStream", ref isFifthStream, value, x => OnStreamIndexChanged(x, 5)); } }
        public bool IsAllStream { get { return isAllStream; } set { SetPropertyValue("IsAllStream", ref isAllStream, value, x => OnStreamIndexChanged(x, 6)); } }
        public int Firmware { get { return firmware; } set { SetPropertyValue("Firmware", ref firmware, value); } }

        #endregion

        public ICommand ReadCommand { get; private set; }
        public ICommand WriteCommand { get; private set; }
        public ICommand CreateDocumentCommand { get; private set; }
        public ICommand ReadCalibrationValueCommand { get; private set; }
        public ICommand WriteCalibrationValueCommand { get; private set; }

        public void Read() {
            Scenarious.CreateScenario("ReadСalibrationCoeffScenario", (byte)MainViewModel.Address);
            Scenarious.AddWriteUnit(0x00E9, new List<UInt16>() { 0x0FED });
            Scenarious.AddReadUnit(0x00DA, 5, unitId: "ReadСalibrationCoeff");
            Scenarious.AddReadUnit(0x00FB, 2, unitId: "ReadDeviceIds");
            ExecuteNext();
            allowCreateProtocol = false;
        }
        public void Write() {
            Scenarious.CreateScenario("WriteСalibrationCoeffScenario", (byte)MainViewModel.Address);
            Scenarious.AddWriteUnit(0x00E9, new List<UInt16>() { 0x0FED });
            Scenarious.AddWriteUnit(0x00DA, new List<UInt16>() { (UInt16)currentChannel, (UInt16)Pwm_4mA,
                (UInt16)Pwm_20mA, (UInt16)(((UInt32)Scale) >> 16), (UInt16)Scale },
                unitId: "WriteСalibrationCoeff");
            ExecuteNext();
        }
        public void ReadAndCreateDocument() {
            if(File.Exists(GetDocumentPath())) {
                var result = ShowWarningMessage("Файл с сохраненными данными уже существует и будет перезаписан. Продолжить?");
                if(result == MessageBoxResult.No || result == MessageBoxResult.Cancel)
                    return;
            }
            Read();
            allowCreateProtocol = true;
        }
        public void ReadCalibrationValue() {
            Scenarious.CreateScenario("ReadСalibrationValueScenario", (byte)MainViewModel.Address);
            Scenarious.AddWriteUnit(0x00E9, new List<UInt16>() { 0x0FED });
            Scenarious.AddReadUnit(0x007F, 1, unitId: "ReadСalibrationValue");
            ExecuteNext();
        }
        public void WriteCalibrationValue() {
            Scenarious.CreateScenario("WriteСalibrationValueScenario", (byte)MainViewModel.Address);
            Scenarious.AddWriteUnit(0x00E9, new List<UInt16>() { 0x0FED });
            Scenarious.AddWriteUnit(0x007F, new List<UInt16>() { (UInt16)Current_pwm }, unitId: "WriteСalibrationValue");
            ExecuteNext();
        }
        public bool CanReadWrite() {
            return true;
        }
        public bool CanReadAndCreateDocument() {
            return !FolderHelper.IsReadOnly;
        }
        void OnStreamIndexChanged(bool newValue, int index) {
            if(newValue)
                currentChannel = index;
        }

        protected override Dictionary<string, Action<List<UInt16>>> CreateReadActions() {
            var result = new Dictionary<string, Action<List<UInt16>>>();
            result.Add("ReadСalibrationValue", data => {
                Current_pwm = data[0];
            });
            result.Add("ReadСalibrationCoeff", data => {
                SetSelectedStream(data[0]);
                Pwm_4mA = data[1];
                Pwm_20mA = data[2];
                Scale = (((UInt32)data[3]) << 16) + data[4];
            });
            result.Add("ReadDeviceIds", data => {
                MainViewModel.Serial = data[0];
                Firmware = data[1];
                if(allowCreateProtocol) {
                    CreateDocument();
                    allowCreateProtocol = false;
                }
            });
            return result;
        }
        void CreateDocument() {
            var path = GetDocumentPath();
            using(StreamWriter sw = new StreamWriter(path)) {
                sw.WriteLine(ReportFactory.CreateCurrentConfigProtocol(DateTime.Now, MainViewModel.OperatorName, MainViewModel.Serial, MainViewModel.Address,
                    Firmware, currentChannel, Pwm_4mA, Pwm_20mA, Scale));
            }
            ReportFactory.ShowProtocol(path);
        }
        string GetDocumentPath() {
            return FilePathHelper.WriteCurrentConfig(MainViewModel.Serial);
        }
        void SetSelectedStream(UInt16 index) {
            switch(index) {
                case 1:
                    IsFirstStream = true;
                    break;
                case 2:
                    IsSecondStream = true;
                    break;
                case 3:
                    IsThirdStream = true;
                    break;
                case 4:
                    IsForthStream = true;
                    break;
                case 5:
                    IsFifthStream = true;
                    break;
                case 6:
                    IsAllStream = true;
                    break;
            }
        }
    }
}
