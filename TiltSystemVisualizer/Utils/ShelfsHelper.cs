using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiltSystemVisualizer.Base;

namespace TiltSystemVisualizer.Utils {
    public static class ShelfsHelper {
        public static ShelfInfo GetShelfBySensorId(int sensorId, IList<ShelfInfo> shelfs) {
            foreach(var shelf in shelfs) {
                if(shelf.Sensors.Any(x => x.Id == sensorId))
                    return shelf;
            }
            return null;
        }
        public static IList<SensorInfo> GetSensors(IList<ShelfInfo> shelfs) {
            return shelfs.SelectMany(s => s.Sensors).ToList();
        }
        public static void IterateSensors(IList<ShelfInfo> shelfs, Action<ShelfInfo, SensorInfo> iterateAction) {
            shelfs.ToList().ForEach(shelf => {
                shelf.Sensors.ForEach(s => iterateAction(shelf, s));
            });
        }
    }
}
