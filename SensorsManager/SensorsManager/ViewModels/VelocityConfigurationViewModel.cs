using Controls.Core;
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
    public class VelocityConfigurationViewModel : SensorViewModelBase {
        public VelocityConfigurationViewModel() {
        }
        double lBlade = 0;
        double wBlade = 0;
        double mBlade = 0;
        double gParam = 0;
        int bladeDensity = 0;
        double bVParam = 0;
        int liqDensity = 0;

        public double LBlade { get { return lBlade; } set { SetPropertyValue("LBlade", ref lBlade, value); } }
        public double WBlade { get { return wBlade; } set { SetPropertyValue("WBlade", ref wBlade, value); } }
        public double MBlade { get { return mBlade; } set { SetPropertyValue("MBlade", ref mBlade, value); } }
        public double GParam { get { return gParam; } set { SetPropertyValue("GParam", ref gParam, value); } }
        public int BladeDensity { get { return bladeDensity; } set { SetPropertyValue("BladeDensity", ref bladeDensity, value); } }
        public double BVParam { get { return bVParam; } set { SetPropertyValue("BVParam", ref bVParam, value); } }
        public int LiqDensity { get { return liqDensity; } set { SetPropertyValue("LiqDensity", ref liqDensity, value); } }

        protected override string GetReadProtocol() {
            return factory.CreateReadVelocityCoeffProtocol(DateTime.Now, MainViewModel.OperatorName, MainViewModel.Serial, MainViewModel.Address, MainViewModel.SensorType,
                LBlade, MBlade, WBlade, GParam, HParam, Angle, LiqDensity, BladeDensity, BVParam, ZeroX, ScaleX, ZeroY, ScaleY, RecordDate);
        }
        protected override string GetWriteProtocol() {
            return factory.CreateWriteVelocityCoeffProtocol(DateTime.Now, MainViewModel.OperatorName, MainViewModel.Serial, MainViewModel.Address, MainViewModel.SensorType,
                LBlade, MBlade, WBlade, GParam, HParam, Angle, LiqDensity, BladeDensity, BVParam, ZeroX, ScaleX, ZeroY, ScaleY, RecordDate);
        }
        protected override int GetIndex() {
            return 3;
        }
        protected override string GetUniqueId() {
            return "Velocity";
        }
        protected override bool IsSensorVerified(UInt16 verifiedValue) {
            return verifiedValue >= 970;
        }
        protected override string GetReadFilePath() {
            return FilePathHelper.ReadVelocity(MainViewModel.Serial);
        }
        protected override string GetWriteFilePath() {
            return FilePathHelper.WriteVelocity(MainViewModel.Serial);
        }
        protected override void ValuesToView(string id, List<UInt16> data) {
            if(id == "Read_Step1") {
                LBlade = SensorsManager.Utils.Converters.LBladeToView(data[0]);
                MBlade = SensorsManager.Utils.Converters.MBladeToView(data[1]);
                WBlade = SensorsManager.Utils.Converters.WBladeToView(data[2]);
                GParam = SensorsManager.Utils.Converters.GParamToView(data[3]);
                HParam = SensorsManager.Utils.Converters.HParamToView(data[4]);
                if(MainViewModel.SensorType == SensorType.IUG5_WIRELESS && !MainViewModel.UseBridge) {
                    Angle = SensorsManager.Utils.Converters.AngleToView(data[5]);
                    RecordDate = string.Format("{0:D2}:{1}", data[7], data[6]);
                    LiqDensity = SensorsManager.Utils.Converters.DensityToView(data[8]);
                    BladeDensity = SensorsManager.Utils.Converters.DensityToView(data[9]);
                    BVParam = SensorsManager.Utils.Converters.BVParamToView(data[10]);
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
                LiqDensity = SensorsManager.Utils.Converters.DensityToView(data[3]);
                BladeDensity = SensorsManager.Utils.Converters.DensityToView(data[4]);
            }
            if(id == "Read_Step3" && MainViewModel.SensorType != SensorType.IUG5_WIRELESS) {
                RecordDate = string.Format("{0:D2}:{1}", data[1], data[0]);
                LiqDensity = SensorsManager.Utils.Converters.DensityToView(data[2]);
                BladeDensity = SensorsManager.Utils.Converters.DensityToView(data[3]);
                BVParam = SensorsManager.Utils.Converters.BVParamToView(data[4]);
            }
            if(id == "Read_Step3" && MainViewModel.SensorType == SensorType.IUG5_WIRELESS
                && MainViewModel.UseBridge) {
                BVParam = SensorsManager.Utils.Converters.BVParamToView(data[0]);
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
                result.Add(SensorsManager.Utils.Converters.LBladeFromView(LBlade));
                result.Add(SensorsManager.Utils.Converters.MBladeFromView(MBlade));
                result.Add(SensorsManager.Utils.Converters.WBladeFromView(WBlade));
                result.Add(SensorsManager.Utils.Converters.GParamFromView(GParam));
                result.Add(SensorsManager.Utils.Converters.HParamFromView(HParam));
                if(MainViewModel.SensorType == SensorType.IUG5_WIRELESS && !MainViewModel.UseBridge) {
                    result.Add(SensorsManager.Utils.Converters.AngleFromView(Angle));
                    result.Add((UInt16)year);
                    result.Add((UInt16)month);
                    RecordDate = string.Format("{0:D2}:{1}", month, year);
                    result.Add(SensorsManager.Utils.Converters.DensityFromView(LiqDensity));
                    result.Add(SensorsManager.Utils.Converters.DensityFromView(BladeDensity));
                    result.Add(SensorsManager.Utils.Converters.BVParamFromView(BVParam));
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
                result.Add(SensorsManager.Utils.Converters.DensityFromView(LiqDensity));
                result.Add(SensorsManager.Utils.Converters.DensityFromView(BladeDensity));
            }
            if(id == "Write_Step3" && MainViewModel.SensorType != SensorType.IUG5_WIRELESS) {
                result.Add((UInt16)year);
                result.Add((UInt16)month);
                RecordDate = string.Format("{0:D2}:{1}", month, year);
                result.Add(SensorsManager.Utils.Converters.DensityFromView(LiqDensity));
                result.Add(SensorsManager.Utils.Converters.DensityFromView(BladeDensity));
                result.Add(SensorsManager.Utils.Converters.BVParamFromView(BVParam));
            }
            if(id == "Write_Step3" && MainViewModel.SensorType == SensorType.IUG5_WIRELESS
                && MainViewModel.UseBridge) {
                result.Add(SensorsManager.Utils.Converters.BVParamFromView(BVParam));
                result.Add(0);
                result.Add(0);
                result.Add(0);
                result.Add(0);
            }
            return result;
        }
    }
}
