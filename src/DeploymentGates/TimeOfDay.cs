using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using DeploymentGates.Models;
using System.Linq;

namespace DeploymentGates
{
    public static class TimeOfDay
    {
        [FunctionName("TimeOfDay")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("TimeOfDay - C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            TimeBasedArgs args;
            try
            {
                args = TimeBasedArgs.FromJson(requestBody);

                log.LogInformation($"start => {args.StartTimeSpan}");
                log.LogInformation($"end => {args.EndTimeSpan}");
                log.LogInformation($"timeZoneId => {args.TimeZoneId}");
                log.LogInformation($"TimeZone => {args.GetTimeZoneInfo()}");
                log.LogInformation($"ValidDaysOfWeek => {string.Join(", ", args.ValidDaysOfWeek)}");
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult($"Invalid body. Exception {ex}");
            }

            // Convert UtC time to the user's requested local time
            log.LogInformation($"UtcNow: {DateTime.UtcNow}");
            DateTime localTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, args.GetTimeZoneInfo());
            log.LogInformation($"Local Time: {localTime}");

            // Return true if the current time is within the specified window
            bool insideWindow = args.IsInsideWindow(localTime);
            bool isValidDayOfWeek = (args.ValidDaysOfWeek.Count() == 0 || args.IsValidDayOfWeek(localTime));
            return new OkObjectResult(insideWindow && isValidDayOfWeek);
        }
    }
}
