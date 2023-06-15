using EmployeeManagement.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public class UserRole
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<int> PermissionIds { get; set; }
        public void Create()
        {
            using (ApiHelper.Client)
            {
                string jsonData = JsonConvert.SerializeObject(this);

                string postResponse = ApiHelper.Post("/role", jsonData);
            }
        }

        public void AddPermission(int selectedPermissionId)
        {
            using (ApiHelper.Client)
            {
                string jsonData = JsonConvert.SerializeObject(this);

                string postResponse = ApiHelper.Post($"/role/{Id}/permission/{selectedPermissionId}", jsonData);
            }
        }
    }
}
