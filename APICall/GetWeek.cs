using System.Globalization;
using System;

namespace APISkola24.APICall
{
    class GetWeek
    {
        public static int Week()
        {
            DateTime datevalue = DateTime.Now;

            // If today is Saturday, move back to Monday (5 days)
            // If today is Sunday, move back to Monday (6 days)
            if (datevalue.DayOfWeek == DayOfWeek.Saturday)
            {
                datevalue = datevalue.AddDays(-5);
            }
            else if (datevalue.DayOfWeek == DayOfWeek.Sunday)
            {
                datevalue = datevalue.AddDays(-6);
            }

            // Extract day, month, year
            int Day = datevalue.Day;
            int Month = datevalue.Month;
            int Year = datevalue.Year;

            DateTime dt = new DateTime(Year, Month, Day);
            Calendar cal = new CultureInfo("en-US").Calendar;
            int week = cal.GetWeekOfYear(dt, CalendarWeekRule.FirstDay, DayOfWeek.Monday);

            return week;
        }
    }
}