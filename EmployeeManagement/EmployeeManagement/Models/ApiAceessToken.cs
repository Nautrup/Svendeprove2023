using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public class ApiAceessToken
    {
        public string token { get; set; }
        public string expiresAt { get; set; }
        public string issuedAt { get; set; }
    }
}
