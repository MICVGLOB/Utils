using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mvvm.Core;

namespace TiltsVisualizerLight.Base {
    public class TiltLabelInfo : ObservableObject {
        public TiltLabelInfo(int labelId, double bottomAngleLimit, double topAngleLimit) {
            this.labelId = labelId;
            this.bottomAngleLimit = bottomAngleLimit;
            this.topAngleLimit = topAngleLimit;
        }
        int labelId;
        double xPosition;
        double yPosition;
        LabelState state;
        double bottomAngleLimit;
        double topAngleLimit;

        public int LabelId { get { return labelId; } }
        public double XPosition { get { return xPosition; } set { SetPropertyValue("XPosition", ref xPosition, value); } }
        public double YPosition { get { return yPosition; } set { SetPropertyValue("YPosition", ref yPosition, value); } }
        public LabelState State { get { return state; } set { SetPropertyValue("State", ref state, value); } }
        public double BottomAngleLimit { get { return bottomAngleLimit; } set { SetPropertyValue("BottomAngleLimit", ref bottomAngleLimit, value); } }
        public double TopAngleLimit { get { return topAngleLimit; } set { SetPropertyValue("TopAngleLimit", ref topAngleLimit, value); } }

    }

    public enum LabelState {
        Default,
        Normal,
        Notice,
        Unsafe
    }
}
