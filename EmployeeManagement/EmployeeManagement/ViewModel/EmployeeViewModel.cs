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
            UpdateUserInformationCommand = new RelayCommand(o => UpdateUserInformation(SelectedUser), o => SelectedUser != null);
            CreateNewUserCommand = new RelayCommand(o => { } , o => SelectedUser != null);
            ShowCreateWindowCommand = new RelayCommand(o => ShowCreateWindow()) ;
            FindNextWeekWorkplanCommand = new RelayCommand(o => CurrentWeekStartDate = CurrentWeekStartDate.AddDays(7), o => SelectedUser != null);
            FindLastWeekWorkplanCommand = new RelayCommand(o => CurrentWeekStartDate = CurrentWeekStartDate.AddDays(-7), o => SelectedUser != null);
            ShowCreateWorkplanCommand = new RelayCommand(o => ShowCreateWorkplanWindow(), o => true);

            CancelCommentCommand = new RelayCommand(o => ShowAddCommentPopup = false);
            ShowAddCommentPopupCommand = new RelayCommand(o => ShowAddCommentPopup = true, o => SelectedTimeEntry != null);

            AddCommentToEntryCommand = new RelayCommand(o => 
            {
                try
                {
                    SelectedTimeEntry.AddComment(NewComment);

                }
                catch (WebException ex)
                {
                    exceptionHttpHelper = new ExceptionHttpHelper(ex);

                    MessageBox.Show($"{exceptionHttpHelper.StatusCode}\n{exceptionHttpHelper.StatusDescription}\n\n{exceptionHttpHelper.ErrorMessage}", "Fejl opstået");
                }
                catch (Exception ex)
                {

                    MessageBox.Show($"{ex.Message}", "Fejl opstået");
                }
                ShowAddCommentPopup = false; 
                
            });
            AddLocationToUserCommand = new RelayCommand(o => AddLokationToUser(), o => SelectedUser != null);

            RemoveSelectedUserLocationCommand = new RelayCommand(o => RemoveLocationToUser(), o => SelectedUser != null);
            ShowAddLocationsCommand = new RelayCommand(o => ShowAddNewLocationPopup(), o => SelectedUser != null);
            CancelAddLocationCommand = new RelayCommand(o => ShowAddLocationsPopUp = false, o => true);
            DeleteUserCommand = new RelayCommand(o => SelectedTimeEntry.ReleaseEntry(), o => SelectedTimeEntry != null);
            
            ReleaseTimeEntryCommand = new RelayCommand(o => SelectedTimeEntry.ReleaseEntry(), o => SelectedTimeEntry != null); ;
         
            LoadLocations();
            
            CurrentWeekStartDate = GetStartOfWeek(DateTime.Now); // Henter første mandags dato i en uge

            if (CurrentLoggedInUser.UserRole.PermissionIds.Contains(2))
            {
                ShowCreateShiftButton = false;
            }
        }


        // holder styr på om brugeren skal have en opet vagt knap visible = true
        public bool ShowCreateShiftButton { get; set; } = true;
       
        // holder styr på om vores popup skal vises
        public bool ShowAddLocationsPopUp {
            get { return _showAddLocationsPopUp; }
            set { _showAddLocationsPopUp = value; OnPropertyChanged(nameof(ShowAddLocationsPopUp)); }
        }

        #region Icommands
        public ICommand UpdateUserInformationCommand { get; set; }
        public ICommand CreateNewUserCommand { get; set; }
        public ICommand ShowCreateWindowCommand { get; set; }
        public ICommand FindNextWeekWorkplanCommand { get; set; }
        public ICommand FindLastWeekWorkplanCommand { get; set; }
        public ICommand DeleteUserCommand { get; set; }
        public ICommand ShowCreateWorkplanCommand { get; set; }
        public ICommand ReleaseTimeEntryCommand { get; set; }
        // Pop up
        public ICommand RemoveSelectedUserLocationCommand { get; set; }
        public ICommand AddCommentToEntryCommand { get; set; }
        public ICommand ShowAddCommentPopupCommand { get; set; }
        public ICommand CancelCommentCommand { get; set; }
        public ICommand ShowAddLocationsCommand { get; set; }
        public ICommand AddLocationToUserCommand { get; set; }
        public ICommand CancelAddLocationCommand { get; set; }
        #endregion

        // Nuværende uges mandags dato
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

        // Valgte lokation i popuppen
        public Location SelectedLocation {
            get { return _selectedLocation; }
            set { _selectedLocation = value; OnPropertyChanged(nameof(SelectedLocation)); }
        }

        // Afdelingslederens valget lokation til at se sine medarbejder pr. lokation
        public Location SelectedUserLocation {
            get { return _SelectedUserLocation; }
            set 
            { 
                _SelectedUserLocation = value; 
                OnPropertyChanged(nameof(SelectedUserLocation));
                
                if (SelectedUserLocation != null)
                {
                    GetUsers();
                }
            }
        }

        // Valgte TimeEntry 
        public TimeEntry SelectedTimeEntry {
            get { return _SelectedTimeEntry; }
            set 
            { 
                _SelectedTimeEntry = value; 
                OnPropertyChanged(nameof(SelectedTimeEntry));
            }
        }

        // Vores text vi tilføjeer til en timeEntry
        public string NewComment {
            get { return _newComment; }
            set { _newComment = value; OnPropertyChanged(nameof(NewComment));
            }
        }

        // Holder sytr på om vi skal vise popup med at tilføje en kommentar 
        public bool ShowAddCommentPopup {
            get { return _showAddCommentPopup; }
            set
            {
                _showAddCommentPopup = value; OnPropertyChanged(nameof(ShowAddCommentPopup));
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
        private decimal _weeklyWorkHours;

        public decimal WeeklyWorkHours {
            get { return _weeklyWorkHours; }
            set { _weeklyWorkHours = value; OnPropertyChanged(nameof(WeeklyWorkHours));
            }
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

                    string json = ApiHelper.Get($"/location/{SelectedUserLocation.ID}/users");
                        
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


        private User RetrieveUserById(int userId)
        {
            try
            {
                using (ApiHelper.Client)
                {
                    string json = ApiHelper.Get($"/user/{userId}");

                    return JsonConvert.DeserializeObject<User>(json);
                    
                }
            }
            catch (WebException ex)
            {
                exceptionHttpHelper = new ExceptionHttpHelper(ex);
                
                MessageBox.Show($"{exceptionHttpHelper.StatusCode}\n{exceptionHttpHelper.StatusDescription}\n\n{exceptionHttpHelper.ErrorMessage}", "Fejl opstået");
                return null;
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

                    //foreach (var location in locations)
                    //{
                    //    location.LocationManager = GetLocationLeaders(location.ID)[0];
                    //}

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

                    List<TimeEntry> list = JsonConvert.DeserializeObject<List<TimeEntry>>(entriesJson); //JsonConvert.DeserializeObject<List<TimeEntry>>(json);

                    // Sætte ugens arbejdstimer til 0
                    WeeklyWorkHours = 0;

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
                        entry.Messages = GetTimeEntryMessages(entry.ID);
                       
                        DayOfWeek day = entry.StartDate.DayOfWeek;

                        switch (day)
                        {
                            case DayOfWeek.Monday:
                                if (entry.StartDate.Date == MondayDate.Date)
                                {
                                    MondayTimeEntriesCollection.Add(entry);
                                    WeeklyWorkHours += entry.Duration;
                                }
                                break;
                            case DayOfWeek.Tuesday:

                                if (entry.StartDate.Date == TuesdayDate.Date)
                                {
                                    TuesdayTimeEntriesCollection.Add(entry);
                                    WeeklyWorkHours += entry.Duration;
                                }
                                break;
                            case DayOfWeek.Wednesday:
                                if(entry.StartDate.Date == WednsdayDate.Date)
                                {
                                    WensdayTimeEntriesCollection.Add(entry);
                                    WeeklyWorkHours += entry.Duration;
                                }
                                break;
                            case DayOfWeek.Thursday:
                                if (entry.StartDate.Date == ThursdayDate.Date)
                                {
                                    ThursdayTimeEntriesCollection.Add(entry);
                                    WeeklyWorkHours += entry.Duration;
                                }
                                break;
                            case DayOfWeek.Friday:
                                if (entry.StartDate.Date == FridayDate.Date)
                                {
                                    FridayTimeEntriesCollection.Add(entry);
                                    WeeklyWorkHours += entry.Duration;
                                }
                                break;
                            case DayOfWeek.Saturday:
                                if (entry.StartDate.Date == SaturdayDate.Date)
                                {
                                    SaturdayTimeEntriesCollection.Add(entry);
                                    WeeklyWorkHours += entry.Duration;
                                }
                                break;
                            case DayOfWeek.Sunday:
                                if (entry.StartDate.Date == SundayDate.Date)
                                {
                                    SundayTimeEntriesCollection.Add(entry);
                                    WeeklyWorkHours += entry.Duration;
                                }
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
        private List<TimeEntryMessage> GetTimeEntryMessages(int? entryId)
        {
            try
            {
                using (ApiHelper.Client)
                {
                    string json = ApiHelper.Get($"/entry/{entryId}/messages");

                    List<TimeEntryMessage> messages = new List<TimeEntryMessage>();
                    foreach (var message in JsonConvert.DeserializeObject<List<TimeEntryMessage>>(json) )
                    {
                        message.User = RetrieveUserById(message.UserId);
                        messages.Add(message);
                    }

                    return messages;
                }
            }
            catch (WebException ex)
            {
                exceptionHttpHelper = new ExceptionHttpHelper(ex);
                
                MessageBox.Show($"{exceptionHttpHelper.StatusCode}\n{exceptionHttpHelper.StatusDescription}\n\n{exceptionHttpHelper.ErrorMessage}", "Fejl opstået");
                return null;
            }
        }

        // Henter alle lokationer
        private void LoadLocations()
        {
            LocationCollection.Clear();
            
            string json = ApiHelper.Get($"/location");

            List<Location> locations = JsonConvert.DeserializeObject<List<Location>>(json);

            foreach (var location in locations)
            {
                LocationCollection.Add(location);
            }

        }

        #endregion

        #region API Put 
        // Opdatere en brugers information
        private void UpdateUserInformation(User selectedUser)
        {
            try
            {
                UserCreate updateUser = new UserCreate();

                updateUser.Update(selectedUser);
                // Henter brugere igen
                GetUsers();
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
                    //SelectedUser.Delete();
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

        // Tilføjer lokation til bruger
        private void AddLokationToUser()
        {
            try
            {
                // Giver bruger lokation
                SelectedUser.AddLocation(SelectedLocation);
                
                // Opdater bruger information
                UpdateUserInformation(SelectedUser);

                ShowAddLocationsPopUp = false;
            }
            catch (WebException ex)
            {
                exceptionHttpHelper = new ExceptionHttpHelper(ex);

                MessageBox.Show($"{exceptionHttpHelper.StatusCode}\n{exceptionHttpHelper.StatusDescription}\n\n{exceptionHttpHelper.ErrorMessage}", "Fejl opstået");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        // Fjerner lokationer
        private void RemoveLocationToUser()
        {
            try
            {
                SelectedUser.RemoveLocation(SelectedLocation);
            }
            catch (WebException ex)
            {
                exceptionHttpHelper = new ExceptionHttpHelper(ex);

                MessageBox.Show($"{exceptionHttpHelper.StatusCode}\n{exceptionHttpHelper.StatusDescription}\n\n{exceptionHttpHelper.ErrorMessage}", "Fejl opstået");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        // Holder styr på om Popup skal vises for at tilføje en lokation
        private void ShowAddNewLocationPopup()
        {
            if (ShowAddLocationsPopUp == true)
            {
                ShowAddLocationsPopUp = false;
            }
            else
            {
                ShowAddLocationsPopUp = true;
            }
        }

        #region Private Variables
        private bool _showAddCommentPopup = false;
        private string _newComment;
        private Location _SelectedUserLocation;
        private Location _selectedLocation;
        private bool _showAddLocationsPopUp;
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
        private TimeEntry _SelectedTimeEntry;
        #endregion
    }
}
