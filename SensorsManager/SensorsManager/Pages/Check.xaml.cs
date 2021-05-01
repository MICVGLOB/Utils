﻿using Mvvm.ViewModels;
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

namespace SensorsManager.Pages {
    public partial class Check : UserControl {
        public Check() {
            InitializeComponent();
        }
        public static readonly DependencyProperty MainViewModelProperty =
            DependencyProperty.Register("MainViewModel", typeof(MainViewModel), typeof(Check), new PropertyMetadata(null, (d, e) => ((Check)d).OnMainViewModelChanged()));

        public MainViewModel MainViewModel { get { return (MainViewModel)GetValue(MainViewModelProperty); } set { SetValue(MainViewModelProperty, value); } }

        void OnMainViewModelChanged() {
            if(DataContext != null)
                ((CheckViewModel)DataContext).MainViewModel = MainViewModel;
        }
    }
}
