using EmployeeManagement.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EmployeeManagement.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            OpenEmployeePageCommand = new RelayCommand(o => { OpenEmployeePage(); });
            OpenEmployeeWorkplanPageCommand = new RelayCommand(o => { OpenEmployeeWorkplanPage(); });
        }

        


        #region ICommands

        public ICommand OpenEmployeePageCommand { get; set; }
        public ICommand OpenEmployeeWorkplanPageCommand { get; set; }
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

        #region methods
        private void OpenEmployeeWorkplanPage()
        {
            FramePage = "Views/EmployeePage.xaml";
        }

        private void OpenEmployeePage()
        {
            FramePage = "Views/EmployeePage.xaml";
        }
        #endregion

        #region Private Variables

        private string _framePage = "Views/WelcomePage.xaml";
        #endregion
    }
}
