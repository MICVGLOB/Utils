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
using TiltSystemVisualizer.ViewModels;

namespace TiltSystemVisualizer.Pages {
    public partial class Charts : UserControl {
        public Charts() {
            InitializeComponent();
        }
        public static readonly DependencyProperty MainViewModelProperty =
DependencyProperty.Register("MainViewModel", typeof(MainViewModel), typeof(Charts), new PropertyMetadata(null, (d, e) => ((Charts)d).OnMainViewModelChanged()));

        public MainViewModel MainViewModel { get { return (MainViewModel)GetValue(MainViewModelProperty); } set { SetValue(MainViewModelProperty, value); } }

        void OnMainViewModelChanged() {
            if(DataContext != null)
                ((ChartsViewModel)DataContext).MainViewModel = MainViewModel;
        }
    }
}
