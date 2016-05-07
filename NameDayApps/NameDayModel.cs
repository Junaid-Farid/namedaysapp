using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NameDayApps
{
    [DataContract]
    public class NameDayModel
    {
        [DataMember]
        public int Month { get; set; } // if we are setting the property from the constructor then we don't need to write the set keyword in property so remove
        [DataMember]
        public int Day { get; set; } // but when working with the Json we have to set the setter
        [DataMember]
        public IEnumerable<string> Names { get; }
        
        public NameDayModel(int day, int month, IEnumerable<string> names)
        {
            Month = month;
            Day = day;
            Names = names;
        }

        //public string NamesAsString
        //{
        //    get { return string.Join(", ", Names); }
        //}

        // In  c sharp 6 we can write the above code as
        public string NamesAsString => string.Join(", ", Names);

        //default constructor
        //public NameDayModel() { }
    }
}
