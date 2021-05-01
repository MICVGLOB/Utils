using Modbus.Core;
using Mvvm.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Mvvm.ViewModels {
    public class TerminalViewModel : StreamViewModelBase {
        protected override string SourceId { get { return "Terminal"; } }

        #region Properties
        ICommand appendTextCommand;
        string zAngleText;
        string xyAngleText;


        public ICommand AppendTextCommand { get { return appendTextCommand; } set { SetPropertyValue("AppendTextCommand", ref appendTextCommand, value); } }
        public string ZAngleText { get { return zAngleText; } set { SetPropertyValue("ZAngleText", ref zAngleText, value); } }
        public string XYAngleText { get { return xyAngleText; } set { SetPropertyValue("XYAngleText", ref xyAngleText, value); } }

        #endregion


        protected override void ReceiveTerminalDataChanged() {
            var newData = MainViewModel.ReceiveTerminalData;
            if(newData == null || !newData.Any())
                return;
            Calc1(newData);
            AddText(StringsHelper.InclinometerRecordToLookUpStringConverter(newData));
        }
        void AddText(string text) {
            if(AppendTextCommand != null && text != string.Empty)
                AppendTextCommand.Execute(text);
        }

        void Calc1(List<byte> data) {
            UInt32 xData = ((UInt32)data[6] << 16) + ((UInt32)data[7] << 8) + (UInt32)data[8];
            UInt32 yData = ((UInt32)data[9] << 16) + ((UInt32)data[10] << 8) + (UInt32)data[11];
            UInt32 zData = ((UInt32)data[12] << 16) + ((UInt32)data[13] << 8) + (UInt32)data[14];

            UInt64 fullValue = ((UInt64)1) << 24;
            UInt64 halfValue = fullValue / 2;
            double normMultiplyer = 1/4194304.0;

            double gX = ((xData > halfValue) ? -((double)(fullValue - (UInt64)xData)) 
                : xData) * normMultiplyer;
            double gY = ((yData > halfValue) ? -((double)(fullValue - (UInt64)yData))
                : yData) * normMultiplyer;
            double gZ = ((zData > halfValue) ? -((double)(fullValue - (UInt64)zData))
                : zData) * normMultiplyer;

            double gT = Math.Sqrt(gX * gX + gY * gY);
            double zAngle = (180.0 / Math.PI) * Math.Atan(gT / gZ);
            ZAngleText = string.Format("ZAngle = {0:F2} deg.", zAngle);

            double teta = 0;

            if(Math.Abs(gX) <= Math.Abs(gY)) {
                teta = (180.0 / Math.PI) * Math.Acos(gX / gT);
                if(gY < 0)
                    teta *= -1;

            } else {
                teta = (180.0 / Math.PI) * Math.Asin(gY / gT);
                if(gX < 0 && gY > 0) {
                    teta = 180.0 - Math.Abs(teta);
                }
                if(gX < 0 && gY < 0) {
                    teta = -180.0 + Math.Abs(teta);
                }
            }
            XYAngleText = string.Format("XYAngle = {0:F2} deg.", teta);
        }

    }

}
