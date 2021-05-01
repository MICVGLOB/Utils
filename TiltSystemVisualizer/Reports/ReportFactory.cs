using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiltSystemVisualizer.Base;

namespace TiltSystemVisualizer.Reports {
    public partial class ReportFactory {

        static ReportFactory instance;
        public static ReportFactory Create() {
            if(instance == null)
                instance = new ReportFactory();
            return instance;
        }

        protected ReportFactory() { }

        List<ReportInfoBase> reportTypes;
        public List<ReportInfoBase> ReportTypes {
            get {
                if(reportTypes == null)
                    reportTypes = CreateReportTypes();
                return reportTypes;
            }
        }
        List<ReportInfoBase> CreateReportTypes() {
            var result = new List<ReportInfoBase>();
            result.Add(new UnsafeAllReportInfo("unsafeAll", "Все опасные уклоны", unsafeAllReportTemplate));
            result.Add(new NoticeAndUnsafeAllReportInfo("noticeAndUnsafeAll", "Все предупреждения и опасные уклоны", noticeAndUnsafeAllReportTemplate));
            result.Add(new UnsafeSensorReportInfo("unsafeSensor", "Опасные уклоны выбранного датчика", unsafeSensorReportTemplate));
            result.Add(new NoticeAndUnsafeSensorReportInfo("noticeAndUnsafeSensor", "Предупреждения и опасные уклоны выбранного датчика", noticeAndUnsafeSensorReportTemplate));
            result.Add(new AllEventsSensorReportInfo("allEventsSensor", "События выбранного датчика", allEventsSensorReportTemplate));
            result.Add(new NoSignalAllReportInfo("noSignalAll", "Все потери связи", noSignalAllReportTemplate));
            result.Add(new NoSignalSensorReportInfo("noSignalSensor", "Потери связи выбранного датчика", noSignalSensorReportTemplate));
            result.Add(new StatesAllReportInfo("statesAll", "Список зарегистрированных датчиков", statesAllReportTemplate));
            return result;
        }

		public string CreateInstantReportString(TiltInfo tilt, string hexId) {
			var dateTimeFormat = "HH:mm:ss";
			var signedTemperature = string.Format("{0}{1}", tilt.Temperature > 0 ? "+" : "", tilt.Temperature.ToString());
			return string.Format("Id={0}, Наклон={1:F2}°, Направление={2:F2}°,  t={3}°C, Время создания = {4}, Время получения данных = {5}", hexId,
				tilt.Gravity, tilt.Direction, signedTemperature, tilt.CreatedTime.ToString(dateTimeFormat), tilt.ReceivedTime.ToString(dateTimeFormat));
		}
    }
}
