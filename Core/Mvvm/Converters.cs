using Controls.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Mvvm.Core {
    public static class Converters {
        static Dictionary<RegisterViewMode, string> Prefixes = new Dictionary<RegisterViewMode, string>() {
                { RegisterViewMode.Decimal, "" },
                { RegisterViewMode.Hex, "0x" },
                { RegisterViewMode.Bin, "0b" },
            };
        static Dictionary<RegisterViewMode, int> MaxTextCount = new Dictionary<RegisterViewMode, int>() {
                { RegisterViewMode.Decimal, 5 },
                { RegisterViewMode.Hex, 6 },
                { RegisterViewMode.Bin, 18 },
            };
        static Dictionary<RegisterViewMode, int> ModeBase = new Dictionary<RegisterViewMode, int>() {
                { RegisterViewMode.Decimal, 10 },
                { RegisterViewMode.Hex, 16 },
                { RegisterViewMode.Bin, 2 },
            };
        public static bool ViewModeTryConvertText(string Text, RegisterViewMode mode, out UInt16 RegValue) {
            if(mode != RegisterViewMode.Decimal && (!Text.StartsWith(Prefixes[mode])
                || Text.Length < Prefixes[mode].Count() + 1) || Text.Length > MaxTextCount[mode]) {
                RegValue = 0;
                return false;
            }
            UInt16 result = 0;
            try {
                result = Convert.ToUInt16(Text.Remove(0, Prefixes[mode].Length), ModeBase[mode]);
            } catch {
                RegValue = 0;
                return false;
            }
            RegValue = result;
            return true;
        }
        public static bool TryConvertTextToText(RegisterViewMode oldMode, RegisterViewMode newMode, string Text, out string resultText) {
            UInt16 result = 0;
            bool isValid = ViewModeTryConvertText(Text, oldMode, out result);
            if(!isValid) {
                resultText = string.Empty;
                return false;
            }
            resultText = Uint16ToText(newMode, result);
            return true;
        }
        public static string Uint16ToText(RegisterViewMode mode, UInt16 value) {
            string text = Convert.ToString(value, ModeBase[mode]);
            if(mode != RegisterViewMode.Decimal)
                text = text.PadLeft(MaxTextCount[mode] - 2, '0');
            return string.Format(Prefixes[mode] + "{0}", text);
        }
    }

    public enum RegisterViewMode {
        Decimal,
        Hex,
        Bin
    }

    public class BooleanNegationConverter : MarkupExtension, IValueConverter {
        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return !((bool)value);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
    public class SerialPortModeToBoolConverter : MarkupExtension, IValueConverter {
        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var mode = (SerialPortControlMode)value;
            return !(mode == SerialPortControlMode.Progress || mode == SerialPortControlMode.IdleProgress
                || mode == SerialPortControlMode.UndeterminateProgress || mode == SerialPortControlMode.RepeatedProgress);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
    public class BooleanToOpacityConverter : MarkupExtension, IValueConverter {
        public double Opacity { get; set; }
        public bool Invert { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var input = (bool)value;
            if(Invert)
                input = !input;
            return input ? Opacity : 1.0;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
    public class BooleanToVisibilityConverter : MarkupExtension, IValueConverter {
        public bool UseHiddenInsteadOfCollapsed { get; set; }
        public bool Invert { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var input = (bool)value;
            var realInput = Invert ? !input : input;
            return realInput ? Visibility.Visible : (UseHiddenInsteadOfCollapsed ? Visibility.Hidden : Visibility.Collapsed);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }


}
