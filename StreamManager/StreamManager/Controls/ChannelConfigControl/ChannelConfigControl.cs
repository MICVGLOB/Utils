using Mvvm.Core;
using StreamManager.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Controls {
    public partial class ChannelConfigControl : Control {
        #region Private Properties
        static readonly DependencyProperty ChannelsInfoProperty =
            DependencyProperty.Register("ChannelsInfo", typeof(ObservableCollection<ChannelInfoData>), typeof(ChannelConfigControl), new PropertyMetadata(null));
        static readonly DependencyProperty CurrentChannelInfoProperty =
            DependencyProperty.Register("CurrentChannelInfo", typeof(ChannelInfoData), typeof(ChannelConfigControl), new PropertyMetadata(null, (d, e) => ((ChannelConfigControl)d).OnCurrentChannelInfoChanged()));

        ObservableCollection<ChannelInfoData> ChannelsInfo { get { return (ObservableCollection<ChannelInfoData>)GetValue(ChannelsInfoProperty); } set { SetValue(ChannelsInfoProperty, value); } }
        ChannelInfoData CurrentChannelInfo { get { return (ChannelInfoData)GetValue(CurrentChannelInfoProperty); } set { SetValue(CurrentChannelInfoProperty, value); } }
        #endregion

        #region Public Properties
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(ChannelConfigControl), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty ChannelDataProperty =
            DependencyProperty.Register("ChannelData", typeof(ChannelData), typeof(ChannelConfigControl),
                new PropertyMetadata(null, (d, e) => ((ChannelConfigControl)d).OnChannelDataChanged()));

        public string Header { get { return (string)GetValue(HeaderProperty); } set { SetValue(HeaderProperty, value); } }
        public ChannelData ChannelData { get { return (ChannelData)GetValue(ChannelDataProperty); } set { SetValue(ChannelDataProperty, value); } }

        public ChannelConfigControlViewMode ViewMode { get; set; }
        #endregion

        bool allowUpdateChannelData;
        bool allowUpdateCoeffs;

        public override void OnApplyTemplate() {
            base.OnApplyTemplate();
            SetChannelsInfo();
            UpdateChannelData();
            allowUpdateChannelData = true;
            allowUpdateCoeffs = true;
        }
        void OnChannelDataChanged() {
            if(ChannelData == null || !allowUpdateCoeffs)
                return;
            allowUpdateChannelData = false;
            if(UpdateCurrentChannelInfo())
                UpdateCoeffs();
            allowUpdateChannelData = true;
        }
        void OnCurrentChannelInfoChanged() {
            if(CurrentChannelInfo == null || !allowUpdateChannelData)
                return;
            allowUpdateCoeffs = false;
            UpdateCoeffsInfo();
            UpdateChannelData();
            allowUpdateCoeffs = true;
        }
        void OnCoeffValueChanged() {
            if(!allowUpdateChannelData)
                return;
            allowUpdateCoeffs = false;
            UpdateChannelData();
            allowUpdateCoeffs = true;
        }

        bool UpdateCurrentChannelInfo() {
            var channelInfo = ChannelsInfo.FirstOrDefault(x => x.ProfileType == ChannelData.ProfileType);
            if(channelInfo == null)
                return false;
            CurrentChannelInfo = channelInfo;
            return true;
        }
        void UpdateCoeffs() {
            UpdateCoeffsInfo();
            UpdateCoeffsValue();
        }
        void UpdateChannelData() {
            if(CurrentChannelInfo == null)
                return;
            var data = new List<double>();
            for(int i = 0; i < CurrentChannelInfo.CoeffsCount; i++) {
                string propertyName = string.Format("Coeff{0}Value", i);
                data.Add((double)this.GetType().GetProperty(propertyName).GetValue(this, null));
            }
            ChannelData = new ChannelData(CurrentChannelInfo.ProfileType, data);
        }

        void SetChannelsInfo() {
            if(ViewMode == ChannelConfigControlViewMode.Full)
                ChannelsInfo = ChannelsConfigInfo.Source;
            else {
                ChannelsInfo = new ObservableCollection<ChannelInfoData>();
                foreach(var item in ChannelsConfigInfo.Source.Take(6))
                    ChannelsInfo.Add(item);
            }
            CurrentChannelInfo = ChannelsInfo[0];
            UpdateCoeffsInfo();
        }
        void UpdateCoeffsInfo() {
            int index = 0;
            foreach(var coeffInfo in CurrentChannelInfo.CoeffsInfo) {
                string propertyName = string.Format("Coeff{0}Info", index);
                this.GetType().GetProperty(propertyName).SetValue(this, coeffInfo, null);
                index++;
            }
        }
        void UpdateCoeffsValue() {
            int index = 0;
            foreach(var data in ChannelData.Data) {
                string propertyName = string.Format("Coeff{0}Value", index);
                this.GetType().GetProperty(propertyName).SetValue(this, data, null);
                index++;
            }
        }
    }






    public enum ChannelConfigControlViewMode {
        Reduced,
        Full
    }


}
