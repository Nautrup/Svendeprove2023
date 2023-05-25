using EmployeeManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Services
{
    public class TimeEntyService
    {
        public static List<TimeEntryType> GetEntryTypes() 
        {
            return new List<TimeEntryType>()
            {
                new TimeEntryType()
                {
                    ID = 1,
                    Name = "Normal"
                },
                   new TimeEntryType()
                {
                    ID = 2,
                    Name = "Syg"
                },

                      new TimeEntryType()
                {
                    ID = 3,
                    Name = "Helligdag"
                },

                         new TimeEntryType()
                {
                    ID = 4,
                    Name = "Ferie"
                },

                            new TimeEntryType()
                {
                    ID = 5,
                    Name = "Noget andet"
                },

                               new TimeEntryType()
                {
                    ID = 6,
                    Name = "SWAG"
                },


            };
        }

        
    }
}
