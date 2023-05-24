using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public class TimeEntryMessage
    {
        public int ID { get; set; }
        public User User { get; set; }
        public TimeEntry TimeEntry { get; set; }
        public DateTime Created { get; set; }
        public string Message { get; set; }

    }
}
