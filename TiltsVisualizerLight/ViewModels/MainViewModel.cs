using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mvvm.Core;
using TiltsVisualizerLight.Base;
using System.Windows.Threading;
using System.Windows.Input;
using System.Collections.ObjectModel;
using TiltsVisualizerLight.Utils;
using Core;
using System.IO;
using TiltsVisualizerLight.Reports;

namespace TiltsVisualizerLight.ViewModels {
    public class DataInfo {
        public DataInfo(double key, double value) {
            Key = key;
            Value = value;
        }
        public double Key { get; set; }
        public double Value { get; set; }
    }

    public class MainViewModel : ObservableObject {

        public static double MaxPredefinedTilt = 3.0;
        public static double MinPredefinedTilt = -3.0;
        public static int SensorInfoMessageLength = 38 + 12;
        List<string> operatorNames = new List<string>() { "Ершов", "Казьмин", "Трофимов" };

        DispatcherTimer systemTimeUpdateTimer;
        DispatcherTimer repeatTimer;

        List<ShelfInfo> shelfs;
        bool isConnectOk;
        SerialConfig serialPortConfig;
        List<byte> receivedData;
        List<byte> transmittedData;
        bool allowPort;
        int terminalMessageLength;
        DateTime systemDateTime;
        int selectedPageIndex;

        public DispatcherTimer RepeatTimer { get { return repeatTimer; } }
        public List<ShelfInfo> Shelfs { get { return shelfs; } set { SetPropertyValue("Shelfs", ref shelfs, value); } }
        public bool IsConnectOk { get { return isConnectOk; } set { SetPropertyValue("IsConnectOk", ref isConnectOk, value); } }
        public SerialConfig SerialPortConfig { get { return serialPortConfig; } set { SetPropertyValue("SerialPortConfig", ref serialPortConfig, value); } }
        public List<byte> ReceivedData { get { return receivedData; } set { SetPropertyValue("ReceivedData", ref receivedData, value, x => OnReceivedDataChanged()); } }
        public List<byte> TransmittedData { get { return transmittedData; } set { SetPropertyValue("TransmittedData", ref transmittedData, value); } }
        public bool AllowTerminal { get { return allowPort; } set { SetPropertyValue("AllowPort", ref allowPort, value); } }
        public int TerminalMessageLength { get { return terminalMessageLength; } set { SetPropertyValue("TerminalMessageLength", ref terminalMessageLength, value); } }
        public DateTime SystemDateTime { get { return systemDateTime; } set { SetPropertyValue("SystemDateTime", ref systemDateTime, value); } }

        public int SelectedPageIndex { get { return selectedPageIndex; } set { SetPropertyValue("SelectedPageIndex", ref selectedPageIndex, value, x =>  OnSelectedPageIndexChanged()); } }

        public List<string> OperatorNames { get { return operatorNames; } }
        public IEnumerable<SensorInfo> Sensors;

        Random random;
        LineDecoder decoder;
        public ReportFactory reportFactory;

        public MainViewModel() {
            reportFactory = ReportFactory.Create();

            Shelfs = CreateShelfs();
            Sensors = Shelfs.SelectMany(x => x.Sensors).ToList();
            SerialPortConfig = FileHelper.GetSerialPortConfig();
            decoder = new LineDecoder();

            SelectedPageIndex = 1;

            TerminalMessageLength = SensorInfoMessageLength;// receive data

            random = new Random();

            systemTimeUpdateTimer = new DispatcherTimer();
            systemTimeUpdateTimer.Interval = TimeSpan.FromSeconds(0.5);
            systemTimeUpdateTimer.Tick += (s, e) => { SystemDateTime = DateTime.Now; };
            systemTimeUpdateTimer.Start();

            repeatTimer = new DispatcherTimer();
            repeatTimer.Interval = TimeSpan.FromSeconds(1);
        }
        public void RestartRepeatTimer() {
            RepeatTimer.Stop();
            RepeatTimer.Start();
        }
        public void StopRepeatTimer() {
            RepeatTimer.Stop();
        }
        void OnReceivedDataChanged() {
            IsConnectOk = true;
            if(TerminalMessageLength == SensorInfoMessageLength) {
                decoder.UpdateData(ReceivedData);
                if(!decoder.IsDataValid())
                    return;
                if(!TryUpdateSensorInfo())
                    return;
                EmulateStates();
            }
        }
        void OnSelectedPageIndexChanged() {
            AllowTerminal = SelectedPageIndex == 0;
        }

        void EmulateStates() {
            var idsForUpdate = new List<int>();
            Enumerable.Range(0, 6).ToList().ForEach(x => {
                var id = random.Next(3, 15);
                if(!idsForUpdate.Contains(id))
                    idsForUpdate.Add(id);
            });
            var stringBuilder = new StringBuilder();
            idsForUpdate.ForEach(id => {
                EmulateSensorInfo(id);
            });
        }

        bool TryUpdateSensorInfo() {
            var id = decoder.GetId();
            var sensor = GetSensorById(id);
            var gravityAngle = decoder.GetGravityAngle();
            if(sensor.Tilts.Count > 2) {
                var averageAngle = (sensor.Tilts.Last().Gravity + sensor.Tilts[sensor.Tilts.Count() - 2].Gravity) / 2.0;
                if(Math.Abs(gravityAngle - averageAngle) > 30)
                    return false;
            }
            sensor.SetCurrentTilt(decoder.GetDirectionAngle(), gravityAngle, decoder.GetCreatedTime(), decoder.GetTemperature());
            return true;
        }
        void EmulateSensorInfo(int id) {
            var sensor = GetSensorById(id);
            var randomGravityAngle = random.Next((int)(MinPredefinedTilt * 100), (int)(MaxPredefinedTilt * 100));
            var randomDirectionAngle = (double)random.Next(10, 288);
            sensor.SetCurrentTilt(randomDirectionAngle, randomGravityAngle / 100.0, DateTime.Now, 15);
        }
        List<ShelfInfo> CreateShelfs() {
            var result = new List<ShelfInfo>();
            result.Add(new ShelfInfo(1, new List<SensorInfo>() {
                new SensorInfo(id: 1, groupId: 1, state:SensorState.NoSignal),
                new SensorInfo(id: 2, groupId: 2),
                new SensorInfo(id: 3, groupId: 3),
            }));
            result.Add(new ShelfInfo(2, new List<SensorInfo>() {
                new SensorInfo(id: 4, groupId: 1),
                new SensorInfo(id: 5, groupId: 2),
                new SensorInfo(id: 6, groupId: 3, state:SensorState.NoSignal),
            }));
            result.Add(new ShelfInfo(3, new List<SensorInfo>() {
                new SensorInfo(id: 7, groupId: 1),
                new SensorInfo(id: 8, groupId: 2, state:SensorState.NoSignal),
                new SensorInfo(id: 9, groupId: 3),
            }));
            result.Add(new ShelfInfo(4, new List<SensorInfo>() {
                new SensorInfo(id: 10, groupId: 1),
                new SensorInfo(id: 11, groupId: 2),
                new SensorInfo(id: 12, groupId: 3),
            }));
            result.Add(new ShelfInfo(5, new List<SensorInfo>() {
                new SensorInfo(id: 13, groupId: 1),
                new SensorInfo(id: 14, groupId: 2),
                new SensorInfo(id: 15, groupId: 3, state:SensorState.NoSignal),
            }));
            return result;
        }

        public string GetShelfNameById(int sensorId) {
            foreach(var shelf in Shelfs) {
                if(shelf.Sensors.Any(x => x.Id == sensorId))
                    return shelf.Name;
            }
            return string.Empty;
        }
        public SensorInfo GetSensorById(int sensorId) {
            return Sensors.SingleOrDefault(x => x.Id == sensorId);
        }
        public void ClearAllHistory() {
            Sensors.ToList().ForEach(s => s.ClearHistory());
        }
    }
}
