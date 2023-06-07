using EmployeeManagement.Common;
using EmployeeManagement.Models;
using EmployeeManagement.Services;
using EmployeeManagement.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace EmployeeManagement.ViewModel
{
    public class EmployeeViewModel : ViewModelBase
    {
        ExceptionHttpHelper exceptionHttpHelper;
        public EmployeeViewModel()
        {
            // ICommands
            UpdateUserInformationCommand = new RelayCommand(o => UpdateUserInformation(), o => SelectedUser != null);
            CreateNewUserCommand = new RelayCommand(o => { } , o => SelectedUser != null);
            ShowCreateWindowCommand = new RelayCommand(o => ShowCreateWindow()) ;
            FindNextWeekWorkplanCommand = new RelayCommand(o => CurrentWeekStartDate = CurrentWeekStartDate.AddDays(7), o => SelectedUser != null);
            FindLastWeekWorkplanCommand = new RelayCommand(o => CurrentWeekStartDate = CurrentWeekStartDate.AddDays(-7), o => SelectedUser != null);
            ShowCreateWorkplanCommand = new RelayCommand(o => ShowCreateWorkplanWindow(), o => true);

            DeleteUserCommand = new RelayCommand(o => DeleteUser(), o => SelectedUser != null);
            
            // Metoder der skal kørers når viewmodel initialisiere
            GetUsers(); // Henter alle brugere som har noget med den person at gøre

            CurrentWeekStartDate = GetStartOfWeek(DateTime.Now); // Henter første mandags dato i en uge

            if (CurrentLoggedInUser.UserRole.PermissionIds.Contains(2))
            {
                ShowCreateShiftButton = false;
            }
        }

        public bool ShowCreateShiftButton { get; set; } = true;

        #region Icommands
        public ICommand UpdateUserInformationCommand { get; set; }
        public ICommand CreateNewUserCommand { get; set; }
        public ICommand ShowCreateWindowCommand { get; set; }
        public ICommand FindNextWeekWorkplanCommand { get; set; }
        public ICommand FindLastWeekWorkplanCommand { get; set; }
        public ICommand DeleteUserCommand { get; set; }
        public ICommand ShowCreateWorkplanCommand { get; set; }
        #endregion

        public DateTime CurrentWeekStartDate {
            get { return _currentWeekStartDate; }
            set 
            {
                _currentWeekStartDate = value;
                if (SelectedUser != null)
                {
                    GetUserTimeEntries(SelectedUser.ID);
                }
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

                if (SelectedUser != null)
                {
                    GetUserTimeEntries(SelectedUser.ID);                       // Henter stemplinger
                 
                    SelectedUser.FirstDateOfEmployment = UnixConversion.UnixTimeStampToDateTime((long)SelectedUser.HiredDate);
                    
                    if (SelectedUser.FiredDate != null)
                        SelectedUser.LastDateOfEmployment = UnixConversion.UnixTimeStampToDateTime((long)SelectedUser.FiredDate);
                    
                }
              
                OnPropertyChanged(nameof(SelectedUser));
			}
		}

        #region ObservableCollectiions
        public ObservableCollection<User> UserCollection { get; set; } = new();

        public ObservableCollection<Company> CompanyCollection { get; set; } = new();

        public ObservableCollection<Location> LocationCollection { get; set; } = new();

        public ObservableCollection<TimeEntry> TimeEntriesCollection { get; set; } = new();

        public ObservableCollection<UserRole> UserRoleCollection { get; set; } = new();

        public ObservableCollection<TimeEntry> MondayTimeEntriesCollection { get; set; } = new();
        public ObservableCollection<TimeEntry> TuesdayTimeEntriesCollection { get; set; } = new();
        public ObservableCollection<TimeEntry> WensdayTimeEntriesCollection { get; set; } = new();
        public ObservableCollection<TimeEntry> ThursdayTimeEntriesCollection { get; set; } = new();
        public ObservableCollection<TimeEntry> FridayTimeEntriesCollection { get; set; } = new();
        public ObservableCollection<TimeEntry> SaturdayTimeEntriesCollection { get; set; } = new();
        public ObservableCollection<TimeEntry> SundayTimeEntriesCollection { get; set; } = new();

        #endregion

        #region Date Properties
        public DateTime MondayDate {
            get { return _mondayDate; }
            set { _mondayDate = value; OnPropertyChanged(nameof(MondayDate)); }
        }

        public DateTime TuesdayDate {
            get { return _tuesdayDate; }
            set { _tuesdayDate = value; OnPropertyChanged(nameof(TuesdayDate)); }
        }

        public DateTime WednsdayDate {
            get { return _wednsdayDate; }
            set { _wednsdayDate = value; OnPropertyChanged(nameof(WednsdayDate)); }
        }

        public DateTime ThursdayDate {
            get { return _thursdayDate; }
            set { _thursdayDate = value; OnPropertyChanged(nameof(ThursdayDate)); }
        }

        public DateTime FridayDate {
            get { return _fridayDate; }
            set { _fridayDate = value;OnPropertyChanged(nameof(FridayDate)); }
        }

        public DateTime SaturdayDate {
            get { return _saturdayDate; }
            set { _saturdayDate = value; OnPropertyChanged(nameof(SaturdayDate)); }
        }

        public DateTime SundayDate {
            get { return _sundayDate; }
            set { _sundayDate = value; OnPropertyChanged(nameof(SundayDate)); }
        }
        #endregion

        #region Methods

        #region API Get

        // Henter alle brugere
        private void GetUsers()
        {
            try
            {
                using (ApiHelper.Client)
                {
                    UserCollection.Clear();

                    string json = ApiHelper.Get($"/user"); 

                    var list = JsonConvert.DeserializeObject<List<User>>(json);
                    // Looper gennem hver bruger, tilføjer lokationer derefter i collection af brugere
                    foreach (var user in list)
                    {
                        user.Locations = GetUserLocations(user.ID);
                        user.UserRole = GetUserRole(user.ID);
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

        // henter brugers rollr
        private UserRole GetUserRole(int userID)
        {
            try
            {
                using (ApiHelper.Client)
                {
                    string json = ApiHelper.Get($"/user/{userID}/roles");

                    List<UserRole> roles = JsonConvert.DeserializeObject<List<UserRole>>(json);

                    return roles[0];
                }
            }
            catch (WebException ex)
            {
                exceptionHttpHelper = new ExceptionHttpHelper(ex);

                MessageBox.Show($"{exceptionHttpHelper.StatusCode}\n{exceptionHttpHelper.StatusDescription}\n\n{exceptionHttpHelper.ErrorMessage}", "Fejl opstået");
                return null;
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
                        location.LocationManager = GetLocationLeaders(location.ID)[0];
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
        // Henter lokation leder
        private List<User> GetLocationLeaders(int locationId)
        {
            try
            {
                using (ApiHelper.Client)
                {
                    string json = ApiHelper.Client.DownloadString($"/location/{locationId}/leaders");

                    List<User> locationLeaders = JsonConvert.DeserializeObject<List<User>>(json);

                    return locationLeaders;
                }
            }
            catch (WebException ex)
            {

                exceptionHttpHelper = new ExceptionHttpHelper(ex);

                MessageBox.Show($"{exceptionHttpHelper.StatusCode}\n{exceptionHttpHelper.StatusDescription}\n\n{exceptionHttpHelper.ErrorMessage}", "Fejl opstået");
                return null;
            }
        }

        // Henter bruger stemplinger
        private void GetUserTimeEntries(int userId)
        {
            try
            {
                using (ApiHelper.Client)
                {
                    long unix = UnixConversion.ToUnixTimeMilliSeconds(CurrentWeekStartDate);

                    Dictionary<string, string> param = new Dictionary<string, string>()
                    {
                        { "user",$"{userId}" },
                        { "timestamp",$"{unix}" } //,{ "timestamp",$"{CurrentWeekStartDate.AddDays(7).Second*1000}" },
                    };

                    string entriesJson = ApiHelper.Get($"/entries", param);

                    
                    //string json = ApiHelper.Client.DownloadString($"/entries?user={userId}");

                    List<TimeEntry> list = JsonConvert.DeserializeObject<List<TimeEntry>>(entriesJson); //JsonConvert.DeserializeObject<List<TimeEntry>>(json);

                    // Rydder listerne
                    TimeEntriesCollection.Clear();
                    MondayTimeEntriesCollection.Clear();
                    TuesdayTimeEntriesCollection.Clear();
                    WensdayTimeEntriesCollection.Clear();
                    ThursdayTimeEntriesCollection.Clear();
                    FridayTimeEntriesCollection.Clear();
                    SaturdayTimeEntriesCollection.Clear ();
                    SundayTimeEntriesCollection.Clear();

                    // Sætter overskrifter på dage
                    MondayDate = CurrentWeekStartDate.Date.AddDays(0);
                    TuesdayDate = CurrentWeekStartDate.Date.AddDays(1);
                    WednsdayDate = CurrentWeekStartDate.Date.AddDays(2);
                    ThursdayDate = CurrentWeekStartDate.Date.AddDays(3);
                    FridayDate = CurrentWeekStartDate.Date.AddDays(4);
                    SaturdayDate = CurrentWeekStartDate.Date.AddDays(5);
                    SundayDate = CurrentWeekStartDate.Date.AddDays(6);

                    foreach (TimeEntry entry in list)
                    {
                        //entry.TimeEntryMessage = GetUserTimeEntryMessages(entry.ID);

                        entry.StartDate = UnixConversion.UnixTimeStampToDateTime(entry.Start); // entry.StartDate.AddSeconds(entry.Start);
                        entry.EndDate = UnixConversion.UnixTimeStampToDateTime(entry.End);
                        entry.TimeEntryMessage = GetTimeEntryMessages(entry.ID);
                        DayOfWeek day = entry.StartDate.DayOfWeek;

                        switch (day)
                        {
                            case DayOfWeek.Monday:
                                if (entry.StartDate.Date == MondayDate.Date)
                                {
                                    MondayTimeEntriesCollection.Add(entry);
                                }
                                break;
                            case DayOfWeek.Tuesday:

                                if (entry.StartDate == TuesdayDate.Date)
                                {
                                    TuesdayTimeEntriesCollection.Add(entry);
                                }
                                break;
                            case DayOfWeek.Wednesday:
                                if(entry.StartDate.Date == WednsdayDate.Date)
                                {
                                    WensdayTimeEntriesCollection.Add(entry);
                                }
                                break;
                            case DayOfWeek.Thursday:
                                if (entry.StartDate.Date == ThursdayDate.Date)
                                {
                                    ThursdayTimeEntriesCollection.Add(entry);
                                }
                                break;
                            case DayOfWeek.Friday:
                                if (entry.StartDate.Date == FridayDate.Date)
                                {
                                    FridayTimeEntriesCollection.Add(entry);
                                }
                                break;
                            case DayOfWeek.Saturday:
                                if (entry.StartDate.Date == SaturdayDate.Date)
                                {
                                    SaturdayTimeEntriesCollection.Add(entry);
                                }
                                break;
                            case DayOfWeek.Sunday:
                                if (entry.StartDate.Date == SundayDate.Date)
                                {

                                }
                                SundayTimeEntriesCollection.Add(entry);
                                break;
                            default:
                                throw new InvalidOperationException("Invalid date found");
                        }

                    }
                }
            }
            catch (WebException ex)
            {
                exceptionHttpHelper = new ExceptionHttpHelper(ex);

                MessageBox.Show($"{exceptionHttpHelper.StatusCode}\n{exceptionHttpHelper.StatusDescription}\n\n{exceptionHttpHelper.ErrorMessage}", "Fejl opstået");
            }
        }

        // Henter en TimeEntrys messsage
        private List<TimeEntryMessage> GetTimeEntryMessages(int entryId)
        {
            try
            {
                using (ApiHelper.Client)
                {
                    string json = ApiHelper.Get($"/entry/{entryId}/messages");

                    return JsonConvert.DeserializeObject<List<TimeEntryMessage>>(json);
                }
            }
            catch (WebException ex)
            {
                exceptionHttpHelper = new ExceptionHttpHelper(ex);
                
                MessageBox.Show($"{exceptionHttpHelper.StatusCode}\n{exceptionHttpHelper.StatusDescription}\n\n{exceptionHttpHelper.ErrorMessage}", "Fejl opstået");
                return null;
            }
        }

        #endregion

        #region API Put 
        // Opdatere en brugers information
        private void UpdateUserInformation()
        {
            try
            {
                UserCreate updateUser = new UserCreate();

                updateUser.Update(SelectedUser);
            }
            catch (WebException ex)
            {
                exceptionHttpHelper = new ExceptionHttpHelper(ex);

                MessageBox.Show($"{exceptionHttpHelper.StatusCode}\n{exceptionHttpHelper.StatusDescription}\n\n{exceptionHttpHelper.ErrorMessage}", "Fejl opstået");
            }
        }

        // Sletter en bruger
        private void DeleteUser()
        {
            try
            {
                MessageBoxResult result = MessageBox.Show($"Er du sikker på du vil slette:\n{SelectedUser.FullName}\n\nBemærk: Disse ændringer vil slette alt data på denne medarbejder.\nØnsker du stadig at slette tryk 'Ja'", "Bekræft sletning", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    SelectedUser.Delete();
                    UserCollection.Remove(SelectedUser); 
                }

            }
            catch (WebException ex)
            {
                exceptionHttpHelper = new ExceptionHttpHelper(ex);

                MessageBox.Show($"{exceptionHttpHelper.StatusCode}\n{exceptionHttpHelper.StatusDescription}\n\n{exceptionHttpHelper.ErrorMessage}", "Fejl opstået");
            } catch(Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }


        #endregion

        // Henter starten  på ugen
        private DateTime GetStartOfWeek(DateTime date)
        {
            int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            return date.AddDays(-1 * diff).Date;
        }

        #endregion

        // Viser vores vindue til at oprette medarbejdere
        private void ShowCreateWindow()
        {
            CreateEmployeeView createView = new CreateEmployeeView();
            
            createView.ShowDialog();
            
            GetUsers();
        }

        // Viser vindue til at oprette ny vagt
        private void ShowCreateWorkplanWindow()
        {
            try
            {
                User temp = SelectedUser;

                CreateShiftWindow createShift = new CreateShiftWindow();
            
                createShift.ShowDialog();

                GetUserTimeEntries(temp.ID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        #region Private Variables
        
        private DateTime _currentWeekStartDate;
        private User _selectedUser;
        private Company _selectedCompany;
        private DateTime _saturdayDate;
        private DateTime _sundayDate;
        private DateTime _fridayDate;
        private DateTime _thursdayDate;
        private DateTime _wednsdayDate;
        private DateTime _tuesdayDate;
        private DateTime _mondayDate;
        #endregion
    }
}
