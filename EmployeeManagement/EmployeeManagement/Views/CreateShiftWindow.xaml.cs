using EmployeeManagement.Models;
using EmployeeManagement.ViewModel;
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
    /// Interaction logic for CreateShiftWindow.xaml
    /// </summary>
    public partial class CreateShiftWindow : Window
    {
        public WorkplanViewModel ViewModel { get; set; }
        public CreateShiftWindow()
        {
            InitializeComponent();
            ViewModel = new WorkplanViewModel();
            if (ViewModel.CloseWindowAction == null)
                ViewModel.CloseWindowAction = new Action(this.Close);

            DataContext = ViewModel;
        }
    }
}
