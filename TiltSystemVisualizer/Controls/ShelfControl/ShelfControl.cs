using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using TiltSystemVisualizer.Base;

namespace TiltSystemVisualizer.Controls {
    public class ShelfControl : Control {
        public static readonly DependencyProperty SensorsProperty =
            DependencyProperty.Register("Sensors", typeof(IList<SensorInfo>), typeof(ShelfControl), new PropertyMetadata(null));
        public static readonly DependencyProperty ShelfNameProperty =
            DependencyProperty.Register("ShelfName", typeof(string), typeof(ShelfControl), new PropertyMetadata(string.Empty));


        public IList<SensorInfo> Sensors { get { return (IList<SensorInfo>)GetValue(SensorsProperty); } set { SetValue(SensorsProperty, value); } }
        public string ShelfName { get { return (string)GetValue(ShelfNameProperty); } set { SetValue(ShelfNameProperty, value); } }
    }
}
