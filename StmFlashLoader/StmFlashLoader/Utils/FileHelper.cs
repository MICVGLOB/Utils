using Microsoft.Win32;
using StmFlashLoader.LoaderCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StmFlashLoader.Utils {
    public static class FileHelper {
        static string PathFileName { get { return "PathToFlashFile.txt"; } }

        public static string GetFilePath() {
            if(File.Exists(PathFileName))
                using(var reader = new StreamReader(PathFileName, Encoding.Unicode)) {
                    return reader.ReadLine();
                }
            return string.Empty;
        }
        public static string SelectFile() {
            var initialPath = GetFilePath();
            var path = string.Empty;
            var initialDirectory = string.IsNullOrEmpty(initialPath) ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                : Path.GetDirectoryName(initialPath);
            var dialog = new OpenFileDialog() {
                InitialDirectory = initialDirectory, Filter = "Binary files (*.bin)|*.bin",
                FilterIndex = 2, RestoreDirectory = true
            };
            if(dialog.ShowDialog() == true) {
                path = dialog.FileName;
                try {
                    using(var sw = new StreamWriter(PathFileName, false, Encoding.Unicode)) {
                        sw.WriteLine(path);
                    }
                } catch(Exception) { }
            }
            return path;
        }
        public static byte[] GetContent(string fileName, ref bool success) {
            byte[] data = null;
            try {
                using(FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read)) {
                    data = new byte[fs.Length];
                    fs.Read(data, 0, (int)fs.Length);
                }
                success = true;
            } catch(Exception) { }
            return data;
        }
    }
}