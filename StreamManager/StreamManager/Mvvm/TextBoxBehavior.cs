using Mvvm.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Mvvm {
    public class TextBoxBehavior : Behavior<TextBox> {
        public static readonly DependencyProperty ClearCommandProperty =
            DependencyProperty.Register("ClearCommand", typeof(ICommand), typeof(TextBoxBehavior), new PropertyMetadata(null));
        public static readonly DependencyProperty AppendTextCommandProperty =
            DependencyProperty.Register("AppendTextCommand", typeof(ICommand), typeof(TextBoxBehavior), new PropertyMetadata(null));

        public ICommand ClearCommand { get { return (ICommand)GetValue(ClearCommandProperty); } set { SetValue(ClearCommandProperty, value); } }
        public ICommand AppendTextCommand { get { return (ICommand)GetValue(AppendTextCommandProperty); } set { SetValue(AppendTextCommandProperty, value); } }


        public TextBoxBehavior() {
            ClearCommand = new DelegateCommand(() => { if(AssociatedObject != null) AssociatedObject.Clear(); }, () => true);
        }
        protected override void OnAttached() {
            base.OnAttached();
            AppendTextCommand = new DelegateCommand<string>((s) => { if(AssociatedObject != null) AssociatedObject.AppendText(s); }, (s) => true);
        }
        protected override void OnDetaching() {
            base.OnDetaching();
        }
    }
}
