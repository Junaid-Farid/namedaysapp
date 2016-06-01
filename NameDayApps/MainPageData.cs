using Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Appointments;
using Windows.ApplicationModel.Contacts;
using Windows.ApplicationModel.Email;

namespace NameDayApps
{
    public class MainPageData : ObserableObject
    {
        //before the filtering we need to keep the data that is un-filter
        private List<NameDayModel> _allNamedays = new List<NameDayModel>();

        //public string Greeting { get; set; } = "Hello World!"; // this is the c sharp 6 feature to set the like in the constructor
        //change the property when the changes done from the list from UI
        public string _greeting = "Hello World!";
        public string Greeting
        {
            get { return _greeting; }
            set { Set(ref _greeting, value); }             
        }

        //public List<NameDayModel> Namedays { get; set; }
        //but it would not changed the UI when filtering so 
        public ObservableCollection<NameDayModel> Namedays { get; set; }

        //for contact
        //means Contacts property to bind the list too
        public ObservableCollection<ContactInfoClass> Contacts { get; } =
            new ObservableCollection<ContactInfoClass>();

        public Settings Settings { get; } = new Settings();

        public MainPageData()
        {
            AddReminderCommand = new AddReminderCommand(this);

            Namedays = new ObservableCollection<NameDayModel>();

            //only for the designer and debugging
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                

                //the below is the fake data we have replaced this to the Json data downloaded form the server
                for (int month = 1; month <= 12; month++)
                {
                    _allNamedays.Add(new NameDayModel(month, 1, new string[] { "Adam" }));

                    _allNamedays.Add(new NameDayModel(month, 24, new string[] { "Eve", "Andrew" }));
                }
                PerformFiltering(); //here the filter is still empty so the PerformFiltering method should copy every items in the _allNamedays List

            }
            else
            {
                //we have created the below method to load the Async data from the server
                LoadData();
            }
        }
        public async void LoadData()
        {
            //get all the data then perform the filtering 
            _allNamedays = await NamedayRepository.GetAllNamedaysAsync();

            //perform the filtering in the structure it will load the all data first time
            PerformFiltering();
        }

        private NameDayModel _selectedNameday; 

        public NameDayModel SelectedNameday
        {
            get { return _selectedNameday; }
            set
            {
                _selectedNameday = value;
                if (_selectedNameday == null)
                {
                    Greeting = "Hello World!";
                }
                else
                    Greeting = "Hello " + value.NamesAsString;

                //update the contact
                UpdateContacts();
                AddReminderCommand.FireCanExecuteChanged();
            }
        }

        private async void UpdateContacts()
        {
            Contacts.Clear();

            if(SelectedNameday != null)
            {
                var contactStore =
                    await ContactManager.RequestStoreAsync(ContactStoreAccessType.AllContactsReadOnly);
                
                // Find all contacts  
                foreach (var name in SelectedNameday.Names)  
                    foreach (var contact in await contactStore.FindContactsAsync(name)) 
                        Contacts.Add(new ContactInfoClass(contact)); 
            }
        }

        private string _filter;
        public string Filter
        {
            get { return _filter; }
            set
            {
                if (Set(ref _filter, value))
                    PerformFiltering();
            }
        }

        //Perform filtering method
        private void PerformFiltering()
        {
            if (_filter == null)
                _filter = "";

            //convert the all to lowercase to enhance the search capability
            var lowerCaseFilter = Filter.ToLowerInvariant().Trim();

            var result =
                _allNamedays.Where(d => d.NamesAsString.ToLowerInvariant()
                .Contains(lowerCaseFilter))
                .ToList();

            var toRemove = Namedays.Except(result).ToList();

            foreach (var x in toRemove)
                Namedays.Remove(x);

            var resultCount = result.Count;

            for (int i = 0; i < resultCount; i++)
            {
                var resultItem = result[i];
                if (i + 1 > Namedays.Count || !Namedays[i].Equals(resultItem))
                    Namedays.Insert(i, resultItem);
            }
        }

        public async Task SendEmailAsync(Contact contact)
        {
            if (contact == null || contact.Emails.Count == 0)
                return;
            var msg = new EmailMessage();
            msg.To.Add(new EmailRecipient(contact.Emails[0].Address));
            msg.Subject = "Happy Nameday";

            await EmailManager.ShowComposeNewEmailAsync(msg);
        }

        public AddReminderCommand AddReminderCommand { get; }

        public async void AddReminderToCalendarAsync()
        {
            var appointment = new Appointment();
            appointment.Subject = "Nameday Reminder for " + SelectedNameday.NamesAsString;
            appointment.AllDay = true;
            appointment.BusyStatus = AppointmentBusyStatus.Free;
            var dateThisYear = new DateTime(DateTime.Now.Year, SelectedNameday.Month, SelectedNameday.Day);

            appointment.StartTime = dateThisYear < DateTime.Now ? dateThisYear.AddYears(1) : dateThisYear;

            //appointment.Recurrence = new AppointmentRecurrence //TODO: recurrence
            //{
            //    Day = (uint)SelectedNameday.Day,
            //    Month = (uint)SelectedNameday.Month,
            //    Unit = AppointmentRecurrenceUnit.Yearly,
            //    Interval = 1
            //};

            await AppointmentManager.ShowEditNewAppointmentAsync(appointment);
        }
    }
    public class AddReminderCommand: System.Windows.Input.ICommand
    {
        private MainPageData _mpd;

        public AddReminderCommand(MainPageData mpd)
        {
            _mpd = mpd;
        }
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => _mpd.SelectedNameday != null;

        public void Execute(object parameter) => _mpd.AddReminderToCalendarAsync();

        public void FireCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
