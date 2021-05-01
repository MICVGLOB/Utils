using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiltSystemVisualizer.Utils {
	public class AnglesCalculator {
		public List<byte> Data { get; set; }
		double gx;
		double gy;
		double gz;
		double gt;

		public double CalculateDirectionAngle() {
			CalculateNormilizeValues();
			var teta1 = (180.0 / Math.PI) * Math.Acos(gx / gt);
			var teta2 = (180.0 / Math.PI) * Math.Asin(gy / gt);
			if(Math.Abs(gx) > Math.Abs(gy)) {
				if(gx >= 0 && gy >= 0)
					return teta2;
				if(gx >= 0 && gy < 0)
					return 360.0 + teta2;
				return 180.0 - teta2;
			}
			if(gy >= 0)
				return teta1;
			return 360.0 - teta1;
		}
		public double CalculateGravityAngle() {
			CalculateNormilizeValues();
			var angle = BytesToIntValue(Data[9], Data[10]);
			if(angle < 32000)
				return (double)angle / 100.0;
			var delta = (double)angle - 65536.0;
			return delta / 100.0;
		}

		void CalculateNormilizeValues() {
			UInt32 xData = 0;
			UInt32 yData = 0;
			UInt32 zData = 0;
			double xMult = 1.0f / 2147483648.0f;
			double yMult = 1.0f / 2147483648.0f;
			double zMult = 1.0f / 2147483648.0f;

			xData = (UInt32)Data[0] << 24;
			xData += (UInt32)Data[1] << 16;
			xData += (UInt32)Data[2] << 8;

			yData = (UInt32)Data[3] << 24;
			yData += (UInt32)Data[4] << 16;
			yData += (UInt32)Data[5] << 8;

			zData = (UInt32)Data[6] << 24;
			zData += (UInt32)Data[7] << 16;
			zData += (UInt32)Data[8] << 8;

			if(xData > 2147483648) {
				xData = (4294967295 - xData) + 1;
				xMult *= -1;
			}
			if(yData > 2147483648) {
				yData = (4294967295 - yData) + 1;
				yMult *= -1;
			}
			if(zData > 2147483648) {
				zData = (4294967295 - zData) + 1;
				zMult *= -1;
			}

			gx = (double)xData * xMult;
			gy = (double)yData * yMult;
			gz = (double)zData * zMult;
			gt = Math.Sqrt(gx * gx + gy * gy);
		}

		static int BytesToIntValue(byte high, byte low) {
			return (((ushort)high) << 8) + low;
		}

	}

	// NEW VERSION (API Content!)

	// Temperature(11-8 bits) -> Temperature (7-0 bits)
	//			0						1
	// XDATA(19-12 bits) -> XDATA (11-4 bits) -> XDATA (3-0 bits)
	//			2					3					4
	// YDATA(19-12 bits) -> YDATA (11-4 bits) -> YDATA (3-0 bits)
	//			5					6					7
	// ZDATA(19-12 bits) -> ZDATA (11-4 bits) -> ZDATA (3-0 bits)
	//			8					9					10
	// Angle (16-8 bits) -> Angle (7-0 bits) -> State (0x00)
	// 		11                 12					13
	// Резерв(год)			Резерв(месяц)		Резерв (день)
	//		14					15					16
	// Резерв(часы)			Резерв(минуты)		Резерв (секунды)
	//		17					18					19
	// Резерв 				Резерв 				content index (0 - 14)
	// 		20 					21 					22


	// OLD VERSION
	// DEVID_AD(0xAD) -> DEVID_MST(0x1D) -> PARTID(0xED) -> REVID(0x01)
	//      0				1   				2				3
	// Temperature(11-8 bits) -> Temperature (7-0 bits)
	//			4						5
	// XDATA(19-12 bits) -> XDATA (11-4 bits) -> XDATA (3-0 bits)
	//			6					7					8
	// YDATA(19-12 bits) -> YDATA (11-4 bits) -> YDATA (3-0 bits)
	//			9					10					11
	// YDATA(19-12 bits) -> YDATA (11-4 bits) -> YDATA (3-0 bits)
	//			12					13					14
	// Angle (16-8 bits) -> Angle (7-0 bits) -> Reserved (0x00)
	// 		15                 16					17
	// EndOfMessage (0xfa) -> EndOfMessage (0xfa) -> EndOfMessage (0xfa)
	//		18					19						20


}
