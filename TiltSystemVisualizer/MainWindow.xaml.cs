using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TiltSystemVisualizer.Base;
using TiltSystemVisualizer.Utils;
using TiltSystemVisualizer.ViewModels;
using DevExpress.Xpf.Core;
using System.ComponentModel;
using System.IO;

namespace TiltSystemVisualizer {
	public partial class MainWindow : ThemedWindow {
		public MainWindow() {
			InitializeComponent();
		}
		protected override void OnClosing(CancelEventArgs e) {
			var path = FileHelper.GetInstantViewPath();
			using(var fileStream = new FileStream(path, FileMode.Create)) {
				ImageHelper.SaveVisualAsPng(TabControl, fileStream);
			}
			base.OnClosing(e);
		}
	}
}
