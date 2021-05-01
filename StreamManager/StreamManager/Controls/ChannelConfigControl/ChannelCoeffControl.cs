using StreamManager.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Controls {
    public class ChannelCoeffControl : Control {
        public static readonly DependencyProperty CoeffInfoProperty =
            DependencyProperty.Register("CoeffInfo", typeof(ChannelCoeffInfo), typeof(ChannelCoeffControl),
                new PropertyMetadata(null, (d, e) => ((ChannelCoeffControl)d).OnCoeffInfoChanged()));
        public static readonly DependencyProperty CoeffNameProperty =
            DependencyProperty.Register("CoeffName", typeof(string), typeof(ChannelCoeffControl), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty CoeffMinValueProperty =
            DependencyProperty.Register("CoeffMinValue", typeof(double), typeof(ChannelCoeffControl), new PropertyMetadata(0.0));
        public static readonly DependencyProperty CoeffMaxValueProperty =
            DependencyProperty.Register("CoeffMaxValue", typeof(double), typeof(ChannelCoeffControl), new PropertyMetadata(0.0));
        public static readonly DependencyProperty CoeffValueProperty =
            DependencyProperty.Register("CoeffValue", typeof(double), typeof(ChannelCoeffControl), new PropertyMetadata(0.0));
        public static readonly DependencyProperty CoeffPrecisionProperty =
            DependencyProperty.Register("CoeffPrecision", typeof(int), typeof(ChannelCoeffControl), new PropertyMetadata(1));

        public ChannelCoeffInfo CoeffInfo { get { return (ChannelCoeffInfo)GetValue(CoeffInfoProperty); } set { SetValue(CoeffInfoProperty, value); } }
        public string CoeffName { get { return (string)GetValue(CoeffNameProperty); } set { SetValue(CoeffNameProperty, value); } }
        public double CoeffMinValue { get { return (double)GetValue(CoeffMinValueProperty); } set { SetValue(CoeffMinValueProperty, value); } }
        public double CoeffMaxValue { get { return (double)GetValue(CoeffMaxValueProperty); } set { SetValue(CoeffMaxValueProperty, value); } }
        public double CoeffValue { get { return (double)GetValue(CoeffValueProperty); } set { SetValue(CoeffValueProperty, value); } }
        public int CoeffPrecision { get { return (int)GetValue(CoeffPrecisionProperty); } set { SetValue(CoeffPrecisionProperty, value); } }

        void OnCoeffInfoChanged() {
            if(CoeffInfo == null)
                return;
            CoeffName = CoeffInfo.Name;
            CoeffMinValue = CoeffInfo.MinValue;
            CoeffMaxValue = CoeffInfo.MaxValue;
            CoeffPrecision = CoeffInfo.Precision;
        }
        public override void OnApplyTemplate() {
            base.OnApplyTemplate();
            OnCoeffInfoChanged();
        }
    }
}
