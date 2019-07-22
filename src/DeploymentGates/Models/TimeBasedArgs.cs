using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DeploymentGates.Models
{
    public class TimeBasedArgs
    {
        public TimeSpan StartTimeSpan { get; set; }

        public TimeSpan EndTimeSpan { get; set; }

        public string TimeZoneId { get; set; }

        public IEnumerable<DayOfWeek> ValidDaysOfWeek { get; set; } = new List<DayOfWeek>();

        public TimeZoneInfo GetTimeZoneInfo()
        {
            return TimeZoneInfo.FindSystemTimeZoneById(this.TimeZoneId);
        }

        public static TimeBasedArgs FromJson(string json)
        {
            // Parse JSON to object
            return JsonConvert.DeserializeObject<TimeBasedArgs>(json);
        }

        public bool IsInsideWindow(DateTime dateTime)
        {
            TimeSpan time = dateTime.TimeOfDay;
            return ((time > this.StartTimeSpan) && (time < this.EndTimeSpan));
        }

        public bool IsValidDayOfWeek(DateTime dateTime)
        {
           return this.ValidDaysOfWeek.Contains(dateTime.DayOfWeek);
        }

    }
}
