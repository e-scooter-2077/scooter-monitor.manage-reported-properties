using System;
using System.Net.Http;
using Azure.Core.Pipeline;
using Azure.DigitalTwins.Core;
using Azure.Identity;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EScooter.Monitor.ManageReportedProperties
{
    public static class ManageReportedProperties
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private static string _digitalTwinUrl = "https://" + Environment.GetEnvironmentVariable("AzureDTHostname");

        [Function("manage-properties")]
        public static async void ManageProperties([ServiceBusTrigger("%TopicName%", "%SubscriptionName%", Connection = "ServiceBusConnectionString")] string mySbMsg, FunctionContext context)
        {
            var logger = context.GetLogger("Function");

            var scooterStatusChanged = JsonConvert.DeserializeObject<ScooterStatusChanged>(mySbMsg);
            var scooterId = scooterStatusChanged.Id;
            var patch = JsonPatchFactory.GetStatusPatch(scooterStatusChanged);

            var credential = new DefaultAzureCredential();
            var digitalTwinsClient = new DigitalTwinsClient(new Uri(_digitalTwinUrl), credential, new DigitalTwinsClientOptions
            {
                Transport = new HttpClientTransport(_httpClient)
            });

            await digitalTwinsClient.UpdateDigitalTwinAsync(scooterId, patch);
            logger.LogInformation($"Updated reported properties of twin: {scooterId}\n");
        }
    }
}
