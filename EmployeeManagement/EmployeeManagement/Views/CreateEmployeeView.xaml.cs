﻿using EmployeeManagement.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EmployeeManagement.Views
{
    /// <summary>
    /// Interaction logic for CreateEmployeeView.xaml
    /// </summary>
    public partial class CreateEmployeeView : Window
    {
        public CreateEmployeeViewModel ViewModel { get; set; }

        public CreateEmployeeView()
        {
            InitializeComponent();

            ViewModel = new CreateEmployeeViewModel();
            if (ViewModel.CloseWindowAction == null)
                ViewModel.CloseWindowAction = new Action(this.Close);
            
            DataContext = ViewModel;
        }
    }
}
