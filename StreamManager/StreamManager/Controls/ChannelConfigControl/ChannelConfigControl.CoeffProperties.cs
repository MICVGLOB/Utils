using StreamManager.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Controls {
    public partial class ChannelConfigControl {
        public static readonly DependencyProperty Coeff0InfoProperty =
            DependencyProperty.Register("Coeff0Info", typeof(ChannelCoeffInfo), typeof(ChannelConfigControl), new PropertyMetadata(null));
        public static readonly DependencyProperty Coeff0ValueProperty =
            DependencyProperty.Register("Coeff0Value", typeof(double), typeof(ChannelConfigControl), new PropertyMetadata(0.0, (d, e) => ((ChannelConfigControl)d).OnCoeffValueChanged()));

        public static readonly DependencyProperty Coeff1InfoProperty =
            DependencyProperty.Register("Coeff1Info", typeof(ChannelCoeffInfo), typeof(ChannelConfigControl), new PropertyMetadata(null));
        public static readonly DependencyProperty Coeff1ValueProperty =
            DependencyProperty.Register("Coeff1Value", typeof(double), typeof(ChannelConfigControl), new PropertyMetadata(0.0, (d, e) => ((ChannelConfigControl)d).OnCoeffValueChanged()));

        public static readonly DependencyProperty Coeff2InfoProperty =
            DependencyProperty.Register("Coeff2Info", typeof(ChannelCoeffInfo), typeof(ChannelConfigControl), new PropertyMetadata(null));
        public static readonly DependencyProperty Coeff2ValueProperty =
            DependencyProperty.Register("Coeff2Value", typeof(double), typeof(ChannelConfigControl), new PropertyMetadata(0.0, (d, e) => ((ChannelConfigControl)d).OnCoeffValueChanged()));

        public static readonly DependencyProperty Coeff3InfoProperty =
            DependencyProperty.Register("Coeff3Info", typeof(ChannelCoeffInfo), typeof(ChannelConfigControl), new PropertyMetadata(null));
        public static readonly DependencyProperty Coeff3ValueProperty =
            DependencyProperty.Register("Coeff3Value", typeof(double), typeof(ChannelConfigControl), new PropertyMetadata(0.0, (d, e) => ((ChannelConfigControl)d).OnCoeffValueChanged()));

        public static readonly DependencyProperty Coeff4InfoProperty =
            DependencyProperty.Register("Coeff4Info", typeof(ChannelCoeffInfo), typeof(ChannelConfigControl), new PropertyMetadata(null));
        public static readonly DependencyProperty Coeff4ValueProperty =
            DependencyProperty.Register("Coeff4Value", typeof(double), typeof(ChannelConfigControl), new PropertyMetadata(0.0, (d, e) => ((ChannelConfigControl)d).OnCoeffValueChanged()));

        public static readonly DependencyProperty Coeff5InfoProperty =
            DependencyProperty.Register("Coeff5Info", typeof(ChannelCoeffInfo), typeof(ChannelConfigControl), new PropertyMetadata(null));
        public static readonly DependencyProperty Coeff5ValueProperty =
            DependencyProperty.Register("Coeff5Value", typeof(double), typeof(ChannelConfigControl), new PropertyMetadata(0.0, (d, e) => ((ChannelConfigControl)d).OnCoeffValueChanged()));

        public static readonly DependencyProperty Coeff6InfoProperty =
            DependencyProperty.Register("Coeff6Info", typeof(ChannelCoeffInfo), typeof(ChannelConfigControl), new PropertyMetadata(null));
        public static readonly DependencyProperty Coeff6ValueProperty =
            DependencyProperty.Register("Coeff6Value", typeof(double), typeof(ChannelConfigControl), new PropertyMetadata(0.0, (d, e) => ((ChannelConfigControl)d).OnCoeffValueChanged()));

        public static readonly DependencyProperty Coeff7InfoProperty =
            DependencyProperty.Register("Coeff7Info", typeof(ChannelCoeffInfo), typeof(ChannelConfigControl), new PropertyMetadata(null));
        public static readonly DependencyProperty Coeff7ValueProperty =
            DependencyProperty.Register("Coeff7Value", typeof(double), typeof(ChannelConfigControl), new PropertyMetadata(0.0, (d, e) => ((ChannelConfigControl)d).OnCoeffValueChanged()));

        public ChannelCoeffInfo Coeff0Info { get { return (ChannelCoeffInfo)GetValue(Coeff0InfoProperty); } set { SetValue(Coeff0InfoProperty, value); } }
        public double Coeff0Value { get { return (double)GetValue(Coeff0ValueProperty); } set { SetValue(Coeff0ValueProperty, value); } }

        public ChannelCoeffInfo Coeff1Info { get { return (ChannelCoeffInfo)GetValue(Coeff1InfoProperty); } set { SetValue(Coeff1InfoProperty, value); } }
        public double Coeff1Value { get { return (double)GetValue(Coeff1ValueProperty); } set { SetValue(Coeff1ValueProperty, value); } }

        public ChannelCoeffInfo Coeff2Info { get { return (ChannelCoeffInfo)GetValue(Coeff2InfoProperty); } set { SetValue(Coeff2InfoProperty, value); } }
        public double Coeff2Value { get { return (double)GetValue(Coeff2ValueProperty); } set { SetValue(Coeff2ValueProperty, value); } }

        public ChannelCoeffInfo Coeff3Info { get { return (ChannelCoeffInfo)GetValue(Coeff3InfoProperty); } set { SetValue(Coeff3InfoProperty, value); } }
        public double Coeff3Value { get { return (double)GetValue(Coeff3ValueProperty); } set { SetValue(Coeff3ValueProperty, value); } }

        public ChannelCoeffInfo Coeff4Info { get { return (ChannelCoeffInfo)GetValue(Coeff4InfoProperty); } set { SetValue(Coeff4InfoProperty, value); } }
        public double Coeff4Value { get { return (double)GetValue(Coeff4ValueProperty); } set { SetValue(Coeff4ValueProperty, value); } }

        public ChannelCoeffInfo Coeff5Info { get { return (ChannelCoeffInfo)GetValue(Coeff5InfoProperty); } set { SetValue(Coeff5InfoProperty, value); } }
        public double Coeff5Value { get { return (double)GetValue(Coeff5ValueProperty); } set { SetValue(Coeff5ValueProperty, value); } }

        public ChannelCoeffInfo Coeff6Info { get { return (ChannelCoeffInfo)GetValue(Coeff6InfoProperty); } set { SetValue(Coeff6InfoProperty, value); } }
        public double Coeff6Value { get { return (double)GetValue(Coeff6ValueProperty); } set { SetValue(Coeff6ValueProperty, value); } }

        public ChannelCoeffInfo Coeff7Info { get { return (ChannelCoeffInfo)GetValue(Coeff7InfoProperty); } set { SetValue(Coeff7InfoProperty, value); } }
        public double Coeff7Value { get { return (double)GetValue(Coeff7ValueProperty); } set { SetValue(Coeff7ValueProperty, value); } }

    }
}
