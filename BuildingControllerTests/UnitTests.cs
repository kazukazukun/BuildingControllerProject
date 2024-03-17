using NUnit.Framework;
using System.Reflection;
using BuildingControllerProject;
using System.Globalization;
using NSubstitute;

namespace BuildingControllerTests
{
    /// <summary>
    /// Test texts for managers.
    /// </summary>
    struct ManagerStatus
    {
        private const string lights = "Lights";
        private const string doors = "Doors";
        private const string alarm = "FireAlarm";

        private const string lightsWithPrefix = lights + ",";
        private const string doorsWithPrefix = doors + ",";
        private const string alarmWithPrefix = alarm + ",";

        private const string oneDeviceOk = "OK,";
        private const string oneDeviceFaulty = "FAULT,";
        private const string twoDevicesOk = oneDeviceOk + oneDeviceOk;
        private const string twoDevicesFaulty = oneDeviceFaulty + oneDeviceFaulty;
        private const string oneDeviceOkOneDeviceFaulty =  oneDeviceOk + oneDeviceFaulty;
        private const string threeDevicesOk = oneDeviceOk + twoDevicesOk;
        private const string threeDevicesFaulty = oneDeviceFaulty + twoDevicesFaulty;
        private const string twoDevicesOkOneDeviceFaulty = oneDeviceOk + oneDeviceFaulty + oneDeviceOk;
        private const string oneDeviceOkTwoDevicesFaulty = oneDeviceFaulty + oneDeviceOk + oneDeviceFaulty;
        private const string fiveDevicesOk = twoDevicesOk + threeDevicesOk;
        private const string fiveDevicesFaulty = threeDevicesFaulty + twoDevicesFaulty;
        private const string tenDevicesOk = fiveDevicesOk + fiveDevicesOk;
        private const string tenDevicesFaulty = fiveDevicesFaulty + fiveDevicesFaulty;
        private const string fiveDevicesOkFiveDevicesFaulty = oneDeviceOkOneDeviceFaulty + 
            oneDeviceOkOneDeviceFaulty + oneDeviceOkOneDeviceFaulty + oneDeviceOkOneDeviceFaulty + 
            oneDeviceOkOneDeviceFaulty;
        private const string fiftyDevicesOk = tenDevicesOk + tenDevicesOk + tenDevicesOk + tenDevicesOk + tenDevicesOk;
        private const string fiftyDevicesFaulty = tenDevicesFaulty + tenDevicesFaulty + tenDevicesFaulty + 
            tenDevicesFaulty + tenDevicesFaulty;
        private const string fiftyDevicesOkFaultyMixed = fiveDevicesOkFiveDevicesFaulty + fiveDevicesOkFiveDevicesFaulty + 
            fiveDevicesOkFiveDevicesFaulty + fiveDevicesOkFiveDevicesFaulty + fiveDevicesOkFiveDevicesFaulty;
    }


    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}