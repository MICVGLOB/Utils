using Mvvm.Core;
using Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Diagnostics;
using TiltsVisualizerLight.Controls;
using Core;

namespace Mvvm.Core {
    public class SerialPortBehavior : Behavior<Window> {
        SerialPort serial;
        byte[] receivedBytes;

        DispatcherTimer receiveTimer;
        List<byte> receivedTerminalData;

        #region Public Properties
        public static readonly DependencyProperty ConfigProperty =
            DependencyProperty.Register("Config", typeof(SerialConfig), typeof(SerialPortBehavior), new PropertyMetadata(null, (d, e) => ((SerialPortBehavior)d).OnConfigChanged(e)));
        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register("Mode", typeof(SerialPortControlMode), typeof(SerialPortBehavior),
                new PropertyMetadata(SerialPortControlMode.ReadyToUse));
        public static readonly DependencyProperty TerminalModeProperty =
            DependencyProperty.Register("TerminalMode", typeof(bool), typeof(SerialPortBehavior), new PropertyMetadata(false, (d, e) => ((SerialPortBehavior)d).OnTerminalModeChanged()));
        public static readonly DependencyProperty TransmitTerminalDataProperty =
            DependencyProperty.Register("TransmitTerminalData", typeof(List<byte>), typeof(SerialPortBehavior), new PropertyMetadata(null, (d, e) => ((SerialPortBehavior)d).OnTransmitTerminalDataChanged()));
        public static readonly DependencyProperty ReceiveTerminalDataProperty =
            DependencyProperty.Register("ReceiveTerminalData", typeof(List<byte>), typeof(SerialPortBehavior), new PropertyMetadata(null));

        public static readonly DependencyProperty AllowPortProperty =
            DependencyProperty.Register("AllowPort", typeof(bool), typeof(SerialPortBehavior), new PropertyMetadata(false, (d, e) => ((SerialPortBehavior)d).OnAllowPortChanged()));

        public static readonly DependencyProperty TerminalMessageLengthProperty =
            DependencyProperty.Register("TerminalMessageLength", typeof(int), typeof(SerialPortBehavior), new PropertyMetadata(0));

        public SerialConfig Config { get { return (SerialConfig)GetValue(ConfigProperty); } set { SetValue(ConfigProperty, value); } }
        public bool TerminalMode { get { return (bool)GetValue(TerminalModeProperty); } set { SetValue(TerminalModeProperty, value); } }
        public List<byte> TransmitTerminalData { get { return (List<byte>)GetValue(TransmitTerminalDataProperty); } set { SetValue(TransmitTerminalDataProperty, value); } }
        public List<byte> ReceiveTerminalData { get { return (List<byte>)GetValue(ReceiveTerminalDataProperty); } set { SetValue(ReceiveTerminalDataProperty, value); } }

        public SerialPortControlMode Mode { get { return (SerialPortControlMode)GetValue(ModeProperty); } set { SetValue(ModeProperty, value); } }
        public bool AllowPort { get { return (bool)GetValue(AllowPortProperty); } set { SetValue(AllowPortProperty, value); } }

        public int TerminalMessageLength { get { return (int)GetValue(TerminalMessageLengthProperty); } set { SetValue(TerminalMessageLengthProperty, value); } }

        #endregion

        void OnTransmitTerminalDataChanged() {
            if(TransmitTerminalData == null)
                return;
            if(TerminalMode && serial != null && serial.IsOpen) {
                try {
                    serial.Write(TransmitTerminalData.ToArray(), 0, TransmitTerminalData.Count);
                } catch(Exception) {
                }
            }
        }
        void OnTerminalModeChanged() {
            ClearDataCollections();
            if(serial.IsOpen)
                CloseSerialPort();
            if(TerminalMode)
                UpdatePort();
        }
        void OnConfigChanged(DependencyPropertyChangedEventArgs e) {
            if(serial.IsOpen)
                CloseSerialPort();
            UpdatePort();
        }
        void OnAllowPortChanged() {
            ClearDataCollections();
            if(serial.IsOpen)
                CloseSerialPort();
            if(AllowPort)
                UpdatePort();
        }
        protected override void OnAttached() {
            base.OnAttached();
            if(serial.IsOpen)
                CloseSerialPort();
            UpdatePort();
        }
        void UpdatePort() {
            ClearDataCollections();
            if(Config == null || !TerminalMode || !AllowPort)
                return;
            serial.PortName = Config.Name;
            serial.BaudRate = TerminalMode ? Config.TerminalBaudrate : Config.ModbusBaudrate;
            serial.StopBits = Config.StopBits;
            serial.Parity = Config.Parity;
            TryOpenSerialPort();
        }

        void ClearDataCollections() {
            TransmitTerminalData = null;
            ReceiveTerminalData = null;
            receivedTerminalData.Clear();
        }

        public SerialPortBehavior() {
            receivedBytes = new byte[1000];
            receivedTerminalData = new List<byte>();

            serial = new SerialPort();
            serial.WriteBufferSize = 8192;
            serial.ReadBufferSize = 8192;
            serial.DataReceived += OnDataReceived;

            receiveTimer = new DispatcherTimer();
            receiveTimer.Tick += OnReceiveTimeout;
            receiveTimer.Interval = TimeSpan.FromMilliseconds(300);
            receiveTimer.IsEnabled = true;
        }

        void OnReceiveTimeout(object sender, EventArgs e) {
            receivedTerminalData.Clear();
        }

        void CloseSerialPort() {
            receiveTimer.Stop();
            TryCloseSerialPort();
        }
        void OnDataReceived(object sender, SerialDataReceivedEventArgs e) {
            var bytesCount = serial.BytesToRead;
            serial.Read(receivedBytes, 0, bytesCount);

            this.Dispatcher.BeginInvoke(new Action(delegate () {
                if(TerminalMode) {
                    for(int i = 0; i < bytesCount; i++) {
                        receivedTerminalData.Add(receivedBytes[i]);
                        receiveTimer.Start();
                    }
                    var count = receivedTerminalData.Count;
                    Debug.WriteLine(count);
                    if(IsMessageComplete()) {
                        ReceiveTerminalData = new List<byte>(receivedTerminalData);
                        receivedTerminalData.Clear();
                        receiveTimer.Stop();
                    }
                    return;
                }
            }));
        }

        bool IsMessageComplete() {
            return receivedTerminalData.Count == TerminalMessageLength;
        }

        bool TryOpenSerialPort() {
            if(serial.IsOpen)
                return true;
            try {
                serial.Open();
            } catch(Exception) {
                Mode = SerialPortControlMode.Error;
                return false;
            }
            return true;
        }

        void TryCloseSerialPort() {
            if(!serial.IsOpen)
                return;
            try {
                serial.Close();
            } catch(Exception) {
                Mode = SerialPortControlMode.Error;
                return;
            }
        }
    }
}
