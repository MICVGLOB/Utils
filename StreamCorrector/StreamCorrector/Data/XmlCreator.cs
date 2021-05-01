using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace StreamCorrector.Data {
    public static class XmlCreator {
        static UpdateInfo updateInfo = GenerateUpdateInfo();
        public static UpdateInfo UpdateInfo { get { return updateInfo; } }

        public static void GenerateXmlFile() {
            var testXmlWriter = new XmlSerializer(typeof(UpdateInfo));
            StreamWriter file = new StreamWriter("UpdateInfo.xml");
            testXmlWriter.Serialize(file, UpdateInfo);
            file.Close();
        }
        static UpdateInfo GenerateUpdateInfo() {
            var values = new List<RegInfo>();
            values.Add(new RegInfo() {
                InputValue = 521,
                InputValueMultplier = 100.065,
                InputValuePrecision = 1,
                InputValueMin= 500.4,
                InputValueMax= 550.4,

                IsSensorReg = true,

                AskTheUser = true,

                RegAddress = 45,
                RegDescription = "Диаметр поплавка, мм",
                ModbusSensorAddress = 10,
            });
            values.Add(new RegInfo() {
                InputValue = 134,
                InputValueMultplier = -98.7,
                InputValuePrecision = 0,
                InputValueMin = -124.8,
                InputValueMax = -50.9,

                IsSensorReg = true,
                AskTheUser = false,

                RegAddress = 33,
                RegDescription = "Длина лопасти, мм",
                ModbusSensorAddress = 18,
            });

            return new UpdateInfo() {
                CorrectorHeader = "Корректор для водоканала г.Челябинска",
                CorrectorDescription = "Программа предназначена для коррекции настроек БИТТа №1526. Она устанавливает диаметр трубы 1486 мм",
                ModbusBridgeAddress = 50,
                AskTheBridgeAddress = false,

                BridgeSerial = 1000,

                RegsInfo = values
            };
        }
    }
}
