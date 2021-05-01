using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mvvm.Core;
using TiltSystemVisualizer.Base;
using System.Windows.Threading;
using System.Windows.Input;
using System.Collections.ObjectModel;
using TiltSystemVisualizer.Utils;
using Core;
using System.IO;
using TiltSystemVisualizer.Reports;

namespace TiltSystemVisualizer.ViewModels {
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
		public static int SensorInfoMessageLength = 39;
		List<string> operatorNames = new List<string>() { "Ершов", "Казьмин", "Трофимов" };

		List<ShelfInfo> shelfs;
		bool isConnectOk;
		SerialConfig serialPortConfig;
		List<byte> receivedData;
		List<byte> transmittedData;
		bool allowPort;
		int messageLength;

		public List<ShelfInfo> Shelfs { get { return shelfs; } set { SetPropertyValue("Shelfs", ref shelfs, value); } }
		public bool IsConnectOk { get { return isConnectOk; } set { SetPropertyValue("IsConnectOk", ref isConnectOk, value); } }
		public SerialConfig SerialPortConfig { get { return serialPortConfig; } set { SetPropertyValue("SerialPortConfig", ref serialPortConfig, value); } }
		public List<byte> ReceivedData { get { return receivedData; } set { SetPropertyValue("ReceivedData", ref receivedData, value, x => OnReceivedDataChanged()); } }
		public List<byte> TransmittedData { get { return transmittedData; } set { SetPropertyValue("TransmittedData", ref transmittedData, value); } }
		public bool AllowPort { get { return allowPort; } set { SetPropertyValue("AllowPort", ref allowPort, value); } }
		public int MessageLength { get { return messageLength; } set { SetPropertyValue("MessageLength", ref messageLength, value); } }


		public List<string> OperatorNames { get { return operatorNames; } }
        public IEnumerable<SensorInfo> Sensors;

        Random random;
		AnglesCalculator calculator;
		public ReportFactory reportFactory;
		string reportPath;

		public MainViewModel() {
			reportFactory = ReportFactory.Create();
			reportPath = FileHelper.GetInstantReportPath();
            Shelfs = CreateShelfs();
            Sensors = Shelfs.SelectMany(x => x.Sensors).ToList();
			SerialPortConfig = FileHelper.GetSerialPortConfig();
			calculator = new AnglesCalculator();
			AllowPort = true;

			MessageLength = SensorInfoMessageLength;// receive data

			random = new Random();			
        }

		void OnReceivedDataChanged() {
			IsConnectOk = true;
			if(MessageLength == SensorInfoMessageLength) {
				UpdateSensorInfo(id: 1, updateReport: true);
				EmulateStates();
			}
		}
        void EmulateStates() {
            var idsForUpdate = new List<int>();
            Enumerable.Range(0, 6).ToList().ForEach(x => {
                var id = random.Next(2, 15);
                if(!idsForUpdate.Contains(id))
                    idsForUpdate.Add(id);
            });
            var stringBuilder = new StringBuilder();
            idsForUpdate.ForEach(id => {
				EmulateSensorInfo(id);
            });
        }
        void UpdateSensorInfo(int id, bool updateReport) {
            var sensor = GetSensorById(id);
			calculator.Data = new List<byte>(ReceivedData.GetRange(17, 11));
			 sensor.SetCurrentTilt(calculator.CalculateDirectionAngle(), calculator.CalculateGravityAngle(), GetCreatedTime(), GetTemperature());
			if(updateReport && sensor.IsIdMatches(ReceivedData.GetRange(4, 8).ToArray())) {
				var tilt = sensor.CurrentTilt;
				var tiltInfoString = reportFactory.CreateInstantReportString(tilt, sensor.HexId);
				File.AppendAllText(reportPath, tiltInfoString + Environment.NewLine);
			}
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
                new SensorInfo(id: 1, groupId: 1, state:SensorState.NoSignal, hexId: "0x0013A20041531C3F"),
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
		DateTime GetCreatedTime() {
			var time = ReceivedData.GetRange(29, 6); //year - month - day - hour - minutes - seconds
			return new DateTime(2000 + (int)time[0], time[1], time[2], time[3], time[4], time[5]);
		}
		int GetTemperature() {
			var sourceTempCollection = ReceivedData.GetRange(15, 2);
			double sourceTemp = 256.0 * sourceTempCollection[0] + sourceTempCollection[1];
			var result = ((1885.0 - sourceTemp) / 9.05) + 25.0;
			return (int)Math.Round(result);
		}
	}
}
