using System;
using Azure.DigitalTwins.Core;
using Azure.Identity;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EScooter.Monitor.ManageReportedProperties
{
    public static class ManageReportedProperties
    {
        private static readonly DigitalTwinsClient _digitalTwinsClient = InstantiateDtClient();

        private static DigitalTwinsClient InstantiateDtClient()
        {
            string digitalTwinUrl = "https://" + Environment.GetEnvironmentVariable("AzureDTHostname");
            var credential = new DefaultAzureCredential();
            var digitalTwinsClient = new DigitalTwinsClient(new Uri(digitalTwinUrl), credential);
            return digitalTwinsClient;
        }

        [Function("manage-properties")]
        public static async void ManageProperties([ServiceBusTrigger("%TopicName%", "%SubscriptionName%", Connection = "ServiceBusConnectionString")] string mySbMsg, FunctionContext context)
        {
            var logger = context.GetLogger("Function");

            var scooterStatusChanged = JsonConvert.DeserializeObject<ScooterStatusChanged>(mySbMsg);
            var scooterId = scooterStatusChanged.Id;
            var patch = JsonPatchFactory.GetStatusPatch(scooterStatusChanged);

            await _digitalTwinsClient.UpdateDigitalTwinAsync(scooterId, patch);
            logger.LogInformation($"Updated reported properties of twin: {scooterId}\n");
        }
    }
}
