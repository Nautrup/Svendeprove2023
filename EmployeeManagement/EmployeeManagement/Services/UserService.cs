using EmployeeManagement.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EmployeeManagement.Services
{
    public static class UserService
    {
        public static ObservableCollection<User> Users { get; set; } = new ObservableCollection<User>()
        {
            new User()
            {
                FirstName = "Jonas",
                MiddleName = "Rudbeck",
                LastName = "Johansen",
                ID = 1,
                FirstDateOfEmployment = DateTime.Now.AddDays(-20),
                Age = 29,
                Email = "jrj@email.dk",
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



            },
            new User()
            {
                FirstName = "Jørgen",
                MiddleName = "IngenArm",
                LastName = "Hansen",
                ID = 1,
                FirstDateOfEmployment = DateTime.Now.AddDays(-90),
                Age = 64,
                Email = "jih@email.dk",
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
                    }
                },
                UserRole = new UserRole()
                {
                    ID = 2,
                    Name = "Medarbejder",
                    Description = "Kassedame",
                    Permissions = new List<UserRolePermission> 
                    {
                        new UserRolePermission()
                        { 
                            ID = 2,
                            Name = "Permission 2",
                            Descriptions = "Måske han har adgang til noget fedt"
                        }
                    }
                },
            },
            new User()
            {
                FirstName = "Lars",
                MiddleName = "",
                LastName = "Larsen",
                ID = 3,
                FirstDateOfEmployment = DateTime.Now.AddDays(-350),
                Age = 45,
                Email = "lal@email.dk",
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
                        Description = "Niice"
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
            }
        };

        // Dummy data
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
                Email = "jrj@email.dk",
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
                        Description = "Et mega fed mejeri",
                        LocationManager = Users[0]
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
                
                
                
            });

            users.Add(new User()
            {
                FirstName = "Jørgen",
                MiddleName = "IngenArm",
                LastName = "Hansen",
                ID = 1,
                FirstDateOfEmployment = DateTime.Now.AddDays(-90),
                Age = 64,
                Email = "jih@email.dk",
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
                        LocationManager = Users[1]
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
                Email = "lal@email.dk",
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
                        LocationManager = Users[2]
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

        public static List<TimeEntry> GetTimeEntries (TimeEntryType entryType)
        {
            List<TimeEntry> TimeEntryList = new List<TimeEntry>()
            {
                new TimeEntry
                {
                    ID =1,
                    User = Users[0],
                    Start = new DateTime(year: 2010, month: 11, day: 10, hour: 08, minute: 00, second: 00),
                    End = DateTime.Now,
                    Duration = 8,
                    GroupingID = 1,
                    TimeEntryMessage = new TimeEntryMessage()
                    {
                        ID =1,
                        User =Users[0],
                        TimeEntry = new TimeEntry()
                        {

                        },
                        Created = DateTime.Now,
                        Message = "TimeEntryMessage her! :D"
                    },
                    Location = new Location
                    {
                        ID = 1,
                        Name = "Thise",
                        Description = "Et mega fed mejeri"
                    },
                    TimeEntryType = entryType
                }, new TimeEntry
                {
                    ID =2,
                    User = Users[0],
                    Start = new DateTime(year: 2010, month: 11, day: 10, hour: 08, minute: 00, second: 00),
                    End = DateTime.Now,
                    Duration = 8,
                    GroupingID = 1,
                    TimeEntryMessage = new TimeEntryMessage()
                    {
                        ID =1,
                        User =Users[0],
                        TimeEntry = new TimeEntry()
                        {

                        },
                        Created = DateTime.Now,
                        Message = "TimeEntryMessage her! :D"
                    },
                    Location = new Location
                    {
                        ID = 2,
                        Name = "Cirkel K",
                        Description = "Et mega fed mejeri"
                    },
                    TimeEntryType = entryType
                },
                new TimeEntry
                {
                    ID = 3,
                    User = Users[1],
                    Start = new DateTime(year: 2000, month: 01, day: 01, hour: 08, minute: 00, second: 00),
                    End = DateTime.Now.AddHours(7),
                    Duration = 8,
                    GroupingID = 1,
                    TimeEntryMessage = new TimeEntryMessage()
                    {
                        ID =1,
                        User =Users[1],
                        TimeEntry = new TimeEntry()
                        {

                        },
                        Created = DateTime.Now,
                        Message = "TimeEntryMessage her! :D"
                    },
                    Location = new Location
                    {
                        ID = 2,
                        Name = "Cirkel K",
                        Description = "Dyrt sprøjt"
                    },
                    TimeEntryType = entryType
                },
                new TimeEntry
                {
                    ID =4,
                    User = Users[2],
                    Start = DateTime.Now.AddHours(8),
                    End = DateTime.Now,
                    Duration = 8,
                    GroupingID = 1,
                    TimeEntryMessage = new TimeEntryMessage()
                    {
                        ID =1,
                        User =Users[2],
                        TimeEntry = new TimeEntry()
                        {

                        },
                        Created = DateTime.Now,
                        Message = "TimeEntryMessage her! :D"
                    },
                    Location = new Location
                    {
                        ID = 3,
                        Name = "TSvansen",
                        Description = "Så kører det for dig!"
                    },
                    TimeEntryType = entryType
                }
            };

            return TimeEntryList;
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
