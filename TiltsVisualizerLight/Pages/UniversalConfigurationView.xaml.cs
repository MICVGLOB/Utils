using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TiltsVisualizerLight.ViewModels;

namespace TiltsVisualizerLight.Pages {
    public partial class UniversalConfigurationView : UserControl {
        public UniversalConfigurationView() {
            InitializeComponent();
        }
        public static readonly DependencyProperty MainViewModelProperty =
            DependencyProperty.Register("MainViewModel", typeof(MainViewModel), typeof(UniversalConfigurationView), new PropertyMetadata(null, (d, e) => ((UniversalConfigurationView)d).OnMainViewModelChanged()));

        public MainViewModel MainViewModel { get { return (MainViewModel)GetValue(MainViewModelProperty); } set { SetValue(MainViewModelProperty, value); } }

        void OnMainViewModelChanged() {
            if(DataContext != null)
                ((UniversalConfigurationViewModel)DataContext).MainViewModel = MainViewModel;
        }
    }
}
