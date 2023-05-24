using EmployeeManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Services
{
    public static class UserService
    {
        public static List<User> GetUsers()
        {
            List<User> users = new List<User>();
            users.Add(new User()
            {
                FirstName = "Jonas",
                MiddleName = "Rudbeck",
                LastName = "Johansen",
                ID = 1,
                FirstDateOfEmployment = DateTime.Now.AddDays(-20),
                Age = 29,
                Company = new Company()
                {
                    ID = 1,
                    Name = "Thise Mejeri"
                },
                Location = new List<Location>()
                {
                    new Location
                    {
                        ID = 1,
                        Name = "Thise",
                        Description = "Et mega fed mejeri"
                    }
                },
                UserRole = new UserRole() 
                { 
                    ID = 1, Name = "The Big Boss",
                    Description = "Bossen over dem alle", 
                    Permissions = new List<UserRolePermission> 
                    { 
                        new UserRolePermission() 
                        {
                            ID = 1,
                            Name = "Permission 1",
                            Descriptions = "Ved ikke helt hvad den her permission gør"
                        } 
                    }
                },
                TimeTagCollection = new List<int> { 1, 2, 3, 4, 5, 6}
                
                
                
            }) ;

            users.Add(new User()
            {
                FirstName = "Jørgen",
                MiddleName = "IngenArm",
                LastName = "Hansen",
                ID = 1,
                FirstDateOfEmployment = DateTime.Now.AddDays(-90),
                Age = 64,
                Company = new Company()
                {
                    ID = 2,
                    Name = "Cirkel K"
                },
                Location = new List<Location>() 
                {
                    new Location
                    {
                         ID = 2,
                        Name = "Skive",
                        Description = "CIRKEL FUCKING K",
                        LocationManager = users[0]
                    } 
                },
                UserRole = new UserRole()
                {
                    ID = 2,
                    Name = "Medarbejder",
                    Description = "Kassedame",
                    Permissions = new List<UserRolePermission> { new UserRolePermission()
                    { ID = 2,
                    Name = "Permission 2",
                    Descriptions = "Måske han har adgang til noget fedt"
                    }
                    }
                },


            });

            users.Add(new User()
            {
                FirstName = "Lars",
                MiddleName = "",
                LastName = "Larsen",
                ID = 3,
                FirstDateOfEmployment = DateTime.Now.AddDays(-90),
                Age =75,
                Company = new Company()
                {
                    ID = 3,
                    Name = "Jysk"
                },
                Location = new List<Location>()
                {
                    new Location
                    {
                        ID = 2,
                        Name = "Skive",
                        Description = "Niice",
                        LocationManager = users[0]
                    }
                },
                UserRole = new UserRole()
                {
                    ID = 1,
                    Name = "The Big Boss",
                    Description = "Bossen over dem alle",
                    Permissions = new List<UserRolePermission> 
                    { 
                        new UserRolePermission()
                        { 
                            ID = 3,
                            Name = "Permission 2",
                            Descriptions = "Måske han har adgang til noget fedt"
                        }
                    }
                },


            });


            return users;
        }

        public static List<Company> GetCompanies()
        {
            List<Company> dummys = new List<Company>
            {
                new Company()
                {
                    ID = 1,
                    Name = "Thise Mejeri",

                },
                new Company()
                {
                    ID = 2,
                    Name = "Arla Mejeri"
                },
                new Company()
                {
                    ID= 3,
                    Name = "Mejeriet Dybbækdal"
                },
                new Company()
                {
                    ID = 4,
                    Name = "Mammen Mejeri"
                }
            };

            return dummys;
        }
    }
}
