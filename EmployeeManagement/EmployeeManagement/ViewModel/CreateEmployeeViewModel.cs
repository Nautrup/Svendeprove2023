using EmployeeManagement.Common;
using EmployeeManagement.Models;
using EmployeeManagement.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    public class CreateEmployeeViewModel : ViewModelBase
    {
        ExceptionHttpHelper exceptionHttpHelper;

        public CreateEmployeeViewModel()
        {
            CreateNewUserCommand = new RelayCommand(o => Create(), o => CanCreate() );

            GetLocations();
            GetUserRoles();
            Company = CurrentLoggedInUser.Company;
        }

        #region ICommand
        public ICommand CreateNewUserCommand { get; set; }
        #endregion

        #region ObservableCollections
        public ObservableCollection<UserRole> UserRoleCollection { get; set; } = new();
        public ObservableCollection<Location> LocationCollection { get; set; } = new();
        #endregion

        #region Properties
        public UserRole UserRole {
            get { return _UserRole; }
            set { _UserRole = value; OnPropertyChanged(nameof(UserRole)); }
        }

        public Action CloseWindowAction { get; set; }

        public Company Company { get; set; }

        public string FirstName {
            get { return _firstName; }
            set { _firstName = value; OnPropertyChanged(nameof(FirstName)); }
        }
       
        public string MiddleName {
            get { return _middleName; }
            set { _middleName = value; OnPropertyChanged(nameof(MiddleName)); }
        }
        
        public string Surname {
            get { return _surname; }
            set { _surname = value; OnPropertyChanged(nameof(Surname));
            }
        }

        public string FullName {
            get { return $"{FirstName} {MiddleName} {Surname}"; }
        }
        
        public int ProfileImage { get; set; }

        public DateTime FirstDateOfEmployment {
            get { return _firstDateOfEmployment; }
            set { _firstDateOfEmployment = value; OnPropertyChanged(nameof(FirstDateOfEmployment)); }
        }

        public DateTime LastDateOfEmployement {
            get { return _lastDateOfEmployment; }
            set { _lastDateOfEmployment = value; OnPropertyChanged(nameof(LastDateOfEmployement));
            }
        }

        /* Selections */
        public Location SelectedLocation {
            get { return _selectedLocation; }
            set { _selectedLocation = value; OnPropertyChanged(nameof(SelectedLocation)); }
        }

        public UserRole SelectedUserRole {
            get { return _selectedUserRole; }
            set { _selectedUserRole = value; OnPropertyChanged(nameof(SelectedUserRole));
            }
        }

        #endregion

        #region Methods
        // Opretter ny bruger
        private void Create()
        {
            try
            {
                // opretter bruger object
                User newUser = new User
                {
                    Company = CurrentLoggedInUser.Company,
                    UserRole = SelectedUserRole,
                    FirstName = FirstName,
                    MiddleName = MiddleName,
                    SurName = Surname,
                    ProfileImage = 1, //
                    HiredDate = UnixConversion.ToUnixTimeMilliSeconds(FirstDateOfEmployment),
                    FiredDate = UnixConversion.ToUnixTimeMilliSeconds(LastDateOfEmployement),
                    Locations = new List<Location> { SelectedLocation } 
                };


                UserCreate newBruger = new UserCreate();

                newBruger.Create(newUser);


                CloseWindowAction();
            }
            catch (WebException ex)
            {
                exceptionHttpHelper = new ExceptionHttpHelper(ex);

                MessageBox.Show($"{exceptionHttpHelper.StatusCode}\n{exceptionHttpHelper.StatusDescription}\n\nMessage:{exceptionHttpHelper.ErrorMessage}", "Fejl opstået");
            }
        }

        // Henter alle firmater
        private void GetLocations()
        {
            try
            {
                LocationCollection.Clear();

                var json = ApiHelper.Get(endpoint: "/location");

                var list = JsonConvert.DeserializeObject<List<Location>>(json);

                foreach (Location company in list)
                {
                    LocationCollection.Add(company);
                }
            }
            catch (WebException ex)
            {
                exceptionHttpHelper = new(ex);
                MessageBox.Show($"{exceptionHttpHelper.StatusCode}\n{exceptionHttpHelper.StatusDescription}\n\n{exceptionHttpHelper.ErrorMessage}", "Exception opstået -> GetCompanies()", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        // Henter roller
        private void GetUserRoles() 
        {
            try
            {
                UserRoleCollection.Clear();
                
                var json = ApiHelper.Get(endpoint: "/role");

                var list = JsonConvert.DeserializeObject<List<UserRole>>(json);

                foreach (UserRole role in list)
                {
                    UserRoleCollection.Add(role);
                }
            }
            catch (WebException ex)
            {
                exceptionHttpHelper = new(ex);
                MessageBox.Show($"{exceptionHttpHelper.StatusCode}\n{exceptionHttpHelper.StatusDescription}\n\n{exceptionHttpHelper.ErrorMessage}", "Exception opstået CreateEmployeeView > GetUserRole()", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        
        // Tjekker om vi kan oprette
        private bool CanCreate()
        {
            if (string.IsNullOrEmpty(FirstName) || FirstName.Length > 50)
            {
                return false;
            }

            if (string.IsNullOrEmpty(Surname))
            {
                return false;
            }


            if (SelectedUserRole == null)
            {
                return false; 
            }

            if (SelectedLocation == null)
            {
                return false;
            }

            return true;
        }
        #endregion

        #region Private variables
        private DateTime _firstDateOfEmployment = DateTime.UtcNow;
        private DateTime _lastDateOfEmployment;
        private UserRole _UserRole;
        private string _firstName;
        private string _middleName;
        private string _surname;
        private Location _selectedLocation;
        private UserRole _selectedUserRole;
        #endregion
    }
}
