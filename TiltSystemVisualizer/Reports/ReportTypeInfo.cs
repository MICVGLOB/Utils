using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiltSystemVisualizer.Base;
using TiltSystemVisualizer.Utils;

namespace TiltSystemVisualizer.Reports {
    public abstract class ReportInfoBase {
        public ReportInfoBase(string id, string name, string template) {
            Id = id;
            Name = name;
            Template = template;
        }
        public string Id { get; private set; }
        public string Name { get; private set; }
        protected string Template { get; set; }
        public abstract string GetReport(string operatorName, SensorInfo sensor,
            DateTime? startReportInterval, DateTime? endReportInterval, IList<ShelfInfo> shelfs);

        protected string GetSensorFullName(SensorInfo sensor, IList<ShelfInfo> shelfs) {
            return string.Format("{0} ({1})", sensor.Name.ToString(), ShelfsHelper.GetShelfBySensorId(sensor.Id, shelfs).Name);
        }
        protected string GetIntervalString(DateTime? start, DateTime? end) {
            var result = "за все время";
            if(start.HasValue && !end.HasValue)
                result = string.Format("{0} - {1}", start.Value.ToString(dateTimeFormat), DateTime.Now.ToString(dateTimeFormat));
            else if(!start.HasValue && end.HasValue)
                result = string.Format("до {0}", end.Value.ToString(dateTimeFormat));
            else if(start.HasValue && end.HasValue)
                result = string.Format("{0} - {1}", start.Value.ToString(dateTimeFormat), end.Value.ToString(dateTimeFormat));
            return result;
        }
        protected IEnumerable<T> GetItemsInInterval<T>(IEnumerable<T> source, DateTime? start, DateTime? end, Func<T, DateTime> getTime) {
            if(!start.HasValue && !end.HasValue)
                return source;
            else if(start.HasValue && !end.HasValue)
                return source.Where(x => getTime(x) >= start);
            else if(!start.HasValue && end.HasValue)
                return source.Where(x => getTime(x) <= end);
            return source.Where(x => getTime(x) >= start && getTime(x) <= end);
        }
        protected string GetStateString(SensorState state) {
            var stateString = "";
            switch(state) {
                case SensorState.Normal:
                    stateString = "Норма";
                    break;
                case SensorState.NoSignal:
                    stateString = "Нет связи";
                    break;
                case SensorState.Notice:
                    stateString = "Предупреждение";
                    break;
                case SensorState.Offline:
                    stateString = "Отключен";
                    break;
                case SensorState.Unsafe:
                    stateString = "ОПАСНЫЙ УКЛОН";
                    break;
            }
            return stateString;
        }
		protected string GetFormattedTableData(StringBuilder sourceBuilder) {
			var tableData = sourceBuilder.ToString();
			if(!string.IsNullOrEmpty(tableData))
				return tableData;
			return "	<нет данных за выбранный интервал времени>";
		}
        protected string dateTimeFormat = "dd/MM/yyyy г."; //"dd/MM/yyyy H:mm"
    }
    public class UnsafeAllReportInfo : ReportInfoBase {
        public UnsafeAllReportInfo(string id, string name, string template) : base(id, name, template) { }

        public override string GetReport(string operatorName, SensorInfo sensor, DateTime? startReportInterval, DateTime? endReportInterval, IList<ShelfInfo> shelfs) {
            var sb = new StringBuilder();

            var selectedTilts = new List<Tuple<string, TiltInfo>>();
            ShelfsHelper.IterateSensors(shelfs, (shelf, s) => {
                foreach(var tilt in s.Tilts) {
                    if(tilt.State == SensorState.Unsafe)
                        selectedTilts.Add(new Tuple<string, TiltInfo>(GetSensorFullName(s, shelfs), tilt));
                }
            });
            var sortedTilts = GetItemsInInterval(selectedTilts, startReportInterval, endReportInterval, x=> x.Item2.ReceivedTime).OrderBy(x => x.Item2.ReceivedTime).ToList();

            sortedTilts.ForEach(tilt => sb.AppendLine(string.Format("	{0}		{1}	{2}°		{3:F1}°",
                tilt.Item2.ReceivedTime.ToString(dateTimeFormat), tilt.Item1, tilt.Item2.Gravity, tilt.Item2.Direction)));

            return string.Format(Template, DateTime.Now.ToString(dateTimeFormat), operatorName, GetIntervalString(startReportInterval, endReportInterval), GetFormattedTableData(sb));
        }
    }

    public class NoticeAndUnsafeAllReportInfo : ReportInfoBase {
        public NoticeAndUnsafeAllReportInfo(string id, string name, string template) : base(id, name, template) { }

        public override string GetReport(string operatorName, SensorInfo sensor, DateTime? startReportInterval, DateTime? endReportInterval, IList<ShelfInfo> shelfs) {
            var sb = new StringBuilder();

            var selectedTilts = new List<Tuple<string, TiltInfo>>();
            ShelfsHelper.IterateSensors(shelfs, (shelf, s) => {
                foreach(var tilt in s.Tilts) {
                    if(tilt.State == SensorState.Unsafe || tilt.State == SensorState.Notice)
                        selectedTilts.Add(new Tuple<string, TiltInfo>(GetSensorFullName(s, shelfs), tilt));
                }
            });
            var sortedTilts = GetItemsInInterval(selectedTilts, startReportInterval, endReportInterval, x => x.Item2.ReceivedTime).OrderBy(x => x.Item2.ReceivedTime).ToList();

            sortedTilts.ForEach(tilt => sb.AppendLine(string.Format("	{0}		{1}	{2}°		{3:F1}°			{4}",
                tilt.Item2.ReceivedTime.ToString(dateTimeFormat), tilt.Item1, tilt.Item2.Gravity, tilt.Item2.Direction, GetStateString(tilt.Item2.State))));

            return string.Format(Template, DateTime.Now.ToString(dateTimeFormat), operatorName, GetIntervalString(startReportInterval, endReportInterval), GetFormattedTableData(sb));
        }
    }
    public class UnsafeSensorReportInfo : ReportInfoBase {
        public UnsafeSensorReportInfo(string id, string name, string template) : base(id, name, template) { }

        public override string GetReport(string operatorName, SensorInfo sensor, DateTime? startReportInterval, DateTime? endReportInterval, IList<ShelfInfo> shelfs) {
            var sb = new StringBuilder();

            var shelf = ShelfsHelper.GetShelfBySensorId(sensor.Id, shelfs);
            var selectedTilts = new List<TiltInfo>();
            foreach(var tilt in sensor.Tilts) {
                if(tilt.State == SensorState.Unsafe)
                    selectedTilts.Add(tilt);
            }
            var sortedTilts = GetItemsInInterval(selectedTilts, startReportInterval, endReportInterval, x => x.ReceivedTime).ToList();

            sortedTilts.ForEach(tilt => sb.AppendLine(string.Format("	{0}			{1}°		{2:F1}°",
                tilt.ReceivedTime.ToString(dateTimeFormat), tilt.Gravity, tilt.Direction)));
            return string.Format(Template, DateTime.Now.ToString(dateTimeFormat), operatorName, GetSensorFullName(sensor, shelfs), 
                sensor.UnsafeRange, GetIntervalString(startReportInterval, endReportInterval), GetFormattedTableData(sb));
        }
    }
    public class NoticeAndUnsafeSensorReportInfo : ReportInfoBase {
        public NoticeAndUnsafeSensorReportInfo(string id, string name, string template) : base(id, name, template) { }

        public override string GetReport(string operatorName, SensorInfo sensor, DateTime? startReportInterval, DateTime? endReportInterval, IList<ShelfInfo> shelfs) {
            var sb = new StringBuilder();

            var shelf = ShelfsHelper.GetShelfBySensorId(sensor.Id, shelfs);
            var selectedTilts = new List<TiltInfo>();
            foreach(var tilt in sensor.Tilts) {
                if(tilt.State == SensorState.Unsafe || tilt.State == SensorState.Notice)
                    selectedTilts.Add(tilt);
            }
            var sortedTilts = GetItemsInInterval(selectedTilts, startReportInterval, endReportInterval, x => x.ReceivedTime).ToList();

            sortedTilts.ForEach(tilt => sb.AppendLine(string.Format("	{0}			{1}°		{2:F1}°			{3}",
                tilt.ReceivedTime.ToString(dateTimeFormat), tilt.Gravity, tilt.Direction, GetStateString(tilt.State))));
            return string.Format(Template, DateTime.Now.ToString(dateTimeFormat), operatorName, GetSensorFullName(sensor, shelfs),
                sensor.NoticeRange, sensor.UnsafeRange, GetIntervalString(startReportInterval, endReportInterval), sb.ToString());
        }
    }

    public class AllEventsSensorReportInfo : ReportInfoBase {
        public AllEventsSensorReportInfo(string id, string name, string template) : base(id, name, template) { }

        public override string GetReport(string operatorName, SensorInfo sensor, DateTime? startReportInterval, DateTime? endReportInterval, IList<ShelfInfo> shelfs) {
            var sb = new StringBuilder();

            var shelf = ShelfsHelper.GetShelfBySensorId(sensor.Id, shelfs);
            var sortedTilts = GetItemsInInterval(sensor.Tilts, startReportInterval, endReportInterval, x => x.ReceivedTime).ToList();

            sortedTilts.ForEach(tilt => sb.AppendLine(string.Format("	{0}			{1}°		{2:F1}°			{3}",
                tilt.ReceivedTime.ToString(dateTimeFormat), tilt.Gravity, tilt.Direction, GetStateString(tilt.State))));
            return string.Format(Template, DateTime.Now.ToString(dateTimeFormat), operatorName, GetSensorFullName(sensor, shelfs),
                sensor.NoticeRange, sensor.UnsafeRange, GetIntervalString(startReportInterval, endReportInterval), sb.ToString());
        }
    }

    public class NoSignalAllReportInfo : ReportInfoBase {
        public NoSignalAllReportInfo(string id, string name, string template) : base(id, name, template) { }

        public override string GetReport(string operatorName, SensorInfo sensor, DateTime? startReportInterval, DateTime? endReportInterval, IList<ShelfInfo> shelfs) {
			var sb = new StringBuilder();

			var selectedTilts = new List<Tuple<string, TiltInfo>>();
			ShelfsHelper.IterateSensors(shelfs, (shelf, s) => {
				foreach(var tilt in s.Tilts) {
					if(tilt.State == SensorState.NoSignal)
						selectedTilts.Add(new Tuple<string, TiltInfo>(GetSensorFullName(s, shelfs), tilt));
				}
			});
			var sortedTilts = GetItemsInInterval(selectedTilts, startReportInterval, endReportInterval, x => x.Item2.ReceivedTime).OrderBy(x => x.Item2.ReceivedTime).ToList();
			sortedTilts.ForEach(tilt => sb.AppendLine(string.Format("	{0}		{1}", tilt.Item2.ReceivedTime.ToString(dateTimeFormat), tilt.Item1)));

			return string.Format(Template, DateTime.Now.ToString(dateTimeFormat), operatorName, GetIntervalString(startReportInterval, endReportInterval), GetFormattedTableData(sb));
        }
    }

    public class NoSignalSensorReportInfo : ReportInfoBase {
        public NoSignalSensorReportInfo(string id, string name, string template) : base(id, name, template) { }

        public override string GetReport(string operatorName, SensorInfo sensor, DateTime? startReportInterval, DateTime? endReportInterval, IList<ShelfInfo> shelfs) {
			var sb = new StringBuilder();

			var shelf = ShelfsHelper.GetShelfBySensorId(sensor.Id, shelfs);
			var selectedTilts = new List<TiltInfo>();
			foreach(var tilt in sensor.Tilts) {
				if(tilt.State == SensorState.NoSignal)
					selectedTilts.Add(tilt);
			}
			var sortedTilts = GetItemsInInterval(selectedTilts, startReportInterval, endReportInterval, x => x.ReceivedTime).ToList();
			sortedTilts.ForEach(tilt => sb.AppendLine(string.Format("	{0}", tilt.ReceivedTime.ToString(dateTimeFormat))));

			return string.Format(Template, DateTime.Now.ToString(), operatorName,
                GetSensorFullName(sensor, shelfs), GetIntervalString(startReportInterval, endReportInterval), GetFormattedTableData(sb));
        }
    }

    public class StatesAllReportInfo : ReportInfoBase {
        public StatesAllReportInfo(string id, string name, string template) : base(id, name, template) { }

        public override string GetReport(string operatorName, SensorInfo sensor, DateTime? startReportInterval, DateTime? endReportInterval, IList<ShelfInfo> shelfs) {
            var sb = new StringBuilder();

			ShelfsHelper.IterateSensors(shelfs, (shelf, s) => {
                var stateString = string.Empty;
                switch(s.State) {
                    case SensorState.Offline:
                        stateString = "Отключен";
                        break;
                    case SensorState.NoSignal:
                        stateString = "Нет связи";
                        break;
                    default:
                        stateString = "Работает";
                        break;
                }
                sb.AppendLine(string.Format("	{0}		{1}", GetSensorFullName(s, shelfs), stateString));
            });
            return string.Format(Template, DateTime.Now.ToString(dateTimeFormat), operatorName, GetFormattedTableData(sb));
        }
    }
}
