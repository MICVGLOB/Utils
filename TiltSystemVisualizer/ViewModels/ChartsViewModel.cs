using Mvvm.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using TiltSystemVisualizer.Base;

namespace TiltSystemVisualizer.ViewModels {
    public class ChartsViewModel : ObservableObject {

        #region Properties
        MainViewModel mainViewModel;

        ShelfInfo selectedShelf1;
        ShelfInfo selectedShelf2;
        ShelfInfo selectedShelf3;

        SensorInfo selectedSensor1;
        SensorInfo selectedSensor2;
        SensorInfo selectedSensor3;

        public MainViewModel MainViewModel { get { return mainViewModel; } set { SetPropertyValue("MainViewModel", ref mainViewModel, value, x => OnMainViewModelChanged()); } }

        public ShelfInfo SelectedShelf1 { get { return selectedShelf1; } set { SetPropertyValue("SelectedShelf1", ref selectedShelf1, value, x => OnSelectedShelf1Changed()); } }
        public ShelfInfo SelectedShelf2 { get { return selectedShelf2; } set { SetPropertyValue("SelectedShelf2", ref selectedShelf2, value, x => OnSelectedShelf2Changed()); } }
        public ShelfInfo SelectedShelf3 { get { return selectedShelf3; } set { SetPropertyValue("SelectedShelf3", ref selectedShelf3, value, x => OnSelectedShelf3Changed()); } }

        public SensorInfo SelectedSensor1 { get { return selectedSensor1; } set { SetPropertyValue("SelectedSensor1", ref selectedSensor1, value); } }
        public SensorInfo SelectedSensor2 { get { return selectedSensor2; } set { SetPropertyValue("SelectedSensor2", ref selectedSensor2, value); } }
        public SensorInfo SelectedSensor3 { get { return selectedSensor3; } set { SetPropertyValue("SelectedSensor3", ref selectedSensor3, value); } }

        #endregion

        public ICommand ClearHistory1Command { get; private set; }
        public ICommand ClearHistory2Command { get; private set; }
        public ICommand ClearHistory3Command { get; private set; }
        public ICommand ClearAllHistoryCommand { get; private set; }

        public ChartsViewModel() {
            ClearHistory1Command = new DelegateCommand(ClearHistory1, () => SelectedSensor1 != null);
            ClearHistory2Command = new DelegateCommand(ClearHistory2, () => SelectedSensor2 != null);
            ClearHistory3Command = new DelegateCommand(ClearHistory3, () => SelectedSensor3 != null);
            ClearAllHistoryCommand = new DelegateCommand(ClearAllHistory, () => MainViewModel != null);
        }

        void OnMainViewModelChanged() {
            if(MainViewModel != null) {
                SelectedShelf1 = MainViewModel.Shelfs.ElementAt(0);
                SelectedShelf2 = MainViewModel.Shelfs.ElementAt(1);
                SelectedShelf3 = MainViewModel.Shelfs.ElementAt(2);
            }
        }

        void OnSelectedShelf1Changed() {
            if(SelectedShelf1 != null)
                SelectedSensor1 = SelectedShelf1.ActiveSensors.FirstOrDefault();
        }
        void OnSelectedShelf2Changed() {
            if(SelectedShelf2 != null)
                SelectedSensor2 = SelectedShelf2.ActiveSensors.FirstOrDefault();
        }
        void OnSelectedShelf3Changed() {
            if(SelectedShelf3 != null)
                SelectedSensor3 = SelectedShelf3.ActiveSensors.FirstOrDefault();
        }
        void ClearHistory1() {
            SelectedSensor1.ClearHistory();
        }
        void ClearHistory2() {
            SelectedSensor2.ClearHistory();
        }
        void ClearHistory3() {
            SelectedSensor3.ClearHistory();
        }
        void ClearAllHistory() {
            MainViewModel.ClearAllHistory();
        }
    }
}
