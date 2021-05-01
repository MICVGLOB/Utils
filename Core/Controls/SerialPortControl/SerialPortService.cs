using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;
using System.Management;


namespace Controls.Core {
    public static class SerialPortService {
        static string[] _serialPorts;

        static ManagementEventWatcher arrival;
        static ManagementEventWatcher removal;

        public static void StartMonitoring() {
            _serialPorts = GetAvailableSerialPorts();
            MonitorDeviceChanges();
        }

        /// <summary>
        /// If this method isn't called, an InvalidComObjectException will be thrown (like below):
        /// System.Runtime.InteropServices.InvalidComObjectException was unhandled
        ///Message=COM object that has been separated from its underlying RCW cannot be used.
        ///Source=mscorlib
        ///StackTrace:
        ///     at System.StubHelpers.StubHelpers.StubRegisterRCW(Object pThis, IntPtr pThread)
        ///     at System.Management.IWbemServices.CancelAsyncCall_(IWbemObjectSink pSink)
        ///     at System.Management.SinkForEventQuery.Cancel()
        ///     at System.Management.ManagementEventWatcher.Stop()
        ///     at System.Management.ManagementEventWatcher.Finalize()
        ///InnerException: 
        /// </summary>
        public static void StopMonitoring() {
            arrival.Stop();
            removal.Stop();
        }

        public static event EventHandler<PortsChangedArgs> PortsChanged;

        private static void MonitorDeviceChanges() {
            try {
                var deviceArrivalQuery = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 2");
                var deviceRemovalQuery = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 3");

                arrival = new ManagementEventWatcher(deviceArrivalQuery);
                removal = new ManagementEventWatcher(deviceRemovalQuery);

                arrival.EventArrived += (o, args) => RaisePortsChangedIfNecessary(ChangeSerialEventType.Insertion);
                removal.EventArrived += (sender, eventArgs) => RaisePortsChangedIfNecessary(ChangeSerialEventType.Removal);

                arrival.Start();
                removal.Start();
            } catch(ManagementException) {

            }
        }

        private static void RaisePortsChangedIfNecessary(ChangeSerialEventType eventType) {
            lock(_serialPorts) {
                var availableSerialPorts = GetAvailableSerialPorts();
                if(!_serialPorts.SequenceEqual(availableSerialPorts)) {
                    _serialPorts = availableSerialPorts;
                    PortsChanged.Raise(null, new PortsChangedArgs(eventType, _serialPorts));
                }
            }
        }

        public static string[] GetAvailableSerialPorts() {
            return SerialPort.GetPortNames();
        }

        public static void Raise<T>(this EventHandler<T> handler, object sender, T args) where T : EventArgs {
            EventHandler<T> copy = handler;
            if(copy != null) {
                copy(sender, args);
            }
        }
    }

    public enum ChangeSerialEventType {
        Insertion,
        Removal,
    }

    public class PortsChangedArgs : EventArgs {
        readonly ChangeSerialEventType _eventType;
        readonly string[] _serialPorts;

        public PortsChangedArgs(ChangeSerialEventType eventType, string[] serialPorts) {
            _eventType = eventType;
            _serialPorts = serialPorts;
        }

        public string[] SerialPorts { get { return _serialPorts; } }
        public ChangeSerialEventType EventType { get { return _eventType; } }
    }
}
