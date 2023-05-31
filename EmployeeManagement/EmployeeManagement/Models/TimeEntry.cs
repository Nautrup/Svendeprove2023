using EmployeeManagement.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace EmployeeManagement.Models
{
    public class TimeEntry
    {
        public int ID { get; set; }
        public User User { get; set; }

        public int UserId { get; set; } // Til test
        
        public int CompanyId { get; set; }

        public long Start { get; set; } // Timestamp i sql
        public long End { get; set; } // TimeStamp i sql

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        public int Duration { get; set; }
        public int? GroupingID { get; set; }

        public TimeEntryMessage TimeEntryMessage { get; set; }
        public Location Location { get; set; }

        public int LocationId { get; set; } // Til tesst

        public TimeEntryType TimeEntryType { get; set; }
        public int TimeEntryTypeId { get; set; } // Til test

        public ICommand ClockInUserCommand { get; set; }
        public ICommand ClockOutUserCommand { get; set; }

        public DateTime Date { get; set; }



        /// <summary>
        /// Opretter en ny vagt
        /// </summary>
        public void Create()
        {
            try
            {
                using (ApiHelper.Client)
                {
                    TimeEntry newEntry = new TimeEntry()
                    {
                        //User = User,
                        UserId = UserId,
                        Start = Start,
                        End = End,
                        CompanyId = CompanyId,
                        Duration = Duration,
                        //GroupingID = GroupingID,
                        //TimeEntryMessage = TimeEntryMessage,
                        TimeEntryTypeId = TimeEntryTypeId,
                        //Location = Location,
                        LocationId = LocationId,
                    };

                    var jsonData = JsonConvert.SerializeObject(newEntry);

                    ApiHelper.Post("/entry", jsonData);

                }
            }
            catch (WebException ex)
            {
                throw new WebException(ex.Message);
            }
        }
    }
}
