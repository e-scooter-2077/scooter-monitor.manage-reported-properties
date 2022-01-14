using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EScooter.Monitor.ManageReportedProperties
{
    public record ScooterStatusChanged(
    string Id,
    bool? Locked,
    string UpdateFrequency,
    double? MaxSpeed,
    bool? Standby);
}
