using Controls.Core;
using Modbus.Core;
using Mvvm.Core;
using StreamManager.Reports;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Mvvm.ViewModels {
    public class StreamViewModelBase : ObservableObject {
        protected ReportFactory ReportFactory;
        public StreamViewModelBase() {
            ReportFactory = new ReportFactory();
            ReadDataActions = CreateReadActions();
        }

        MainViewModel mainViewModel;

        protected virtual string SourceId { get { return string.Empty; } }
        protected virtual Action<string> WriteDataProcessing {
            get {
                return id => {
                    if(Scenarious.IsLastUnit() && id == "LastWriteUnit") {
                        MainViewModel.IsCancelSerialPortOperation = true;
                        RaiseCanExecuteCommand();
                    }
                };
            }
        }

        Dictionary<string, Action<List<UInt16>>> ReadDataActions;
        protected virtual Dictionary<string, Action <List<UInt16>>> CreateReadActions() {
            return new Dictionary<string, Action<List<UInt16>>>();
        }

        public MainViewModel MainViewModel { get { return mainViewModel; } set { SetPropertyValue("MainViewModel", ref mainViewModel, value, x => OnMainViewModelChanged(x)); } }

        protected virtual void ReceiveUnitProgress() {
            var id = Scenarious.GetCurrentUnit().Id;
            List<UInt16> data = GetReceivedData(MainViewModel.ReceiveUnit);
            if(data != null && ReadDataActions.ContainsKey(id))
                ReadDataActions[id].Invoke(data);
            if(!Scenarious.IsLastUnit())
                ExecuteNext();
            WriteDataProcessing.Invoke(id);
        }
        protected virtual void ReceiveTerminalDataChanged() {

        }

        protected MessageBoxResult ShowWarningMessage(string message, MessageBoxImage messageBoxImage = MessageBoxImage.Warning) {
            return MessageBox.Show(message, "Внимание!", MessageBoxButton.YesNo, messageBoxImage);
        }
        protected virtual void ExecuteNext() {
            MainViewModel.TransmitUnit = Scenarious.GetNextUnit().CreateTransmitUnit();
        }
        protected void RaiseCanExecuteCommand() {
            CommandManager.InvalidateRequerySuggested();
        }
        List<UInt16> GetReceivedData(ReceiveUnit unit) {
            var readUnit = Scenarious.GetCurrentUnit() as ModbusReadUnit;
            return readUnit == null ? null : readUnit.GetReceivedData(unit.Data);
        }
        void OnMainViewModelChanged(MainViewModel newValue) {
            if(newValue == null)
                return;
            newValue.PropertyChanged -= MainViewModel_PropertyChanged;
            newValue.PropertyChanged += MainViewModel_PropertyChanged;
            newValue.RepeatTimer.Tick -= OnRepeatTimerTick;
            newValue.RepeatTimer.Tick += OnRepeatTimerTick;
        }
        protected virtual void OnRepeatTimerTick(object sender, EventArgs e) {}

        void MainViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if(e.PropertyName == "ReceiveUnit" && MainViewModel.SourceId == SourceId)
                ReceiveUnitProgress();
            if(e.PropertyName == "ReceiveTerminalData" && MainViewModel.SourceId == SourceId)
                ReceiveTerminalDataChanged();
            if(e.PropertyName == "SelectedPageIndex")
                OnSelectedPageIndexChanged(MainViewModel.SelectedPageIndex);
        }
        protected virtual void OnSelectedPageIndexChanged(int newPageIndex) {
        }
        public void SetProperty(string propertyName, object value) {
            GetType().GetProperty(propertyName).SetValue(this, value, null);
        }
        public object GetProperty(string propertyName) {
            return GetType().GetProperty(propertyName).GetValue(this, null);
        }
    }
}
