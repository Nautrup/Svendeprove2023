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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EmployeeManagement
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel ViewModel { get; set; }
        
        public MainWindow(User currentLoggedIn)
        {
            ViewModelBase.CurrentLoggedInUser = currentLoggedIn;
            InitializeComponent();
            ViewModel = new MainViewModel();
            if (ViewModel.CloseWindowAction == null)
                ViewModel.CloseWindowAction = new Action(this.Close);
            this.DataContext = ViewModel;
        }
    }
}
