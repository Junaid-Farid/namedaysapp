using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NameDayApps
{
    class MainPageData : INotifyPropertyChanged
    {
        //before the filtering we need to keep the data that is unfilter
        private List<NameDayModel> _allNamedays = new List<NameDayModel>();

        //public string Greeting { get; set; } = "Hello World!"; // this is the c sharp 6 feature to set the like in the constructor
        //change the property when the changes done from the list from UI
        public string _greeting = "Hello World!";
        public string Greeting
        {
            get { return _greeting; }
            set
            {
                if (value == _greeting)
                    return;
                _greeting = value;
                //if (PropertyChanged != null)
                //    PropertyChanged(this, new PropertyChangedEventArgs("Greeting")); //here Greeting is the Property that value changes
                //In C Sharp 6 we can also write the above code using the conditional operator as
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Greeting))); // here we replace the above hard code to the nameof keywod parameter

            }
        }
        
        //public List<NameDayModel> Namedays { get; set; }
        //but it would not changed the UI when filtering so 
        public ObservableCollection<NameDayModel> Namedays { get; set; }

        public MainPageData()
        {
            Namedays =  new ObservableCollection<NameDayModel>();

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
            //get all the data then perform the filtring 
            _allNamedays = await NamedayRepository.GetAllNamedaysAsync();

            //perform the filtering in the structor it will load the all data first time
            PerformFiltering();
        }

        private NameDayModel _selectedNameday;

        public event PropertyChangedEventHandler PropertyChanged;

        public NameDayModel SelectedNameday
        {
            get { return _selectedNameday; }
            set
            {
                _selectedNameday = value;
                if (_selectedNameday == null)
                {
                    Greeting = "Hello World! ";
                }
                else
                    Greeting = "Hello " + value.NamesAsString;
            }
        }

        private string _filter;
        public string Filter
        {
            get { return _filter; }
            set
            {
                if (value == _filter)
                    return;
                _filter = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Filter)));

                //Perform Filtering
                PerformFiltering();
            }
        }

        //Perform filtering method
        private void PerformFiltering()
        {
            if (_filter == null)
                _filter = "";

            //convert the all to lovercase to enhance the search capibility
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
    }
}
