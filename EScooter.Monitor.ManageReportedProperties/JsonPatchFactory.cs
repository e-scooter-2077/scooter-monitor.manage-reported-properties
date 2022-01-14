using Azure;
using System;

namespace EScooter.Monitor.ManageReportedProperties
{
    public static class JsonPatchFactory
    {
        public static JsonPatchDocument GetStatusPatch(ScooterStatusChanged scooterStatusChanged)
        {
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
            return patch;
        }

        private static int ConvertTimeSpanStringToSeconds(string timespan) => (int)TimeSpan.Parse(timespan).TotalSeconds;

        private static double ConvertSpeed(double speedInMS) => Math.Round(speedInMS * 3.6, 4);
    }
}
