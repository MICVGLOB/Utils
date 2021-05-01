using Controls.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace Mvvm {
    public class BooleanToBrushConverter : MarkupExtension, IValueConverter {
        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var input = (bool?)value;
            if(input.HasValue)
                return input.Value ? new SolidColorBrush(Colors.LightCoral) : new SolidColorBrush(Colors.Green);
            return new SolidColorBrush(Colors.DarkGray);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
    public class NumericToRegNumberConverter : MarkupExtension, IValueConverter {
        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
        public int Offset { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return "Регистр " + ((int)value + Offset);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    public class BooleanToOpacityMultiConverter : MarkupExtension, IMultiValueConverter {
        public double Opacity { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            if(values[1] == System.Windows.DependencyProperty.UnsetValue)
                return 1.0;
            var decodeErrorType = (ViewModels.DecodeErrorType)values[0];
            var sensorType = (ViewModels.SensorType)values[1];
            return (decodeErrorType != ViewModels.DecodeErrorType.No) 
                && (sensorType == ViewModels.SensorType.IUG5_WIRELESS) ? 1.0 : Opacity;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
    public class RegsQuantityToBooleanConverter : MarkupExtension, IValueConverter {
        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
        public int Count { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return (int)value >= Count;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
