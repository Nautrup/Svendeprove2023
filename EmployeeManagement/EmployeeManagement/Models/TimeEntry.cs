using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EmployeeManagement.Models
{
    public class TimeEntry
    {
        public int ID { get; set; }
        public User User { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        
        public int Duration { get; set; }
        public int GroupingID { get; set; }
        public TimeEntryMessage TimeEntryMessage { get; set; }
        public Location Location { get; set; }
        public TimeEntryType TimeEntryType { get; set; }

        public ICommand ClockInUserCommand { get; set; }
        public ICommand ClockOutUserCommand { get; set; }

        public DateTime Date { get; set; }
    }
}
