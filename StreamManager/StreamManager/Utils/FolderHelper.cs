using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StreamManager.Utils {

    public static class FilePathHelper {
        public static string WriteCurrentConfig(int serial) {
            return Path.Combine(FolderHelper.CurrentConfigFolderPath, string.Format("CURRENT_REC_SN{0}.txt", serial));
        }
        public static string WriteChannelsConfig(int serial) {
            return Path.Combine(FolderHelper.ChannelsConfigFolderPath, string.Format("CHANNELS_REC_SN{0}.txt", serial));
        }
        public static string WriteCommonConfig(int serial) {
            return Path.Combine(FolderHelper.CommonConfigFolderPath, string.Format("COMMON_REC_SN{0}.txt", serial));
        }
    }

    public static class FolderHelper {
        public static bool IsReadOnly { get { return isReadOnly; } }
        public static string CurrentFolderPath { get { return currentFolderPath; } }

        public static string CurrentConfigFolderPath { get { return currentConfigFolderPath; } }
        public static string ChannelsConfigFolderPath { get { return channelsConfigFolderPath; } }
        public static string CommonConfigFolderPath { get { return commonConfigFolderPath; } }

        static readonly bool isReadOnly = DetectReadOnly();
        static readonly string currentFolderPath = GetCurrentFolderPath();
        static readonly string currentConfigFolderPath = GetFolderPathOrCreate(Constants.CurrentConfigFolderName);
        static readonly string channelsConfigFolderPath = GetFolderPathOrCreate(Constants.ChannelsConfigFolderName);
        static readonly string commonConfigFolderPath = GetFolderPathOrCreate(Constants.CommonConfigFolderName);


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
