using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public class ApiAuthenticationData
    {
        public ApiAceessToken accessToken { get; set; }

        public ApiAuthenticationData()
        {
            accessToken = new ApiAceessToken();
        }
    }
}
