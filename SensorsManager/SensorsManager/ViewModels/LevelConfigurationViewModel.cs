using Modbus.Core;
using Mvvm.Core;
using SensorsManager.Reports;
using SensorsManager.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Mvvm.ViewModels {
    public class LevelConfigurationViewModel : SensorViewModelBase {
        public LevelConfigurationViewModel() {
        }

        double lCenter = 0;
        int rSphere = 0;
        int deltaParam = 0;
        double aLParam = 0;
        int overlevel = 0;
        double bLParam = 0;
        double pushParam = 0;

        public double LCenter { get { return lCenter; } set { SetPropertyValue("LCenter", ref lCenter, value); } }
        public int RSphere { get { return rSphere; } set { SetPropertyValue("RSphere", ref rSphere, value); } }
        public int DeltaParam { get { return deltaParam; } set { SetPropertyValue("DeltaParam", ref deltaParam, value); } }
        public double ALParam { get { return aLParam; } set { SetPropertyValue("ALParam", ref aLParam, value); } }
        public int Overlevel { get { return overlevel; } set { SetPropertyValue("Overlevel", ref overlevel, value); } }
        public double BLParam { get { return bLParam; } set { SetPropertyValue("BLParam", ref bLParam, value); } }
        public double PushParam { get { return pushParam; } set { SetPropertyValue("PushParam", ref pushParam, value); } }

        protected override string GetReadProtocol() {
            return factory.CreateReadLevelCoeffProtocol(DateTime.Now, MainViewModel.OperatorName, MainViewModel.Serial, MainViewModel.Address, MainViewModel.SensorType,
                LCenter, RSphere, DeltaParam, PushParam, HParam, Angle, Overlevel, ALParam, BLParam, ZeroX, ScaleX, ZeroY, ScaleY, RecordDate);
        }
        protected override string GetWriteProtocol() {
            return factory.CreateWriteLevelCoeffProtocol(DateTime.Now, MainViewModel.OperatorName, MainViewModel.Serial, MainViewModel.Address, MainViewModel.SensorType,
                LCenter, RSphere, DeltaParam, PushParam, HParam, Angle, Overlevel, ALParam, BLParam, ZeroX, ScaleX, ZeroY, ScaleY, RecordDate);
        }
        protected override int GetIndex() {
            return 2;
        }
        protected override string GetUniqueId() {
            return "Level";
        }
        protected override bool IsSensorVerified(UInt16 verifiedValue) {
            return verifiedValue < 970;
        }
        protected override string GetReadFilePath() {
            return FilePathHelper.ReadLevel(MainViewModel.Serial);
        }
        protected override string GetWriteFilePath() {
            return FilePathHelper.WriteLevel(MainViewModel.Serial);
        }
    
        protected override void ValuesToView(string id, List<UInt16> data) {
            if(id == "Read_Step1") {
                LCenter = SensorsManager.Utils.Converters.LCenterToView(data[0]);
                RSphere = SensorsManager.Utils.Converters.RSphereToView(data[1]);
                DeltaParam = SensorsManager.Utils.Converters.DeltaParamToView(data[2]);
                PushParam = SensorsManager.Utils.Converters.PushParamToView(data[3]);
                HParam = SensorsManager.Utils.Converters.HParamToView(data[4]);
                if(MainViewModel.SensorType == SensorType.IUG5_WIRELESS && !MainViewModel.UseBridge) {
                    Angle = SensorsManager.Utils.Converters.AngleToView(data[5]);
                    RecordDate = string.Format("{0:D2}:{1}", data[7], data[6]);
                    Overlevel = SensorsManager.Utils.Converters.OverlevelToView(data[8]);
                    ALParam = SensorsManager.Utils.Converters.ALParamToView(data[9]);
                    BLParam = SensorsManager.Utils.Converters.BLParamToView(data[10]);
                }
            }
            if(id == "Read_Step2" && MainViewModel.SensorType != SensorType.IUG5_WIRELESS) {
                Angle = SensorsManager.Utils.Converters.AngleToView(data[0]);
                ZeroX = data[1];
                ScaleX = data[2];
                ZeroY = data[3];
                ScaleY = data[4];
            }
            if(id == "Read_Step2" && MainViewModel.SensorType == SensorType.IUG5_WIRELESS
                && MainViewModel.UseBridge) {
                Angle = SensorsManager.Utils.Converters.AngleToView(data[0]);
                RecordDate = string.Format("{0:D2}:{1}", data[2], data[1]);
                Overlevel = SensorsManager.Utils.Converters.OverlevelToView(data[3]);
                ALParam = SensorsManager.Utils.Converters.ALParamToView(data[4]);
            }
            if(id == "Read_Step3" && MainViewModel.SensorType != SensorType.IUG5_WIRELESS) {
                RecordDate = string.Format("{0:D2}:{1}", data[1], data[0]);
                Overlevel = SensorsManager.Utils.Converters.OverlevelToView(data[2]);
                ALParam = SensorsManager.Utils.Converters.ALParamToView(data[3]);
                BLParam = SensorsManager.Utils.Converters.BLParamToView(data[4]);
            }
            if(id == "Read_Step3" && MainViewModel.SensorType == SensorType.IUG5_WIRELESS
                && MainViewModel.UseBridge) {
                BLParam = SensorsManager.Utils.Converters.BLParamToView(data[0]);
            }
            if(id == "Read_Step4" && MainViewModel.SensorType == SensorType.IUG5_WIRELESS) {
                ZeroX = data[0];
                ScaleX = data[1];
                ZeroY = data[2];
                ScaleY = data[3];
            }
        }
        protected override List<UInt16> ValuesFromView(string id) {
            var result = new List<UInt16>();
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;

            if(id == "Write_Step1") {
                result.Add(SensorsManager.Utils.Converters.LCenterFromView(LCenter));
                result.Add(SensorsManager.Utils.Converters.RSphereFromView(RSphere));
                result.Add(SensorsManager.Utils.Converters.DeltaParamFromView(DeltaParam));
                result.Add(SensorsManager.Utils.Converters.PushParamFromView(PushParam));
                result.Add(SensorsManager.Utils.Converters.HParamFromView(HParam));
                if(MainViewModel.SensorType == SensorType.IUG5_WIRELESS && !MainViewModel.UseBridge) {
                    result.Add(SensorsManager.Utils.Converters.AngleFromView(Angle));
                    result.Add((UInt16)year);
                    result.Add((UInt16)month);
                    RecordDate = string.Format("{0:D2}:{1}", month, year);
                    result.Add(SensorsManager.Utils.Converters.OverlevelFromView(Overlevel));
                    result.Add(SensorsManager.Utils.Converters.ALParamFromView(ALParam));
                    result.Add(SensorsManager.Utils.Converters.BLParamFromView(BLParam));
                    result.Add(0);
                    result.Add(0);
                    result.Add(0);
                    result.Add(0);
                }
            }
            if(id == "Write_Step2" && MainViewModel.SensorType != SensorType.IUG5_WIRELESS) {
                result.Add(SensorsManager.Utils.Converters.AngleFromView(Angle));
                result.Add((UInt16)ZeroX);
                result.Add((UInt16)ScaleX);
                result.Add((UInt16)ZeroY);
                result.Add((UInt16)ScaleY);
            }
            if(id == "Write_Step2" && MainViewModel.SensorType == SensorType.IUG5_WIRELESS
                && MainViewModel.UseBridge) {
                result.Add(SensorsManager.Utils.Converters.AngleFromView(Angle));
                result.Add((UInt16)year);
                result.Add((UInt16)month);
                RecordDate = string.Format("{0:D2}:{1}", month, year);
                result.Add(SensorsManager.Utils.Converters.OverlevelFromView(Overlevel));
                result.Add(SensorsManager.Utils.Converters.ALParamFromView(ALParam));
            }
            if(id == "Write_Step3" && MainViewModel.SensorType != SensorType.IUG5_WIRELESS) {
                result.Add((UInt16)year);
                result.Add((UInt16)month);
                RecordDate = string.Format("{0:D2}:{1}", month, year);
                result.Add(SensorsManager.Utils.Converters.OverlevelFromView(Overlevel));
                result.Add(SensorsManager.Utils.Converters.ALParamFromView(ALParam));
                result.Add(SensorsManager.Utils.Converters.BLParamFromView(BLParam));
            }
            if(id == "Write_Step3" && MainViewModel.SensorType == SensorType.IUG5_WIRELESS
                && MainViewModel.UseBridge) {
                result.Add(SensorsManager.Utils.Converters.BLParamFromView(BLParam));
                result.Add(0);
                result.Add(0);
                result.Add(0);
                result.Add(0);
            }
            return result;
        }
    }
}
