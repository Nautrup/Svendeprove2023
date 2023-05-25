using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public class User
    {
        public int ID { get; set; }
        public Company Company { get; set; }
        public UserRole UserRole { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string FullName { get { return $"{FirstName} {MiddleName} {LastName}"; } }
        public string Email { get; set; }
        public int Age { get; set; }
        public int ProfileImage { get; set; }
        public List<int> TimeTagCollection { get; set; }
        public DateTime FirstDateOfEmployment { get; set; }
        public DateTime LastDateOfEmployement { get; set; }
        public List<Location> Location { get; set; }

        public void Update()
        {
            // Api til update
        }

        public void Create()
        {
            // Api til insert
        }

        public void Delete()
        {
            // Api til slet
        }
        
    }
}
