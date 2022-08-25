using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TiltsVisualizerLight.Utils {
	public static class ImageHelper {
		public static void SaveVisualAsPng(Control sourceVisual, Stream output) {
			var sourceBitmap = GetImage(sourceVisual);
			var encoder = new PngBitmapEncoder();
			encoder.Frames.Add(BitmapFrame.Create(sourceBitmap));
			encoder.Save(output);
		}
		static RenderTargetBitmap GetImage(Control sourceVisual) {
			var size = new Size(sourceVisual.ActualWidth, sourceVisual.ActualHeight);
			if(size.IsEmpty)
				return null;
			var result = new RenderTargetBitmap((int)size.Width, (int)size.Height, 96, 96, PixelFormats.Pbgra32);

			var drawingvisual = new DrawingVisual();
			using(DrawingContext context = drawingvisual.RenderOpen()) {
				context.DrawRectangle(new VisualBrush(sourceVisual), null, new Rect(new Point(), size));
				context.Close();
			}
			result.Render(drawingvisual);
			return result;
		}
	}
}
