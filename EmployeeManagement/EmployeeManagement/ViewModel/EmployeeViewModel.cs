using EmployeeManagement.Common;
using EmployeeManagement.Models;
using EmployeeManagement.Services;
using EmployeeManagement.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EmployeeManagement.ViewModel
{
    public class EmployeeViewModel : ViewModelBase
    {
        public EmployeeViewModel()
        {
            UpdateUserInformationCommand = new RelayCommand(o => UpdateUserInformation(), o => SelectedUser != null);
            CreateNewUserCommand = new RelayCommand(o => CreateNewUser(), o => SelectedUser != null);
            ShowCreateWindowCommand = new RelayCommand(o => ShowCreateWindow());

            LoadUsers();
            LoadCompanies();


            CurrentWeekStartDate = GetStartOfWeek(DateTime.Now);
            LoadTimeEntries();
        }

      


        #region Icommands
        public ICommand UpdateUserInformationCommand { get; set; }
        public ICommand CreateNewUserCommand { get; set; }
        public ICommand ShowCreateWindowCommand { get; set; }
        #endregion

        private DateTime _currentWeekStartDate;

        public DateTime CurrentWeekStartDate {
            get { return _currentWeekStartDate; }
            set 
            {
                _currentWeekStartDate = value;
                GenerateTimeEntries();
                OnPropertyChanged(nameof(CurrentWeekStartDate));
            }
        }


        // Valgte bruger
        public User SelectedUser
		{
			get { return _selectedUser; }
			set 
			{ 
				_selectedUser = value;
                GetUserLocations();
                LoadTimeEntries();
                OnPropertyChanged(nameof(SelectedUser));
			}
		}

        // Valgte firma
        public Company SelecedCompany {
            get { return _selectedCompany; }
            set 
            { 
                _selectedCompany = value; OnPropertyChanged(nameof(SelecedCompany));
            }
        }


        public ObservableCollection<User> UserCollection { get; set; } = new();

        public ObservableCollection<Company> CompanyCollection { get; set; } = new();

        public ObservableCollection<Location> LocationCollection { get; set; } = new();

        public ObservableCollection<TimeEntry> TimeEntriesCollection { get; set; } = new();

        #region Methods

        private void GenerateTimeEntries()
        {
            TimeEntriesCollection = new ObservableCollection<TimeEntry>();

            for (int i = 0; i < 7; i++)
            {
                var date = CurrentWeekStartDate.AddDays(i);
                TimeEntriesCollection.Add(new TimeEntry
                {
                    Date = date,
                    Start = new DateTime() + new TimeSpan(8, 0, 0),
                    End = new DateTime() + new TimeSpan(16, 0, 0)
                });
            }

            
        }

        private DateTime GetStartOfWeek(DateTime date)
        {
            int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            return date.AddDays(-1 * diff).Date;
        }

        // Henter brugere
        private void LoadUsers()
		{
			UserCollection.Clear();
			List<User> users = UserService.GetUsers();

            foreach (var user in users)
			{
				UserCollection.Add(user);
			}
		}

        // Henter firma
        private void LoadCompanies()
        {
            CompanyCollection.Clear();
            List<Company> companiies = UserService.GetCompanies();
            foreach (var company in companiies)
            {
                CompanyCollection.Add(company);
            }
        }

        // Henter de firmaer brugeren er tildelt til
        private void GetUserLocations()
        {
            LocationCollection.Clear();
            foreach (var locaton in SelectedUser.Location)
            {
                LocationCollection.Add(locaton);
            }
        }

        // Viser vores vindue til at oprette medarbejdere
        private void ShowCreateWindow()
        {
            CreateEmployeeView createView = new CreateEmployeeView();
            createView.Show();
        }

        public TimeEntryType test { get; set; } = new TimeEntryType()
        {
            ID = 1,
            Name = "Normal"
        };
        //Vis arbejdstider
        private void LoadTimeEntries()
        {
            TimeEntriesCollection.Clear();

            foreach (var entry in UserService.GetTimeEntries(test).Where(x => x.TimeEntryType.ID == test.ID))
            {
                TimeEntriesCollection.Add(entry);
            }
        }


        private void UpdateUserInformation()
        {
            throw new NotImplementedException();
        }

        private void CreateNewUser()
        {
            User newUser = SelectedUser;

            UserCollection.Add(newUser);
        }
        #endregion

        #region Private Variables
        private User _selectedUser;
        private Company _selectedCompany;
        #endregion
    }
}
