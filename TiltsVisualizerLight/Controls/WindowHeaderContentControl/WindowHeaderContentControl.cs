using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TiltsVisualizerLight.Base;

namespace TiltsVisualizerLight.Controls {
	public class WindowHeaderContentControl : Control {

		static string okStateText = "Связь с модемом установлена";
		static string failStateText = "Нет связи с модемом";
		static Brush okStateBrush = new SolidColorBrush(Colors.Green);
		static Brush failStateBrush = new SolidColorBrush(Colors.Red);

		public static readonly DependencyProperty IsConnectOkProperty =
			DependencyProperty.Register("IsConnectOk", typeof(bool), typeof(WindowHeaderContentControl),
				new PropertyMetadata(false, (d, e) => ((WindowHeaderContentControl)d).OnConnectOkPropertyChanged()));
		public static readonly DependencyProperty StateBrushProperty =
			DependencyProperty.Register("StateBrush", typeof(Brush), typeof(WindowHeaderContentControl), new PropertyMetadata(null));
		public static readonly DependencyProperty StateTextProperty =
			DependencyProperty.Register("StateText", typeof(string), typeof(WindowHeaderContentControl), new PropertyMetadata(string.Empty));

		public bool IsConnectOk { get { return (bool)GetValue(IsConnectOkProperty); } set { SetValue(IsConnectOkProperty, value); } }
		public Brush StateBrush { get { return (Brush)GetValue(StateBrushProperty); } set { SetValue(StateBrushProperty, value); } }
		public string StateText { get { return (string)GetValue(StateTextProperty); } set { SetValue(StateTextProperty, value); } }

		public WindowHeaderContentControl() {
			StateBrush = failStateBrush;
			StateText = failStateText;
		}
		void OnConnectOkPropertyChanged() {
			if(IsConnectOk) {
				StateBrush = okStateBrush;
				StateText = okStateText;
			} else {
				StateBrush = failStateBrush;
				StateText = failStateText;
			}
		}
	}

}
