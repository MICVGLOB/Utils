using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Mvvm.Core;

namespace TiltsVisualizerLight.Base {
    public class ShelfInfo : ObservableObject {

        int shelfId;
        List<SensorInfo> sensors;
        ObservableCollection<SensorInfo> activeSensors;
        string name;

        public int ShelfId { get { return shelfId; } }

        public List<SensorInfo> Sensors { get { return sensors; } }
        public ObservableCollection<SensorInfo> ActiveSensors { get { return activeSensors; } set { SetPropertyValue("ActiveSensors", ref activeSensors, value); } }

        public string Name { get { return name; } set { SetPropertyValue("Name", ref name, value); } }

        public ShelfInfo(int shelfId, List<SensorInfo> sensors, string name = "") {
            this.shelfId = shelfId;
            this.sensors = sensors;
            this.name = string.IsNullOrEmpty(name) ? "Стеллаж " + shelfId : name;
            this.activeSensors = new ObservableCollection<SensorInfo>();
            UpdateActiveSensors();
            foreach(var sensor in this.Sensors) 
                sensor.PropertyChanged += OnSensorPropertyChanged;           
        }
        void OnSensorPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if(e.PropertyName == "State") {
                var sensor = (SensorInfo)sender;
                if((sensor.State == SensorState.Offline && ActiveSensors.Contains(sensor))
                    || (sensor.State != SensorState.Offline && !ActiveSensors.Contains(sensor)))
                    UpdateActiveSensors();
            }
        }
        void UpdateActiveSensors() {
            activeSensors.Clear();
            foreach(var sensor in this.Sensors) { 
                if(sensor.State != SensorState.Offline)
                activeSensors.Add(sensor);
            }
        }
    }
}
