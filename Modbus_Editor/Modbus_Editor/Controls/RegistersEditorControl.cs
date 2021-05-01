using Mvvm.Core;
using Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Controls {
    public class RegistersEditorControl : Control {
        #region Registers dependency properties
        public static readonly DependencyProperty Reg1ValueProperty = DependencyProperty.Register("Reg1Value", typeof(UInt16), typeof(RegistersEditorControl), new PropertyMetadata(default(UInt16), (d, e) => ((RegistersEditorControl)d).OnRegValueChanged(d, e, 1)));
        public static readonly DependencyProperty Reg2ValueProperty = DependencyProperty.Register("Reg2Value", typeof(UInt16), typeof(RegistersEditorControl), new PropertyMetadata(default(UInt16), (d, e) => ((RegistersEditorControl)d).OnRegValueChanged(d, e, 2)));
        public static readonly DependencyProperty Reg3ValueProperty = DependencyProperty.Register("Reg3Value", typeof(UInt16), typeof(RegistersEditorControl), new PropertyMetadata(default(UInt16), (d, e) => ((RegistersEditorControl)d).OnRegValueChanged(d, e, 3)));
        public static readonly DependencyProperty Reg4ValueProperty = DependencyProperty.Register("Reg4Value", typeof(UInt16), typeof(RegistersEditorControl), new PropertyMetadata(default(UInt16), (d, e) => ((RegistersEditorControl)d).OnRegValueChanged(d, e, 4)));
        public static readonly DependencyProperty Reg5ValueProperty = DependencyProperty.Register("Reg5Value", typeof(UInt16), typeof(RegistersEditorControl), new PropertyMetadata(default(UInt16), (d, e) => ((RegistersEditorControl)d).OnRegValueChanged(d, e, 5)));
        public static readonly DependencyProperty Reg6ValueProperty = DependencyProperty.Register("Reg6Value", typeof(UInt16), typeof(RegistersEditorControl), new PropertyMetadata(default(UInt16), (d, e) => ((RegistersEditorControl)d).OnRegValueChanged(d, e, 6)));
        public static readonly DependencyProperty Reg7ValueProperty = DependencyProperty.Register("Reg7Value", typeof(UInt16), typeof(RegistersEditorControl), new PropertyMetadata(default(UInt16), (d, e) => ((RegistersEditorControl)d).OnRegValueChanged(d, e, 7)));
        public static readonly DependencyProperty Reg8ValueProperty = DependencyProperty.Register("Reg8Value", typeof(UInt16), typeof(RegistersEditorControl), new PropertyMetadata(default(UInt16), (d, e) => ((RegistersEditorControl)d).OnRegValueChanged(d, e, 8)));
        public static readonly DependencyProperty Reg9ValueProperty = DependencyProperty.Register("Reg9Value", typeof(UInt16), typeof(RegistersEditorControl), new PropertyMetadata(default(UInt16), (d, e) => ((RegistersEditorControl)d).OnRegValueChanged(d, e, 9)));
        public static readonly DependencyProperty Reg10ValueProperty = DependencyProperty.Register("Reg10Value", typeof(UInt16), typeof(RegistersEditorControl), new PropertyMetadata(default(UInt16), (d, e) => ((RegistersEditorControl)d).OnRegValueChanged(d, e, 10)));
        public static readonly DependencyProperty Reg11ValueProperty = DependencyProperty.Register("Reg11Value", typeof(UInt16), typeof(RegistersEditorControl), new PropertyMetadata(default(UInt16), (d, e) => ((RegistersEditorControl)d).OnRegValueChanged(d, e, 11)));
        public static readonly DependencyProperty Reg12ValueProperty = DependencyProperty.Register("Reg12Value", typeof(UInt16), typeof(RegistersEditorControl), new PropertyMetadata(default(UInt16), (d, e) => ((RegistersEditorControl)d).OnRegValueChanged(d, e, 12)));
        public static readonly DependencyProperty Reg13ValueProperty = DependencyProperty.Register("Reg13Value", typeof(UInt16), typeof(RegistersEditorControl), new PropertyMetadata(default(UInt16), (d, e) => ((RegistersEditorControl)d).OnRegValueChanged(d, e, 13)));
        public static readonly DependencyProperty Reg14ValueProperty = DependencyProperty.Register("Reg14Value", typeof(UInt16), typeof(RegistersEditorControl), new PropertyMetadata(default(UInt16), (d, e) => ((RegistersEditorControl)d).OnRegValueChanged(d, e, 14)));
        public static readonly DependencyProperty Reg15ValueProperty = DependencyProperty.Register("Reg15Value", typeof(UInt16), typeof(RegistersEditorControl), new PropertyMetadata(default(UInt16), (d, e) => ((RegistersEditorControl)d).OnRegValueChanged(d, e, 15)));
        public static readonly DependencyProperty Reg16ValueProperty = DependencyProperty.Register("Reg16Value", typeof(UInt16), typeof(RegistersEditorControl), new PropertyMetadata(default(UInt16), (d, e) => ((RegistersEditorControl)d).OnRegValueChanged(d, e, 16)));
        public static readonly DependencyProperty Reg17ValueProperty = DependencyProperty.Register("Reg17Value", typeof(UInt16), typeof(RegistersEditorControl), new PropertyMetadata(default(UInt16), (d, e) => ((RegistersEditorControl)d).OnRegValueChanged(d, e, 17)));
        public static readonly DependencyProperty Reg18ValueProperty = DependencyProperty.Register("Reg18Value", typeof(UInt16), typeof(RegistersEditorControl), new PropertyMetadata(default(UInt16), (d, e) => ((RegistersEditorControl)d).OnRegValueChanged(d, e, 18)));
        public static readonly DependencyProperty Reg19ValueProperty = DependencyProperty.Register("Reg19Value", typeof(UInt16), typeof(RegistersEditorControl), new PropertyMetadata(default(UInt16), (d, e) => ((RegistersEditorControl)d).OnRegValueChanged(d, e, 19)));
        public static readonly DependencyProperty Reg20ValueProperty = DependencyProperty.Register("Reg20Value", typeof(UInt16), typeof(RegistersEditorControl), new PropertyMetadata(default(UInt16), (d, e) => ((RegistersEditorControl)d).OnRegValueChanged(d, e, 20)));

        public static readonly DependencyProperty Reg1ValidProperty = DependencyProperty.Register("Reg1Valid", typeof(bool), typeof(RegistersEditorControl), new PropertyMetadata(true, (d, e) => ((RegistersEditorControl)d).OnRegValidChanged(d, e, 1)));
        public static readonly DependencyProperty Reg2ValidProperty = DependencyProperty.Register("Reg2Valid", typeof(bool), typeof(RegistersEditorControl), new PropertyMetadata(true, (d, e) => ((RegistersEditorControl)d).OnRegValidChanged(d, e, 2)));
        public static readonly DependencyProperty Reg3ValidProperty = DependencyProperty.Register("Reg3Valid", typeof(bool), typeof(RegistersEditorControl), new PropertyMetadata(true, (d, e) => ((RegistersEditorControl)d).OnRegValidChanged(d, e, 3)));
        public static readonly DependencyProperty Reg4ValidProperty = DependencyProperty.Register("Reg4Valid", typeof(bool), typeof(RegistersEditorControl), new PropertyMetadata(true, (d, e) => ((RegistersEditorControl)d).OnRegValidChanged(d, e, 4)));
        public static readonly DependencyProperty Reg5ValidProperty = DependencyProperty.Register("Reg5Valid", typeof(bool), typeof(RegistersEditorControl), new PropertyMetadata(true, (d, e) => ((RegistersEditorControl)d).OnRegValidChanged(d, e, 5)));
        public static readonly DependencyProperty Reg6ValidProperty = DependencyProperty.Register("Reg6Valid", typeof(bool), typeof(RegistersEditorControl), new PropertyMetadata(true, (d, e) => ((RegistersEditorControl)d).OnRegValidChanged(d, e, 6)));
        public static readonly DependencyProperty Reg7ValidProperty = DependencyProperty.Register("Reg7Valid", typeof(bool), typeof(RegistersEditorControl), new PropertyMetadata(true, (d, e) => ((RegistersEditorControl)d).OnRegValidChanged(d, e, 7)));
        public static readonly DependencyProperty Reg8ValidProperty = DependencyProperty.Register("Reg8Valid", typeof(bool), typeof(RegistersEditorControl), new PropertyMetadata(true, (d, e) => ((RegistersEditorControl)d).OnRegValidChanged(d, e, 8)));
        public static readonly DependencyProperty Reg9ValidProperty = DependencyProperty.Register("Reg9Valid", typeof(bool), typeof(RegistersEditorControl), new PropertyMetadata(true, (d, e) => ((RegistersEditorControl)d).OnRegValidChanged(d, e, 9)));
        public static readonly DependencyProperty Reg10ValidProperty = DependencyProperty.Register("Reg10Valid", typeof(bool), typeof(RegistersEditorControl), new PropertyMetadata(true, (d, e) => ((RegistersEditorControl)d).OnRegValidChanged(d, e, 10)));
        public static readonly DependencyProperty Reg11ValidProperty = DependencyProperty.Register("Reg11Valid", typeof(bool), typeof(RegistersEditorControl), new PropertyMetadata(true, (d, e) => ((RegistersEditorControl)d).OnRegValidChanged(d, e, 11)));
        public static readonly DependencyProperty Reg12ValidProperty = DependencyProperty.Register("Reg12Valid", typeof(bool), typeof(RegistersEditorControl), new PropertyMetadata(true, (d, e) => ((RegistersEditorControl)d).OnRegValidChanged(d, e, 12)));
        public static readonly DependencyProperty Reg13ValidProperty = DependencyProperty.Register("Reg13Valid", typeof(bool), typeof(RegistersEditorControl), new PropertyMetadata(true, (d, e) => ((RegistersEditorControl)d).OnRegValidChanged(d, e, 13)));
        public static readonly DependencyProperty Reg14ValidProperty = DependencyProperty.Register("Reg14Valid", typeof(bool), typeof(RegistersEditorControl), new PropertyMetadata(true, (d, e) => ((RegistersEditorControl)d).OnRegValidChanged(d, e, 14)));
        public static readonly DependencyProperty Reg15ValidProperty = DependencyProperty.Register("Reg15Valid", typeof(bool), typeof(RegistersEditorControl), new PropertyMetadata(true, (d, e) => ((RegistersEditorControl)d).OnRegValidChanged(d, e, 15)));
        public static readonly DependencyProperty Reg16ValidProperty = DependencyProperty.Register("Reg16Valid", typeof(bool), typeof(RegistersEditorControl), new PropertyMetadata(true, (d, e) => ((RegistersEditorControl)d).OnRegValidChanged(d, e, 16)));
        public static readonly DependencyProperty Reg17ValidProperty = DependencyProperty.Register("Reg17Valid", typeof(bool), typeof(RegistersEditorControl), new PropertyMetadata(true, (d, e) => ((RegistersEditorControl)d).OnRegValidChanged(d, e, 17)));
        public static readonly DependencyProperty Reg18ValidProperty = DependencyProperty.Register("Reg18Valid", typeof(bool), typeof(RegistersEditorControl), new PropertyMetadata(true, (d, e) => ((RegistersEditorControl)d).OnRegValidChanged(d, e, 18)));
        public static readonly DependencyProperty Reg19ValidProperty = DependencyProperty.Register("Reg19Valid", typeof(bool), typeof(RegistersEditorControl), new PropertyMetadata(true, (d, e) => ((RegistersEditorControl)d).OnRegValidChanged(d, e, 19)));
        public static readonly DependencyProperty Reg20ValidProperty = DependencyProperty.Register("Reg20Valid", typeof(bool), typeof(RegistersEditorControl), new PropertyMetadata(true, (d, e) => ((RegistersEditorControl)d).OnRegValidChanged(d, e, 20)));
        public static readonly DependencyProperty StartRegValidProperty = DependencyProperty.Register("StartRegValid", typeof(bool), typeof(RegistersEditorControl), new PropertyMetadata(true, (d, e) => ((RegistersEditorControl)d).OnRegValidChanged(d, e, 100)));

        public UInt16 Reg1Value { get { return (UInt16)GetValue(Reg1ValueProperty); } set { SetValue(Reg1ValueProperty, value); } }
        public UInt16 Reg2Value { get { return (UInt16)GetValue(Reg2ValueProperty); } set { SetValue(Reg2ValueProperty, value); } }
        public UInt16 Reg3Value { get { return (UInt16)GetValue(Reg3ValueProperty); } set { SetValue(Reg3ValueProperty, value); } }
        public UInt16 Reg4Value { get { return (UInt16)GetValue(Reg4ValueProperty); } set { SetValue(Reg4ValueProperty, value); } }
        public UInt16 Reg5Value { get { return (UInt16)GetValue(Reg5ValueProperty); } set { SetValue(Reg5ValueProperty, value); } }
        public UInt16 Reg6Value { get { return (UInt16)GetValue(Reg6ValueProperty); } set { SetValue(Reg6ValueProperty, value); } }
        public UInt16 Reg7Value { get { return (UInt16)GetValue(Reg7ValueProperty); } set { SetValue(Reg7ValueProperty, value); } }
        public UInt16 Reg8Value { get { return (UInt16)GetValue(Reg8ValueProperty); } set { SetValue(Reg8ValueProperty, value); } }
        public UInt16 Reg9Value { get { return (UInt16)GetValue(Reg9ValueProperty); } set { SetValue(Reg9ValueProperty, value); } }
        public UInt16 Reg10Value { get { return (UInt16)GetValue(Reg10ValueProperty); } set { SetValue(Reg10ValueProperty, value); } }
        public UInt16 Reg11Value { get { return (UInt16)GetValue(Reg11ValueProperty); } set { SetValue(Reg11ValueProperty, value); } }
        public UInt16 Reg12Value { get { return (UInt16)GetValue(Reg12ValueProperty); } set { SetValue(Reg12ValueProperty, value); } }
        public UInt16 Reg13Value { get { return (UInt16)GetValue(Reg13ValueProperty); } set { SetValue(Reg13ValueProperty, value); } }
        public UInt16 Reg14Value { get { return (UInt16)GetValue(Reg14ValueProperty); } set { SetValue(Reg14ValueProperty, value); } }
        public UInt16 Reg15Value { get { return (UInt16)GetValue(Reg15ValueProperty); } set { SetValue(Reg15ValueProperty, value); } }
        public UInt16 Reg16Value { get { return (UInt16)GetValue(Reg16ValueProperty); } set { SetValue(Reg16ValueProperty, value); } }
        public UInt16 Reg17Value { get { return (UInt16)GetValue(Reg17ValueProperty); } set { SetValue(Reg17ValueProperty, value); } }
        public UInt16 Reg18Value { get { return (UInt16)GetValue(Reg18ValueProperty); } set { SetValue(Reg18ValueProperty, value); } }
        public UInt16 Reg19Value { get { return (UInt16)GetValue(Reg19ValueProperty); } set { SetValue(Reg19ValueProperty, value); } }
        public UInt16 Reg20Value { get { return (UInt16)GetValue(Reg20ValueProperty); } set { SetValue(Reg20ValueProperty, value); } }

        public bool Reg1Valid { get { return (bool)GetValue(Reg1ValidProperty); } set { SetValue(Reg1ValidProperty, value); } }
        public bool Reg2Valid { get { return (bool)GetValue(Reg2ValidProperty); } set { SetValue(Reg2ValidProperty, value); } }
        public bool Reg3Valid { get { return (bool)GetValue(Reg3ValidProperty); } set { SetValue(Reg3ValidProperty, value); } }
        public bool Reg4Valid { get { return (bool)GetValue(Reg4ValidProperty); } set { SetValue(Reg4ValidProperty, value); } }
        public bool Reg5Valid { get { return (bool)GetValue(Reg5ValidProperty); } set { SetValue(Reg5ValidProperty, value); } }
        public bool Reg6Valid { get { return (bool)GetValue(Reg6ValidProperty); } set { SetValue(Reg6ValidProperty, value); } }
        public bool Reg7Valid { get { return (bool)GetValue(Reg7ValidProperty); } set { SetValue(Reg7ValidProperty, value); } }
        public bool Reg8Valid { get { return (bool)GetValue(Reg8ValidProperty); } set { SetValue(Reg8ValidProperty, value); } }
        public bool Reg9Valid { get { return (bool)GetValue(Reg9ValidProperty); } set { SetValue(Reg9ValidProperty, value); } }
        public bool Reg10Valid { get { return (bool)GetValue(Reg10ValidProperty); } set { SetValue(Reg10ValidProperty, value); } }
        public bool Reg11Valid { get { return (bool)GetValue(Reg11ValidProperty); } set { SetValue(Reg11ValidProperty, value); } }
        public bool Reg12Valid { get { return (bool)GetValue(Reg12ValidProperty); } set { SetValue(Reg12ValidProperty, value); } }
        public bool Reg13Valid { get { return (bool)GetValue(Reg13ValidProperty); } set { SetValue(Reg13ValidProperty, value); } }
        public bool Reg14Valid { get { return (bool)GetValue(Reg14ValidProperty); } set { SetValue(Reg14ValidProperty, value); } }
        public bool Reg15Valid { get { return (bool)GetValue(Reg15ValidProperty); } set { SetValue(Reg15ValidProperty, value); } }
        public bool Reg16Valid { get { return (bool)GetValue(Reg16ValidProperty); } set { SetValue(Reg16ValidProperty, value); } }
        public bool Reg17Valid { get { return (bool)GetValue(Reg17ValidProperty); } set { SetValue(Reg17ValidProperty, value); } }
        public bool Reg18Valid { get { return (bool)GetValue(Reg18ValidProperty); } set { SetValue(Reg18ValidProperty, value); } }
        public bool Reg19Valid { get { return (bool)GetValue(Reg19ValidProperty); } set { SetValue(Reg19ValidProperty, value); } }
        public bool Reg20Valid { get { return (bool)GetValue(Reg20ValidProperty); } set { SetValue(Reg20ValidProperty, value); } }
        public bool StartRegValid { get { return (bool)GetValue(StartRegValidProperty); } set { SetValue(StartRegValidProperty, value); } }
        #endregion

        #region Public Properties

        public static readonly DependencyProperty IsValidProperty =
            DependencyProperty.Register("IsValid", typeof(bool), typeof(RegistersEditorControl), new PropertyMetadata(false));
        public static readonly DependencyProperty QuantityProperty =
            DependencyProperty.Register("Quantity", typeof(int), typeof(RegistersEditorControl),
                new PropertyMetadata(0, (d, e) => ((RegistersEditorControl)d).OnQuantityChanged(d, e)));
        public static readonly DependencyProperty ValuesProperty =
            DependencyProperty.Register("Values", typeof(List<UInt16>), typeof(RegistersEditorControl),
                new PropertyMetadata(null, (d, e) => ((RegistersEditorControl)d).OnValuesChanged(d, e)));
        public static readonly DependencyProperty RegNumberProperty =
            DependencyProperty.Register("RegNumber", typeof(int), typeof(RegistersEditorControl), new PropertyMetadata(0));

        public bool IsValid { get { return (bool)GetValue(IsValidProperty); } set { SetValue(IsValidProperty, value); } }
        public int Quantity { get { return (int)GetValue(QuantityProperty); } set { SetValue(QuantityProperty, value); } }
        public List<UInt16> Values { get { return (List<UInt16>)GetValue(ValuesProperty); } set { SetValue(ValuesProperty, value); } }
        public int RegNumber { get { return (int)GetValue(RegNumberProperty); } set { SetValue(RegNumberProperty, value); } }

        #endregion

        public static int[] RegisterNumbers = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
        public static RegisterViewMode[] ViewModes = new RegisterViewMode[] { RegisterViewMode.Decimal, RegisterViewMode.Hex, RegisterViewMode.Bin };
        bool[] regsValid;


        public RegistersEditorControl() {
            regsValid = new bool[] { true, true, true, true, true, true, true, true, true, true,
            true, true, true, true, true, true, true, true, true, true};
        }
        void OnRegValidChanged(object d, DependencyPropertyChangedEventArgs e, int index) {
            var newValue = (bool)(e.NewValue);
            if(index == 100) {
                if(!newValue) {
                    IsValid = false;
                    return;
                }
            } else
                regsValid[index - 1] = newValue;

            for(int i = 0; i < Quantity; i++) {
                if(!regsValid[i]) {
                    IsValid = false;
                    return;
                }
            }
            IsValid = true;
        }

        bool IsRegValueEnableChanged = true;
        bool IsValuesEnableChanged = true;

        void OnRegValueChanged(object d, DependencyPropertyChangedEventArgs e, int index) {
            if(!IsRegValueEnableChanged)
                return;
            SetValues();
        }
        void OnQuantityChanged(object d, DependencyPropertyChangedEventArgs e) {
            for(int i = 0; i < Quantity; i++)
                if(!(bool)Helpers.GetPropertyValue(string.Format("Reg{0}Valid", i + 1), this)) {
                    IsValid = false;
                    return;
                }
            IsValid = true;
            SetValues();
        }
        void OnValuesChanged(object d, DependencyPropertyChangedEventArgs e) {
            if(!IsValuesEnableChanged)
                return;
            int count = ((List<UInt16>)(e.NewValue)).Count;
            Quantity = count;
            IsRegValueEnableChanged = false;
            for(int i = 0; i < count; i++)
                Helpers.SetPropertyValue(string.Format("Reg{0}Value", i + 1), Values[i], this);
            IsRegValueEnableChanged = true;
        }
        void SetValues() {
            var values = new List<UInt16>();
            for(int i = 0; i < Quantity; i++)
                values.Add((UInt16)Helpers.GetPropertyValue(string.Format("Reg{0}Value", i + 1), this));
            IsValuesEnableChanged = false;
            Values = values;
            IsValuesEnableChanged = true;
        }
    }
}