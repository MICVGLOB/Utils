using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Mvvm.Core {

    public class NumericValidationBehavior : Behavior<TextBox> {
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(UInt16), typeof(NumericValidationBehavior),
                new PropertyMetadata(default(UInt16), (d, e) => ((NumericValidationBehavior)d).OnValueChanged(d, e)));
        public static readonly DependencyProperty IsValidProperty =
            DependencyProperty.Register("IsValid", typeof(bool), typeof(NumericValidationBehavior), new PropertyMetadata(true));
        public static readonly DependencyProperty VerifiedValueProperty =
            DependencyProperty.Register("VerifiedValue", typeof(VerifiedValue<UInt16>), typeof(NumericValidationBehavior),
                new PropertyMetadata(null));

        public UInt16 Value { get { return (UInt16)GetValue(ValueProperty); } set { SetValue(ValueProperty, value); } }
        public bool IsValid { get { return (bool)GetValue(IsValidProperty); } set { SetValue(IsValidProperty, value); } }
        public VerifiedValue<UInt16> VerifiedValue { get { return (VerifiedValue<UInt16>)GetValue(VerifiedValueProperty); } set { SetValue(VerifiedValueProperty, value); } }

        public UInt16? MinValue { get; set; }
        public UInt16? MaxValue { get; set; }
        public UInt16? DefaultValue { get; set; }

        bool regChangeLock;

        public NumericValidationBehavior() {
        }
        protected override void OnAttached() {
            base.OnAttached();
            this.AssociatedObject.TextChanged += OnTextChanged;
            this.AssociatedObject.LostFocus += OnLostFocus;
            Value = MinValue == null ? (UInt16)0 : MinValue.Value;
            if(DefaultValue != null)
                this.AssociatedObject.Text = DefaultValue.ToString();
            OnValueChanged(null, new DependencyPropertyChangedEventArgs());
        }
        void OnLostFocus(object sender, RoutedEventArgs e) {
            OnValueChanged(null, new DependencyPropertyChangedEventArgs());
        }
        void OnTextChanged(object sender, TextChangedEventArgs e) {
            VerifyText();
        }
        void VerifyText() {
            if(AssociatedObject == null)
                return;
            UInt16 result = 0;
            bool isValid = Converters.ViewModeTryConvertText(AssociatedObject.Text, RegisterViewMode.Decimal, out result);
            if(isValid && (result < MinValue || result > MaxValue))
                isValid = false;
            AssociatedObject.Background = isValid ? new SolidColorBrush(Colors.Transparent)
                : new SolidColorBrush(Colors.LightCoral);
            regChangeLock = true;
            IsValid = isValid;
            if(isValid)
                Value = result;
            VerifiedValue = new VerifiedValue<UInt16>(isValid ? Value : default(UInt16), isValid);
            regChangeLock = false;
        }

        protected override void OnDetaching() {
            this.AssociatedObject.TextChanged -= OnTextChanged;
            this.AssociatedObject.LostFocus -= OnLostFocus;
            base.OnDetaching();
        }

        public void OnValueChanged(object d, DependencyPropertyChangedEventArgs e) {
            if(regChangeLock || AssociatedObject == null)
                return;
            AssociatedObject.Text = Converters.Uint16ToText(RegisterViewMode.Decimal, Value);
        }
    }

    public class VerifiedValue<T> where T : struct {
        public VerifiedValue(T value, bool isValid) {
            Value = value;
            IsValid = isValid;
        }
        public T Value { get; private set; }
        public bool IsValid { get; private set; }
    }

    public abstract class ValidationBehaviorBase<T> : Behavior<TextBox> where T : struct {
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(T), typeof(ValidationBehaviorBase<T>),
                new PropertyMetadata(default(T), (d, e) => ((ValidationBehaviorBase<T>)d).OnValueChanged(d, e)));
        public static readonly DependencyProperty IsValidProperty =
            DependencyProperty.Register("IsValid", typeof(bool), typeof(ValidationBehaviorBase<T>), new PropertyMetadata(true));
        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(T?), typeof(ValidationBehaviorBase<T>), new PropertyMetadata(null));
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(T?), typeof(ValidationBehaviorBase<T>), new PropertyMetadata(null));
        public static readonly DependencyProperty DefaultValueProperty =
            DependencyProperty.Register("DefaultValue", typeof(T?), typeof(ValidationBehaviorBase<T>), new PropertyMetadata(null));

        public T Value { get { return (T)GetValue(ValueProperty); } set { SetValue(ValueProperty, value); } }
        public bool IsValid { get { return (bool)GetValue(IsValidProperty); } set { SetValue(IsValidProperty, value); } }
        public T? MinValue { get { return (T?)GetValue(MinValueProperty); } set { SetValue(MinValueProperty, value); } }
        public T? MaxValue { get { return (T?)GetValue(MaxValueProperty); } set { SetValue(MaxValueProperty, value); } }
        public T? DefaultValue { get { return (T?)GetValue(DefaultValueProperty); } set { SetValue(DefaultValueProperty, value); } }

        bool regChangeLock;

        protected override void OnAttached() {
            base.OnAttached();
            this.AssociatedObject.TextChanged += OnTextChanged;
            this.AssociatedObject.LostFocus += OnLostFocus;
            this.AssociatedObject.IsVisibleChanged += OnIsVisibleChanged;
            Value = MinValue == null ? default(T) : MinValue.Value;
            if(DefaultValue != null)
                this.AssociatedObject.Text = ConvertValueToText(true);
            OnValueChanged(null, new DependencyPropertyChangedEventArgs());
        }

        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
            OnValueChanged(null, new DependencyPropertyChangedEventArgs());
        }

        void OnLostFocus(object sender, RoutedEventArgs e) {
            OnValueChanged(null, new DependencyPropertyChangedEventArgs());
        }
        void OnTextChanged(object sender, TextChangedEventArgs e) {
            VerifyText();
        }
        void VerifyText() {
            if(AssociatedObject == null)
                return;
            T result = default(T);
            bool isValid = ConvertTextToValue(out result);
            AssociatedObject.Background = isValid ? new SolidColorBrush(Colors.Transparent)
                : new SolidColorBrush(Colors.LightCoral);
            regChangeLock = true;
            IsValid = isValid;
            if(isValid)
                Value = result;
            regChangeLock = false;
        }

        protected abstract bool ConvertTextToValue(out T result);
        protected abstract string ConvertValueToText(bool isDefaultValue = false);

        protected override void OnDetaching() {
            this.AssociatedObject.TextChanged -= OnTextChanged;
            this.AssociatedObject.LostFocus -= OnLostFocus;
            this.AssociatedObject.IsVisibleChanged -= OnIsVisibleChanged;
            base.OnDetaching();
        }
        public void OnValueChanged(object d, DependencyPropertyChangedEventArgs e) {
            if(regChangeLock || AssociatedObject == null)
                return;
            AssociatedObject.Text = ConvertValueToText();
        }
    }

    public class DoubleValidationBehvior : ValidationBehaviorBase<double> {
        public static readonly DependencyProperty PrecisionProperty =
            DependencyProperty.Register("Precision", typeof(int), typeof(DoubleValidationBehvior), new PropertyMetadata(1));

        public int Precision { get { return (int)GetValue(PrecisionProperty); } set { SetValue(PrecisionProperty, value); } }

        protected override bool ConvertTextToValue(out double result) {
            double convertedResult = 0;
            try {
                convertedResult = Convert.ToDouble(AssociatedObject.Text);
            } catch {
                result = 0;
                return false;
            }
            result = convertedResult;
            if(convertedResult < MinValue.Value || convertedResult > MaxValue.Value)
                return false;
            return true;
        }
        protected override string ConvertValueToText(bool isDefaultValue = false) {
            return !isDefaultValue ? Value.ToString(string.Format("F{0}", Precision)) :
                DefaultValue.Value.ToString(string.Format("F{0}", Precision));
        }
    }

    public class IntegerValidationBehvior : ValidationBehaviorBase<int> {
        protected override bool ConvertTextToValue(out int result) {
            int convertedResult = 0;
            try {
                convertedResult = Convert.ToInt32(AssociatedObject.Text);
            } catch {
                result = 0;
                return false;
            }
            result = convertedResult;
            if(convertedResult < MinValue.Value || convertedResult > MaxValue.Value)
                return false;
            return true;
        }
        protected override string ConvertValueToText(bool isDefaultValue = false) {
            return !isDefaultValue ? Value.ToString() : DefaultValue.Value.ToString();
        }
    }
}
