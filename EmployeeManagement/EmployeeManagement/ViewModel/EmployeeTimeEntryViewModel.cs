﻿using EmployeeManagement.Common;
using EmployeeManagement.Models;
using EmployeeManagement.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace EmployeeManagement.ViewModel
{
    public class EmployeeTimeEntryViewModel : ViewModelBase
    {
        private ExceptionHttpHelper exceptionHttpHelper;
        private int DUMMYTESTLOCATIONID = 3;

        public EmployeeTimeEntryViewModel()
        {
            LoadTimeEntryTypes(CurrentLoggedInUser.Company.ID);

            LoadTimeEntries(CurrentLoggedInUser.Locations[0].ID);
        }

        public ICommand ClockInUserCommand { get; set; }
        public ICommand ClockOutUserCommand { get; set; }

        #region ObservableCollection
        public ObservableCollection<User> UserCollection { get; set; } = new();

        public ObservableCollection<TimeEntryType> TimeEntryTypeCollection { get; set; } = new();

        public ObservableCollection<TimeEntry> TimeEntryCollection { get; set; } = new();

        public ObservableCollection<TimeEntry> TimeEntryOutCollection { get; set; } = new();
        #endregion

        // Det indstastede bruger ID i textbox
        public int UserID {
            get { return _userID; }
            set
            {
                _userID = value;
                OnPropertyChanged(nameof(UserID));
            }
        }

        // Valgte entry type
        public TimeEntryType SelectedTimeEntryType {
            get { return _selectedTimeEntryType; }
            set
            {
                _selectedTimeEntryType = value;
                OnPropertyChanged(nameof(SelectedTimeEntryType));
            }
        }

        private DateTime CurrentDate = DateTime.UtcNow;
        
        // Stempler bruger ind
        private void ClockInUser(TimeEntry currentEntry)
        {
            try
            {
                if (currentEntry == null)
                {
                    throw new MissingFieldException(nameof(currentEntry.User.ID));
                }

                // ERSSTATTES AF SQL TIL INSERT NY STEMPLING
                MessageBox.Show($"{currentEntry.User.FullName}\nStemplet ind godkendt!\n{DateTime.Now}", "Godkendt stempling registeret", MessageBoxButton.OK, MessageBoxImage.Information);
                TimeEntryCollection.Remove(currentEntry);
                TimeEntryOutCollection.Add(currentEntry);
            }
            catch (MissingFieldException exception)
            {
                MessageBox.Show($"{exception.Message} mangler", "Information mangler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        private void ClockOutUser(TimeEntry currentEntry) 
        {
            MessageBox.Show($"{currentEntry.User.FullName}\n\nDin udstempliong blev godkendt\n\nTid:{DateTime.Now}", "Godkendt stempling registeret", MessageBoxButton.OK, MessageBoxImage.Information);
            TimeEntryOutCollection.Remove(currentEntry);
        }

        // Henter vores brugere
        private User GetUser(int? userId)
        {
            try
            {
                using (ApiHelper.Client)
                {
                    UserCollection.Clear();

                    string json = ApiHelper.Get($"/user/{userId}");

                    return JsonConvert.DeserializeObject<User>(json);
                 
                }
            }
            catch (WebException ex)
            {
                exceptionHttpHelper = new ExceptionHttpHelper(ex);

                MessageBox.Show($"{exceptionHttpHelper.StatusCode}\n{exceptionHttpHelper.StatusDescription}\n\n{exceptionHttpHelper.ErrorMessage}", "Fejl opstået");
                return null;
            }
        }

        private void LoadTimeEntries(int locationId)
        {
            try
            {
                using (ApiHelper.Client)
                {
                    TimeEntryCollection.Clear();

                    string json = ApiHelper.Get($"/entries?location={locationId}");

                    var list = JsonConvert.DeserializeObject<List<TimeEntry>>(json);
                    // Looper gennem hver bruger, tilføjer lokationer derefter i collection af brugere
                    foreach (var entry in list)
                    {
                        entry.StartDate = UnixConversion.UnixTimeStampToDateTime(entry.Start);
                        entry.EndDate = UnixConversion.UnixTimeStampToDateTime(entry.End);
                        entry.Messages = GetMessages(entry.Id);
                        
                        if (entry.StartDate.Date == CurrentDate.Date)
                        {
                            entry.User = GetUser(entry.UserId);
                            entry.ClockInUserCommand = new RelayCommand(o => ClockInUser(entry));
                            entry.ClockOutUserCommand = new RelayCommand(o => ClockOutUser(entry));
                            TimeEntryCollection.Add(entry);
                        }
                        
                    }
                }
            }
            catch (WebException ex)
            {
                exceptionHttpHelper = new ExceptionHttpHelper(ex);

                MessageBox.Show($"{exceptionHttpHelper.StatusCode}\n{exceptionHttpHelper.StatusDescription}\n\n{exceptionHttpHelper.ErrorMessage}", "Fejl opstået");
            }
        }

        // Henter vores TimeEntryTypes
        private void LoadTimeEntryTypes(int companyId)
        {
            // rydder kollektionen
            TimeEntryTypeCollection.Clear();
            try
            {
                // Henter json svar fra api
                string response = ApiHelper.Get($"/entryType");

                if (!string.IsNullOrEmpty(response))
                {
                    // laver json svar om til c# object
                    List<TimeEntryType> types = JsonConvert.DeserializeObject<List<TimeEntryType>>(response);
                    // Looper gennem dem og tilføjet til kollektionen
                    foreach (var type in types) // old TimeEntyService.GetEntryTypes())
                    {
                        TimeEntryTypeCollection.Add(type);
                    }
                }
            }
            catch (WebException ex)
            {
                exceptionHttpHelper = new ExceptionHttpHelper(ex);
                
                MessageBox.Show($"{exceptionHttpHelper.StatusCode}\n{exceptionHttpHelper.StatusDescription}\n\n{exceptionHttpHelper.ErrorMessage}", $"Fejl opstået");
            }
        }

        // Henter TimeEntryMessages
        private List<TimeEntryMessage> GetMessages(int? entryId)
        {
            using (ApiHelper.Client)
            {
                string json = ApiHelper.Get($"/entry/{entryId}/messages");

                return JsonConvert.DeserializeObject<List<TimeEntryMessage>>(json);
                
            }
        }

        #region Private Variables
        private TimeEntryType _selectedTimeEntryType;
        private int _userID;
        private User _foundUser;
        #endregion
    }
}
