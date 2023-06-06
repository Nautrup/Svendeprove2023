using EmployeeManagement.Common;
using EmployeeManagement.Models;
using EmployeeManagement.Services;
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

            ShowMenuDependingOnPermissions();
        }

       

        #region ICommands
        public ICommand OpenEmployeePageCommand { get; set; }
        public ICommand OpenEmployeeWorkplanPageCommand { get; set; }
        public ICommand OpenEmployeeTimeStampCommand  { get; set; }
        #endregion

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
        #region methods

        private void ShowMenuDependingOnPermissions()
        {
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
                ShowEmployeePage = false;
                ShowTimeStampPage = true;
                return;
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

        private void OpenEmployeeEntryReportPage()
        {
            FramePage = "";
        }
        #endregion

        #region Private Variables
        private string _framePage = "Views/WelcomePage.xaml";

        #endregion
    }
}
