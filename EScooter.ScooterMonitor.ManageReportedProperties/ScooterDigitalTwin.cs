using Azure.DigitalTwins.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EScooter.DigitalTwins.Commons
{
    internal class ScooterDigitalTwin
    {
        [JsonPropertyName(DigitalTwinsJsonPropertyNames.DigitalTwinId)]
        public string Id { get; set; }

        [JsonPropertyName(DigitalTwinsJsonPropertyNames.DigitalTwinETag)]
        public string ETag { get; set; }

        [JsonPropertyName(DigitalTwinsJsonPropertyNames.DigitalTwinMetadata)]
        public ScooterDigitalTwinMetadata Metadata { get; set; }

        [JsonPropertyName("UpdateFrequency")]
        public int UpdateFrequency { get; set; }

        [JsonPropertyName("Locked")]
        public bool Locked { get; set; }

        [JsonPropertyName("Enabled")]
        public bool Enabled { get; set; }

        [JsonPropertyName("MaxSpeed")]
        public double MaxSpeed { get; set; }

        [JsonPropertyName("PowerSavingThreshold")]
        public double PowerSavingThreshold { get; set; }

        [JsonPropertyName("Standby")]
        public bool Standby { get; set; }

        [JsonPropertyName("Connected")]
        public bool Connected { get; set; }

        [JsonPropertyName("BatteryLevel")]
        public double BatteryLevel { get; set; }

        [JsonPropertyName("Latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("Longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("Speed")]
        public double Speed { get; set; }

        [JsonPropertyName("Acceleration")]
        public double Acceleration { get; set; }

        public ScooterDigitalTwin()
        {
            this.Connected = false;
            this.Metadata = new ScooterDigitalTwinMetadata();
        }
    }

    internal class ScooterDigitalTwinMetadata
    {
        [JsonPropertyName(DigitalTwinsJsonPropertyNames.MetadataModel)]
        public string ModelId { get; set; }
    }
}
