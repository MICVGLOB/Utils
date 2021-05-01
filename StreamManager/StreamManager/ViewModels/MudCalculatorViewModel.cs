using Mvvm.Core;
using StreamManager.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Mvvm.ViewModels {
    public class MudCalculatorViewModel : StreamViewModelBase {
        protected override string SourceId { get { return "MudCalculator"; } }

        public MudCalculatorViewModel() {
            CalculateCommand = new DelegateCommand(Calculate, () => true);
        }

        #region Properties

        ChannelType channelType;
        int mudLevel;
        int coeff1;
        int coeff2;
        int coeff3;
        int mudSquare;

        public int MudLevel { get { return mudLevel; } set { SetPropertyValue("MudLevel", ref mudLevel, value); } }
        public int Coeff1 { get { return coeff1; } set { SetPropertyValue("Coeff1", ref coeff1, value); } }
        public int Coeff2 { get { return coeff2; } set { SetPropertyValue("Coeff2", ref coeff2, value); } }
        public int Coeff3 { get { return coeff3; } set { SetPropertyValue("Coeff3", ref coeff3, value); } }

        public int MudSquare { get { return mudSquare; } set { SetPropertyValue("MudSquare", ref mudSquare, value); } }
        public ChannelType ChannelType { get { return channelType; } set { SetPropertyValue("ChannelType", ref channelType, value); } }

        #endregion

        public ICommand CalculateCommand { get; private set; } 

        void Calculate() {
            switch(ChannelType) {
                case ChannelType.Rectangle:
                    MudSquare = (int)(MudLevel * Coeff1 / 100.0);
                    break;
                case ChannelType.Pipe:
                    MudSquare = CalculatePipeSquare();
                    break;
                case ChannelType.Trapeze:
                    MudSquare = (int)Math.Round((MudLevel * Coeff1 + MudLevel * MudLevel * (Coeff2 - Coeff1) / 2.0 / Coeff3) / 100.0);
                    break;
                case ChannelType.Flask:
                    MudSquare = (MudLevel > Coeff1 / 2.0) ? (int)Math.Round((Math.PI * Coeff1 * Coeff1 / 8.0
                            + Coeff1 * (MudLevel - Coeff1 / 2.0)) / 100.0) : CalculatePipeSquare();
                    break;
                default:
                    break;
            }
        }
        int CalculatePipeSquare() {
            double r = Coeff1 / 20.0;
            double level = MudLevel / 10.0;
            double tmp = 1.0 - level / r;
            if(tmp <= -1.0)
                tmp = -0.9999;
            if(tmp >= 1.0)
                tmp = 0.9999;
            double angle = Math.Acos(tmp);
            return (int)Math.Round(angle * r * r - (r - level) * r * Math.Sin(angle));
        }
    }
}
