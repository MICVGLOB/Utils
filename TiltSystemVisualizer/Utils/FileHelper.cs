using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Win32;
using System.Collections;
using Core;
using System.IO;

namespace TiltSystemVisualizer.Utils {
	public static class FileHelper {
		public static string SelectFile() {
			var path = string.Empty;
			var initialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			var dialog = new SaveFileDialog() {
				InitialDirectory = initialDirectory,
				Filter = "*.txt | *.txt | All files(*.*) | *.* ",
				FilterIndex = 0,
				FileName = "Report",
				RestoreDirectory = true
			};
			if(dialog.ShowDialog() == true)
				path = dialog.FileName;
			return path;
		}

		public static string GetInstantReportPath() {
			var directory = Directory.GetCurrentDirectory();
			var time = DateTime.Now;
			var fileName = string.Format("Report_{0}_{1}_{2}_{3}_{4}_{5}.txt", time.Year, time.Month.ToString("D2"), 
				time.Day.ToString("D2"), time.Hour.ToString("D2"), time.Minute.ToString("D2"), time.Second.ToString("D2"));
			return Path.Combine(directory, fileName);
		}

		public static string GetInstantViewPath() {
			var directory = Directory.GetCurrentDirectory();
			var time = DateTime.Now;
			var fileName = string.Format("ApplicationView_{0}_{1}_{2}_{3}_{4}_{5}.png", time.Year, time.Month.ToString("D2"),
				time.Day.ToString("D2"), time.Hour.ToString("D2"), time.Minute.ToString("D2"), time.Second.ToString("D2"));
			return Path.Combine(directory, fileName);
		}

		public static SerialConfig GetSerialPortConfig() {
			var config = new SerialConfig();
			var document = new XmlDocument();
			document.Load("Config.xml");
			var root = document.DocumentElement;
			foreach(XmlNode childNode in root.ChildNodes) {
				if(childNode.Name == "SerialPort") {
					foreach(XmlNode node in childNode) {
						switch(node.Name) {
							case "Name":
								config.Name = node.InnerText;
								break;
							case "BaudRate":
								config.Baudrate = int.Parse(node.InnerText);
								break;
							case "StopBits":
								config.StopBits = (System.IO.Ports.StopBits)Enum.Parse(typeof(System.IO.Ports.StopBits), node.InnerText);
								break;
							case "Parity":
								config.Parity = (System.IO.Ports.Parity)Enum.Parse(typeof(System.IO.Ports.Parity), node.InnerText); ;
								break;
						}
					}
				}
			}
			return config;
		}
	}
}
