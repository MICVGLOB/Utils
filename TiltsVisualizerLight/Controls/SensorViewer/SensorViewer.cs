using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TiltsVisualizerLight.Base;

namespace TiltsVisualizerLight.Controls {
    public class SensorViewer : Control {
        //public static readonly DependencyProperty SensorBrushProperty =
        //    DependencyProperty.Register("SensorBrush", typeof(Brush), typeof(SensorViewer), new PropertyMetadata(null));
        public static readonly DependencyProperty InfoProperty =
            DependencyProperty.Register("Info", typeof(SensorInfo), typeof(SensorViewer), new PropertyMetadata(null));


        //public Brush SensorBrush { get { return (Brush)GetValue(SensorBrushProperty); } set { SetValue(SensorBrushProperty, value); } }
        public SensorInfo Info { get { return (SensorInfo)GetValue(InfoProperty); } set { SetValue(InfoProperty, value); } }
    }

}
