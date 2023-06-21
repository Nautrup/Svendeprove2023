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
using System.Windows.Controls.Primitives;
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

            ShowCreateWorkplanCommand = new RelayCommand(o => ShowCreateWorkplanWindow(), o => SelectedUserLocation != null);

            //New Role popup
            ShowPopupCreateNewRoleCommand = new RelayCommand(o => { ShowAddNewRolePopup = true; RoleName = string.Empty; RoleDescription = string.Empty; }) ;

            CreateNewRoleCommand = new RelayCommand(o => CreateNewRole(), o => SelectedPermissions != null && !string.IsNullOrEmpty(RoleName) && !string.IsNullOrEmpty(RoleDescription)) ;
            
            CancelCreateRoleCommand = new RelayCommand(o => ShowAddNewRolePopup = false);
            
            // Tilføj Lokation Popup
            AddCommentToEntryCommand = new RelayCommand(o => 
            {
                try
                {
                    SelectedTimeEntry.User = CurrentLoggedInUser;
                    SelectedTimeEntry.AddComment(NewComment);
                    
                    GetUserTimeEntries((int)SelectedTimeEntry.UserId);
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
            CancelAddLocationCommand = new RelayCommand(o => ShowAddLocationsPopUp = false, o => true);
            ShowAddLocationsCommand = new RelayCommand(o => ShowAddNewLocationPopup(), o => SelectedUser != null);

            RemoveSelectedUserLocationCommand = new RelayCommand(o => RemoveLocationToUser(), o => SelectedUser != null);
            DeleteUserCommand = new RelayCommand(o => SelectedTimeEntry.ReleaseEntry(), o => SelectedTimeEntry != null);

            // Tilføj permission til rolle
            ShowAddPermissionPopupCommand = new RelayCommand(o => { ShowAddRolePermissionPopup = true; LoadUserRoles(); }, o => true);
            AddPermissionCommand = new RelayCommand(o => { SelectedUserRole.AddPermission(SelectedPermission.ID); ShowAddRolePermissionPopup = false; }, o => SelectedPermission != null);
            
            // Tilføj kommentar popup
            ShowAddCommentPopupCommand = new RelayCommand(o => ShowAddCommentPopup = true, o => SelectedTimeEntry != null);
            CancelCommentCommand = new RelayCommand(o => { ShowAddCommentPopup = false; ShowAddNewRolePopup = false; ShowAddLocationsPopUp = false; ShowChangeUserRolePopup = false; ShowAddRolePermissionPopup = false; });
            
            // Skift rolle poup
            ChangeRoleCommand = new RelayCommand(o => { UpdateUserInformation(SelectedUser); ShowChangeUserRolePopup = false; }, o => SelectedUser != null);
            ShowChangeRolePopupCommand = new RelayCommand(o => ShowChangeUserRolePopup = true);
            
            ReleaseTimeEntryCommand = new RelayCommand(o => 
            {
                try
                {
                    SelectedTimeEntry.ReleaseEntry();
                }
                catch (WebException ex)
                {
                    exceptionHttpHelper = new ExceptionHttpHelper(ex);

                    MessageBox.Show($"{exceptionHttpHelper.StatusCode}\n{exceptionHttpHelper.StatusDescription}\n\n{exceptionHttpHelper.ErrorMessage}\nTimeEntry id:{SelectedTimeEntry.Id}", "Fejl opstået");
                }
                
            }, o => SelectedTimeEntry != null);
         
            LoadLocations(); //  Henter Lokationer
            LoadRolePermissions(); // Henter roller

            CurrentWeekStartDate = GetStartOfWeek(DateTime.Now); // Henter første mandags dato i en uge

            // Tjekker bruger rettigheder
            if (CurrentLoggedInUser.UserRole.PermissionIds.Contains(1) || CurrentLoggedInUser.UserRole.PermissionIds.Contains(3))
            {
                ShowCreateShiftButton = true;
            }

            if (CurrentLoggedInUser.UserRole.PermissionIds.Contains(12))
            {
                ShowCreateRoleButton = true;
            }
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
        
        // Permission popup
        public ICommand AddPermissionCommand { get; set; }
        public ICommand ShowAddPermissionPopupCommand { get; set; }
        
        // Comment popup
        public ICommand AddCommentToEntryCommand { get; set; }
        public ICommand ShowAddCommentPopupCommand { get; set; }
        // Create new role poup
        public ICommand ShowPopupCreateNewRoleCommand { get; set; }
        public ICommand CreateNewRoleCommand { get; set; }
        public ICommand CancelCreateRoleCommand { get; set; }
        public ICommand CancelCommentCommand { get; set; }
        // add location popup
        public ICommand ShowAddLocationsCommand { get; set; }
        public ICommand AddLocationToUserCommand { get; set; }
        public ICommand CancelAddLocationCommand { get; set; }
        public ICommand ChangeRoleCommand { get; set; }
        // Change role popup
        public ICommand ShowChangeRolePopupCommand { get; set; }
        public ICommand RemoveSelectedUserLocationCommand { get; set; }
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

        // Valgte permission
       

        public UserRolePermission SelectedPermission {
            get { return _selectedPermission; }
            set { _selectedPermission = value; OnPropertyChanged(nameof(SelectedPermission)); }
        }

        // Valgte roller
        private UserRolePermission _selectedPermissions;

        // Valge bruger adgang til rolle
        public UserRolePermission SelectedPermissions {
            get { return _selectedPermissions; }
            set 
            { 
                _selectedPermissions = value;  
                OnPropertyChanged(nameof(SelectedPermissions)); 
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

                    if (SelectedUser.HiredDate != null)
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

        // Valgte brugers rolle (change)

        public UserRole SelectedUserRole {
            get { return _selectedUserRole; }
            set { 
                _selectedUserRole = value; OnPropertyChanged(nameof(SelectedUserRole));
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

        // holder styr på om brugeren skal have en opet vagt knap visible = true
        public bool ShowCreateShiftButton { get; set; } = false;
        public bool ShowCreateRoleButton { get; set; } = false;

        // holder styr på om vores popup skal vises
        public bool ShowAddLocationsPopUp {
            get { return _showAddLocationsPopUp; }
            set { _showAddLocationsPopUp = value; OnPropertyChanged(nameof(ShowAddLocationsPopUp)); }
        }

        // Holder styr på om vi skal se popup med opretteelse af roller
        public bool ShowAddNewRolePopup {
            get { return _showAddNewRolePopup; }
            set
            {
                _showAddNewRolePopup = value; OnPropertyChanged(nameof(ShowAddNewRolePopup));
            }
        }

        // Holder styr på om popup for at ændre en brugeres rolle skal vises
        
        public bool ShowChangeUserRolePopup {
            get { return _showChangeUserRolePopup; }
            set 
            {
                _showChangeUserRolePopup = value;
                if (value == true)
                {
                    LoadUserRoles();
                }
                OnPropertyChanged(nameof(ShowChangeUserRolePopup));
            }
        }

        

        // Holder styr på om vi skal vise popup til at tilføje flere permission til en roller
        public bool ShowAddRolePermissionPopup {
            get { return _showAddRolePermissionPopup; }
            set { _showAddRolePermissionPopup = value; OnPropertyChanged(nameof(ShowAddRolePermissionPopup)); }
        }



        // Rolle navn ved oprettelse
        public string RoleName {
            get { return _roleName; }
            set { _roleName = value; OnPropertyChanged(nameof(RoleName)); }
        }

        // Rolle beskrivelse ved oprettelse
        public string RoleDescription {
            get { return _roleDescription; }
            set { _roleDescription = value; OnPropertyChanged(nameof(RoleDescription));
            }
        }



        #region ObservableCollectiions
        public ObservableCollection<User> UserCollection { get; set; } = new();

        public ObservableCollection<Company> CompanyCollection { get; set; } = new();

        public ObservableCollection<Location> LocationCollection { get; set; } = new();

        public ObservableCollection<TimeEntry> TimeEntriesCollection { get; set; } = new();

        public ObservableCollection<UserRole> UserRoleCollection { get; set; } = new();

        public ObservableCollection<UserRolePermission> PermissionsRollection { get; set; } = new();

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

        // Henter bruger på ID
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
                        entry.Messages = GetTimeEntryMessages(entry.Id);
                       
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

        // Henter adgange for roller
        private void LoadRolePermissions()
        {
            try
            {
                PermissionsRollection.Clear();

                using (ApiHelper.Client)
                {
                    string json = ApiHelper.Get($"/roles/permission");

                    
                    foreach (var role in JsonConvert.DeserializeObject<List<UserRolePermission>>(json))
                    {
                        PermissionsRollection.Add(role);
                    }

                }
            }
            catch (WebException ex)
            {
                exceptionHttpHelper = new ExceptionHttpHelper(ex);

                MessageBox.Show($"{exceptionHttpHelper.StatusCode}\n{exceptionHttpHelper.StatusDescription}\n\n{exceptionHttpHelper.ErrorMessage}", "Fejl opstået");
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

        // Henter alle roller
        private void LoadUserRoles()
        {
            try
            {
                UserRoleCollection.Clear();

                string jsonResponse = ApiHelper.Get("/role");

                foreach (var role in JsonConvert.DeserializeObject<List<UserRole>>(jsonResponse)) 
                {
                    UserRoleCollection.Add(role);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        // Oprette ny rolle 
        private void CreateNewRole()
        {
            try
            {
                UserRole neweRole = new UserRole();
                neweRole.Name = RoleName;
                neweRole.Description = RoleDescription;

                neweRole.PermissionIds = new List<int>()
                {
                    SelectedPermissions.ID
                };

                neweRole.Create();

                // Fjerner popup igen
                ShowAddNewRolePopup = false;
            }
            catch (WebException ex)
            {
                exceptionHttpHelper = new ExceptionHttpHelper(ex);

                MessageBox.Show($"{exceptionHttpHelper.StatusCode}\n{exceptionHttpHelper.StatusDescription}\n\n{exceptionHttpHelper.ErrorMessage}", "Fejl opstået");
            } catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Fejl opstået");
            }
        }

        #region API Put 
        // Opdatere en brugers information
        private void UpdateUserInformation(User selectedUser)
        {
            try
            {
                UserCreate updateUser = new UserCreate();
                if (SelectedUserRole != null)
                {
                    selectedUser.UserRole = SelectedUserRole;
                }
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

                CreateShiftWindow createShift = new CreateShiftWindow(SelectedUserLocation.ID);
            
                createShift.ShowDialog();

                if (temp != null)
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
        private UserRolePermission _selectedPermission;
        private bool _showAddRolePermissionPopup = false;
        private bool _showChangeUserRolePopup = false;
        private bool _showAddNewRolePopup = false;
        private bool _showAddCommentPopup = false;
        private bool _showAddLocationsPopUp;
        private string _newComment;
        private Location _SelectedUserLocation;
        private Location _selectedLocation;
        private string _roleName;
        private string _roleDescription;
        private UserRole _selectedUserRole;
        private DateTime _currentWeekStartDate;
        private User _selectedUser;
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
