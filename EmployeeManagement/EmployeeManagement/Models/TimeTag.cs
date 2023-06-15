using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public class TimeTag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Number { get; set; }
        public List<int> RuleIds { get; set; }

    }
}
