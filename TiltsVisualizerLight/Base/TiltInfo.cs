using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TiltsVisualizerLight.Base {
    public class TiltInfo {
        public TiltInfo(double direction, double gravity, SensorState state, DateTime receivedTime, DateTime createdTime, int temperature = -99) {
            ReceivedTime = receivedTime;
			CreatedTime = createdTime;
			Direction = direction;
            Gravity = gravity;
            State = state;
			Temperature = temperature;
			var signedTemperature = string.Format("{0}{1}°", temperature > 0 ? "+" : "", temperature.ToString());
			DisplayTemperature = string.Format("Температура датчика {0}", Temperature > -60 ? signedTemperature : "неизвестна");
		}
        public DateTime ReceivedTime { get; private set; }
		public DateTime CreatedTime { get; private set; }
		public double Direction { get; private set; }
        public double Gravity { get; private set; }
        public SensorState State { get; private set; }
		public int Temperature { get; private set; }
		public string DisplayTemperature { get; private set; }
    }
}
