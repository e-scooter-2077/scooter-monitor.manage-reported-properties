using System;
using System.Collections.Generic;
using System.Text.Json;
using Azure;
using Azure.DigitalTwins.Core;
using Azure.Identity;
using EScooter.DigitalTwins.Commons;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EScooter.ScooterMonitor.ManageReportedProperties
{
    public static class ManageReportedProperties
    {
        [Function("manage-properties")]
        public static async void ManageProperties([ServiceBusTrigger("%TopicName%", "%SubscriptionName%", Connection = "ServiceBusConnectionString")] string mySbMsg, IDictionary<string, object> userProperties, FunctionContext context)
        {
            var logger = context.GetLogger("Function");
            string digitalTwinUrl = "https://" + Environment.GetEnvironmentVariable("AzureDTHostname");
            var credential = new DefaultAzureCredential();
            var digitalTwinsClient = new DigitalTwinsClient(new Uri(digitalTwinUrl), credential);

            // get Id
            userProperties.TryGetValue("deviceId", out object value);
            string scooterId = ((JsonElement)value).GetString();

            logger.LogInformation("id: " + scooterId);

            // update Digital twin reported properties, ignore the others.
            var scooterDeviceTwin = JsonConvert.DeserializeObject<Twin>(mySbMsg, new TwinJsonConverter());
            var reportedProperties = scooterDeviceTwin.Properties.Reported;
            if (reportedProperties.Count > 0)
            {
                var patch = new JsonPatchDocument();
                patch.AppendReplace("/Connected", true);
                patch.AppendReplace("/Locked", reportedProperties["Locked"]);
                patch.AppendReplace("/MaxSpeed", reportedProperties["MaxSpeed"]);
                var updateFrequency = 30; // TODO: change to proper property
                patch.AppendReplace("/UpdateFrequency", updateFrequency);
                patch.AppendReplace("/Standby", false); // TODO: change to proper property

                await digitalTwinsClient.UpdateDigitalTwinAsync(scooterId, patch);
            }

            logger.LogInformation($"Updated reported properties of twin: {scooterId}");
        }
    }
}
