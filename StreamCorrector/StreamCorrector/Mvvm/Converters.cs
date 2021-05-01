using Mvvm.ViewModels;
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
    public class BoolToVisibilityMultiConverter : MarkupExtension, IMultiValueConverter {
        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            foreach(object value in values) {
                if((value is bool) && (bool)value)
                    return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
    public class BooleanToHeightConverter : MarkupExtension, IValueConverter {
        public double Height { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var input = (bool)value;
            return input ? new GridLength(Height) : new GridLength(0);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
    public class UpdateStatusToBrushConverter : MarkupExtension, IValueConverter {
        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var updateStatus = (UpdateStatus)value;
            switch(updateStatus) {
                case UpdateStatus.Progress: return new SolidColorBrush(Colors.Black);
                case UpdateStatus.Waiting: return new SolidColorBrush(Colors.Orange);
                case UpdateStatus.Error:return new SolidColorBrush(Colors.LightCoral);
            }
            return new SolidColorBrush(Colors.Green);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
