using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

public static class ExtensionMethods
{
    public static void LogVariable<T>(this ILogger log, string name, T item)
    {
        log.LogInformation($"{name}: '{item}'");
    }
}
