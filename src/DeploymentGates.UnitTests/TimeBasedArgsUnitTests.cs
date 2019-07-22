using DeploymentGates.Models;
using System;
using System.Linq;
using Xunit;

namespace DeploymentGates.UnitTests
{
    public class TimeBasedArgsUnitTests
    {
        [Fact]
        public void TimeBasedArgs_FromJson_DefaultValidDays()
        {
            string json = @"
{
  'startTimeSpan': '12:30',
  'endTimeSpan': '17:00',
  'timeZoneId': 'Eastern Standard Time'
}";
            TimeBasedArgs args = TimeBasedArgs.FromJson(json);

            Assert.Equal(new TimeSpan(12, 30, 0), args.StartTimeSpan);
            Assert.Equal(new TimeSpan(17, 0, 0), args.EndTimeSpan);
            Assert.Equal(TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), args.GetTimeZoneInfo());
            Assert.Empty(args.ValidDaysOfWeek);
        }

        [Fact]
        public void TimeBasedArgs_FromJson_validDaysOfWeek()
        {
            string json = @"
{
  'startTimeSpan': '12:30',
  'endTimeSpan': '17:00',
  'timeZoneId': 'Eastern Standard Time',
  'validDaysOfWeek': [ 'Monday', 'Tuesday' ]
}";
            TimeBasedArgs args = TimeBasedArgs.FromJson(json);

            Assert.Equal(new TimeSpan(12, 30, 0), args.StartTimeSpan);
            Assert.Equal(new TimeSpan(17, 0, 0), args.EndTimeSpan);
            Assert.Equal(TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), args.GetTimeZoneInfo());
            Assert.Equal(2, args.ValidDaysOfWeek.Count());
            Assert.Contains(DayOfWeek.Monday, args.ValidDaysOfWeek);
            Assert.Contains(DayOfWeek.Tuesday, args.ValidDaysOfWeek);
        }

        [Fact]
        public void TimeBasedArgs_IsInWindow()
        {
            string json = @"
{
  'startTimeSpan': '12:30',
  'endTimeSpan': '17:00',
  'timeZoneId': 'Eastern Standard Time'
}";
            TimeBasedArgs args = TimeBasedArgs.FromJson(json);
            Assert.True(args.IsInsideWindow(DateTime.Parse("7/22/2019 18:00:00Z")));
            Assert.False(args.IsInsideWindow(DateTime.Parse("7/22/2019 1:00:00Z")));
        }

        [Fact]
        public void TimeBasedArgs_IsValidDayOfWeek()
        {
            string json = @"
{
  'timeZoneId': 'Eastern Standard Time',
  'validDaysOfWeek': [ 'Monday', 'Tuesday' ]
}";
            TimeBasedArgs args = TimeBasedArgs.FromJson(json);
            Assert.False(args.IsValidDayOfWeek(DateTime.Parse("7/21/2019 18:00:00Z")));
            Assert.True(args.IsValidDayOfWeek(DateTime.Parse("7/22/2019 18:00:00Z")));
            Assert.True(args.IsValidDayOfWeek(DateTime.Parse("7/23/2019 18:00:00Z")));
            Assert.False(args.IsValidDayOfWeek(DateTime.Parse("7/24/2019 18:00:00Z")));
        }


        [Fact]
        public void IsValidDate()
        {
            string json = @"
{
  'timeZoneId': 'Eastern Standard Time',
  'validDaysOfWeek': [ 'Monday' ],
  'invalidDates': [
		'7/22/2019', '7/23/2019'
	]
}";
            TimeBasedArgs args = TimeBasedArgs.FromJson(json);
            Assert.Equal(DateTime.Parse("7/22/2019"), args.InvalidDates.First());
            Assert.True(args.IsValidDate(DateTime.Parse("7/24/2019 18:00:00Z")));
            Assert.False(args.IsValidDate(DateTime.Parse("7/22/2019 18:00:00Z")));
        }

        [Fact]
        public void IsValidDate_0InvalidDates()
        {
            string json = @"
{
  'timeZoneId': 'Eastern Standard Time',
  'validDaysOfWeek': [ 'Monday' ],
  'invalidDates': []
}";
            TimeBasedArgs args = TimeBasedArgs.FromJson(json);
            Assert.True(args.IsValidDate(DateTime.Parse("7/22/2019 18:00:00Z")));
        }
    }
}
