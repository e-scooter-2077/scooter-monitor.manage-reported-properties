using System;
using System.Collections.Generic;
using System.Text.Json;
using Azure;
using Azure.DigitalTwins.Core;
using Azure.Identity;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EScooter.Monitor.ManageReportedProperties
{
    public static class ManageReportedProperties
    {
        public record ScooterStatusChanged(
            string Id,
            bool? Locked,
            string UpdateFrequency,
            double? MaxSpeed,
            bool? Standby);

        [Function("manage-properties")]
        public static async void ManageProperties([ServiceBusTrigger("%TopicName%", "%SubscriptionName%", Connection = "ServiceBusConnectionString")] string mySbMsg, FunctionContext context)
        {
            var logger = context.GetLogger("Function");

            var scooterStatusChanged = JsonConvert.DeserializeObject<ScooterStatusChanged>(mySbMsg);
            var scooterId = scooterStatusChanged.Id;

            logger.LogInformation("id: " + scooterId);
            logger.LogInformation("message: " + mySbMsg);

            var patch = new JsonPatchDocument();
            if (scooterStatusChanged.Locked != null)
            {
                patch.AppendReplace("/Locked", scooterStatusChanged.Locked);
            }
            if (scooterStatusChanged.UpdateFrequency != null)
            {
                patch.AppendReplace("/UpdateFrequency", ConvertTimeSpanStringToSeconds(scooterStatusChanged.UpdateFrequency));
            }
            if (scooterStatusChanged.MaxSpeed != null)
            {
                patch.AppendReplace("/MaxSpeed", ConvertSpeed(scooterStatusChanged.MaxSpeed.Value));
            }
            if (scooterStatusChanged.Standby != null)
            {
                patch.AppendReplace("/Standby", scooterStatusChanged.Standby);
            }
            logger.LogInformation($"Patch: ${patch}");

            var digitalTwinUrl = "https://" + Environment.GetEnvironmentVariable("AzureDTHostname");
            var credential = new DefaultAzureCredential();
            var digitalTwinsClient = new DigitalTwinsClient(new Uri(digitalTwinUrl), credential);
            await digitalTwinsClient.UpdateDigitalTwinAsync(scooterId, patch);
            logger.LogInformation($"Updated reported properties of twin: {scooterId}\n");
        }

        private static int ConvertTimeSpanStringToSeconds(string timespan) =>
            (int)TimeSpan.Parse(timespan).TotalSeconds;

        private static double ConvertSpeed(double speedInMS)
        {
            return Math.Round(speedInMS * 3.6, 4);
        }
    }
}
