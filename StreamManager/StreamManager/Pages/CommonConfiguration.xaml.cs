using Mvvm.ViewModels;
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

namespace StreamManager.Pages {
    /// <summary>
    /// Interaction logic for CommonConfiguration.xaml
    /// </summary>
    public partial class CommonConfiguration : UserControl {
        public CommonConfiguration() {
            InitializeComponent();
        }
        public static readonly DependencyProperty MainViewModelProperty =
    DependencyProperty.Register("MainViewModel", typeof(MainViewModel), typeof(CommonConfiguration), new PropertyMetadata(null, (d, e) => ((CommonConfiguration)d).OnMainViewModelChanged()));

        public MainViewModel MainViewModel { get { return (MainViewModel)GetValue(MainViewModelProperty); } set { SetValue(MainViewModelProperty, value); } }

        void OnMainViewModelChanged() {
            if(DataContext != null)
                ((CommonConfigurationViewModel)DataContext).MainViewModel = MainViewModel;
        }
    }
}
