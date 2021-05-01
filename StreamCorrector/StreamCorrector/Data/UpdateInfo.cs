using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Resources;
using System.Xml.Serialization;

namespace StreamCorrector.Data {
    [XmlRoot("UpdateInfo")]
    public class SerializeUpdateInfo : UpdateInfo {
        static UpdateInfo updateInfo = null;
        public static UpdateInfo Data {
            get {
                if(updateInfo != null)
                    return updateInfo;
                var serializer = new XmlSerializer(typeof(UpdateInfo));
                var resourceInfo = Application.GetResourceStream(new Uri(@"pack://application:,,,/Data/UpdateInfo.xml"));
                using(Stream stream = resourceInfo.Stream) {
                    updateInfo = (UpdateInfo)serializer.Deserialize(stream);
                }
                return updateInfo;
            }
        }
    }

    public class UpdateInfo {
        public UpdateInfo() {
            CorrectorHeader = "";
            CorrectorDescription = "";
            ModbusBridgeAddress = 0;
            AskTheBridgeAddress = false;
            BridgeSerial = null;
            RegsInfo = new List<RegInfo>();
        }

        public string CorrectorHeader { get; set; }
        public string CorrectorDescription { get; set; }

        public int ModbusBridgeAddress { get; set; }
        public bool AskTheBridgeAddress { get; set; }

        public int? BridgeSerial { get; set; }

        public List<RegInfo> RegsInfo { get; set; }
    }

    public class RegInfo {
        public RegInfo() {
            InputValue = 0.0d;
            InputValueMultplier = 1.0d;
            InputValuePrecision = 0;
            InputValueMin = 0.0d;
            InputValueMax = 0.0d;
            IsSensorReg = false;
            AskTheUser = false;
            RegAddress = 0;
            RegDescription = "";
            ModbusSensorAddress = 0;
        }

        public double InputValue { get; set; }
        public double InputValueMultplier { get; set; }
        public int InputValuePrecision { get; set; }
        public double InputValueMin { get; set; }
        public double InputValueMax { get; set; }

        public bool IsSensorReg { get; set; }

        public bool AskTheUser { get; set; }

        public int RegAddress { get; set; }
        public string RegDescription { get; set; }
        public int ModbusSensorAddress { get; set; }
    }
}
