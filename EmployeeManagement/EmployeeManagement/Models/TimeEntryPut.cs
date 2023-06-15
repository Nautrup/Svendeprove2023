using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public class TimeEntryPut
    {
        public int? Id { get; set; }
        public int? UserId { get; set; }
        public long? Start { get; set; }
        public long End { get; set; }
        public decimal? Duration { get; set; }
        public int? GroupingId { get; set; }
        public int? LocationId { get; set; }
        public int? TimeEntryTypeId { get; set; }

        public TimeEntryPut(TimeEntry entry)
        {
            Id = entry.Id;
            UserId = entry.UserId;
            Start = entry.Start;
            End = entry.End;
            Duration = entry.Duration;
            GroupingId = entry.GroupingID;
            LocationId = entry.LocationId;
            TimeEntryTypeId = entry.TimeEntryTypeId;
        }
    }
}
