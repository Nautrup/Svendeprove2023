using EmployeeManagement.Common;
using EmployeeManagement.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Services
{
    public static class UserService
    {
        // Heenter en specifik bruger info
        public static List<TimeEntry> GetSpecifikUserInformation(int userId)
        {
            try
            {
                using (ApiHelper.Client)
                {
                    string json = ApiHelper.Get($"/entries?user={userId}");

                    return JsonConvert.DeserializeObject<List<TimeEntry>>(json);

                   
                }
            }
            catch (WebException ex)
            {
                throw new WebException(ex.Message);
            }
        }

        
    }
}
