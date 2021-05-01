using Mvvm.ViewModels;
using StreamManager.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StreamManager.Reports {
    public partial class ReportFactory {
        public string CreateCurrentConfigProtocol(DateTime dateTime, string operatorName, int serialNumber, int modbusAddress,
            int firmwareVersion, int currentChannel, int pwm4mA, int pwm20mA, long currentScale) {

            var date = string.Format("{0:D2}:{1:D2}:{2}", dateTime.Day, dateTime.Month, dateTime.Year);
            var time = string.Format("{0:D2}:{1:D2}:{2:D2}", dateTime.Hour, dateTime.Minute, dateTime.Second);
            return string.Format(currentConfigProtocolTemplate, date, time, operatorName, serialNumber, modbusAddress,
                Converters.FirmwareVersionToView(firmwareVersion), currentChannel == 6 ? "все потоки" : currentChannel.ToString(), pwm4mA, pwm20mA, currentScale);
        }
        public string CreateChannelsConfigProtocol(DateTime dateTime, string operatorName, int serialNumber, int modbusAddress,
           int firmwareVersion, int channelsCount, ChannelData[] channelsData) {

            var date = string.Format("{0:D2}:{1:D2}:{2}", dateTime.Day, dateTime.Month, dateTime.Year);
            var time = string.Format("{0:D2}:{1:D2}:{2:D2}", dateTime.Hour, dateTime.Minute, dateTime.Second);
            return string.Format(channelsConfigProtocolTemplate, date, time, operatorName, serialNumber, modbusAddress,
                Converters.FirmwareVersionToView(firmwareVersion), channelsCount, GetProfileName(channelsData[0].ProfileType).ToUpper(), GetCoeffs(channelsData[0]),
                (channelsCount > 1) ? GetProfileName(channelsData[1].ProfileType).ToUpper() : "-", (channelsCount > 1) ? GetCoeffs(channelsData[1]) : string.Empty,
                (channelsCount > 2) ? GetProfileName(channelsData[2].ProfileType).ToUpper() : "-", (channelsCount > 2) ? GetCoeffs(channelsData[2]) : string.Empty,
                (channelsCount > 3) ? GetProfileName(channelsData[3].ProfileType).ToUpper() : "-", (channelsCount > 3) ? GetCoeffs(channelsData[3]) : string.Empty,
                (channelsCount == 5) ? GetProfileName(channelsData[4].ProfileType).ToUpper() : "-", (channelsCount == 5) ? GetCoeffs(channelsData[4]) : string.Empty);
        }
        public string CreateCommonConfigProtocol(DateTime dateTime, string operatorName, int serialNumber, int modbusAddress,
            string firmwareVersion, int channelsCount, int[] levelAddresses, int[] velocityAddresses,
            bool allowIndicatorOff, int brightness, int reportInterval, bool allowCartridge, double powerSupplyVoltage,
            bool isProtect, int startDeviceVolume, double velocityOverload, double velocityValueOnOverload) {

            var date = string.Format("{0:D2}:{1:D2}:{2}", dateTime.Day, dateTime.Month, dateTime.Year);
            var time = string.Format("{0:D2}:{1:D2}:{2:D2}", dateTime.Hour, dateTime.Minute, dateTime.Second);

            return string.Format(commonConfigProtocolTemplate, date, time, operatorName, serialNumber, modbusAddress,
                firmwareVersion, channelsCount,
                levelAddresses[0], velocityAddresses[0],
                (channelsCount > 1) ? levelAddresses[1].ToString() : "-", (channelsCount > 1) ? velocityAddresses[1].ToString() : "-",
                (channelsCount > 2) ? levelAddresses[2].ToString() : "-", (channelsCount > 2) ? velocityAddresses[2].ToString() : "-",
                (channelsCount > 3) ? levelAddresses[3].ToString() : "-", (channelsCount > 3) ? velocityAddresses[3].ToString() : "-",
                (channelsCount == 5) ? levelAddresses[4].ToString() : "-", (channelsCount == 5) ? velocityAddresses[4].ToString() : "-",
                allowIndicatorOff ? "разрешено" : "запрещено", brightness, reportInterval, allowCartridge ? "разрешены" : "запрещены",
                powerSupplyVoltage, isProtect ? "включена(пользовательский режим)" : "выключена (конфигурационный режим)",
                startDeviceVolume, velocityOverload, velocityValueOnOverload);
        }
        public void ShowProtocol(string path) {
            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
            info.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;
            info.FileName = path;
            System.Diagnostics.Process.Start(info);
        }

        string GetProfileName(string profileType) {
            return ChannelsConfigInfo.FindInfo(profileType).Name;
        }
        string GetCoeffs(ChannelData channelData, int maxLength = 28) {
            int coeffIndex = 0;
            var channelConfig = ChannelsConfigInfo.FindInfo(channelData.ProfileType);
            StringBuilder sb = new StringBuilder();
            foreach(var coeffInfo in channelConfig.CoeffsInfo) {
                sb.AppendLine(string.Format(channelsConfigCoeffTemplate, (coeffInfo.Header + ":").PadRight(maxLength),
                    channelData.Data[coeffIndex++].ToString(string.Format("F{0}", coeffInfo.Precision)), coeffInfo.Unit));
            }
            return sb.ToString();
        }
    }
}
