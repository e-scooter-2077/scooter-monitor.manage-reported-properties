using Shouldly;
using System;
using Xunit;

namespace EScooter.Monitor.ManageReportedProperties.UnitTests
{
    public class JsonPatchFactoryTest
    {
        [Fact]
        public void TestNull()
        {
            var scooterStatusEvent = new ScooterStatusChanged("id", null, null, null, null);
            var patch = JsonPatchFactory.GetStatusPatch(scooterStatusEvent);
            patch.ToString().ShouldBe("[]");
        }

        [Fact]
        public void TestMaxSpeed()
        {
            var scooterStatusEvent = new ScooterStatusChanged("id", null, null, 10, null);
            var patch = JsonPatchFactory.GetStatusPatch(scooterStatusEvent);
            string expected = "[{'op':'replace','path':'/MaxSpeed','value':36}]".Replace('\'', '\"');
            patch.ToString().ShouldBe(expected);
        }

        [Fact]
        public void TestUpdateFrequency()
        {
            var scooterStatusEvent = new ScooterStatusChanged("id", null, "00:00:30", null, null);
            var patch = JsonPatchFactory.GetStatusPatch(scooterStatusEvent);
            string expected = "[{'op':'replace','path':'/UpdateFrequency','value':30}]".Replace('\'', '\"');
            patch.ToString().ShouldBe(expected);
        }

        [Fact]
        public void TestUpdateFrequencyMinutes()
        {
            var scooterStatusEvent = new ScooterStatusChanged("id", null, "00:10:30", null, null);
            var patch = JsonPatchFactory.GetStatusPatch(scooterStatusEvent);
            string expected = "[{'op':'replace','path':'/UpdateFrequency','value':630}]".Replace('\'', '\"');
            patch.ToString().ShouldBe(expected);
        }

        [Fact]
        public void TestLocked()
        {
            var scooterStatusEvent = new ScooterStatusChanged("id", true, null, null, null);
            var patch = JsonPatchFactory.GetStatusPatch(scooterStatusEvent);
            string expected = "[{'op':'replace','path':'/Locked','value':true}]".Replace('\'', '\"');
            patch.ToString().ShouldBe(expected);
        }

        [Fact]
        public void TestStandby()
        {
            var scooterStatusEvent = new ScooterStatusChanged("id", null, null, null, true);
            var patch = JsonPatchFactory.GetStatusPatch(scooterStatusEvent);
            string expected = "[{'op':'replace','path':'/Standby','value':true}]".Replace('\'', '\"');
            patch.ToString().ShouldBe(expected);
        }
    }
}
