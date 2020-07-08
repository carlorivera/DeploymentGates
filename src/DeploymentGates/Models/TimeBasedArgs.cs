using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

namespace DeploymentGates.Models
{
    public class TimeBasedArgs
    {
        public TimeSpan StartTimeSpan { get; set; }

        public TimeSpan EndTimeSpan { get; set; }

        public string TimeZoneId { get; set; }

        public IEnumerable<DayOfWeek> ValidDaysOfWeek { get; set; } = new List<DayOfWeek>();

        public IEnumerable<DateTime> InvalidDates { get; set; } = new List<DateTime>();

        public TimeZoneInfo GetTimeZoneInfo()
        {
            return TimeZoneInfo.FindSystemTimeZoneById(this.TimeZoneId);
        }

        public static TimeBasedArgs FromJson(string json)
        {
            // Parse JSON to object
            return ResolveInvalidDates(JsonConvert.DeserializeObject<TimeBasedArgs>(json));
        }

        private static TimeBasedArgs ResolveInvalidDates(TimeBasedArgs args)
        {
            var newInvalidDates = new List<DateTime>();
            if (args.InvalidDates == null || !args.InvalidDates.Any()) return args;

            foreach (var invalidDate in args.InvalidDates)
            {
                newInvalidDates.Add(invalidDate.Year.Equals(9999)
                    ? new DateTime(DateTime.UtcNow.Year, invalidDate.Month, invalidDate.Day)
                    : invalidDate);
            }
            
            args.InvalidDates = newInvalidDates;
            return args;
        }

        public bool IsInsideWindow(DateTime dateTime)
        {
            if (StartTimeSpan == default && EndTimeSpan == default)
            {
                return true;
            }

            TimeSpan time = dateTime.TimeOfDay;
            return time > StartTimeSpan && time < EndTimeSpan;
        }

        public bool IsValidDayOfWeek(DateTime dateTime)
        {
           return ValidDaysOfWeek.Contains(dateTime.DayOfWeek);
        }

        /// <summary>
        /// Ensure the date is not in the InvalidDates list.
        /// </summary>
        public bool IsValidDate(DateTime dateTime)
        {
            return !InvalidDates.Contains(dateTime.Date);
        }

        public override string ToString()
        {
            return
                $"StartTimeSpan: '{StartTimeSpan}{Environment.NewLine}" +
                $"EndTimeSpan: '{EndTimeSpan}{Environment.NewLine}" +
                $"TimeZoneId: '{TimeZoneId}{Environment.NewLine}" +
                $"GetTimeZoneInfo(): '{GetTimeZoneInfo()}{Environment.NewLine}" +
                $"ValidDaysOfWeek: '{string.Join(", ", ValidDaysOfWeek)}{Environment.NewLine}" +
                $"InvalidDates: '{string.Join(", ", InvalidDates)}{Environment.NewLine}";
        }
    }
}
