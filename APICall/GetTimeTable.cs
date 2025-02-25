using System;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using GetSignature;
using GetNyckel;
using SchoolYear;
using Guid;
using System.IO;
using APISkola24.APICall;
using GetNyckel.APICall;
using GetSignature.APICall;
using Guid.APICall;
using SchoolYear.APICall;

class GetTimeTable
{
    static async Task Main()
    {
        DateTime datevalue = DateTime.Now;

        string signature = await GetSig.GetSigAsync("Teim22");
        string Key = await GetKey.KeyGet();
        string Year = await SchoolYearService.GetSchoolYearAsync("Orebro.skola24.se");
        var UnitGuid = await TimetableViewer.GetUnitGuidFromSchool("Orebro", "Tullängsgymnasiet");

        int Week = GetWeek.Week();
        int Day = 1;
        switch (datevalue.DayOfWeek)
        {
            case DayOfWeek.Monday:
                Day = 1; break;
            case DayOfWeek.Tuesday:
                Day = 2; break;
            case DayOfWeek.Wednesday:
                Day = 3; break;
            case DayOfWeek.Thursday:
                Day = 4; break;
            case DayOfWeek.Friday:
                Day = 5; break;
            case DayOfWeek.Saturday:
                Day = 1; break;
            case DayOfWeek.Sunday:
                Day = 1; break;
        }
        int CurYear = datevalue.Year;

        await GetTimetable(Year, "Orebro", signature, Key, CurYear, Week, Day, UnitGuid);
    }
    public static async Task GetTimetable(string schoolYear, string kommun, string signature, string key, int year, int week, int dayOfTheWeek, string unitGuid)
    {
        try
        {
            using (var client = new HttpClient())
            {
                var url = "https://web.skola24.se/api/render/timetable";
                client.DefaultRequestHeaders.Add("X-Scope", "8a22163c-8662-4535-9050-bc5e1923df48");

                var requestBody = new
                {
                    renderKey = key,
                    selection = signature,
                    scheduleDay = dayOfTheWeek,
                    week = week,
                    year = year,
                    host = $"{kommun}.skola24.se",
                    unitGuid = unitGuid,
                    schoolYear = schoolYear,
                    startDate = (string)null,
                    endDate = (string)null,
                    blackAndWhite = false,
                    width = 125,
                    height = 550,
                    selectionType = 4,
                    showHeader = false,
                    periodText = "",
                    privateFreeTextMode = false,
                    privateSelectionMode = (object)null,
                    customerKey = ""
                };

                var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Network response was not ok: " + response.StatusCode);
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                dynamic data = JsonConvert.DeserializeObject(jsonResponse);
                var lessons = data.data.lessonInfo ?? new object[] { };

                foreach (var lesson in lessons)
                {
                    string room = (lesson.texts != null && lesson.texts.Count > 2) ? lesson.texts[2] : "N/A";
                    string startTime = lesson.timeStart ?? "Unknown";
                    string endTime = lesson.timeEnd ?? "Unknown";

                    Console.WriteLine($"Sal: {room} Start tid: {startTime} Slut tid: {endTime}");
                }

                // Optionally write the data to a file
                // File.WriteAllText("jsonFile.json", JsonConvert.SerializeObject(data, Formatting.Indented));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}
