using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GetSignature.APICall
{
    class GetSig
    {
        private static readonly HttpClient client = new HttpClient();

        public static async Task<string> GetSigAsync(string id)
        {
            try
            {
                var url = "https://web.skola24.se/api/encrypt/signature";
                var requestBody = new { signature = id };
                var jsonBody = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                // Add headers
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("X-Scope", "8a22163c-8662-4535-9050-bc5e1923df48");

                HttpResponseMessage response = await client.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Network response was not ok: " + response.ReasonPhrase);
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<dynamic>(responseBody);

                return responseData?.data?.signature;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in GetSigAsync: " + ex.Message);
                return null;
            }
        }
    }
}