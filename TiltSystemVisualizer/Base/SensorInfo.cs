using Mvvm.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace TiltSystemVisualizer.Base {
    public class SensorInfo : ObservableObject {

        public static double DefaultNoticeRange = 1.0;
        public static double DefaultUnsafeRange = 2.0;

        public SensorInfo(int id, int groupId, string name = "", string description = "", SensorState state = SensorState.Offline, string hexId = "") {
            this.noticeRange = DefaultNoticeRange;
            this.unsafeRange = DefaultUnsafeRange;

            this.id = id;
            this.groupId = groupId;
			this.HexId = hexId;

            this.Name = string.IsNullOrEmpty(name) ? "Дат. " + groupId : name;
            this.Description = string.IsNullOrEmpty(description) ? "Датчик " + groupId : description;
            this.state = state;

            this.currentTilt = new TiltInfo(0, 0, state, DateTime.Now, DateTime.Now);
            this.tilts = new ObservableCollection<TiltInfo>();
			localTilts = new ObservableCollection<TiltInfo>();
		}

        int id;
        int groupId;

        string name;
        string description;
        SensorState state;
        double noticeRange;
        double unsafeRange;

        TiltInfo currentTilt;
        ObservableCollection<TiltInfo> tilts;

        public int Id { get { return id; } }
        public int GroupId { get { return id; } }
		public string HexId { get; set; }

        public string Name { get { return name; } set { SetPropertyValue("Name", ref name, value); } }
        public string Description { get { return description; } set { SetPropertyValue("Description", ref description, value); } }
        public SensorState State { get { return state; } private set { SetPropertyValue("State", ref state, value); } }
        public double NoticeRange { get { return noticeRange; } set { SetPropertyValue("NoticeRange", ref noticeRange, value, newValue => UpdateState(CurrentTilt.Gravity)); } }
        public double UnsafeRange { get { return unsafeRange; } set { SetPropertyValue("UnsafeRange", ref unsafeRange, value, newValue => UpdateState(CurrentTilt.Gravity)); } }

        public TiltInfo CurrentTilt { get { return currentTilt; } private set { SetPropertyValue("CurrentTilt", ref currentTilt, value, newValue => OnCurrentTiltChanged(newValue)); } }
        public ObservableCollection<TiltInfo> Tilts { get { return tilts; } set { SetPropertyValue("Tilts", ref tilts, value); } }

        public void SetCurrentTilt(double direction, double gravity, DateTime createdTime, int temperature) {
            UpdateState(gravity);
            CurrentTilt = new TiltInfo(direction, gravity, State, DateTime.Now, createdTime, temperature);
        }

        public void TurnOnSensor() {
            Tilts.Clear();
            State = SensorState.NoSignal;
            CurrentTilt = new TiltInfo(0, 0, State, DateTime.Now, DateTime.Now);
        }
        public void TurnOffSensor() {
            Tilts.Clear();
            State = SensorState.Offline;
            CurrentTilt = new TiltInfo(0, 0, State, DateTime.Now, DateTime.Now);
        }
        public void ClearHistory() {
            Tilts.Clear();
        }
		public bool IsIdMatches(byte[] source) {
			int sourceCount = 0;
			for(int i = 2; i < 17; i+=2) {
				var digitString = HexId.Substring(i, 2);
				var digit = int.Parse(digitString, NumberStyles.HexNumber);
				if(digit != source[sourceCount])
					return false;
				sourceCount++;
			}
			return true;
		}

		void OnCurrentTiltChanged(TiltInfo tilt) {
            UpdateTiltsCollection(tilt);
            UpdateState(tilt.Gravity);
        }
        void UpdateState(double gravity) {
            if(State == SensorState.Offline)
                return;
            var actualGravity = Math.Abs(gravity);
                if(actualGravity < NoticeRange)
                    State = SensorState.Normal;
                else if(actualGravity >= NoticeRange && actualGravity < UnsafeRange)
                    State = SensorState.Notice;
                else
                    State = SensorState.Unsafe;
        }

		ObservableCollection<TiltInfo> localTilts;


        void UpdateTiltsCollection(TiltInfo tilt) {
            if(State == SensorState.Offline)
                return;
			localTilts.Clear();
			foreach(var localTilt in Tilts) {
				localTilts.Add(localTilt);
			}
			localTilts.Add(tilt);

			this.Tilts = null;
			this.Tilts = new ObservableCollection<TiltInfo>(localTilts);
        }
    }
    public enum SensorState {
        Offline,
        NoSignal,
        Normal,
        Notice,
        Unsafe,
    }
}
