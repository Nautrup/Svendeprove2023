using EmployeeManagement.Common;
using EmployeeManagement.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EmployeeManagement.Models
{
    public class UserCreate
    {
        public int UserRoleId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string SurName { get; set; }

        public int? ProfileImage { get; set; }
        public long? HiredDate { get; set; }
        public long? FiredDate { get; set; }
        public List<int> LocationIds { get; set; }

        /// <summary>
        /// Opretter en ny bruger
        /// </summary>
        public void Create(User user)
        {
            using (ApiHelper.Client)
            {

                UserCreate newUser = new UserCreate()
                {
                    UserRoleId = user.UserRole.Id,
                    FirstName = user.FirstName,
                    MiddleName = user.MiddleName,
                    SurName = user.SurName,
                    ProfileImage = user.ProfileImage,
                    HiredDate =user.HiredDate,
                    //FiredDate = user.FiredDate
                };

                if (string.IsNullOrEmpty(MiddleName))
                {
                    MiddleName = null;
                }

                List<int> ids = new List<int>();

                foreach (var location in user.Locations)
                {
                    ids.Add(location.ID);
                }

                newUser.LocationIds = ids;

                var jsonData = JsonConvert.SerializeObject(newUser);

                ApiHelper.Post("/user", jsonData);
                
            }
            
        }

        /// <summary>
        /// Opdatere bruger information
        /// </summary>
        /// <param name="user"></param>
        public void Update(User user)
        {
            // Api til update

            using (ApiHelper.Client)
            {

                UserCreate newUser = new UserCreate()
                {
                    UserRoleId = user.UserRole.Id,
                    FirstName = user.FirstName,
                    MiddleName = user.MiddleName,
                    SurName = user.SurName,
                    ProfileImage = user.ProfileImage,
                    HiredDate = user.HiredDate,
                    //FiredDate = user.FiredDate
                };

                if (string.IsNullOrEmpty(MiddleName))
                {
                    MiddleName = null;
                }

                List<int> ids = new List<int>();

                foreach (var location in user.Locations)
                {
                    ids.Add(location.ID);
                }

                newUser.LocationIds = ids;

                var jsonData = JsonConvert.SerializeObject(newUser);

                ApiHelper.Put($"/user/{user.ID}", jsonData);

            }

        }

        /// <summary>
        /// Sletter en bruger
        /// </summary>
        /// <param name="user"></param>
        public void Delete(User user)
        {

        }

    }
}
