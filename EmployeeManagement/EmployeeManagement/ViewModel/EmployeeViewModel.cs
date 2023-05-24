using EmployeeManagement.Common;
using EmployeeManagement.Models;
using EmployeeManagement.Services;
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
            
            LoadUsers();
            LoadCompanies();
        }

       

        #region Icommands
        public ICommand UpdateUserInformationCommand { get; set; }
        #endregion

        // Valgte bruger
        public User SelectedUser
		{
			get { return _selectedUser; }
			set 
			{ 
				_selectedUser = value;

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

        #region Methods
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

        private void LoadCompanies()
        {
            CompanyCollection.Clear();
            List<Company> companiies = UserService.GetCompanies();
            foreach (var company in companiies)
            {
                CompanyCollection.Add(company);
            }
        }

        private void UpdateUserInformation()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Private Variables
        private User _selectedUser;
        private Company _selectedCompany;
        #endregion
    }
}
