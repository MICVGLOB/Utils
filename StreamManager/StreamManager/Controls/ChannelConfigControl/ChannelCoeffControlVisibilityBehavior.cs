using Mvvm.Core;
using StreamManager.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Controls {
    public class ChannelCoeffControlVisibilityBehavior : Behavior<ChannelCoeffControl> {
        public ChannelCoeffControlVisibilityBehavior() {
            CoeffControlIndicesRedicedModeCount = 6;
        }
        public static readonly DependencyProperty ChannelInfoProperty =
            DependencyProperty.Register("ChannelInfo", typeof(ChannelInfoData), typeof(ChannelCoeffControlVisibilityBehavior),
                new PropertyMetadata(null, (d, e) => ((ChannelCoeffControlVisibilityBehavior)d).OnChannelParameterChanged()));
        public static readonly DependencyProperty ChannelConfigControlViewModeProperty =
            DependencyProperty.Register("ChannelConfigControlViewMode", typeof(ChannelConfigControlViewMode), typeof(ChannelCoeffControlVisibilityBehavior),
        new PropertyMetadata(ChannelConfigControlViewMode.Reduced, (d, e) => ((ChannelCoeffControlVisibilityBehavior)d).OnChannelParameterChanged()));

        public ChannelInfoData ChannelInfo { get { return (ChannelInfoData)GetValue(ChannelInfoProperty); } set { SetValue(ChannelInfoProperty, value); } }
        public ChannelConfigControlViewMode ChannelConfigControlViewMode { get { return (ChannelConfigControlViewMode)GetValue(ChannelConfigControlViewModeProperty); } set { SetValue(ChannelConfigControlViewModeProperty, value); } }

        public int CoeffControlIndex { get; set; }
        public int CoeffControlIndicesRedicedModeCount { get; set; }

        protected override void OnAttached() {
            base.OnAttached();
            OnChannelParameterChanged();
        }
        protected override void OnDetaching() {
            base.OnDetaching();
        }
        void OnChannelParameterChanged() {
            if(AssociatedObject == null)
                return;
            if(ChannelInfo != null && CoeffControlIndex + 1 > Math.Max(CoeffControlIndicesRedicedModeCount, ChannelInfo.CoeffsCount)) {
                AssociatedObject.Visibility = Visibility.Collapsed;
                return;
            }
            if(ChannelInfo == null) {
                AssociatedObject.Visibility = Visibility.Hidden;
                return;
            }
            AssociatedObject.Visibility = (CoeffControlIndex > ChannelInfo.CoeffsCount - 1) ? Visibility.Hidden : Visibility.Visible;
        }
    }
}
