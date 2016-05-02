using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace NameDayApps
{
    public static class NamedayRepository
    {
        private static List<NameDayModel> allNamedaysCache;

        public static async Task<List<NameDayModel>> GetAllNamedaysAsync()
        {
            if (allNamedaysCache != null)
                return allNamedaysCache;

            var client = new HttpClient(); 
            var stream = await client.GetStreamAsync(
                "http://response.hu/namedays_hu.json");

            var serializer = new DataContractJsonSerializer(typeof(List<NameDayModel>));
            allNamedaysCache = (List<NameDayModel>)serializer.ReadObject(stream);

            return allNamedaysCache;
        }
    }
} 