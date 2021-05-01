using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SensorsManager.Utils {
    public static class Converters {
        public static int ValueToMinValue(UInt16 value, int minValue) {
            int intValue = (int)value;
            int newValue = (int)value;
            if(newValue > 35000 && newValue < 65536) {
                newValue = -(65536 - newValue);
                if(minValue == 0)
                    return intValue;
                int minX = -(65536 - minValue);
                if(newValue < minX)
                    return intValue;
            }
            return minValue;
        }
        public static int ValueToMaxValue(UInt16 value, int maxValue) {
            int newValue = (int)value;
            if(newValue > 0 && newValue < 25000 && newValue > maxValue)
                return newValue;
            return maxValue;
        }
        public static UInt16 LCenterFromView(double lCenter) {
            return (UInt16)Math.Round(2.0 * lCenter);
        }
        public static double LCenterToView(UInt16 lCenter) {
            return ((double)lCenter) / 2.0;
        }
        public static UInt16 RSphereFromView(int rSphere) {
            return (UInt16)rSphere;
        }
        public static int RSphereToView(UInt16 rSphere) {
            return (int)rSphere;
        }
        public static UInt16 HParamFromView(int hParam) {
            return (UInt16)hParam;
        }
        public static int HParamToView(UInt16 hParam) {
            return (int)hParam;
        }
        public static UInt16 DeltaParamFromView(int deltaParam) {
            return (UInt16)deltaParam;
        }
        public static int DeltaParamToView(UInt16 deltaParam) {
            return (int)deltaParam;
        }
        public static UInt16 AngleFromView(double angle) {
            if(angle >= 0)
                return (UInt16)Math.Round(10.0 * angle);
            var tempAngle = 65536.0 + 10.0 * angle;
            return (UInt16)Math.Round(tempAngle);
        }
        public static double AngleToView(UInt16 angle) {
            if(angle < 32000)
                return (double)angle / 10.0;
            var delta = (double)angle - 65536.0;
            return delta / 10.0;
        }
        public static UInt16 ALParamFromView(double ALParam) {
            if(ALParam >= 0)
                return (UInt16)Math.Round(ALParam);
            var tempParam = 65536.0 + ALParam;
            return (UInt16)Math.Round(tempParam);
        }
        public static double ALParamToView(UInt16 aLParam) {
            if(aLParam < 32000)
                return (double)aLParam;
            var delta = (double)aLParam - 65536.0;
            return delta;
        }
        public static UInt16 OverlevelFromView(int overlevel) {
            return (UInt16)overlevel;
        }
        public static int OverlevelToView(UInt16 overlevel) {
            return (int)overlevel;
        }
        public static UInt16 BLParamFromView(double bLParam) {
            if(bLParam >= 0)
                return (UInt16)Math.Round(1000.0 * bLParam);
            var tempbL = 65536.0 + 1000.0 * bLParam;
            return (UInt16)Math.Round(tempbL);
        }
        public static double BLParamToView(UInt16 bLParam) {
            if(bLParam < 32000)
                return (double)bLParam / 1000.0;
            var delta = (double)bLParam - 65536.0;
            return delta / 1000.0;
        }
        public static UInt16 PushParamFromView(double pushParam) {
            return (UInt16)Math.Round(1000.0 * pushParam);
        }
        public static double PushParamToView(UInt16 pushParam) {
            return (double)pushParam / 1000.0;
        }
        public static UInt16 LBladeFromView(double lBlade) {
            return (UInt16)Math.Round(2.0 * lBlade);
        }
        public static double LBladeToView(UInt16 lBlade) {
            return (double)lBlade / 2.0;
        }
        public static UInt16 MBladeFromView(double mBlade) {
            return (UInt16)Math.Round(2.0 * mBlade);
        }
        public static double MBladeToView(UInt16 mBlade) {
            return (double)mBlade / 2.0;
        }
        public static UInt16 WBladeFromView(double wBlade) {
            return (UInt16)Math.Round(10.0 * wBlade);
        }
        public static double WBladeToView(UInt16 wBlade) {
            return (double)wBlade / 10.0;
        }
        public static UInt16 GParamFromView(double gParam) {
            return (UInt16)Math.Round(100.0 * gParam);
        }
        public static double GParamToView(UInt16 gParam) {
            return (double)gParam / 100.0;
        }
        public static UInt16 DensityFromView(int Density) {
            return (UInt16)Density;
        }
        public static int DensityToView(UInt16 Density) {
            return (int)Density;
        }
        public static UInt16 BVParamFromView(double bVParam) {
            if(bVParam >= 0)
                return (UInt16)Math.Round(1000.0 * bVParam);
            var tempbL = 65536.0 + 1000.0 * bVParam;
            return (UInt16)Math.Round(tempbL);
        }
        public static double BVParamToView(UInt16 bVParam) {
            if(bVParam < 32000)
                return (double)bVParam / 1000.0;
            var delta = (double)bVParam - 65536.0;
            return delta / 1000.0;
        }
    }
}
