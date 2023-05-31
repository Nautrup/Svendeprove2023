using EmployeeManagement.Common;
using EmployeeManagement.Models;
using EmployeeManagement.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace EmployeeManagement.ViewModel
{
    public class WorkplanViewModel : ViewModelBase
    {
        ExceptionHttpHelper exceptionHttpHelper;

        public WorkplanViewModel() 
        {
            CreateTimeEntryCommand = new RelayCommand(o => CreateEntry(), o => SelectedUser != null );

            GetUsers(CurrentLoggedInUser.ID);
        }

        public ICommand CreateTimeEntryCommand { get; set; }

        public ObservableCollection<User> UserCollection { get; set; } = new();
        public ObservableCollection<TimeEntryType> TimeEntryTypeCollection { get; set; } = new();

        public User SelectedUser {
            get { return _selectedUser; }
            set { _selectedUser = value; OnPropertyChanged(nameof(SelectedUser));
            }
        }


        public DateTime Start {
            get { return _start; }
            set { _start = value; OnPropertyChanged(nameof(Start)); }
        }

        
        public DateTime End {
            get { return _end; }
            set { _end = value; OnPropertyChanged(nameof(End)); }
        }


        public TimeEntryType SelectedTimeEntryType {
            get { return _selectedTimeEntryType; }
            set { _selectedTimeEntryType = value; OnPropertyChanged(nameof(SelectedTimeEntryType)); }
        }


        // Henter alle brugere
        private void GetUsers(int userId)
        {
            try
            {
                using (ApiHelper.Client)
                {
                    string json = ApiHelper.Client.DownloadString($"/user"); // Rigtige /user/{userId}/locations

                    var list = JsonConvert.DeserializeObject<List<User>>(json);
                    // Looper gennem hver bruger, tilføjer lokationer derefter i collection af brugere
                    foreach (var user in list)
                    {
                        //user.Location = GetUserLocations(user.ID);
                        //user.Company = GetUserCompany(user.ID);
                        //user.UserRole = GetUserRole(user.ID);
                        UserCollection.Add(user);
                    }
                }
            }
            catch (WebException ex)
            {
                exceptionHttpHelper = new ExceptionHttpHelper(ex);
                MessageBox.Show($"{exceptionHttpHelper.StatusCode}\n{exceptionHttpHelper.StatusDescription}\n\n{exceptionHttpHelper.ErrorMessage}", "Fejl opstået");
            }
        }

        private void CreateEntry()
        {
            try
            {
                TimeEntry newEntry = new TimeEntry()
                {
                    //User = SelectedUser,
                    CompanyId = SelectedUser.Company.ID,
                    UserId = SelectedUser.ID,
                    //Start = Start,
                    //End = End,
                    Duration = Start.TimeOfDay.Hours - End.TimeOfDay.Hours,
                    //TimeEntryType = SelectedTimeEntryType
                    TimeEntryTypeId = SelectedTimeEntryType.ID,
                    LocationId = 1,
                };

                newEntry.Create();
            }
            catch (WebException ex)
            {
                exceptionHttpHelper = new ExceptionHttpHelper(ex);

                MessageBox.Show($"{(int)exceptionHttpHelper.StatusCode}\n{exceptionHttpHelper.StatusDescription}\n\n{exceptionHttpHelper.ErrorMessage}", "Fejl");
            }
        }

        private void GetTimeEntryTypes()
        {

        }

        #region Private Variables
        private DateTime _end;
        private User _selectedUser;
        private DateTime _start;
        private TimeEntryType _selectedTimeEntryType;
        #endregion
    }
}
