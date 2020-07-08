using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using DeploymentGates.Models;
using System.Linq;

namespace DeploymentGates
{
    /*
    Azure function that takes a body like
     {
      "timeZoneId": "Eastern Standard Time",
      "startTimeSpan": "12:00",
      "endTimeSpan": "17:00",
      "validDaysOfWeek": [ 'Monday', 'Tuesday', 'Wednesday','Thursday','Friday' ],
      "invalidDates": [
		    '1/1/2019', '12/25/2019'
	    ]
    }

    Using the specified timezones, it will 
    - ensure the Time of day is between Start & End
    - ensure the day of th week is withing (ValidDaysOfWeek)
    - ensure it isn't an "Invalid Date'

    */
    public static class DateTimeGates
    {
        [FunctionName("DateTimeGates")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("DateTimeGates - C# HTTP trigger function processed a request.");

            TimeBasedArgs args;
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                args = TimeBasedArgs.FromJson(requestBody);
                log.LogInformation(args.ToString());
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult($"Invalid body. Exception {ex}");
            }

            // Convert UTC time to the user's requested local time
            DateTime localTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, args.GetTimeZoneInfo());
            log.LogVariable("DateTime.UtcNow", DateTime.UtcNow);
            log.LogVariable(nameof(localTime), localTime);

            // Determine if the datetime is valid.
            bool isInsideTimeWindow = args.IsInsideWindow(localTime);
            bool isValidDayOfWeek = !args.ValidDaysOfWeek.Any() || args.IsValidDayOfWeek(localTime);
            bool isValidDate = args.IsValidDate(localTime);

            /// Log data
            log.LogVariable(nameof(isInsideTimeWindow), isInsideTimeWindow);
            log.LogVariable(nameof(isValidDayOfWeek), isValidDayOfWeek);
            log.LogVariable(nameof(isValidDate), isValidDate);

            return new OkObjectResult(new
            {
                meetsCriteria = isInsideTimeWindow && isValidDayOfWeek && isValidDate,
                isInsideTimeWindow,
                isValidDayOfWeek,
                isValidDate,
            });
        }
    }
}