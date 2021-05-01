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

namespace Controls.Core {
    public class SerialPortControl : Control {
        ComboBox comSelector;
        TextBlock statusText;
        ProgressBar progressBar;
        SerialPort serial;
        DispatcherTimer receiveTimer;
        DispatcherTimer transmitTimer;
        byte[] receivedBytes;

        List<byte> receivedData;

        public const int DefaultBaudrate = 9600;

        #region Internal Properties
        public static readonly DependencyProperty PortsProperty =
            DependencyProperty.Register("Ports", typeof(List<string>), typeof(SerialPortControl), new PropertyMetadata(null));
        public static readonly DependencyProperty SelectedPortProperty =
            DependencyProperty.Register("SelectedPort", typeof(string), typeof(SerialPortControl), new PropertyMetadata(null));

        public List<string> Ports { get { return (List<string>)GetValue(PortsProperty); } set { SetValue(PortsProperty, value); } }
        public string SelectedPort { get { return (string)GetValue(SelectedPortProperty); } set { SetValue(SelectedPortProperty, value); } }
        #endregion

        #region Public Properties
        public static readonly DependencyProperty HasPortsProperty =
            DependencyProperty.Register("HasPorts", typeof(bool), typeof(SerialPortControl), new PropertyMetadata(false));
        public static readonly DependencyProperty ProgressValueProperty =
            DependencyProperty.Register("ProgressValue", typeof(int), typeof(SerialPortControl),
                new PropertyMetadata(0, (d, e) => ((SerialPortControl)d).OnProgressValueChanged(d, e)));
        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register("Mode", typeof(SerialPortControlMode), typeof(SerialPortControl),
                new PropertyMetadata(SerialPortControlMode.ReadyToUse, (d, e) => ((SerialPortControl)d).OnModeChanged(d, e)));
        public static readonly DependencyProperty TransmitUnitProperty =
            DependencyProperty.Register("TransmitUnit", typeof(TransmitUnit), typeof(SerialPortControl),
                new PropertyMetadata(null, (d, e) => ((SerialPortControl)d).OnTransmitUnitChanged(d, e)));
        public static readonly DependencyProperty ReceiveUnitProperty =
            DependencyProperty.Register("ReceiveUnit", typeof(ReceiveUnit), typeof(SerialPortControl), new PropertyMetadata(null));
        public static readonly DependencyProperty IsCancelProperty =
            DependencyProperty.Register("IsCancel", typeof(bool), typeof(SerialPortControl), new PropertyMetadata(false, (d, e) => ((SerialPortControl)d).OnIsCancelChanged()));
        public static readonly DependencyProperty IsNormalStateCancelProperty =
            DependencyProperty.Register("IsNormalStateCancel", typeof(bool), typeof(SerialPortControl), new PropertyMetadata(false, (d, e) => ((SerialPortControl)d).OnIsNormalStateCancelChanged()));
        public static readonly DependencyProperty BaudrateProperty =
            DependencyProperty.Register("Baudrate", typeof(int), typeof(SerialPortControl), new PropertyMetadata(DefaultBaudrate, (d, e) => ((SerialPortControl)d).OnBaudrateChanged()));
        public static readonly DependencyProperty TerminalModeProperty =
            DependencyProperty.Register("TerminalMode", typeof(bool), typeof(SerialPortControl), new PropertyMetadata(false, (d, e) => ((SerialPortControl)d).OnTerminalModeChanged()));

        public static readonly DependencyProperty TransmitTerminalDataProperty =
            DependencyProperty.Register("TransmitTerminalData", typeof(List<char>), typeof(SerialPortControl), new PropertyMetadata(null, (d, e) => ((SerialPortControl)d).OnTransmitTerminalDataChanged()));
        public static readonly DependencyProperty ReceiveTerminalDataProperty =
            DependencyProperty.Register("ReceiveTerminalData", typeof(List<byte>), typeof(SerialPortControl), new PropertyMetadata(null));


        public bool TerminalMode { get { return (bool)GetValue(TerminalModeProperty); } set { SetValue(TerminalModeProperty, value); } }
        public List<byte> TransmitTerminalData { get { return (List<byte>)GetValue(TransmitTerminalDataProperty); } set { SetValue(TransmitTerminalDataProperty, value); } }
        public List<byte> ReceiveTerminalData { get { return (List<byte>)GetValue(ReceiveTerminalDataProperty); } set { SetValue(ReceiveTerminalDataProperty, value); } }

        List<byte> receivedTerminalData;


        void OnTerminalModeChanged() {
            TransmitTerminalData = null;
            ReceiveTerminalData = null;
            receivedTerminalData.Clear();
            if(TerminalMode) {
                if(!serial.IsOpen) {
                    serial.PortName = SelectedPort;
                    if(!TryOpenSerialPort())
                        return;
                    ReceiveTerminalData = new List<byte>();
                }
            } else if(serial.IsOpen)
                TryCloseSerialPort();
        }
        void OnTransmitTerminalDataChanged() {
            if(TransmitTerminalData == null)
                return;
            if(TerminalMode) {
                try {
                    serial.Write(TransmitTerminalData.ToArray(), 0, TransmitTerminalData.Count);
                } catch(Exception) {
                }
            }
        }

        public bool HasPorts { get { return (bool)GetValue(HasPortsProperty); } set { SetValue(HasPortsProperty, value); } }
        public int ProgressValue { get { return (int)GetValue(ProgressValueProperty); } set { SetValue(ProgressValueProperty, value); } }
        public SerialPortControlMode Mode { get { return (SerialPortControlMode)GetValue(ModeProperty); } set { SetValue(ModeProperty, value); } }
        public TransmitUnit TransmitUnit { get { return (TransmitUnit)GetValue(TransmitUnitProperty); } set { SetValue(TransmitUnitProperty, value); } }
        public ReceiveUnit ReceiveUnit { get { return (ReceiveUnit)GetValue(ReceiveUnitProperty); } set { SetValue(ReceiveUnitProperty, value); } }
        public bool IsCancel { get { return (bool)GetValue(IsCancelProperty); } set { SetValue(IsCancelProperty, value); } }
        public bool IsNormalStateCancel { get { return (bool)GetValue(IsNormalStateCancelProperty); } set { SetValue(IsNormalStateCancelProperty, value); } }

        public int Baudrate { get { return (int)GetValue(BaudrateProperty); } set { SetValue(BaudrateProperty, value); } }

        public ICommand CancelCommand { get; private set; }
        #endregion

        public SerialPortControl() {
            SerialPortService.StartMonitoring();
            serial = new SerialPort();
            serial.WriteBufferSize = 8192;
            serial.ReadBufferSize = 8192;
            receiveTimer = new DispatcherTimer();
            transmitTimer = new DispatcherTimer();
            ConfigSerialPort();
            serial.DataReceived += OnDataReceived;
            receiveTimer.Tick += OnReceiveTimeout;
            transmitTimer.Tick += OnTransmitTimeout;
            CancelCommand = new DelegateCommand(CancelDataExchange, CanCancelDataExchange);
            receivedBytes = new byte[1000];
            receivedTerminalData = new List<byte>();
        }
        public override void OnApplyTemplate() {
            base.OnApplyTemplate();
            comSelector = (ComboBox)this.GetTemplateChild("PART_ComSelector");
            statusText = (TextBlock)this.GetTemplateChild("PART_StatusText");
            progressBar = (ProgressBar)this.GetTemplateChild("PART_ProgressBar");
            ReloadPorts(null, null);
            SubscribeEvents();
        }
        void OnIsCancelChanged() {
            if(IsCancel && CancelCommand.CanExecute(null))
                CancelCommand.Execute(null);
        }
        void OnIsNormalStateCancelChanged() {
            if(!IsNormalStateCancel)
                return;
            if(CanCancelDataExchange()) {
                if(serial.IsOpen)
                    CloseSerialPort();
                Mode = SerialPortControlMode.Success;
                comSelector.IsEnabled = true;
            }
            if(IsNormalStateCancel)
                Dispatcher.BeginInvoke((Action)(() => { IsNormalStateCancel = false; }));
        }

        void CancelDataExchange() {
            if(serial.IsOpen)
                CloseSerialPort();
            Mode = SerialPortControlMode.TerminateByUser;
            ReceiveUnit = new ReceiveUnit(null, Mode);
            comSelector.IsEnabled = true;
        }
        bool CanCancelDataExchange() {
            return Mode == SerialPortControlMode.Progress || Mode == SerialPortControlMode.IdleProgress
                || Mode == SerialPortControlMode.UndeterminateProgress
                || Mode == SerialPortControlMode.RepeatedProgress;
        }
        public void CloseSerialPort() {
            transmitTimer.Stop();
            receiveTimer.Stop();
            TryCloseSerialPort();
        }

        protected virtual void ConfigSerialPort(int baudRate = DefaultBaudrate, StopBits stopBits = StopBits.Two, Parity parity = Parity.None) {
            if(serial == null)
                return;
            serial.BaudRate = baudRate;
            serial.StopBits = stopBits;
            serial.Parity = parity;
        }
        void OnBaudrateChanged() {
            if(serial != null)
                serial.BaudRate = Baudrate;
        }
        void OnTransmitUnitChanged(object d, DependencyPropertyChangedEventArgs e) {
            if(e.NewValue == null)
                return;
            var unit = (TransmitUnit)e.NewValue;
            if(Mode == SerialPortControlMode.NoPorts || Mode == SerialPortControlMode.NeedPortReconnect) {
                ReceiveUnit = new ReceiveUnit(null, Mode);
                return;
            }
            switch(unit.ProgressType) {
                case SerialPortProgressType.Undeterminate:
                    if(Mode != SerialPortControlMode.UndeterminateProgress)
                        Mode = SerialPortControlMode.UndeterminateProgress;
                    break;
                case SerialPortProgressType.Repeated:
                    if(Mode != SerialPortControlMode.RepeatedProgress)
                        Mode = SerialPortControlMode.RepeatedProgress;
                    break;
                default:
                    Mode = SerialPortControlMode.Progress;
                    break;
            }
            if(!serial.IsOpen) {
                serial.PortName = SelectedPort;
                if(!TryOpenSerialPort())
                    return;
                comSelector.IsEnabled = false;
            }
            transmitTimer.Interval = TimeSpan.FromMilliseconds(unit.TransmitTimeout);
            transmitTimer.Start();
        }
        void OnTransmitTimeout(object sender, EventArgs e) {
            transmitTimer.Stop();
            receivedData = new List<byte>();
            bool exceptionOccured = false;
            try {
                serial.Write(TransmitUnit.Data.ToArray(), 0, TransmitUnit.Data.Count);
            } catch(Exception) {
                exceptionOccured = true;
            }
            if(!receiveTimer.IsEnabled && !exceptionOccured) {
                receiveTimer.Interval = TimeSpan.FromMilliseconds(TransmitUnit.ReceiveTimeout);
                receiveTimer.Start();
            }
        }
        void OnDataReceived(object sender, SerialDataReceivedEventArgs e) {
            var bytesCount = serial.BytesToRead;
            serial.Read(receivedBytes, 0, bytesCount);

            this.Dispatcher.BeginInvoke(new Action(delegate () {
                if(TerminalMode) {
                    for(int i = 0; i < bytesCount; i++) {
                        receivedTerminalData.Add(receivedBytes[i]);
                    }
                    var count = receivedTerminalData.Count;
                    Debug.WriteLine(count);
                    if(count > 3 && receivedTerminalData[count - 1] == 0xfa
                        && receivedTerminalData[count - 2] == 0xfa
                        && receivedTerminalData[count - 3] == 0xfa) {
                        ReceiveTerminalData = new List<byte>(receivedTerminalData);
                        receivedTerminalData.Clear();
                    }
                    return;
                }
                for(int i = 0; i < bytesCount; i++) {
                    receivedData.Add(receivedBytes[i]);
                }
                if(receivedData.Count == TransmitUnit.BytesToReceive) {
                    receiveTimer.Stop();
                    //System.Diagnostics.Debug.WriteLine(receivedData[4]);

                    if(!TransmitUnit.IsReceivedDataValid(receivedData)) {
                        TryCloseSerialPort();
                        ConfigErrorReceiveUnit(true);
                        return;
                    }
                    if(Mode == SerialPortControlMode.RepeatedProgress) {
                        ReceiveUnit = new ReceiveUnit(receivedData, SerialPortControlMode.RepeatedProgress);
                        return;
                    }
                    if(Mode != SerialPortControlMode.UndeterminateProgress) {
                        progressBar.Value = Math.Round(100 * (double)TransmitUnit.CurrentStep / (double)TransmitUnit.AllSteps);
                        ReceiveUnit = new ReceiveUnit(receivedData, SerialPortControlMode.Success);
                        if(progressBar.Value == 100.0) {
                            TryCloseSerialPort();
                            comSelector.IsEnabled = true;
                            Mode = SerialPortControlMode.Success;
                            return;
                        }
                    } else {
                        Mode = SerialPortControlMode.Progress;
                        TryCloseSerialPort();
                        comSelector.IsEnabled = true;
                        ReceiveUnit = new ReceiveUnit(receivedData, SerialPortControlMode.Success);
                    }
                }
            }));
        }
        void OnReceiveTimeout(object sender, EventArgs e) {
            receiveTimer.Stop();
            if(TransmitUnit.ErrorMode == SerialPortErrorMode.ErrorOnTimeout) {
                TryCloseSerialPort();
                ConfigErrorReceiveUnit(true);
                return;
            }
            ReceiveUnit = new ReceiveUnit(null, SerialPortControlMode.UndeterminateProgress, isTimeoutOccures: true);
        }
        void ConfigErrorReceiveUnit(bool isTimeoutOccures = false) {
            Mode = SerialPortControlMode.Error;
            ReceiveUnit = new ReceiveUnit(null, Mode, isTimeoutOccures: isTimeoutOccures);
            comSelector.IsEnabled = true;
        }
        void SubscribeEvents() {
            if(!DesignerProperties.GetIsInDesignMode(this)) {
                Window.GetWindow(this).Closing -= OnWindowClosing;
                Window.GetWindow(this).Closing += OnWindowClosing;
            }
            SerialPortService.PortsChanged -= ReloadPorts;
            SerialPortService.PortsChanged += ReloadPorts;
        }
        void OnWindowClosing(object sender, CancelEventArgs e) {
            SerialPortService.StopMonitoring();
        }
        void ReloadPorts(object sender, PortsChangedArgs e) {
            this.Dispatcher.BeginInvoke(new Action(delegate () {
                Ports = SerialPort.GetPortNames().OrderBy(x => x).ToList();
                if(!Ports.Any()) {
                    HasPorts = false;
                    Mode = SerialPortControlMode.NoPorts;
                    SelectedPort = string.Empty;
                } else {
                    HasPorts = true;
                    Mode = SerialPortControlMode.ReadyToUse;
                    OnModeChanged(null, new DependencyPropertyChangedEventArgs(ModeProperty, SerialPortControlMode.ReadyToUse, SerialPortControlMode.ReadyToUse));
                    var serialPortsIndices = Ports.Select(x => int.Parse(x.ToUpper().Replace("COM", "")));
                    SelectedPort = (serialPortsIndices.Last() < 5) ? Ports[0] : Ports.Last();
                }
            }));
        }

        void OnProgressValueChanged(object d, DependencyPropertyChangedEventArgs e) {
            switch(Mode) {
                case SerialPortControlMode.Progress:
                case SerialPortControlMode.IdleProgress:
                    var value = (int)(e.NewValue);
                    if(value >= 0 && value <= 100 && progressBar != null)
                        progressBar.Value = value;
                    break;
            }
        }
        void OnModeChanged(object d, DependencyPropertyChangedEventArgs e) {
            if(statusText == null || progressBar == null)
                return;
            switch((SerialPortControlMode)e.NewValue) {
                case SerialPortControlMode.ReadyToUse:
                    SetModeParameters("Готов к работе", Colors.Green, 0);
                    break;
                case SerialPortControlMode.NeedPortReconnect:
                    SetModeParameters("Ошибка инициализации", Colors.LightCoral, 0);
                    break;
                case SerialPortControlMode.NoPorts:
                    SetModeParameters("Порты не обнаружены", Colors.LightCoral, 0);
                    break;
                case SerialPortControlMode.UndeterminateProgress:
                    SetModeParameters("Операция выполняется", Colors.Green, 0);
                    progressBar.IsIndeterminate = true;
                    break;
                case SerialPortControlMode.Progress:
                    SetModeParameters("Операция выполняется", Colors.Green, 0);
                    break;
                case SerialPortControlMode.IdleProgress:
                    SetModeParameters("Операция выполняется", Colors.Orange, 0, Colors.Orange);
                    break;
                case SerialPortControlMode.Success:
                    SetModeParameters("Операция выполнена", Colors.Green, 100, Colors.Green);
                    break;
                case SerialPortControlMode.TerminateByUser:
                    SetModeParameters("Операция отменена", Colors.Orange, 0);
                    break;
                case SerialPortControlMode.Error:
                    SetModeParameters("Ошибка выполнения", Colors.LightCoral, 100, Colors.LightCoral);
                    break;
                case SerialPortControlMode.RepeatedProgress:
                    SetModeParameters("Операция выполняется", Colors.Green, 0);
                    progressBar.IsIndeterminate = true;
                    break;
            }
        }
        void SetModeParameters(string text, Color textColor, int? value = null, Color? progressColor = null) {
            statusText.Text = text;
            statusText.Foreground = new SolidColorBrush(textColor);
            if(value.HasValue)
                progressBar.Value = value.Value;
            if(progressColor.HasValue)
                progressBar.Foreground = new SolidColorBrush(progressColor.Value);
            else
                progressBar.Foreground = new SolidColorBrush(Colors.Green);
            progressBar.IsIndeterminate = false;
        }
        bool TryOpenSerialPort() {
            try {
                serial.Open();
            } catch(Exception) {
                Mode = SerialPortControlMode.Error;
                if(!TerminalMode)
                    ReceiveUnit = new ReceiveUnit(null, Mode);
                return false;
            }
            return true;
        }

        void TryCloseSerialPort() {
            try {
                serial.Close();
            } catch(Exception) {
                Mode = SerialPortControlMode.Error;
                if(!TerminalMode)
                    ReceiveUnit = new ReceiveUnit(null, Mode);
                return;
            }
        }
    }

    public class TransmitUnit {
        public TransmitUnit(List<byte> data, int bytesToReceive, Func<List<byte>, bool> isReceiveDataValid = null,
            int currentStep = 1, int allSteps = 1, int transmitTimeout = 30, int receiveTimeout = 1000,
            SerialPortErrorMode errorMode = SerialPortErrorMode.ErrorOnTimeout, SerialPortProgressType progressType = SerialPortProgressType.Normal) {
            Data = data;
            BytesToReceive = bytesToReceive;
            TransmitTimeout = transmitTimeout;
            ReceiveTimeout = receiveTimeout;
            if(isReceiveDataValid == null)
                IsReceivedDataValid = (d) => true;
            else
                IsReceivedDataValid = isReceiveDataValid;
            CurrentStep = currentStep;
            AllSteps = allSteps;
            ErrorMode = errorMode;
            ProgressType = progressType;
        }
        public List<byte> Data { get; private set; }
        public int BytesToReceive { get; private set; }
        public int CurrentStep { get; private set; }
        public int AllSteps { get; private set; }
        public Func<List<byte>, bool> IsReceivedDataValid { get; private set; }
        public int TransmitTimeout { get; private set; }
        public int ReceiveTimeout { get; private set; }
        public SerialPortErrorMode ErrorMode { get; private set; }
        public SerialPortProgressType ProgressType { get; private set; }
    }

    public class ReceiveUnit {
        public ReceiveUnit(List<byte> data, SerialPortControlMode result, bool isTimeoutOccures = false) {
            Data = data;
            Result = result;
            IsTimeoutOccures = isTimeoutOccures;
        }
        public List<byte> Data { get; private set; }
        public SerialPortControlMode Result { get; private set; }
        public bool IsTimeoutOccures { get; private set; }
    }

    public enum SerialPortErrorMode {
        ErrorOnTimeout,
        InfoOnTimeout
    }
    public enum SerialPortProgressType {
        Undeterminate,
        Normal,
        Idle,
        Repeated
    }

    public enum SerialPortControlMode {
        ReadyToUse,
        NeedPortReconnect,
        NoPorts,
        UndeterminateProgress,
        Progress,
        IdleProgress,
        Success,
        TerminateByUser,
        Error,
        RepeatedProgress
    }

}
