using EmployeeManagement.Common;
using EmployeeManagement.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EmployeeManagement.Models
{
    public class User : IDataErrorInfo
    {
        public int ID { get; set; }
        public Company Company { get; set; }
        public UserRole UserRole { get; set; }

        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string SurName { get; set; }
        public string FullName { get { return $"{FirstName} {MiddleName} {SurName}"; } }
        public int? ProfileImage { get; set; }

        public List<int> TimeTagCollection { get; set; } // skla have s til sidst
        public long? HiredDate { get; set; }
        public long? FiredDate { get; set; }

        // brug til vise dato i UI
        public DateTime FirstDateOfEmployment { get; set; }
        public DateTime? LastDateOfEmployment { get; set; }

        public List<Location> Locations { get; set; }

        public void UpdateTest()
        {
            // Api til update

            using (ApiHelper.Client)
            {
                User user = new User()
                {
                    UserRole = UserRole,
                    FirstName = FirstName,
                    MiddleName = MiddleName,
                    SurName = SurName,
                    Company = Company,
                    ProfileImage = ProfileImage,
                    HiredDate = HiredDate,
                    FiredDate = FiredDate,
                    Locations = Locations
                };

                var jsonData = JsonConvert.SerializeObject(user);

                ApiHelper.Put($"/user/{ID}", jsonData);

            }
        }

        /// <summary>
        /// Opdatere en bruger
        /// </summary>
        public void Update()
        {
            // Api til update

            using (ApiHelper.Client)
            {
                User currentUser = new User()
                {
                    UserRole = UserRole,
                    FirstName = FirstName,
                    MiddleName = MiddleName,
                    SurName = SurName,
                    Company = Company,
                    ProfileImage = ProfileImage,
                    HiredDate = HiredDate,
                    FiredDate = FiredDate,
                    Locations = Locations
                };

                UserCreate updateUser = new UserCreate();
                updateUser.Update(currentUser);

                //var jsonData = JsonConvert.SerializeObject(currentUser);

                //ApiHelper.Put($"/user/{currentUser.ID}", jsonData);

            }

        }

        /// <summary>
        /// Sletter alle information om en bruger
        /// </summary>
        /// <exception cref="WebException"></exception>
        public void Delete()
        {
            try
            {
                using (ApiHelper.Client)
                {
                    ApiHelper.Delete($"/user/{ID}");

                }
            }
            catch (WebException ex)
            {
                throw new WebException(ex.Message);
            }
        }

        /// <summary>
        /// Tilføjer ny lokation til bruger
        /// </summary>
        /// <param name="location"></param>
        public void AddLocation(Location location)
        {
            Locations.Add(location);
        }

        /// <summary>
        /// Fjerner lokation på en bruger
        /// </summary>
        /// <param name="location"></param>
        public void RemoveLocation(Location location)
        {
            if (Locations.Contains(location))
            {
                Locations.Remove(location);
                //Delete($"/location/{location.ID}");
            }
        }

        #region Data Error
        public string this[string columnName] {
            get
            {
                string error = string.Empty;

                switch (columnName)
                {
                    case nameof(UserRole):
                        if (UserRole == null)
                            error = "Medarbejder skal have en rolle.";
                        break;

                    case nameof(FirstName):
                        if (string.IsNullOrWhiteSpace(FirstName))
                            error = "Navn kan ikke være tomt";
                        if (FirstName?.Length > 50)
                            error = "Navn skal være mindre end 50 tegn.";
                        break;

                    case nameof(SurName):
                        if (string.IsNullOrWhiteSpace(SurName))
                            error = "Efternavn må ikke være tom.";
                        break;
                   

                    case nameof(HiredDate):
                        if (HiredDate < 0)
                            error = "Først ansættelsesdato må ikke være tom.";
                        break;

                    case nameof(Locations):
                        if (Locations.Count < 0)
                            error = "Medarbejder skal være tilknyttet en lokation";
                        break;

                }

                return error;
            }
        }

        public string Error => string.Empty;
        #endregion
    }
}
