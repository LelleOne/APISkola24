using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Guid.APICall
{
    public class TimetableViewer
    {
        private static readonly HttpClient client = new HttpClient();

        public static async Task<string> GetUnitGuidFromSchool(string kommun, string skola)
        {
            var requestBody = new
            {
                getTimetableViewerUnitsRequest = new
                {
                    hostName = $"{kommun}.skola24.se"
                }
            };

            var jsonContent = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            content.Headers.Add("X-Scope", "8a22163c-8662-4535-9050-bc5e1923df48");

            var response = await client.PostAsync(
                "https://web.skola24.se/api/services/skola24/get/timetable/viewer/units",
                content
            );

            var responseContent = await response.Content.ReadAsStringAsync();
            dynamic listOfUnitsData = JsonConvert.DeserializeObject(responseContent);
            var units = listOfUnitsData.data.getTimetableViewerUnitsResponse.units;

            foreach (var unit in units)
            {
                if (unit.unitId == skola)
                {
                    return unit.unitGuid;
                }
            }

            return null; // Return null if no match is found
        }
    }
}