using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class NamedayRepository
    {
        private static List<NameDayModel> allNamedaysCache;   
        
        public static async Task<List<NameDayModel>> GetAllNamedaysAsync()
        {
            
            if (allNamedaysCache != null)
                return allNamedaysCache;

            var client = new HttpClient(); 
            var stream = await client.GetStringAsync(
                "http://www.response.hu/namedays_hu.json");

            allNamedaysCache = JsonConvert.DeserializeObject<List<NameDayModel>>(stream);
            
            //            allNamedaysCache = JsonConvert.
            //var serializer = new DataContractJsonSerializer(typeof(List<NameDayModel>));
            //allNamedaysCache = (List<NameDayModel>)serializer.ReadObject(stream);

            return allNamedaysCache;
        }
        public static async Task<string> GetTodaysNamesAsStringAsync()
        {
            var allNamedays = await GetAllNamedaysAsync();
            var todaysNamedays = allNamedays.Find(
                d => d.Day == DateTime.Now.Day && d.Month == DateTime.Now.Month);

            return todaysNamedays?.NamesAsString;
        }
    }
} 