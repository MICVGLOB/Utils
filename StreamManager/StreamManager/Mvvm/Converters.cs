using Controls.Core;
using StreamManager.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
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
            return "0x" + ((UInt16)value + Offset).ToString("X2");
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
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

    public class СhannelTypeToBoolConverter : MarkupExtension, IValueConverter {
        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
        public ChannelType ActiveType { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return (ChannelType)value == ActiveType;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            if((bool)value)
                return ActiveType;
            return Binding.DoNothing;
        }
    }
    public class ChannelTypeToMudCoeffNameConverter : MarkupExtension, IValueConverter {
        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
        public int CoeffIndex { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            switch((ChannelType)value) {
                case ChannelType.Rectangle:
                    if(CoeffIndex == 1)
                        return "Ширина, мм";
                    break;
                case ChannelType.Pipe:
                case ChannelType.Flask:
                    if(CoeffIndex == 1)
                        return "Диаметр, мм";
                    break;
                case ChannelType.Trapeze:
                    if(CoeffIndex == 1)
                        return "Меньшее основание, мм";
                    if(CoeffIndex == 2)
                        return "Большее основание, мм";
                    if(CoeffIndex == 3)
                        return "Высота, мм";
                    break;
            }
            return string.Empty;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
    public class ChannelTypeToMudCoeffVisibilityConverter : MarkupExtension, IValueConverter {
        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if((ChannelType)value == ChannelType.Trapeze)
                return Visibility.Visible;
            return Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
    public class ChannelsCountToBoolConverter : MarkupExtension, IValueConverter {
        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
        public int ChannelIndex { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return (int)value >= ChannelIndex;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
    public class ProtectionViewConverter : MarkupExtension, IValueConverter {
        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if(value == null)
                return "?";
            return (bool)value ? "Включена" : "Снята";
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
    public class ChannelNumberToLastRegConverter : MarkupExtension, IValueConverter {
        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return ((int)value) * 2 - 1;
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

    public class IndexToSizeConverter : MarkupExtension, IValueConverter {
        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
        public double DefaultWidth { get; set; }
        public double ExtraWidth { get; set; }
        public int ExtraIndex { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return (int)value == ExtraIndex ? ExtraWidth : DefaultWidth;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
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
