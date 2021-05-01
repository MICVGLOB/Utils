using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using TiltSystemVisualizer.Base;

namespace TiltSystemVisualizer.Controls {
    public class SensorStateToBrushConverter : MarkupExtension, IValueConverter {
        public double Opacity { get; set; }
        public bool Invert { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var state = (SensorState)value;
            var color = Colors.Transparent;
            switch(state) {
                case SensorState.NoSignal:
                    color = Colors.Black;
                    break;
                case SensorState.Unsafe:
                    color = Colors.Red;
                    break;
                case SensorState.Normal:
                    color = Colors.Green;
                    break;
                case SensorState.Notice:
                    color = Colors.Yellow;
                    break;
                default:
                    color = Colors.Gray;
                    break;
            }
            return new SolidColorBrush(color);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
    public class LabelStateToBrushConverter : MarkupExtension, IValueConverter {
        public double Opacity { get; set; }
        public bool Invert { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var state = (LabelState)value;
            var color = Colors.Transparent;
            switch(state) {
                case LabelState.Default:
                    color = Colors.LightGray;
                    break;
                case LabelState.Normal:
                    color = Colors.Green;
                    break;
                case LabelState.Notice:
                    color = Colors.Yellow;
                    break;
                case LabelState.Unsafe:
                    color = Colors.Red;
                    break;
            }
            return new SolidColorBrush(color);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    public class PositionConverter : MarkupExtension, IValueConverter {
        public double Multiplier { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return Multiplier * ((double)value);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    public class ItemsSourceToEnableConverter : MarkupExtension, IValueConverter {
        public double Multiplier { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var collection = (IList)value;
            return (collection != null) && collection.Count > 0;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
	public class SensorSelectorEnableConverter : MarkupExtension, IMultiValueConverter {
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			var collection = values[0] as IList;
			return collection != null && collection.Count > 0 && (bool)values[1];
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class EmptyStringToVisibilityConverter : MarkupExtension, IValueConverter {
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return string.IsNullOrEmpty((string)value) ? Visibility.Visible : Visibility.Collapsed;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}

}
