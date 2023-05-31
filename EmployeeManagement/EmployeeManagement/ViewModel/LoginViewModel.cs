using EmployeeManagement.Common;
using EmployeeManagement.Models;
using EmployeeManagement.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace EmployeeManagement.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        // Hjælper med at håndtere websocket exceptions
        ExceptionHttpHelper exceptionHttpHelper;

        public LoginViewModel()
        {
            ApiHelper.InitializeClient();

            AuthenticateCommand = new RelayCommand(o => AuthenticateUser());
            CloseWindowCommand = new RelayCommand(o => Application.Current.Shutdown());
        }

        #region ICommands
        public ICommand AuthenticateCommand { get; set; }
        public ICommand CloseWindowCommand { get; set; }
        #endregion

        #region Properties
        public string Password {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public string Username {
            get { return _userName; }
            set
            {
                _userName = value;
                OnPropertyChanged(nameof(Username));
            }
        }
        #endregion

        #region Methods
        
        private void AuthenticateUser()
        {
            try
            {
                // opretter bruger object
                UserLogin login = new UserLogin()
                {
                    Username = Username,
                    Password = Password
                };

                // laver bruger object til json
                string? json = JsonConvert.SerializeObject(login);
                string stream = ApiHelper.Post("/auth/authenticate", json);

                ApiAuthenticationData apiData = JsonConvert.DeserializeObject<ApiAuthenticationData>(stream);

                // Henter den authenticated brugers information
                string response = ApiHelper.Get(endpoint: "/user/current");
                
                
                // Laver det til C# object
                User user = JsonConvert.DeserializeObject<User>(response);
                
                // Viser mainform
                MainWindow window = new MainWindow(user);
                window.Show();
            }
            catch (WebException ex)
            {
                exceptionHttpHelper = new(ex);
                
                MessageBox.Show($"{(int)exceptionHttpHelper.StatusCode}\n{exceptionHttpHelper.StatusDescription}\n\n{exceptionHttpHelper.ErrorMessage}", "Fejl");
            }
        }
        #endregion

        #region  Private Variables
        private string _password = "hpp";
        private string _userName = "hpp";
        #endregion
    }
}
