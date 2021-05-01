using Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using System.Globalization;
using Controls.Core;

namespace Mvvm {
    public class NumericToRegNumberConverter : MarkupExtension, IValueConverter {
        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
        public int Offset { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
           return "0x" + ((UInt16)value + Offset).ToString("X2");
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
    public class IntToUint16Converter : MarkupExtension, IValueConverter {
        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var intValue = (int)value;
            return (UInt16)intValue;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            var uintValue = (UInt16)value;
            return (int)uintValue;
        }
    }
    public class ValidMultiConverter : MarkupExtension, IMultiValueConverter {
        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            foreach(object value in values) {
                if((value is bool) && !(bool)value)
                    return false;
            }
            return true;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
