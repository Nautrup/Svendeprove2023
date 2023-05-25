using EmployeeManagement.Common;
using EmployeeManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EmployeeManagement.ViewModel
{
    public class CreateEmployeeViewModel : ViewModelBase
    {
        public CreateEmployeeViewModel()
        {
            CreateNewUserCommand = new RelayCommand(o => CreateUser());
        }

        public ICommand CreateNewUserCommand { get; set; }

        public Company Company { get; set; }
        public UserRole UserRole { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public int ProfileImage { get; set; }
        public DateTime FirstDateOfEmployment { get; set; }
        public DateTime LastDateOfEmployement { get; set; }
        public List<Location> Location { get; set; }

        // Opretter brugeren
        private void CreateUser()
        {
            User newUser = new User
            {
                Company = Company,
                UserRole = UserRole,
                FirstName = FirstName,
                MiddleName = MiddleName,
                LastName = LastName,
                Email = Email,
                Age = Age,
                ProfileImage = ProfileImage,
                FirstDateOfEmployment = FirstDateOfEmployment,
                LastDateOfEmployement = LastDateOfEmployement,
                Location = Location,
            };

            newUser.Create();
        }
    }
}
