using Controls;
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

    public class ChannelConfigControlVisibilityBehavior : Behavior<ChannelConfigControl> {

        public bool CollapseInsteadDisable { get; set; }
        public int ChannelNumber { get; set; }

        public static readonly DependencyProperty ChannelsCountProperty =
            DependencyProperty.Register("ChannelsCount", typeof(int), typeof(ChannelConfigControlVisibilityBehavior),
                new PropertyMetadata(1, (d, e) => ((ChannelConfigControlVisibilityBehavior)d).OnChannelsCountChanged()));

        public int ChannelsCount { get { return (int)GetValue(ChannelsCountProperty); } set { SetValue(ChannelsCountProperty, value); } }


        void OnChannelsCountChanged() {
            if(AssociatedObject == null)
                return;
            if(ChannelNumber <= ChannelsCount) {
                AssociatedObject.Visibility = Visibility.Visible;
                AssociatedObject.IsEnabled = true;
            } else {
                AssociatedObject.Visibility = CollapseInsteadDisable ? Visibility.Collapsed : Visibility.Visible;
                AssociatedObject.IsEnabled = false;
            }
        }

        protected override void OnAttached() {
            base.OnAttached();
            OnChannelsCountChanged();
        }
        protected override void OnDetaching() {
            base.OnDetaching();
        }
    }

}
