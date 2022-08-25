using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mvvm.Core;
using TiltsVisualizerLight.Base;
using System.Windows.Threading;
using System.Windows.Input;
using System.ComponentModel;

namespace TiltsVisualizerLight.ViewModels {
    public class SummaryViewViewModel : ObservableObject {
        IEnumerable<ShelfInfo> shelfs;

        MainViewModel mainViewModel;
        ICommand appendTextCommand;
        ICommand clearTextCommand;

        public IEnumerable<ShelfInfo> Shelfs { get { return shelfs; } set { SetPropertyValue("Shelfs", ref shelfs, value); } }
        public MainViewModel MainViewModel { get { return mainViewModel; } set { SetPropertyValue("MainViewModel", ref mainViewModel, value, x => OnMainViewModelChanged(x)); } }

        public ICommand AppendTextCommand { get { return appendTextCommand; } set { SetPropertyValue("AppendTextCommand", ref appendTextCommand, value); } }
        public ICommand ClearTextCommand { get { return clearTextCommand; } set { SetPropertyValue("ClearTextCommand", ref clearTextCommand, value); } }

        public SummaryViewViewModel() {
        }
        void OnMainViewModelChanged(MainViewModel newValue) {
            if(newValue == null)
                return;
            this.Shelfs = newValue.Shelfs;
            foreach(var sensor in newValue.Sensors)
                sensor.PropertyChanged += OnSensorPropertyChanged;            
            newValue.PropertyChanged += OnMainViewModelPropertyChanged;
        }

        private void OnSensorPropertyChanged(object sender, PropertyChangedEventArgs e) {
            var info = sender as SensorInfo;
            if(e.PropertyName == "State") {
                AppendTextCommand.Execute(CreateLogMessage(info.Id, info.State));
            }
        }

        void OnMainViewModelPropertyChanged(object sender, PropertyChangedEventArgs e) {
        }

        string CreateLogMessage(int sensorId, SensorState newState) {
			if(MainViewModel == null)
				return string.Empty;
            string header = string.Format("{0} -> {1}:   ", MainViewModel.GetShelfNameById(sensorId), 
                MainViewModel.GetSensorById(sensorId).Description);
            if(newState == SensorState.Normal)
                return header + "Угол отклонения в норме.\n";
            else if(newState == SensorState.Notice)
                return header + "Внимание! Угол отклонения больше ожидаемого!\n";
            else if(newState == SensorState.Unsafe)
                return header + "ОПАСНО! УГОЛ ОТКЛОНЕНИЯ СЛИШКОМ ВЕЛИК!\n";
            else if(newState == SensorState.NoSignal)
                return header + "Ошибка! Потеряна связь с инклинометром!\n";
            return string.Empty;
        }
    }
}
