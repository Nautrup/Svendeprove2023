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
        public EmployeeViewModel EmployeeViewModel { get; set; }
        public WorkplanViewModel() 
        {
            EmployeeViewModel = new EmployeeViewModel();
            EmployeeViewModel.SelectedUser = CurrentLoggedInUser;
            CreateTimeEntryCommand = new RelayCommand(o => CreateEntry(), o => SelectedUser != null );

            if (CurrentLoggedInUser.Locations.Count != 0)
            {
                GetUsers(CurrentLoggedInUser.ID);
            }

            if (CurrentLoggedInUser.UserRole.PermissionIds.Contains(2))
            {
                ShowCreateShiftButton = false;
            }
        }

        public bool ShowCreateShiftButton { get; set; } = false;

        public ICommand CreateTimeEntryCommand { get; set; }

        public ObservableCollection<User> UserCollection { get; set; } = new();
        public ObservableCollection<TimeEntryType> TimeEntryTypeCollection { get; set; } = new();
        public ObservableCollection<TimeEntry> TimeEntryCollections { get; set; } = new();
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

        public Action CloseWindowAction { get; set; }

        public TimeEntryType SelectedTimeEntryType {
            get { return _selectedTimeEntryType; }
            set { _selectedTimeEntryType = value; OnPropertyChanged(nameof(SelectedTimeEntryType)); }
        }

        // Henter nuværende logged inds brugers vagter
        private void GetUser(int userId)
        {
            try
            {
                TimeEntryCollections.Clear();

                foreach (var item in Services.UserService.GetSpecifikUserInformation(userId))
                {
                    TimeEntryCollections.Add(item);
                }
            }
            catch (WebException ex)
            {
                exceptionHttpHelper = new ExceptionHttpHelper(ex);
                MessageBox.Show($"{exceptionHttpHelper.StatusCode}\n{exceptionHttpHelper.StatusDescription}\n\n{exceptionHttpHelper.ErrorMessage}", "Fejl opstået");
            }
        }

        // Henter alle brugere depending fra lokation
        private void GetUsers(int userId)
        {
            try
            {
                using (ApiHelper.Client)
                {
                    string json = ApiHelper.Client.DownloadString($"location/{CurrentLoggedInUser.Locations[0].ID}/users"); // Rigtige location/1/users

                    var list = JsonConvert.DeserializeObject<List<User>>(json);
                    // Looper gennem hver bruger, tilføjer lokationer derefter i collection af brugere
                    foreach (var user in list)
                    {
                        user.Locations = GetUserLocations(user.ID);
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

        // Henter brugerss lokationer
        private List<Location> GetUserLocations(int userId)
        {
            try
            {
                using (ApiHelper.Client)
                {
                    string json = ApiHelper.Client.DownloadString($"/user/{userId}/locations");

                    List<Location> locations = JsonConvert.DeserializeObject<List<Location>>(json);

                    foreach (var location in locations)
                    {

                        //location.LocationManager = GetLocationLeaders(locationId: location.ID)[0];

                    }


                    return locations;
                }
            }
            catch (WebException ex)
            {

                exceptionHttpHelper = new ExceptionHttpHelper(ex);

                MessageBox.Show($"{exceptionHttpHelper.StatusCode}\n{exceptionHttpHelper.StatusDescription}\n\n{exceptionHttpHelper.ErrorMessage}", "Fejl opstået");
                return null;
            }
        }

        private void CreateEntry()
        {
            try
            {
                TimeEntry newEntry = new TimeEntry()
                {
                    UserId = SelectedUser.ID,
                    Start = UnixConversion.ToUnixTimeMilliSeconds(Start),
                    End = UnixConversion.ToUnixTimeMilliSeconds(End),
                    Duration = End.TimeOfDay.Hours - Start.TimeOfDay.Hours,
                    TimeEntryTypeId = SelectedTimeEntryType.ID,
                    LocationId = SelectedUser.Locations[0].ID,
                };

                newEntry.Create();
            }
            catch (WebException ex)
            {
                exceptionHttpHelper = new ExceptionHttpHelper(ex);

                MessageBox.Show($"{(int)exceptionHttpHelper.StatusCode}\n{exceptionHttpHelper.StatusDescription}\n\n{exceptionHttpHelper.ErrorMessage}", "Fejl");
            }
        }

        #region Private Variables
        private DateTime _end = DateTime.UtcNow;
        private User _selectedUser;
        private DateTime _start = DateTime.UtcNow;
        private TimeEntryType _selectedTimeEntryType;
        #endregion
    }
}
