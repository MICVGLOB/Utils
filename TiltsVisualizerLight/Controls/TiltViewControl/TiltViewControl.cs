using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TiltsVisualizerLight.Base;

namespace TiltsVisualizerLight.Controls {
    public class TiltViewControl : Control {

        static int LabelsCount = 12;
        static double labelsRadius = 40;
        static Size DefaultCanvasSize = new Size(100, 100);

        #region Public properties
        public static readonly DependencyProperty TiltProperty =
            DependencyProperty.Register("Tilt", typeof(TiltInfo), typeof(TiltViewControl), new PropertyMetadata(null, (d, e) => ((TiltViewControl)d).OnTiltChanged()));
        public static readonly DependencyProperty NoticeRangeProperty =
            DependencyProperty.Register("NoticeRange", typeof(double), typeof(TiltViewControl), new PropertyMetadata(1.0, (d, e) => ((TiltViewControl)d).OnTiltChanged()));
        public static readonly DependencyProperty UnsafeRangeProperty =
            DependencyProperty.Register("UnsafeRange", typeof(double), typeof(TiltViewControl), new PropertyMetadata(2.0, (d, e) => ((TiltViewControl)d).OnTiltChanged()));
        #endregion

        public static readonly DependencyProperty TiltLabelsProperty =
            DependencyProperty.Register("TiltLabels", typeof(IList<TiltLabelInfo>), typeof(TiltViewControl), new PropertyMetadata(null));
        public static readonly DependencyProperty LabelSizeProperty =
            DependencyProperty.Register("LabelSize", typeof(double), typeof(TiltViewControl), new PropertyMetadata(10.0));
        public static readonly DependencyProperty CanvasSizeProperty =
            DependencyProperty.Register("CanvasSize", typeof(Size), typeof(TiltViewControl), new PropertyMetadata(default(Size)));
        public static readonly DependencyProperty ReferenceEllipsePositionProperty =
            DependencyProperty.Register("ReferenceEllipsePosition", typeof(Point), typeof(TiltViewControl), new PropertyMetadata(default(Point)));
        public static readonly DependencyProperty ReferenceEllipseSizeProperty =
            DependencyProperty.Register("ReferenceEllipseSize", typeof(double), typeof(TiltViewControl), new PropertyMetadata(0.0));
        public static readonly DependencyProperty ArrowPositionProperty =
            DependencyProperty.Register("ArrowPosition", typeof(Point), typeof(TiltViewControl), new PropertyMetadata(default(Point)));
        public static readonly DependencyProperty IsEnabledLocalProperty =
            DependencyProperty.Register("IsEnabledLocal", typeof(bool), typeof(TiltViewControl), new PropertyMetadata(true, (d, e) => ((TiltViewControl)d).OnIsEnabledLocalChanged()));

        public TiltInfo Tilt { get { return (TiltInfo)GetValue(TiltProperty); } set { SetValue(TiltProperty, value); } }
        public double NoticeRange { get { return (double)GetValue(NoticeRangeProperty); } set { SetValue(NoticeRangeProperty, value); } }
        public double UnsafeRange { get { return (double)GetValue(UnsafeRangeProperty); } set { SetValue(UnsafeRangeProperty, value); } }

        public IList<TiltLabelInfo> TiltLabels { get { return (IList<TiltLabelInfo>)GetValue(TiltLabelsProperty); } set { SetValue(TiltLabelsProperty, value); } }
        public double LabelSize { get { return (double)GetValue(LabelSizeProperty); } set { SetValue(LabelSizeProperty, value); } }
        public Size CanvasSize { get { return (Size)GetValue(CanvasSizeProperty); } set { SetValue(CanvasSizeProperty, value); } }
        public Point ReferenceEllipsePosition { get { return (Point)GetValue(ReferenceEllipsePositionProperty); } set { SetValue(ReferenceEllipsePositionProperty, value); } }
        public double ReferenceEllipseSize { get { return (double)GetValue(ReferenceEllipseSizeProperty); } set { SetValue(ReferenceEllipseSizeProperty, value); } }
        public Point ArrowPosition { get { return (Point)GetValue(ArrowPositionProperty); } set { SetValue(ArrowPositionProperty, value); } }
        public bool IsEnabledLocal { get { return (bool)GetValue(IsEnabledLocalProperty); } set { SetValue(IsEnabledLocalProperty, value); } }

        public TiltViewControl() {
            TiltLabels = new ObservableCollection<TiltLabelInfo>();
            Enumerable.Range(0, LabelsCount).ToList().ForEach(i => {
                double bottomLimit = -15;
                double topLimit = 15;
                if(i != 0) {
                    bottomLimit = 30 * i - 15;
                    topLimit = 30 * i + 15;
                }
                TiltLabels.Add(new TiltLabelInfo(i, bottomLimit, topLimit));
            });
            CanvasSize = DefaultCanvasSize;
            ArrowPosition = GetDefaultArrowPosition();
            UpdateBindings();
        }

        void UpdateBindings() {
            BindingOperations.SetBinding(this, IsEnabledLocalProperty, new Binding("IsEnabled") { RelativeSource = RelativeSource.Self });
        }

        void OnIsEnabledLocalChanged() {
            if(IsEnabledLocal) {
                OnTiltChanged();
                return;
            }
            ArrowPosition = GetDefaultArrowPosition();
            SetDefaultLabelsState();
        }

        void OnTiltChanged() {
            if(TiltLabels == null || Tilt == null)
                return;
            UpdateArrowPosition();
            UpdateLabelsState();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo) {
            base.OnRenderSizeChanged(sizeInfo);
            UpdateLabelPositions();
        }

        void UpdateArrowPosition() {
            var arrowPosition = GetDefaultArrowPosition();
            double positionX = arrowPosition.X;
            double positionY = arrowPosition.Y;

            if(Tilt.Direction >= 0 && Tilt.Direction <= 90.0) {
                positionX += labelsRadius * Math.Cos(Math.PI * Tilt.Direction / 180.0);
                positionY -= labelsRadius * Math.Sin(Math.PI * Tilt.Direction / 180.0);

            } else if(Tilt.Direction > 90 && Tilt.Direction <= 180.0) {
                positionX -= labelsRadius * Math.Cos(Math.PI * (180.0 - Tilt.Direction) / 180.0);
                positionY -= labelsRadius * Math.Sin(Math.PI * (180.0 - Tilt.Direction) / 180.0);

            } else if(Tilt.Direction > 180 && Tilt.Direction <= 270.0) {
                positionX -= labelsRadius * Math.Cos(Math.PI * (Tilt.Direction - 180.0) / 180.0);
                positionY += labelsRadius * Math.Sin(Math.PI * (Tilt.Direction - 180.0) / 180.0);
            } else {
                positionX += labelsRadius * Math.Cos(Math.PI * (360.0 - Tilt.Direction) / 180.0);
                positionY += labelsRadius * Math.Sin(Math.PI * (360.0 - Tilt.Direction) / 180.0);
            }
            ArrowPosition = new Point(positionX, positionY);
        }

        Point GetDefaultArrowPosition() {
            return new Point(CanvasSize.Width / 2, CanvasSize.Height / 2);
        }

        void UpdateLabelsState() {
            foreach(var label in TiltLabels) {
                var actualDirection = Tilt.Direction > 345.0 ? (360.0 - Tilt.Direction) : Tilt.Direction;
                if(actualDirection >= label.BottomAngleLimit && actualDirection < label.TopAngleLimit) {
                    var actualGravity = Math.Abs(Tilt.Gravity);
                    if(actualGravity < NoticeRange)
                        label.State = LabelState.Normal;
                    else if(actualGravity >= NoticeRange && actualGravity < UnsafeRange)
                        label.State = LabelState.Notice;
                    else
                        label.State = LabelState.Unsafe;
                } else
                    label.State = LabelState.Default;
            }
        }

        void UpdateLabelPositions() {
            if(TiltLabels == null)
                return;
            foreach(var label in TiltLabels) {
                label.XPosition = GetLabelXPosition(label.LabelId);
                label.YPosition = GetLabelYPosition(label.LabelId);
            }
            ReferenceEllipsePosition = new Point(CanvasSize.Width / 2 - labelsRadius, CanvasSize.Height / 2 - labelsRadius);
            ReferenceEllipseSize = 2 * labelsRadius;
        }

        double GetLabelXPosition(int id) {
            double offsetX = CanvasSize.Width / 2;
            double offsetLabelSize = LabelSize / 2;
            double offset = offsetX - offsetLabelSize;

            switch(id) {
                case 0: return offset + labelsRadius;
                case 1: case 11: return offset + labelsRadius * Math.Cos(Math.PI * 30.0 / 180.0);
                case 2: case 10: return offset + labelsRadius * Math.Cos(Math.PI * 60.0 / 180.0);
                case 3: case 9: return offset;
                case 4: case 8: return offset - labelsRadius * Math.Cos(Math.PI * 60.0 / 180.0);
                case 5: case 7: return offset - labelsRadius * Math.Cos(Math.PI * 30.0 / 180.0);
                case 6: return offset - labelsRadius;
            }

            return 0;
        }
        double GetLabelYPosition(int id) {
            double offsetY = CanvasSize.Height / 2;
            double offsetLabelSize = LabelSize / 2;
            double offset = offsetY - offsetLabelSize;

            switch(id) {
                case 0: case 6: return offset;
                case 1: case 5: return offset - labelsRadius * Math.Sin(Math.PI * 30.0 / 180.0);
                case 2: case 4: return offset - labelsRadius * Math.Sin(Math.PI * 60.0 / 180.0);
                case 3: return offset - labelsRadius;
                case 7: case 11: return offset + labelsRadius * Math.Sin(Math.PI * 30.0 / 180.0);
                case 8: case 10: return offset + labelsRadius * Math.Sin(Math.PI * 60.0 / 180.0);
                case 9: return offset + labelsRadius;
            }
            return 0;
        }
        void SetDefaultLabelsState() {
            foreach(var label in TiltLabels)
                label.State = LabelState.Default;            
        }
    }
}
