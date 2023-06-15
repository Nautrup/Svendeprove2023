using EmployeeManagement.Common;
using EmployeeManagement.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
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
        public int? Id { get; set; }
        
        public User User { get; set; }

        public int? UserId { get; set; } // Til test
        
        public int CompanyId { get; set; }

        public long Start { get; set; } // Timestamp i sql
        public long End { get; set; } // TimeStamp i sql

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string Status { get; set; }
        public decimal Duration { get; set; }
        public int? GroupingID { get; set; }
        public List<int> MessageIds { get; set; }
        public List<TimeEntryMessage> Messages { get; set; }
        public Location Location { get; set; }

        public int LocationId { get; set; } // Til tesst

        public TimeEntryType TimeEntryType { get; set; }
        public int TimeEntryTypeId { get; set; } // Til test

        public ICommand ClockInUserCommand { get; set; }
        public ICommand ClockOutUserCommand { get; set; }

        public DateTime Date { get; set; }

        /// <summary>
        /// Opretter ny vagt
        /// </summary>
        /// <returns>Den nye vagt</returns>
        public TimeEntry Create()
        {
           
            using (ApiHelper.Client)
            {
                TimeEntry newEntry = new TimeEntry()
                {
                    //User = User,
                    UserId = UserId,
                    Start = Start,
                    End = End,
                    Duration = Duration,
                    Status = Status,
                    //GroupingID = GroupingID,
                    //TimeEntryMessage = TimeEntryMessage,
                    TimeEntryTypeId = TimeEntryTypeId,
                    //Location = Location,
                    LocationId = LocationId,
                };

                var jsonData = JsonConvert.SerializeObject(newEntry);

                string postResponse = ApiHelper.Post("/entry", jsonData);
                return JsonConvert.DeserializeObject<TimeEntry>(postResponse);

            }
           
        }

        /// <summary>
        /// Tilføjer en kommentar på en vagt
        /// </summary>
        /// <param name="comment"></param>
       public void AddComment(string comment)
        {
            TimeEntryMessage newMessage = new TimeEntryMessage(User, this, UnixConversion.ToUnixTime(DateTime.Today), comment);
            newMessage.Create();
        }

        /// <summary>
        /// Frigiver en vagt
        /// </summary>
        public void ReleaseEntry()
        {
            using (ApiHelper.Client)
            {
                TimeEntryPut releaseEntry = new TimeEntryPut(new TimeEntry());
                releaseEntry.UserId = null;

                var jsonData = JsonConvert.SerializeObject(releaseEntry);
                string response = ApiHelper.Put($"/entry/{Id}", jsonData);
                
            }
        }   
    }
}
