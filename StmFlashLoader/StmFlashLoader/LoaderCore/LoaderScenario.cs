using Controls.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StmFlashLoader.LoaderCore {
    public static class LoaderScenario {

        static string scenarioId;
        static List<LoaderCommandUnit> units;
        static int Serial;
        static int StagesCount;
        static int internalIndex;
        static int indexOfCurrentUnit;

        public static List<LoaderCommandUnit> Units { get { return units; } }

        public static void Create(string id, int serial) {
            units = new List<LoaderCommandUnit>();
            scenarioId = id;
            Serial = serial;
            StagesCount = 0;
            internalIndex = 1;
            indexOfCurrentUnit = -1;
        }
        public static void AddCommandUnit(CommandIds commandId, List<byte> data, int transmitTimeout = 30,
            int receiveTimeout = 1000, SerialPortErrorMode errorMode = SerialPortErrorMode.ErrorOnTimeout,
            SerialPortProgressType progressType = SerialPortProgressType.Normal, string unitId = null) {

            units.Add(new LoaderCommandUnit(commandId, Serial, data, StagesCount,
                internalIndex++, transmitTimeout, receiveTimeout, errorMode, progressType, unitId ?? scenarioId));
            UpdateStagesCount();
        }

        public static LoaderCommandUnit GetCurrentUnit() {
            return (indexOfCurrentUnit < Units.Count && indexOfCurrentUnit >= 0) 
                ? Units[indexOfCurrentUnit] : null;
        }
        public static CommandIds GetCommandId() {
            return GetCurrentUnit().GetCommandId();
        }
        public static LoaderCommandUnit GetNextUnit() {
            indexOfCurrentUnit++;
            return (indexOfCurrentUnit < Units.Count) ? Units[indexOfCurrentUnit] : null;
        }
        public static LoaderCommandUnit GetPreviousUnit() {
            indexOfCurrentUnit--;
            return (indexOfCurrentUnit >= 0) ? Units[indexOfCurrentUnit] : null;
        }
        public static bool IsLastUnit(LoaderCommandUnit unit = null) {
            return (unit == null) ? (indexOfCurrentUnit == Units.Count - 1)
                : (Units.IndexOf(unit) == Units.Count - 1);
        }

        static void UpdateStagesCount() {
            StagesCount++;
            units.ForEach(x => x.UpdateStagesCount(StagesCount));
        }
    }
}
