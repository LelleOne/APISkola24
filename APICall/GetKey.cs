using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace GetNyckel.APICall
{
    class GetKey
    {
        private static readonly HttpClient client = new HttpClient();

        public static async Task<string> KeyGet()
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://web.skola24.se/api/get/timetable/render/key");
                request.Headers.Add("X-Scope", "8a22163c-8662-4535-9050-bc5e1923df48");
                request.Content = new StringContent("{}", Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                JObject jsonResponse = JObject.Parse(responseBody);

                string key = jsonResponse["data"]?["key"]?.ToString();
                return key;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in GetKey: " + ex.Message);
                throw;
            }
        }
    }
}
