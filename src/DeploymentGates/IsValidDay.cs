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
    public static class IsValidDay
    {
        [FunctionName("IsValidDay")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("TimeOfDay - C# HTTP trigger function processed a request.");

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

            bool isValidDayOfWeek = (args.ValidDaysOfWeek.Count() == 0 || args.IsValidDayOfWeek(localTime));
            log.LogVariable(nameof(isValidDayOfWeek), isValidDayOfWeek);

            bool isValidDate = args.IsValidDate(localTime);
            log.LogVariable(nameof(isValidDate), isValidDate);

            return new OkObjectResult(isValidDayOfWeek && isValidDate);
        }
    }
}
