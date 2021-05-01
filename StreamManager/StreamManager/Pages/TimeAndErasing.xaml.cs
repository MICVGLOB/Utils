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
    public partial class TimeAndErasing : UserControl {
        public TimeAndErasing() {
            InitializeComponent();
        }
        public static readonly DependencyProperty MainViewModelProperty =
DependencyProperty.Register("MainViewModel", typeof(MainViewModel), typeof(TimeAndErasing), new PropertyMetadata(null, (d, e) => ((TimeAndErasing)d).OnMainViewModelChanged()));

        public MainViewModel MainViewModel { get { return (MainViewModel)GetValue(MainViewModelProperty); } set { SetValue(MainViewModelProperty, value); } }

        void OnMainViewModelChanged() {
            if(DataContext != null)
                ((TimeAndErasingViewModel)DataContext).MainViewModel = MainViewModel;
        }
    }
}
