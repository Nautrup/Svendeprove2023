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

                //string stream = ApiHelper.Client.UploadString(ApiHelper.EndPoint, json);

                string stream = ApiHelper.Post("/auth/authenticate", json);

                User user = new()
                {
                    FirstName = "Jonas"
                };
                
                MainWindow window = new MainWindow(user);
                window.Show();
            }
            catch (WebException ex)
            {
                exceptionHttpHelper = new(ex);
                
                MessageBox.Show($"{(int)exceptionHttpHelper.StatusCode}\n{exceptionHttpHelper.StatusDescription}\n\n{exceptionHttpHelper.ErrorMessage}", "Fejl");
            } catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Fejl");
            }
        }
        #endregion

        #region  Private Variables
        private string _password;
        private string _userName;
        #endregion
    }
}
