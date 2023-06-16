using EmployeeManagement.Common;
using EmployeeManagement.Models;
using EmployeeManagement.Services;
using EmployeeManagement.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace EmployeeManagement.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            OpenEmployeePageCommand = new RelayCommand(o => { OpenEmployeePage(); });
            OpenEmployeeWorkplanPageCommand = new RelayCommand(o => { OpenEmployeeWorkplanPage(); });
            OpenEmployeeTimeStampCommand = new RelayCommand(o => OpenEmployeeTimeStamp());
            OpenEmployeeStatisticsPageCommand = new RelayCommand(o => OpenEmployeeStatisticsPage());
            LogOutCommand = new RelayCommand(o => 
            { 
                LoginWindow login = new LoginWindow();
                CloseWindowAction();
                login.ShowDialog(); 
            });
            ShowMenuDependingOnPermissions();
        }

     



        #region ICommands
        public ICommand OpenEmployeePageCommand { get; set; }
        public ICommand OpenEmployeeWorkplanPageCommand { get; set; }
        public ICommand OpenEmployeeTimeStampCommand  { get; set; }
        public ICommand OpenEmployeeStatisticsPageCommand { get; set; }

        public ICommand LogOutCommand { get; set; }
        #endregion

        // Lukker vinduet
        public Action CloseWindowAction { get; set; }

        public string FramePage 
        {
            get { return _framePage; }
            set 
            { 
                _framePage = value;
                OnPropertyChanged(nameof(FramePage)); 
            }
        }

        public bool ShowWorkPlan { get; set; } = false;
        public bool ShowEmployeePage { get; set; } = false;
        public bool ShowTimeStampPage { get; set; } = false;
        public bool ShowStatisticsPage { get; set; } = false;
        #region methods

        // Holder styr på hvilke menuer brugeren skal have vist
        private void ShowMenuDependingOnPermissions()
        {
            ShowStatisticsPage = true;

            // 1 admin
            if (CurrentLoggedInUser.UserRole.PermissionIds.Contains(1))
            {
                ShowWorkPlan = true;
                ShowEmployeePage = true;
                ShowTimeStampPage = true;
                
                return;
            }

            // 2 see own entries
            if (CurrentLoggedInUser.UserRole.PermissionIds.Contains(2))
            {
                ShowWorkPlan = true;
            }

            // 3 Create own entries
            if (CurrentLoggedInUser.UserRole.PermissionIds.Contains(3))
            {
                ShowWorkPlan = true;
            }
        }

        private void OpenEmployeeWorkplanPage()
        {
            FramePage = "Views/WorkplanPage.xaml";
        }

        private void OpenEmployeePage()
        {
            FramePage = "Views/EmployeePage.xaml";
        }

        private void OpenEmployeeTimeStamp()
        {
            FramePage = "Views/EmployeeTimeEntryPage.xaml";
        }

        private void OpenEmployeeStatisticsPage()
        {
            FramePage = "Views/StatisticsPage.xaml";
        }
        #endregion

        #region Private Variables
        private string _framePage = "Views/WelcomePage.xaml";

        #endregion
    }
}
