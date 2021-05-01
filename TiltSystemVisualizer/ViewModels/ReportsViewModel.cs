using Mvvm.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TiltSystemVisualizer.Base;
using TiltSystemVisualizer.Reports;
using TiltSystemVisualizer.Utils;

namespace TiltSystemVisualizer.ViewModels {
	public class ReportsViewModel : ObservableObject {

		#region Properties
		MainViewModel mainViewModel;
		ReportInfoBase selectedReport;
		ShelfInfo selectedShelf;
		SensorInfo selectedSensor;
		string operatorName;

		bool canSelectSensor;
		bool canSelectReportInterval;

		DateTime? startDate;
		DateTime? endDate;
		string reportText;

		public ReportFactory ReportFactory { get; set; }
		public MainViewModel MainViewModel { get { return mainViewModel; } set { SetPropertyValue("MainViewModel", ref mainViewModel, value, x => OnMainViewModelChanged()); } }
		public ReportInfoBase SelectedReport { get { return selectedReport; } set { SetPropertyValue("SelectedReport", ref selectedReport, value, x => OnSelectedReportChanged()); } }

		public ShelfInfo SelectedShelf { get { return selectedShelf; } set { SetPropertyValue("SelectedShelf", ref selectedShelf, value, x => OnSelectedShelfChanged()); } }
		public SensorInfo SelectedSensor { get { return selectedSensor; } set { SetPropertyValue("SelectedSensor", ref selectedSensor, value); } }
		public string OperatorName { get { return operatorName; } set { SetPropertyValue("OperatorName", ref operatorName, value); } }

		public bool CanSelectSensor { get { return canSelectSensor; } set { SetPropertyValue("CanSelectSensor", ref canSelectSensor, value); } }
		public bool CanSelectReportInterval { get { return canSelectReportInterval; } set { SetPropertyValue("CanSelectReportInterval", ref canSelectReportInterval, value); } }


		public DateTime? StartDate { get { return startDate; } set { SetPropertyValue("StartDate", ref startDate, value); } }
		public DateTime? EndDate { get { return endDate; } set { SetPropertyValue("EndDate", ref endDate, value); } }

		public string ReportText { get { return reportText; } set { SetPropertyValue("ReportText", ref reportText, value); } }

		#endregion

		public ICommand CreateReportCommand { get; private set; }
		public ICommand SaveReportCommand { get; private set; }
		public ICommand ClearReportCommand { get; private set; }

		public ReportsViewModel() {
			ReportFactory = ReportFactory.Create();
			SelectedReport = ReportFactory.ReportTypes.First();			
			CreateReportCommand = new DelegateCommand(CreateReport, () => CanCreateReport());
			SaveReportCommand = new DelegateCommand(SaveReport, () => !string.IsNullOrEmpty(ReportText));
			ClearReportCommand = new DelegateCommand(ClearReport, () => !string.IsNullOrEmpty(ReportText));
		}

		void OnMainViewModelChanged() {
			if(MainViewModel != null) {
				SelectedShelf = MainViewModel.Shelfs.ElementAt(0);
				OperatorName = MainViewModel.OperatorNames.First();
			}
		}
		void OnSelectedShelfChanged() {
			if(SelectedShelf != null)
				SelectedSensor = SelectedShelf.ActiveSensors.FirstOrDefault();
			UpdateCanSelectSensor();
			UpdateCanSelectReportInterval();
		}
		void OnSelectedReportChanged() {
			ReportText = string.Empty;
			UpdateCanSelectSensor();
			UpdateCanSelectReportInterval();
		}

		void UpdateCanSelectSensor() {
			CanSelectSensor = SelectedReport != null && SelectedReport.Id.ToLower().EndsWith("sensor");
		}
		void UpdateCanSelectReportInterval() {
			CanSelectReportInterval = SelectedReport != null && SelectedReport.Id != "statesAll";
		}
		bool CanCreateReport() {
			UpdateCanSelectSensor();
			return !CanSelectSensor || (SelectedShelf != null && SelectedShelf.ActiveSensors != null);
		}

		void CreateReport() {
			var actualEndDate = !EndDate.HasValue ? EndDate : EndDate.Value.AddDays(1);
			var report = SelectedReport.GetReport(OperatorName, MainViewModel.Shelfs.First().Sensors.First(), StartDate, actualEndDate, MainViewModel.Shelfs);
			ReportText = report;
		}
		void SaveReport() {
			var path = FileHelper.SelectFile();
			if(!string.IsNullOrEmpty(path))
				File.WriteAllText(path, ReportText);
		}
		void ClearReport() {
			ReportText = string.Empty;
		}
	}
}
