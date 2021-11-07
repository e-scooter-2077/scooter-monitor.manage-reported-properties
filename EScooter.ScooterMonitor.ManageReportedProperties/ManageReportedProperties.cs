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
            logger.LogInformation("message: " + mySbMsg);

            // update Digital twin reported properties, ignore the others.
            var scooterDeviceTwin = JsonConvert.DeserializeObject<Twin>(mySbMsg, new TwinJsonConverter());
            var reportedProperties = scooterDeviceTwin.Properties.Reported;
            logger.LogInformation("twin: " + scooterDeviceTwin.ToJson());

            if (reportedProperties.Count > 0)
            {
                var patch = new JsonPatchDocument();
                patch.AppendReplace("/Connected", true);
                AppendReplaceProperty<bool>(patch, "locked", "/Locked", reportedProperties);
                AppendReplaceProperty<double>(patch, "maxSpeed", "/MaxSpeed", reportedProperties, x => ConvertSpeed((double)x));
                AppendReplaceProperty(patch, "updateFrequency", "/UpdateFrequency", reportedProperties, x => (int)TimeSpan.Parse((string)x).TotalSeconds);
                AppendReplaceProperty<bool>(patch, "standby", "/Standby", reportedProperties);

                logger.LogInformation($"Patch: ${patch}");
                await digitalTwinsClient.UpdateDigitalTwinAsync(scooterId, patch);
                logger.LogInformation($"Updated reported properties of twin: {scooterId}\n");
            }
            else
            {
                logger.LogInformation($"No reported property to update!\n");
            }
        }

        private static double ConvertSpeed(double speedInMS)
        {
            return Math.Round(speedInMS * 3.6, 4);
        }

        private static void AppendReplaceProperty<T>(JsonPatchDocument patch, string propertyName, string patchProperty, TwinCollection reported)
        {
            AppendReplaceProperty<T>(patch, propertyName, patchProperty, reported, x => x);
        }

        private static void AppendReplaceProperty<T>(JsonPatchDocument patch, string propertyName, string patchProperty, TwinCollection reported, Func<dynamic, T> mapper)
        {
            if (reported.Contains(propertyName))
            {
                var value = mapper(reported[propertyName]);
                patch.AppendReplace<T>(patchProperty, value);
            }
        }
    }
}
