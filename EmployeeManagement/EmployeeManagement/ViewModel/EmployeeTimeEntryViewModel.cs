using EmployeeManagement.Common;
using EmployeeManagement.Models;
using EmployeeManagement.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace EmployeeManagement.ViewModel
{
    public class EmployeeTimeEntryViewModel : ViewModelBase
    {

        public EmployeeTimeEntryViewModel()
        {
            ClockInUserCommand = new RelayCommand(o => ClockInUser(FoundUser));
            LoadTimeEntryTypes();
            LoadUsers();
            LoadTimeEntries();
        }

        public ICommand ClockInUserCommand { get; set; }
        public ICommand ClockOutUserCommand { get; set; }

        public ObservableCollection<User> UserCollection { get; set; } = new();

        public ObservableCollection<TimeEntryType> TimeEntryTypeCollection { get; set; } = new();
        public ObservableCollection<TimeEntry> TimeEntryCollection { get; set; } = new();

        public ObservableCollection<TimeEntry> TimeEntryOutCollection { get; set; } = new();

        // Det indstastede bruger ID i textbox
        public int UserID {
            get { return _userID; }
            set
            {
                _userID = value;
                //CheckForExistingUser();
                OnPropertyChanged(nameof(UserID));
            }
        }

        // Den fundet bruger
        public User FoundUser {
            get { return _foundUser; }
            set { _foundUser = value; OnPropertyChanged(nameof(FoundUser)); }
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

        // Nuværende lokation
        public int CurrentLocationID { get; set; } = 1;
        
        // Stempler bruger ind
        private void ClockInUser(User user)
        {
            try
            {
                if (user == null)
                {
                    throw new MissingFieldException(nameof(UserID));
                }

                CheckForExistingUser(user);

                // ERSSTATTES AF SQL TIL INSERT NY STEMPLING
                MessageBox.Show($"{FoundUser.FullName}\nID:{user.ID}\nStemplet ind godkendt!\n{DateTime.Now}", "Godkendt stempling registeret", MessageBoxButton.OK, MessageBoxImage.Information);
                
            }
            catch (MissingFieldException exception)
            {
                MessageBox.Show($"{exception.Message} mangler", "Indtastning af medarbejder ID mangler");
            }
            
        }

        private void ClockOutUser(User user) 
        {
            MessageBox.Show($"{FoundUser.FullName}\n\nDin udstempliong blev godkendt\n\nTid:{DateTime.Now}", "Godkendt stempling registeret", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Henter TimeEntries
        private void LoadTimeEntries()
        {
            TimeEntryCollection.Clear();

            foreach (TimeEntry timeEntry in UserService.GetTimeEntries(entryType: SelectedTimeEntryType).Where(x => x.Location.ID == CurrentLocationID))
            {
                timeEntry.ClockInUserCommand = new RelayCommand(o => 
                { 
                    ClockInUser(timeEntry.User);
                    timeEntry.ClockOutUserCommand = new RelayCommand(o => { ClockOutUser(timeEntry.User); TimeEntryOutCollection.Remove(timeEntry); });
                    TimeEntryOutCollection.Add(timeEntry);
                    TimeEntryCollection.Remove(timeEntry);
                });

                TimeEntryCollection.Add(timeEntry);
            }
        }

        // Tjekker om vi har en bruger med det indastede Medarbejder ID
        private void CheckForExistingUser(User userId)
        {
            for (int i = 0; i < UserCollection.Count; i++)
            {
                if (UserCollection[i].ID == userId.ID)
                {
                    FoundUser = UserCollection[i];
                    return;
                }

                FoundUser = null;
            }
        }

        // Henter vores brugere
        private void LoadUsers()
        {
            UserCollection.Clear();

            foreach (var user in UserService.GetUsers())
            {
                UserCollection.Add(user);
            }
        }

        // Henter vores TimeEntryTypes
        private void LoadTimeEntryTypes()
        {
            TimeEntryTypeCollection.Clear();

            foreach (var type in TimeEntyService.GetEntryTypes())
            {
                TimeEntryTypeCollection.Add(type);
            }
        }

        #region Private Variables
        private TimeEntryType _selectedTimeEntryType;
        private int _userID;
        private User _foundUser;
        #endregion
    }
}
