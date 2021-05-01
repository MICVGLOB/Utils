using Mvvm.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Mvvm {
    public class TabSelectionSuppressBehavior : Behavior<TabControl> {
        public static readonly DependencyProperty IsEnableSelectionProperty =
            DependencyProperty.Register("IsEnableSelection", typeof(bool), typeof(TabSelectionSuppressBehavior),
                new PropertyMetadata(true));

        public bool IsEnableSelection { get { return (bool)GetValue(IsEnableSelectionProperty); } set { SetValue(IsEnableSelectionProperty, value); } }

        protected override void OnAttached() {
            base.OnAttached();
            AssociatedObject.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
        }

        void OnPreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            if(!IsEnableSelection && e.Source is TabItem)
                e.Handled = true;
        }

        protected override void OnDetaching() {
            AssociatedObject.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
            base.OnDetaching();
        }
    }
}
