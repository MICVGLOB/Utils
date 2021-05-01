using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SensorsManager.Utils {

    public static class FilePathHelper {
        public static string Calibration(int serial) {
            return Path.Combine(FolderHelper.CalibrationFolderPath, string.Format("IUG_CAL_SN{0}.txt", serial));
        }
        public static string ReadLevel(int serial) {
            return Path.Combine(FolderHelper.ReadLevelFolderPath, string.Format("LEV_COEFF_SN{0}.txt", serial));
        }
        public static string ReadVelocity(int serial) {
            return Path.Combine(FolderHelper.ReadVelocityFolderPath, string.Format("VELOCITY_COEFF_SN{0}.txt", serial));
        }
        public static string WriteLevel(int serial) {
            return Path.Combine(FolderHelper.WriteLevelFolderPath, string.Format("LEV_REC_SN{0}.txt", serial));
        }
        public static string WriteVelocity(int serial) {
            return Path.Combine(FolderHelper.WriteVelocityFolderPath, string.Format("VELOCITY_REC_SN{0}.txt", serial));
        }
    }

    public static class FolderHelper {
        public static bool IsReadOnly { get { return isReadOnly; } }
        public static string CurrentFolderPath { get { return currentFolderPath; } }

        public static string CalibrationFolderPath { get { return calibrationFolderPath; } }
        public static string ReadLevelFolderPath { get { return readLevelFolderPath; } }
        public static string ReadVelocityFolderPath { get { return readVelocityFolderPath; } }
        public static string WriteLevelFolderPath { get { return writeLevelFolderPath; } }
        public static string WriteVelocityFolderPath { get { return writeVelocityFolderPath; } }

        static readonly bool isReadOnly = DetectReadOnly();
        static readonly string currentFolderPath = GetCurrentFolderPath();
        static readonly string readLevelFolderPath = GetFolderPathOrCreate(Constants.ReadLevelFolderName);
        static readonly string readVelocityFolderPath = GetFolderPathOrCreate(Constants.ReadVelocityFolderName);
        static readonly string writeLevelFolderPath = GetFolderPathOrCreate(Constants.WriteLevelFolderName);
        static readonly string writeVelocityFolderPath = GetFolderPathOrCreate(Constants.WriteVelocityFolderName);
        static readonly string calibrationFolderPath = GetFolderPathOrCreate(Constants.CalibrationFolderName);

        static bool DetectReadOnly() {
            var info = new DirectoryInfo(Directory.GetCurrentDirectory());
            var readOnlyDireatory = info.Attributes.HasFlag(FileAttributes.ReadOnly);
            var testPath = Path.Combine(Directory.GetCurrentDirectory(), "testDirectory");
            bool tryWriteResult = false;
            try {
                Directory.CreateDirectory(testPath);
            } catch(Exception) {
                tryWriteResult = true;
            }
            if(!tryWriteResult)
                Directory.Delete(testPath);
            return tryWriteResult || info.Attributes.HasFlag(FileAttributes.ReadOnly);
        }

        static string GetCurrentFolderPath() {
            return Directory.GetCurrentDirectory();
        }
        static string GetFolderPathOrCreate(string folderName) {
            if(IsReadOnly)
                return string.Empty;
            string folder = Path.Combine(CurrentFolderPath, folderName);
            if(!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            return folder;
        }
        //string path = Path.Combine(currentDirectory, "IUG_CAL_SN351.txt");
    }
}
