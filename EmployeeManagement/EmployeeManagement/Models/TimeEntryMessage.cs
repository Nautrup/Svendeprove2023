using EmployeeManagement.Common;
using EmployeeManagement.Services;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EmployeeManagement.Models
{
    public class TimeEntryMessage
    {
        public int ID { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
        public TimeEntry TimeEntry { get; set; }
        public long CreatedAt { get; set; }

        public string Message { get; set; }

        public DateTime CreatedAtToDateTime { get; set; }
        public TimeEntryMessage(User user, TimeEntry timeEntry, long createdAt, string message)
        {
            User = user;
            TimeEntry = timeEntry;
            CreatedAt = createdAt;
            Message = message;
            CreatedAtToDateTime = UnixConversion.UnixTimeStampToDateTime(createdAt);
        }

        /// <summary>
        /// Opretter en besked til en vagt
        /// </summary>
        /// <param name="entryId">ID på vagten</param>
        public void Create()
        {
            using (ApiHelper.Client)
            {

                var jsonData = JsonConvert.SerializeObject(this);

                ApiHelper.Post($"/entry/{TimeEntry.ID}/messages", jsonData);

            }
        }
    }
}
