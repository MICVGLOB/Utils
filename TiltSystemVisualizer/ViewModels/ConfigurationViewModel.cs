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
using TiltSystemVisualizer.Base;
using TiltSystemVisualizer.Reports;
using TiltSystemVisualizer.Utils;

namespace TiltSystemVisualizer.ViewModels {
	public class ConfigurationViewModel : ObservableObject {
		List<byte> SendMessageHeader;
		//									header	   length(hi+low)   frametype  id(no answ)  | 64-bit dest. address (coordinator)		  |
		//	uint8_t RuntimeDataMessageHeader[] = {0x7e,     0x00, 0x00,       0x10,     0x00,       0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
		//  16-bit address | radius | options
		//	0x00, 0x00,   0x00,    0x00     }; // 17 bytes

		public static int ConfigResponseMessageLength = 11;

		public ConfigurationViewModel() {
			displayTimeTimer = new DispatcherTimer();
			displayTimeTimer.Interval = TimeSpan.FromMilliseconds(1000);
			displayTimeTimer.Tick += OnDisplayTimeTimerTick;
			displayTimeTimer.Start();
			sessionTimer = new DispatcherTimer();
			sessionTimer.Interval = TimeSpan.FromMilliseconds(600);
			sessionTimer.Tick += OnSessionTimerTick;
			WriteDateTimeCommand = new DelegateCommand(WriteDateTime, () => !OperationInProgress);
			LoadParametersCommand = new DelegateCommand(LoadParameters, () => !OperationInProgress);
			LoadCalibrationCoefficientsCommand = new DelegateCommand(LoadCalibrationCoefficients, () => !OperationInProgress);
			CancelCommand = new DelegateCommand(CancelOperation, () => OperationInProgress);
			//									 header	 length(hi+low)   frametype  id(no answ)  | 64-bit dest. address (fixed)
			SendMessageHeader = new List<byte>() { 0x7e, 0x00,	0x00,		0x10,		0x01,		0x00, 0x13, 0xA2, 0x00, 0x41, 0x53, 0x1C, 0x3F,
			//  16-bit address | radius | options
				0xFF, 0xFE,		  0x00,    0x00 }; // 17 bytes
			SensorAddress = "0x0013A20041531C3F";
		}

		DispatcherTimer displayTimeTimer;
		DispatcherTimer sessionTimer;

		#region Properties
		MainViewModel mainViewModel;	
		DateTime currentDateTime;
		List<byte> configMessage;

		int measureInterval;
		int outputInterval;
		int modeValue;
		double noticeAngle;
		double unsafeAngle;

		int zeroX;
		int scaleX;
		int zeroY;
		int scaleY;
		int zeroZ;
		int scaleZ;

		bool operationInProgress;
		string sensorAddress;
		ICommand appendTextCommand;

		public MainViewModel MainViewModel { get { return mainViewModel; } set { SetPropertyValue("MainViewModel", ref mainViewModel, value, x => OnMainViewModelChanged()); } }
        public DateTime CurrentDateTime { get { return currentDateTime; } set { SetPropertyValue("CurrentDateTime", ref currentDateTime, value); } }

		public int MeasureInterval { get { return measureInterval; } set { SetPropertyValue("MeasureInterval", ref measureInterval, value); } }
		public int OutputInterval { get { return outputInterval; } set { SetPropertyValue("OutputInterval", ref outputInterval, value); } }
		public int ModeValue { get { return modeValue; } set { SetPropertyValue("ModeValue", ref modeValue, value); } }
		public double NoticeAngle { get { return noticeAngle; } set { SetPropertyValue("NoticeAngle", ref noticeAngle, value); } }
		public double UnsafeAngle { get { return unsafeAngle; } set { SetPropertyValue("UnsafeAngle", ref unsafeAngle, value); } }

		public int ZeroX { get { return zeroX; } set { SetPropertyValue("ZeroX", ref zeroX, value); } }
		public int ScaleX { get { return scaleX; } set { SetPropertyValue("ScaleX", ref scaleX, value); } }
		public int ZeroY { get { return zeroY; } set { SetPropertyValue("ZeroY", ref zeroY, value); } }
		public int ScaleY { get { return scaleY; } set { SetPropertyValue("ScaleY", ref scaleY, value); } }
		public int ZeroZ { get { return zeroZ; } set { SetPropertyValue("ZeroZ", ref zeroZ, value); } }
		public int ScaleZ { get { return scaleZ; } set { SetPropertyValue("ScaleZ", ref scaleZ, value); } }

		public bool OperationInProgress { get { return operationInProgress; } set { SetPropertyValue("OperationInProgress", ref operationInProgress, value); } }
		public ICommand AppendTextCommand { get { return appendTextCommand; } set { SetPropertyValue("AppendTextCommand", ref appendTextCommand, value); } }
		public string SensorAddress { get { return sensorAddress; } set { SetPropertyValue("SensorAddress", ref sensorAddress, value); } }

		#endregion

		public ICommand WriteDateTimeCommand { get; private set; }
		public ICommand LoadParametersCommand { get; private set; }
		public ICommand LoadCalibrationCoefficientsCommand { get; private set; }
		public ICommand CancelCommand { get; private set; }

		void OnMainViewModelChanged() {
			if(MainViewModel != null) {
				MainViewModel.PropertyChanged += OnMainViewModelPropertyChanged;
			}
		}
		void WriteDateTime() { // Content => 0x01(content Id) => Year, Month, Day, Hour, Minutes, Seconds
			configMessage = new List<byte>(GetMessageHeader());
			configMessage.Add(0x01);
			var dateTime = CurrentDateTime;
			var dateTimeContent = new byte[]{ (byte)(dateTime.Year - 2000), (byte)dateTime.Month, (byte)dateTime.Day,
				(byte)dateTime.Hour, (byte)dateTime.Minute, (byte)dateTime.Second};
			configMessage = configMessage.Concat(dateTimeContent).ToList();
			for(int i = 0; i < 6; i++)
				configMessage.Add(0);
			configMessage[0x02] = (byte)(configMessage.Count - 3);
			var checksum = CalculateChecksum(configMessage);
			configMessage.Add(checksum);
			InitSensorConfig(configMessage);
		}

		void LoadParameters() {
			configMessage = new List<byte>(GetMessageHeader());
			configMessage.Add(0x02);
			configMessage.Add((byte)MeasureInterval);
			configMessage.Add((byte)OutputInterval);
			configMessage.Add((byte)ModeValue);
			configMessage.Add((byte)(10.0 * NoticeAngle));
			configMessage.Add((byte)(10.0 * UnsafeAngle));
			for(int i = 0; i < 7; i++)
				configMessage.Add(0);
			configMessage[0x02] = (byte)(configMessage.Count - 3);
			var checksum = CalculateChecksum(configMessage);
			configMessage.Add(checksum);
			InitSensorConfig(configMessage);
		}
		void LoadCalibrationCoefficients() {
			//var message = new List<byte>(GetMessageHeader());
			//InitSensorConfig(message);
		}
		void CancelOperation() {
			MainViewModel.MessageLength = MainViewModel.SensorInfoMessageLength;
			OperationInProgress = false;
			AddText("Конфигурирование датчика отменено.\n");
		}
		void InitSensorConfig(List<byte> message) {
			OperationInProgress = true;
			AddText("Конфигурационное сообщение создано. Ждите (до 15 минут) сеанса связи с датчиком...\n");
		}

		void OnMainViewModelPropertyChanged(object sender, PropertyChangedEventArgs e) {
			if(e.PropertyName == "ReceivedData" && OperationInProgress) {
				if(MainViewModel.MessageLength == MainViewModel.SensorInfoMessageLength) { 
					sessionTimer.Stop();
					sessionTimer.Start();
				}
				if(MainViewModel.MessageLength == ConfigResponseMessageLength) {
					AddText(IsResponseValid() ? "Конфигурационное сообщение записано успешно!\n"
						: string.Format("Ошибка записи конфигурации! Статус доставки сообщения 0х{0}\n", MainViewModel.ReceivedData[8].ToString("X2")));
					RestoreSensorsListener();
				}
			}
		}
		void OnSessionTimerTick(object sender, EventArgs e) {
			if(MainViewModel.MessageLength == MainViewModel.SensorInfoMessageLength) {
				if(configMessage[GetMessageHeader().Count] == 0x01)
					UpdateDateTimeInMessage();
				MainViewModel.MessageLength = ConfigResponseMessageLength;
				MainViewModel.TransmittedData = configMessage;
				return;
			}
			if(MainViewModel.MessageLength == ConfigResponseMessageLength) {
				AddText("ОШИБКА! Ответ на конфигурационное сообщение не получен!\n");
				RestoreSensorsListener();
				return;
			}
		}
		void UpdateDateTimeInMessage() {
			var dateTime = CurrentDateTime;
			var index = GetMessageHeader().Count + 1;
			configMessage[index++] = (byte)(dateTime.Year - 2000);
			configMessage[index++] = (byte)dateTime.Month;
			configMessage[index++] = (byte)dateTime.Day;
			configMessage[index++] = (byte)dateTime.Hour;
			configMessage[index++] = (byte)dateTime.Minute;
			configMessage[index] = (byte)dateTime.Second;
			configMessage.RemoveAt(configMessage.Count - 1); // update checksum
			var checksum = CalculateChecksum(configMessage);
			configMessage.Add(checksum);
		}
		bool IsResponseValid() {
			return MainViewModel.ReceivedData[8] == 0;
		}
		void RestoreSensorsListener() {
			sessionTimer.Stop();
			OperationInProgress = false;
			CommandManager.InvalidateRequerySuggested();
			Dispatcher.CurrentDispatcher.BeginInvoke((Action)(() => {
				MainViewModel.MessageLength = MainViewModel.SensorInfoMessageLength;
			}));

		}
		void OnDisplayTimeTimerTick(object sender, EventArgs e) {
			CurrentDateTime = DateTime.Now;
		}
		List<byte> GetMessageHeader() {
			return new List<byte>(SendMessageHeader);
		}
		byte CalculateChecksum(List<byte> source) {
			UInt16 sum = 0;
			for(int i = 3; i < source.Count(); i++) {
				sum += source[i];
			}
			var checksum = (byte)sum;
			return (byte)(0xff - checksum);
		}

		void AddText(string text) {
			if(AppendTextCommand != null)
				AppendTextCommand.Execute(text);
		}
	}
}

// Test Sensor

//SH serialNumber High 0013A200h
//SL serialNumber LOW 41531C3Fh

// MY 16-bit network address FFFEh
// MP 16-bit parent address FFFEh

// DH destination address HI 0h
// DL destination address LOW 1945h