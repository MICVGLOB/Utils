using Mvvm.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace Mvvm {
    public class RegisterValidationBehavior : Behavior<TextBox> {
        int index;
        public static readonly DependencyProperty RegistersQuantityProperty =
            DependencyProperty.Register("RegistersQuantity", typeof(int), typeof(RegisterValidationBehavior),
                new PropertyMetadata(0, (d, e) => ((RegisterValidationBehavior)d).OnRegistersQuantityChanged(d, e)));
        public static readonly DependencyProperty ViewModeProperty =
            DependencyProperty.Register("ViewMode", typeof(RegisterViewMode), typeof(RegisterValidationBehavior),
                new PropertyMetadata(RegisterViewMode.Decimal, (d, e) => ((RegisterValidationBehavior)d).OnViewModeChanged(d, e)));
        public static readonly DependencyProperty RegValueProperty =
            DependencyProperty.Register("RegValue", typeof(UInt16), typeof(RegisterValidationBehavior),
                new PropertyMetadata(default(UInt16), (d, e) => ((RegisterValidationBehavior)d).OnRegValueChanged(d, e)));
        public static readonly DependencyProperty IsValidProperty =
            DependencyProperty.Register("IsValid", typeof(bool), typeof(RegisterValidationBehavior), new PropertyMetadata(true));

        public RegisterViewMode ViewMode { get { return (RegisterViewMode)GetValue(ViewModeProperty); } set { SetValue(ViewModeProperty, value); } }
        public int RegistersQuantity { get { return (int)GetValue(RegistersQuantityProperty); } set { SetValue(RegistersQuantityProperty, value); } }
        public UInt16 RegValue { get { return (UInt16)GetValue(RegValueProperty); } set { SetValue(RegValueProperty, value); } }
        public bool IsValid { get { return (bool)GetValue(IsValidProperty); } set { SetValue(IsValidProperty, value); } }


        protected override void OnAttached() {
            base.OnAttached();
            this.AssociatedObject.TextChanged += OnTextChanged;
            this.AssociatedObject.LostFocus += OnLostFocus;
            index = int.Parse(this.AssociatedObject.Name.Replace("Reg_Value", ""));
            OnRegistersQuantityChanged(null, new DependencyPropertyChangedEventArgs());
        }

        void OnLostFocus(object sender, RoutedEventArgs e) {
            OnRegValueChanged(null, new DependencyPropertyChangedEventArgs());
        }
        void OnTextChanged(object sender, TextChangedEventArgs e) {
            VerifyText();
        }

        bool regChangeLock;

        void VerifyText() {
            if(AssociatedObject == null)
                return;
            UInt16 result = 0;
            bool isValid = Converters.ViewModeTryConvertText(AssociatedObject.Text, ViewMode, out result);
            if(AssociatedObject.IsEnabled)
                AssociatedObject.Background = isValid ? new SolidColorBrush(Colors.Transparent)
                    : new SolidColorBrush(Colors.LightCoral);
            else
                AssociatedObject.Background = new SolidColorBrush(Colors.Transparent);
            regChangeLock = true;
            IsValid = isValid;
            if(isValid)
                RegValue = result;
            regChangeLock = false;
        }

        protected override void OnDetaching() {
            this.AssociatedObject.TextChanged -= OnTextChanged;
            this.AssociatedObject.LostFocus -= OnLostFocus;
            base.OnDetaching();
        }
        public void OnViewModeChanged(object d, DependencyPropertyChangedEventArgs e) {
            var oldMode = (RegisterViewMode)(e.OldValue);
            var newMode = (RegisterViewMode)(e.NewValue);
            string result = string.Empty;
            if(Converters.TryConvertTextToText(oldMode, newMode, AssociatedObject.Text, out result))
                AssociatedObject.Text = result;
        }
        public void OnRegistersQuantityChanged(object d, DependencyPropertyChangedEventArgs e) {
            if(AssociatedObject == null)
                return;
            AssociatedObject.IsEnabled = RegistersQuantity >= index;
            if(!AssociatedObject.IsEnabled)
                AssociatedObject.Background = new SolidColorBrush(Colors.Transparent);
            else
                VerifyText();
        }
        public void OnRegValueChanged(object d, DependencyPropertyChangedEventArgs e) {
            if(regChangeLock)
                return;
            AssociatedObject.Text = Converters.Uint16ToText(ViewMode, RegValue);
        }
    }
}
