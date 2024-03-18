using NUnit.Framework;
using System.Reflection;
using BuildingControllerProject;
using System.Globalization;
using NSubstitute;

namespace BuildingControllerTests
{
    /// <summary>
    /// Test Fixture for the class <see cref="BuildingController"/>.
    /// </summary>
    [TestFixture]
    public class BuildingControllerTests
    {
        /// <summary>
        /// Valid states for the <see cref="BuildingController"/>
        /// </summary>
        struct BuildingState
        {
            public const string closed = "closed";
            public const string outOfHours = "out of hours";
            public const string open = "open";
            public const string fireDrill = "fire drill";
            public const string fireAlarm = "fire alarm";
        }

        /// <summary>
        /// Argument names for the <see cref="BuildingController"/> constructor.
        /// </summary>
        struct ControllerArgumentNames
        {
            public const string buildingID = "id";
            public const string startState = "startState";
            public const string lightManager = "iLightManager";
            public const string fireAlarmManager = "iFireAlarmManager";
            public const string doorManager = "iDoorManager";
            public const string webService = "iWebService";
            public const string emailService = "iEmailService";
        }

        /// <summary>
        /// Store expected strings for <see cref="BuildingController"/> tests.
        /// </summary>
        struct ExpectedTexts
        {
            public const string initialStateException = "Argument Exception: BuildingController can only be initialised "
                + "to the following states 'open', 'closed', 'out of hours'";
            public const string emailSubject = "failed to log alarm";
            public const string emailAddress = "smartbuilding@uclan.ac.uk";
        }

        /// <summary>
        /// Testing strings for managers.
        /// </summary>
        struct ManagerStatus
        {
            public const string lights = "Lights";
            public const string doors = "Doors";
            public const string alarm = "FireAlarm";

            public const string lightsWithComma = lights + ",";
            public const string doorsWithComma = doors + ",";
            public const string alarmWithComma = alarm + ",";

            public const string oneDeviceOk = "OK,";
            public const string oneDeviceFaulty = "FAULT,";
            public const string twoDevicesOk = oneDeviceOk + oneDeviceOk;
            public const string twoDevicesFaulty = oneDeviceFaulty + oneDeviceFaulty;
            public const string oneDeviceOkOneDeviceFaulty = oneDeviceOk + oneDeviceFaulty;
            public const string threeDevicesOk = oneDeviceOk + twoDevicesOk;
            public const string threeDevicesFaulty = oneDeviceFaulty + twoDevicesFaulty;
            public const string twoDevicesOkOneDeviceFaulty = oneDeviceOk + oneDeviceFaulty + oneDeviceOk;
            public const string oneDeviceOkTwoDevicesFaulty = oneDeviceFaulty + oneDeviceOk + oneDeviceFaulty;
            public const string fiveDevicesOk = twoDevicesOk + threeDevicesOk;
            public const string fiveDevicesFaulty = threeDevicesFaulty + twoDevicesFaulty;
            public const string tenDevicesOk = fiveDevicesOk + fiveDevicesOk;
            public const string tenDevicesFaulty = fiveDevicesFaulty + fiveDevicesFaulty;
            public const string fiveDevicesOkFiveDevicesFaulty = oneDeviceOkOneDeviceFaulty +
                oneDeviceOkOneDeviceFaulty + oneDeviceOkOneDeviceFaulty + oneDeviceOkOneDeviceFaulty +
                oneDeviceOkOneDeviceFaulty;
            public const string fiftyDevicesOk = tenDevicesOk + tenDevicesOk + tenDevicesOk + tenDevicesOk + tenDevicesOk;
            public const string fiftyDevicesFaulty = tenDevicesFaulty + tenDevicesFaulty + tenDevicesFaulty +
                tenDevicesFaulty + tenDevicesFaulty;
            public const string fiftyDevicesOkFaultyMixed = fiveDevicesOkFiveDevicesFaulty + fiveDevicesOkFiveDevicesFaulty +
                fiveDevicesOkFiveDevicesFaulty + fiveDevicesOkFiveDevicesFaulty + fiveDevicesOkFiveDevicesFaulty;

        }

        private static readonly object[] validStates =
        {
            BuildingState.closed,
            BuildingState.outOfHours,
            BuildingState.open,
            BuildingState.fireAlarm,
            BuildingState.fireDrill
        };

        private static readonly object[] normalStates =
        {
            BuildingState.closed,
            BuildingState.outOfHours,
            BuildingState.open
        };

        private static readonly object[] InvalidBuildingStates =
        {
            "out of service",
            "invalid"
        };

        /// <summary>
        /// Array containing a variety of strings to test against.
        /// </summary>
        private static readonly object?[] TestStrings =
        {
            null,
            "",
            "null",
            "abcdefghijklmnopqrstuvwxyz",
            "01234567890",
            "Testing 1 testing 2 testing 3",
            "卐卐卐卐卐卐卐卐卐卐卐卐卐卐卐卐",
            "@, !, #, $, &, *, (, ), _, +",
            "'",
            ",",
            "\"",
            "!@#$%^&*(){}?+_:;/=-]['",
            "   ",
            "\t",
            "\n",
            "\0X12FAE6",
            "\x13",
            "Society's needs come before individuals' needs."
        };

        private static readonly object?[] OkManagerStatuses =
        {
            ManagerStatus.oneDeviceOk,
            ManagerStatus.threeDevicesOk,
            ManagerStatus.fiveDevicesOk,
            ManagerStatus.tenDevicesOk,
            ManagerStatus.fiftyDevicesOk,
        };

        private static readonly object?[] FaultyManagerStatuses =
        {
            ManagerStatus.oneDeviceFaulty,
            ManagerStatus.twoDevicesOkOneDeviceFaulty,
            ManagerStatus.tenDevicesFaulty,
            ManagerStatus.fiveDevicesOkFiveDevicesFaulty,
            ManagerStatus.fiftyDevicesOkFaultyMixed,
            ManagerStatus.fiftyDevicesFaulty
        };

        /// <summary>
        /// Sample return texts for <see cref="ILightManager"/> stubs.
        /// </summary>
        private static readonly object?[] LightManagerStatuses =
        {
            ManagerStatus.lightsWithComma,
            ManagerStatus.lightsWithComma + ManagerStatus.twoDevicesOkOneDeviceFaulty,
            ManagerStatus.lightsWithComma + ManagerStatus.tenDevicesOk,
            ManagerStatus.lightsWithComma + ManagerStatus.tenDevicesFaulty,
            ManagerStatus.lightsWithComma + ManagerStatus.fiftyDevicesOk,
        };

        /// <summary>
        /// Sample return texts for <see cref="IDoorManager"/> stubs.
        /// </summary>
        private static readonly object?[] DoorManagerStatuses =
        {
            ManagerStatus.doorsWithComma,
            ManagerStatus.doorsWithComma + ManagerStatus.twoDevicesOkOneDeviceFaulty,
            ManagerStatus.doorsWithComma + ManagerStatus.tenDevicesOk,
            ManagerStatus.doorsWithComma + ManagerStatus.tenDevicesFaulty,
            ManagerStatus.doorsWithComma + ManagerStatus.fiftyDevicesOk,
        };

        /// <summary>
        /// Sample return texts for <see cref="IFireAlarmManager"/> stubs.
        /// </summary>
        private static readonly object?[] AlarmManagerStatuses =
        {
            ManagerStatus.alarmWithComma,
            ManagerStatus.alarmWithComma + ManagerStatus.twoDevicesOkOneDeviceFaulty,
            ManagerStatus.alarmWithComma + ManagerStatus.tenDevicesOk,
            ManagerStatus.alarmWithComma + ManagerStatus.tenDevicesFaulty,
            ManagerStatus.alarmWithComma + ManagerStatus.fiftyDevicesOk,
        };


        // LEVEL 1 TESTS //

        /// <summary>
        /// Test if a valid constructor exists for <see cref="BuildingController"/> through reflection.
        /// For L1R1.
        /// </summary>
        [Test]
        public void Constructor_WhenSingleParameter_HasCorrectSignature()
        {
            string? parameterName = null;
            ConstructorInfo? constructorInfoObject;
            Type[] argTypes = new Type[] { typeof(string) };

            // Lookup constructor with specified parameter
            constructorInfoObject = typeof(BuildingController).GetConstructor(argTypes);
            Assume.That(constructorInfoObject, Is.Not.Null);

            if (constructorInfoObject != null)
            {
                // Verify parameter name
                ParameterInfo[] constructorParams = constructorInfoObject.GetParameters();
                ParameterInfo firstParam = constructorParams.First();
                parameterName = firstParam.Name;
            }

            Assert.That(parameterName, Is.EqualTo(ControllerArgumentNames.buildingID));
        }

        /// <summary>
        /// Test initialisation of <c>buildingID</c> when constructor parameter set.
        /// For L1R2, L1R3.
        /// </summary>
        [TestCase("Building ID")]
        [TestCaseSource(nameof(TestStrings))]
        public void Constructor_WhenSet_InitialisesBuildingID(string buildingID)
        {
            BuildingController controller;

            controller = new BuildingController(buildingID);
            string? result = controller.GetBuildingID();


            string? expected = buildingID;
            if (!string.IsNullOrEmpty(buildingID))
            {
                expected = expected.ToLower();
            }

            Assert.That(result, Is.EqualTo(expected));
        }

        /// <summary>
        /// Test <c>buildingID</c> setter.
        /// For L1R4.
        /// </summary>
        [TestCase("Building ID")]
        [TestCaseSource(nameof(TestStrings))]
        public void SetBuildingID_WhenSet_SetsID(string buildingID)
        {
            BuildingController controller = new("");

            controller.SetBuildingID(buildingID);
            string? result = controller.GetBuildingID();

            string expected = buildingID;
            if (!string.IsNullOrEmpty(buildingID))
            {
                expected = expected.ToLower();
            }

            Assert.That(result, Is.EqualTo(expected));
        }

        /// <summary>
        /// Test default initialisation of <c>currentState</c>.
        /// For L1R5, L1R6.
        /// </summary>
        [Test]
        public void Constructor_ByDefault_InitialisesCurrentState()
        {
            BuildingController controller;

            controller = new BuildingController("");
            string? result = controller.GetCurrentState();

            Assert.That(result, Is.EqualTo(BuildingState.outOfHours));
        }

        /// <summary>
        /// Test <see cref="BuildingController.SetCurrentState"/> with invalid states.
        /// For L1R7.
        /// </summary>
        [Test]
        public void SetCurrentState_WhenInvalidState_ReturnsFalse(
            [ValueSource(nameof(InvalidBuildingStates))][ValueSource(nameof(TestStrings))] string state,
            [ValueSource(nameof(validStates))] string sourceState)
        {
            BuildingController controller = new("");

            controller.SetCurrentState(sourceState);
            bool result = controller.SetCurrentState(state);

            Assert.That(result, Is.False);
        }

        // LEVEL 2 TESTS //

        //For L2R1

        // From Normal States

        /// <summary>
        /// Test <see cref="BuildingController.SetCurrentState"/> when transitioning from 'closed' state.
        /// For L2R1.
        /// </summary>
        [TestCase(BuildingState.outOfHours)]
        [TestCase(BuildingState.fireDrill)]
        [TestCase(BuildingState.fireAlarm)]
        [TestCase(BuildingState.open, false)]
        public void SetCurrentState_WhenCurrentStateClosed_ReturnsBoolean(string state, bool success = true)
        {
            BuildingController controller = new("");

            controller.SetCurrentState(BuildingState.closed);
            bool result = controller.SetCurrentState(state);

            Assert.That(result, Is.EqualTo(success));
        }

        /// <summary>
        /// Test <see cref="BuildingController.SetCurrentState"/> when transitioning from 'open' state.
        /// For L2R1.
        /// </summary>
        [TestCase(BuildingState.outOfHours)]
        [TestCase(BuildingState.fireDrill)]
        [TestCase(BuildingState.fireAlarm)]
        [TestCase(BuildingState.closed, false)]
        public void SetCurrentState_WhenCurrentStateOpen_ReturnsBoolean(string state, bool success = true)
        {
            BuildingController controller = new("");

            controller.SetCurrentState(BuildingState.open);
            bool result = controller.SetCurrentState(state);

            Assert.That(result, Is.EqualTo(success));
        }

        /// <summary>
        /// Test <see cref="BuildingController.SetCurrentState"/> when transitioning from 'open' state.
        /// For L2R1.
        /// </summary>
        [TestCase(BuildingState.outOfHours)]
        [TestCase(BuildingState.fireDrill)]
        [TestCase(BuildingState.fireAlarm)]
        public void SetCurrentState_WhenCurrentStateOpen_SetsState(string state)
        {
            BuildingController controller = new("");

            controller.SetCurrentState(BuildingState.open);
            controller.SetCurrentState(state);
            string? result = controller.GetCurrentState();

            Assert.That(result, Is.EqualTo(state));
        }

        /// <summary>
        /// Test <see cref="BuildingController.SetCurrentState"/> when transitioning from 'closed' state.
        /// For L2R1.
        /// </summary>
        [TestCase(BuildingState.outOfHours)]
        [TestCase(BuildingState.fireDrill)]
        [TestCase(BuildingState.fireAlarm)]
        public void SetCurrentState_WhenCurrentStateClosed_SetsState(string state)
        {
            BuildingController controller = new("");

            controller.SetCurrentState(BuildingState.closed);
            controller.SetCurrentState(state);
            string? result = controller.GetCurrentState();

            Assert.That(result, Is.EqualTo(state));
        }

        /// <summary>
        /// Test <see cref="BuildingController.SetCurrentState"/> when setting the same state.
        /// </summary>
        /// For L2R2.
        [TestCase(BuildingState.outOfHours)]
        [TestCase(BuildingState.open)]
        [TestCase(BuildingState.closed)]
        public void SetCurrentState_WhenCurrentStateSame_SetsState(string state)
        {
            BuildingController controller = new("", state);
            bool result = controller.SetCurrentState(state);

            Assert.That(result, Is.True);  
        }

        /// <summary>
        /// Test that a two-parameter constructor for <see cref="BuildingController"/> exists.
        /// For L2R3.
        /// </summary>
        [Test]
        public void Constructor_WhenTwoParameters_HasCorrectSignature()
        {
            string? firstArgName = null;
            string? secondArgName = null;
            ConstructorInfo? constructorInfoObj;
            Type[] argTypes = new Type[] { typeof(string), typeof(string) };

            // Lookup two parameter constructor
            constructorInfoObj = typeof(BuildingController).GetConstructor(argTypes);
            Assume.That(constructorInfoObj, Is.Not.Null);

            if (constructorInfoObj != null)
            {
                ParameterInfo[] constructorParams = constructorInfoObj.GetParameters();
                ParameterInfo firstParam = constructorParams.ElementAt(0);
                ParameterInfo secondParam = constructorParams.ElementAt(1);

                // Verify parameter names
                firstArgName = firstParam.Name;
                secondArgName = secondParam.Name;
            }

            Assert.That(firstArgName, Is.EqualTo(ControllerArgumentNames.buildingID));
            Assert.That(secondArgName, Is.EqualTo(ControllerArgumentNames.startState));
        }

        /// <summary>
        /// Test constructor when using startState argument with a normal state.
        /// For L2R3.
        /// </summary>
        [Test, TestCaseSource(nameof(normalStates))]
        public void Constructor_WhenNormalState_SetsStartState(string state)
        {
            BuildingController controller;

            controller = new BuildingController("", state);
            string? result = controller.GetCurrentState();

            Assert.That(result, Is.EqualTo(state));
        }

        /// <summary>
        /// Test constructor when using startState argument with a normal state in capital letters.
        /// For L2R3.
        /// </summary>
        [TestCaseSource(nameof(normalStates))]
        public void Constructor_WhenNormalStateCapitals_SetsStartState(string state)
        {
            BuildingController controller;

            controller = new BuildingController("", state.ToUpper());
            string? result = controller.GetCurrentState();

            Assert.That(result, Is.EqualTo(state));
        }

        /// <summary>
        /// Test constructor when using startState argument with a normal state in title case.
        /// For L2R3.
        /// </summary>
        [TestCaseSource(nameof(normalStates))]
        public void Constructor_WhenNormalStateMixedCapitals_SetsStartState(string state)
        {
            BuildingController controller;
            TextInfo ti = CultureInfo.CurrentCulture.TextInfo;

            controller = new BuildingController("", ti.ToTitleCase(state));
            string? result = controller.GetCurrentState();

            Assert.That(result, Is.EqualTo(state));
        }

        /// <summary>
        /// Test constructor when using startState argument with an invalid state.
        /// For L2R3.
        /// </summary>
        [TestCase(BuildingState.fireDrill)]
        [TestCase(BuildingState.fireAlarm)]
        [TestCaseSource(nameof(InvalidBuildingStates))]
        [TestCaseSource(nameof(TestStrings))]
        public void Constructor_WhenNotNormalState_ThrowsException(string state)
        {
            BuildingController controller;

            Assert.That(() => { controller = new("", state); },
                Throws.ArgumentException.With.Property("Message").EqualTo(ExpectedTexts.initialStateException));
        }


        // LEVEL 3 TESTS //

        /// <summary>
        /// Test that a six-parameter constructor for <see cref="BuildingController"/> exists.
        /// For L3R1.
        /// </summary>
        [Test]
        public void Constructor_WhenSixParameters_HasCorrectSignature()
        {
            ConstructorInfo? constructorInfoObj;
            bool parameterNamesMatch = true;

            Type[] argTypes = new Type[] {
                typeof(string),
                typeof(ILightManager),
                typeof(IFireAlarmManager),
                typeof(IDoorManager),
                typeof(IWebService),
                typeof(IEmailService)
            };

            string[] argNames = new string[] {
               ControllerArgumentNames.buildingID,
               ControllerArgumentNames.lightManager,
               ControllerArgumentNames.fireAlarmManager,
               ControllerArgumentNames.doorManager,
               ControllerArgumentNames.webService,
               ControllerArgumentNames.emailService
            };

            // Get constructor with 6 parameters, then check names
            constructorInfoObj = typeof(BuildingController).GetConstructor(argTypes);
            Assume.That(constructorInfoObj, Is.Not.Null);

            if (constructorInfoObj != null)
            {
                ParameterInfo[] constructorParams = constructorInfoObj.GetParameters();
                Assume.That(constructorParams, Has.Exactly(argNames.Length).Items);

                for (int i = 0; i < constructorParams.Length; i++)
                {
                    ParameterInfo parameter = constructorParams.ElementAt(i);

                    if (parameter.Name != argNames.ElementAt(i))
                    {
                        parameterNamesMatch = false;
                    }
                }
            }


            Assert.That(parameterNamesMatch, Is.True);
        }

        /// <summary>
        /// Test the <see cref="BuildingController.GetStatusReport"/> method using stubs.
        /// For L3R3.
        /// </summary>
        [Test]
        public void GetStatusReport_WhenCalled_ReturnsStatusMessages(
            [ValueSource(nameof(LightManagerStatuses))] string lightStatus,
            [ValueSource(nameof(DoorManagerStatuses))] string doorStatus,
            [ValueSource(nameof(AlarmManagerStatuses))][ValueSource(nameof(TestStrings))] string alarmStatus)
        {
            ILightManager lightManager = Substitute.For<ILightManager>();
            IFireAlarmManager fireAlarmManager = Substitute.For<IFireAlarmManager>();
            IDoorManager doorManager = Substitute.For<IDoorManager>();
            IWebService webService = Substitute.For<IWebService>();
            IEmailService emailService = Substitute.For<IEmailService>();
            lightManager.GetStatus().Returns(lightStatus);
            doorManager.GetStatus().Returns(doorStatus);
            fireAlarmManager.GetStatus().Returns(alarmStatus);

            BuildingController controller = new("", lightManager, fireAlarmManager, doorManager, webService, emailService);

            string? result = controller.GetStatusReport();

            Assert.That(result, Is.EqualTo(string.Format("{0}{1}{2}",
                lightStatus ?? "" , doorStatus ?? "" , alarmStatus ?? "")));
        }

        /// <summary>
        /// Test the <see cref="BuildingController.SetCurrentState"/> when moving to <c>open</c> state.
        /// For L3R4.
        /// </summary>
        [Test]
        public void SetCurrentState_WhenMovingToOpenFromInitial_CallsOpenAllDoors([Values] bool doorsOpen)
        {
            ILightManager lightManager = Substitute.For<ILightManager>();
            IFireAlarmManager fireAlarmManager = Substitute.For<IFireAlarmManager>();
            IDoorManager doorManager = Substitute.For<IDoorManager>();
            IWebService webService = Substitute.For<IWebService>();
            IEmailService emailService = Substitute.For<IEmailService>();
            doorManager.OpenAllDoors().Returns(doorsOpen);

            BuildingController controller = new("", lightManager, fireAlarmManager, doorManager, webService, emailService);

            controller.SetCurrentState(BuildingState.open);

            doorManager.Received(1).OpenAllDoors();
        }

        /// <summary>
        /// Test the <see cref="BuildingController.SetCurrentState"/> when moving to <c>open</c> state.
        /// For L3R4.
        /// </summary>
        [Test]
        public void SetCurrentState_WhenMovingToOpenFromEmergency_CallsOpenAllDoors([Values(BuildingState.fireAlarm, BuildingState.fireDrill)] string initialState, [Values] bool doorsOpen)
        {
            ILightManager lightManager = Substitute.For<ILightManager>();
            IFireAlarmManager fireAlarmManager = Substitute.For<IFireAlarmManager>();
            IDoorManager doorManager = Substitute.For<IDoorManager>();
            IWebService webService = Substitute.For<IWebService>();
            IEmailService emailService = Substitute.For<IEmailService>();
            doorManager.OpenAllDoors().Returns(true);

            BuildingController controller = new("", lightManager, fireAlarmManager, doorManager, webService, emailService);

            // Must be in open state before switching to emergency
            controller.SetCurrentState(BuildingState.open);
            doorManager.OpenAllDoors().Returns(true);
            controller.SetCurrentState(initialState);
            doorManager.OpenAllDoors().Returns(doorsOpen);
            // Store only the last state transition's calls
            doorManager.ClearReceivedCalls();
            controller.SetCurrentState(BuildingState.open);

            doorManager.Received(1).OpenAllDoors();
        }

        /// <summary>
        /// Test the <see cref="BuildingController.SetCurrentState"/> when moving to <c>open</c> state.
        /// For L3R4, L3R5.
        /// </summary>
        [Test]
        public void SetCurrentState_WhenMovingToOpen_ReturnsBoolean([Values] bool doorsOpen)
        {
            ILightManager lightManager = Substitute.For<ILightManager>();
            IFireAlarmManager fireAlarmManager = Substitute.For<IFireAlarmManager>();
            IDoorManager doorManager = Substitute.For<IDoorManager>();
            IWebService webService = Substitute.For<IWebService>();
            IEmailService emailService = Substitute.For<IEmailService>();
            doorManager.OpenAllDoors().Returns(doorsOpen);

            BuildingController controller = new("", lightManager, fireAlarmManager, doorManager, webService, emailService);

            bool result = controller.SetCurrentState(BuildingState.open);

            Assert.That(result, Is.EqualTo(doorsOpen));
        }

        /// <summary>
        /// Test the <see cref="BuildingController.SetCurrentState"/> when moving to <c>open</c> state.
        /// For L3R4, L3R5.
        /// </summary>
        [Test]
        public void SetCurrentState_WhenMovingToOpenFromEmergency_ReturnsBoolean([Values(BuildingState.fireAlarm, BuildingState.fireDrill)] string initialState, [Values] bool doorsOpen)
        {
            ILightManager lightManager = Substitute.For<ILightManager>();
            IFireAlarmManager fireAlarmManager = Substitute.For<IFireAlarmManager>();
            IDoorManager doorManager = Substitute.For<IDoorManager>();
            IWebService webService = Substitute.For<IWebService>();
            IEmailService emailService = Substitute.For<IEmailService>();
            doorManager.OpenAllDoors().Returns(true);

            BuildingController controller = new("", lightManager, fireAlarmManager, doorManager, webService, emailService);

            controller.SetCurrentState(BuildingState.open);
            doorManager.OpenAllDoors().Returns(true);
            controller.SetCurrentState(initialState);
            doorManager.OpenAllDoors().Returns(doorsOpen);
            doorManager.ClearReceivedCalls();
            bool result = controller.SetCurrentState(BuildingState.open);

            Assert.That(result, Is.EqualTo(doorsOpen));
        }

        /// <summary>
        /// Test the <see cref="BuildingController.SetCurrentState"/> when moving to <c>open</c> state.
        /// For L3R5.
        /// </summary>
        [Test]
        public void SetCurrentState_WhenMovingToOpen_SetsState()
        {
            ILightManager lightManager = Substitute.For<ILightManager>();
            IFireAlarmManager fireAlarmManager = Substitute.For<IFireAlarmManager>();
            IDoorManager doorManager = Substitute.For<IDoorManager>();
            IWebService webService = Substitute.For<IWebService>();
            IEmailService emailService = Substitute.For<IEmailService>();
            doorManager.OpenAllDoors().Returns(true);

            BuildingController controller = new("", lightManager, fireAlarmManager, doorManager, webService, emailService);

            controller.SetCurrentState(BuildingState.open);
            string? result = controller.GetCurrentState();

            Assert.That(result, Is.EqualTo(BuildingState.open));
        }

        /// <summary>
        /// Test the <see cref="BuildingController.SetCurrentState"/> when moving to <c>open</c> state.
        /// For L3R4.
        /// </summary>
        [Test]
        public void SetCurrentState_WhenMovingToOpen_DoesNotSetState()
        {
            ILightManager lightManager = Substitute.For<ILightManager>();
            IFireAlarmManager fireAlarmManager = Substitute.For<IFireAlarmManager>();
            IDoorManager doorManager = Substitute.For<IDoorManager>();
            IWebService webService = Substitute.For<IWebService>();
            IEmailService emailService = Substitute.For<IEmailService>();
            doorManager.OpenAllDoors().Returns(false);

            BuildingController controller = new("", lightManager, fireAlarmManager, doorManager, webService, emailService);

            controller.SetCurrentState(BuildingState.open);
            string? result = controller.GetCurrentState();

            Assert.That(result, Is.EqualTo(BuildingState.outOfHours));
        }

        /// <summary>
        /// Test the <see cref="BuildingController.SetCurrentState"/> when moving to <c>open</c> state.
        /// For L3R5.
        /// </summary>
        [Test]
        public void SetCurrentState_WhenMovingToOpenFromEmergency_SetsState([Values(BuildingState.fireAlarm, BuildingState.fireDrill)] string initialState)
        {
            ILightManager lightManager = Substitute.For<ILightManager>();
            IFireAlarmManager fireAlarmManager = Substitute.For<IFireAlarmManager>();
            IDoorManager doorManager = Substitute.For<IDoorManager>();
            IWebService webService = Substitute.For<IWebService>();
            IEmailService emailService = Substitute.For<IEmailService>();
            doorManager.OpenAllDoors().Returns(true);

            BuildingController controller = new("", lightManager, fireAlarmManager, doorManager, webService, emailService);

            controller.SetCurrentState(BuildingState.open);
            controller.SetCurrentState(initialState);
            controller.SetCurrentState(BuildingState.open);
            string? result = controller.GetCurrentState();

            Assert.That(result, Is.EqualTo(BuildingState.open));
        }

        /// <summary>
        /// Test the <see cref="BuildingController.SetCurrentState"/> when moving to <c>open</c> state.
        /// For L3R4.
        /// </summary>
        [Test]
        public void SetCurrentState_WhenMovingToOpenFromEmergency_DoesNotSetState([Values(BuildingState.fireAlarm, BuildingState.fireDrill)] string initialState)
        {
            ILightManager lightManager = Substitute.For<ILightManager>();
            IFireAlarmManager fireAlarmManager = Substitute.For<IFireAlarmManager>();
            IDoorManager doorManager = Substitute.For<IDoorManager>();
            IWebService webService = Substitute.For<IWebService>();
            IEmailService emailService = Substitute.For<IEmailService>();
            doorManager.OpenAllDoors().Returns(true);

            BuildingController controller = new("", lightManager, fireAlarmManager, doorManager, webService, emailService);

            controller.SetCurrentState(BuildingState.open);
            doorManager.OpenAllDoors().Returns(true);
            controller.SetCurrentState(initialState);
            doorManager.OpenAllDoors().Returns(false);

            controller.SetCurrentState(BuildingState.open);
            string? result = controller.GetCurrentState();

            Assert.That(result, Is.EqualTo(initialState));
        }

        /// <summary>
        /// Test the <see cref="BuildingController.SetCurrentState"/>
        /// when moving to <c>open</c> state if already there.
        /// For L3R4.
        /// </summary>
        [Test]
        public void SetCurrentState_WhenMovingFromOpenToOpen_DoesNotCallOpenAllDoors()
        {
            ILightManager lightManager = Substitute.For<ILightManager>();
            IFireAlarmManager fireAlarmManager = Substitute.For<IFireAlarmManager>();
            IDoorManager doorManager = Substitute.For<IDoorManager>();
            IWebService webService = Substitute.For<IWebService>();
            IEmailService emailService = Substitute.For<IEmailService>();
            doorManager.OpenAllDoors().Returns(true);

            BuildingController controller = new("", lightManager, fireAlarmManager, doorManager, webService, emailService);

            // Since the state will be open already
            // there is no need to call the OpenAllDoors method again
            controller.SetCurrentState(BuildingState.open);
            doorManager.ClearReceivedCalls();
            controller.SetCurrentState(BuildingState.open);

            doorManager.DidNotReceive().OpenAllDoors();
        }

        // LEVEL 4 TESTS //

        // L4R1 (Moving to Closed state)

        /// <summary>
        /// Test that <see cref="BuildingController.SetCurrentState"/>
        /// calls the <see cref="IDoorManager.LockAllDoors"/> method
        /// when moving to <c>closed</c> state from the initial state.
        /// For L4R1.
        /// </summary>
        [Test]
        public void SetCurrentState_WhenMovingToClosedFromInitial_CallsLockAllDoors([Values] bool doorsLock)
        {
            ILightManager lightManager = Substitute.For<ILightManager>();
            IFireAlarmManager fireAlarmManager = Substitute.For<IFireAlarmManager>();
            IDoorManager doorManager = Substitute.For<IDoorManager>();
            IWebService webService = Substitute.For<IWebService>();
            IEmailService emailService = Substitute.For<IEmailService>();
            doorManager.LockAllDoors().Returns(doorsLock);

            BuildingController controller = new("", lightManager, fireAlarmManager, doorManager, webService, emailService);

            controller.SetCurrentState(BuildingState.closed);

            doorManager.Received(1).LockAllDoors();
        }

        /// <summary>
        /// Test that <see cref="BuildingController.SetCurrentState"/>
        /// calls the <see cref="IDoorManager.LockAllDoors"/> method
        /// when moving back to <c>closed</c> state from an emergency state.
        /// For L4R1.
        /// </summary>
        [Test]
        public void SetCurrentState_WhenMovingToClosedFromEmergency_CallsLockAllDoors([Values(BuildingState.fireAlarm, BuildingState.fireDrill)] string sourceState, [Values] bool doorsLock)
        {
            ILightManager lightManager = Substitute.For<ILightManager>();
            IFireAlarmManager fireAlarmManager = Substitute.For<IFireAlarmManager>();
            IDoorManager doorManager = Substitute.For<IDoorManager>();
            IWebService webService = Substitute.For<IWebService>();
            IEmailService emailService = Substitute.For<IEmailService>();
            doorManager.LockAllDoors().Returns(true);

            BuildingController controller = new("", lightManager, fireAlarmManager, doorManager, webService, emailService);

            controller.SetCurrentState(BuildingState.closed);
            doorManager.ClearReceivedCalls();
            doorManager.LockAllDoors().Returns(doorsLock);
            controller.SetCurrentState(sourceState);
            controller.SetCurrentState(BuildingState.closed);

            doorManager.Received(1).LockAllDoors();
        }

        /// <summary>
        /// Test that <see cref="BuildingController.SetCurrentState"/>
        /// calls the <see cref="ILightManager.SetAllLights"/> method
        /// when moving to <c>closed</c> state from the initial state.
        /// For L4R1.
        /// </summary>
        [Test]
        public void SetCurrentState_WhenMovingToClosedFromInitial_CallsSetAllLights()
        {
            ILightManager lightManager = Substitute.For<ILightManager>();
            IFireAlarmManager fireAlarmManager = Substitute.For<IFireAlarmManager>();
            IDoorManager doorManager = Substitute.For<IDoorManager>();
            IWebService webService = Substitute.For<IWebService>();
            IEmailService emailService = Substitute.For<IEmailService>();
            doorManager.LockAllDoors().Returns(true);

            BuildingController controller = new("", lightManager, fireAlarmManager, doorManager, webService, emailService);

            controller.SetCurrentState(BuildingState.closed);

            lightManager.Received(1).SetAllLights(false);
        }

        /// <summary>
        /// Test that <see cref="BuildingController.SetCurrentState"/>
        /// calls the <see cref="ILightManager.SetAllLights"/> method
        /// when moving back to <c>closed</c> state from an emergency state.
        /// For L4R1.
        /// </summary>
        [Test]
        public void SetCurrentState_WhenMovingToClosedFromEmergency_CallsSetAllLights([Values(BuildingState.fireAlarm, BuildingState.fireDrill)] string sourceState)
        {
            ILightManager lightManager = Substitute.For<ILightManager>();
            IFireAlarmManager fireAlarmManager = Substitute.For<IFireAlarmManager>();
            IDoorManager doorManager = Substitute.For<IDoorManager>();
            IWebService webService = Substitute.For<IWebService>();
            IEmailService emailService = Substitute.For<IEmailService>();
            doorManager.LockAllDoors().Returns(true);

            BuildingController controller = new("", lightManager, fireAlarmManager, doorManager, webService, emailService);

            controller.SetCurrentState(BuildingState.closed);
            controller.SetCurrentState(sourceState);
            lightManager.ClearReceivedCalls();
            controller.SetCurrentState(BuildingState.closed);

            lightManager.Received(1).SetAllLights(false);
        }

        // L4R2 (Moving to Fire Alarm state)

        /// <summary>
        /// Test that <see cref="BuildingController.SetCurrentState"/>
        /// calls the <see cref="IFireAlarmManager.SetAlarm"/> method
        /// when moving to <c>fire alarm</c> state.
        /// For L4R2.
        /// </summary>
        [TestCaseSource(nameof(normalStates))]
        public void SetCurrentState_WhenMovingToAlarmState_CallsSetAlarm(string sourceState)
        {
            ILightManager lightManager = Substitute.For<ILightManager>();
            IFireAlarmManager fireAlarmManager = Substitute.For<IFireAlarmManager>();
            IDoorManager doorManager = Substitute.For<IDoorManager>();
            IWebService webService = Substitute.For<IWebService>();
            IEmailService emailService = Substitute.For<IEmailService>();
            doorManager.OpenAllDoors().Returns(true);
            doorManager.LockAllDoors().Returns(true);

            BuildingController controller = new BuildingController("", lightManager, fireAlarmManager, doorManager, webService, emailService);

            controller.SetCurrentState(sourceState);
            controller.SetCurrentState(BuildingState.fireAlarm);

            fireAlarmManager.Received(1).SetAlarm(true);
        }

        /// <summary>
        /// Test that <see cref="BuildingController.SetCurrentState"/>
        /// calls the <see cref="IDoorManager.OpenAllDoors"/> method
        /// when moving to <c>fire alarm</c> state.
        /// For L4R2.
        /// </summary>
        [TestCaseSource(nameof(normalStates))]
        public void SetCurrentState_WhenMovingToAlarmState_CallsOpenAllDoors(string sourceState)
        {
            ILightManager lightManager = Substitute.For<ILightManager>();
            IFireAlarmManager fireAlarmManager = Substitute.For<IFireAlarmManager>();
            IDoorManager doorManager = Substitute.For<IDoorManager>();
            IWebService webService = Substitute.For<IWebService>();
            IEmailService emailService = Substitute.For<IEmailService>();
            doorManager.OpenAllDoors().Returns(true);
            doorManager.LockAllDoors().Returns(true);

            BuildingController controller = new("", lightManager, fireAlarmManager, doorManager, webService, emailService);

            controller.SetCurrentState(sourceState);
            doorManager.ClearReceivedCalls();
            controller.SetCurrentState(BuildingState.fireAlarm);

            doorManager.Received(1).OpenAllDoors();
        }

        /// <summary>
        /// Test that <see cref="BuildingController.SetCurrentState"/>
        /// calls the <see cref="ILightManager.SetAllLights"/> method
        /// when moving to <c>fire alarm</c> state.
        /// For L4R2.
        /// </summary>
        [TestCaseSource(nameof(normalStates))]
        public void SetCurrentState_WhenMovingToAlarmState_CallsSetAllLights(string sourceState)
        {
            ILightManager lightManager = Substitute.For<ILightManager>();
            IFireAlarmManager fireAlarmManager = Substitute.For<IFireAlarmManager>();
            IDoorManager doorManager = Substitute.For<IDoorManager>();
            IWebService webService = Substitute.For<IWebService>();
            IEmailService emailService = Substitute.For<IEmailService>();
            doorManager.OpenAllDoors().Returns(true);
            doorManager.LockAllDoors().Returns(true);

            BuildingController controller = new("", lightManager, fireAlarmManager, doorManager, webService, emailService);

            controller.SetCurrentState(sourceState);
            controller.SetCurrentState(BuildingState.fireAlarm);

            lightManager.Received(1).SetAllLights(true);
        }

        /// <summary>
        /// Test that <see cref="BuildingController.SetCurrentState"/>
        /// calls the <see cref="IWebService.LogFireAlarm"/> method
        /// when moving to <c>fire alarm</c> state.
        /// For L4R2.
        /// </summary>
        [TestCaseSource(nameof(normalStates))]
        public void SetCurrentState_WhenMovingToAlarmState_CallsLogFireAlarm(string sourceState)
        {
            ILightManager lightManager = Substitute.For<ILightManager>();
            IFireAlarmManager fireAlarmManager = Substitute.For<IFireAlarmManager>();
            IDoorManager doorManager = Substitute.For<IDoorManager>();
            IWebService webService = Substitute.For<IWebService>();
            IEmailService emailService = Substitute.For<IEmailService>();
            doorManager.OpenAllDoors().Returns(true);
            doorManager.LockAllDoors().Returns(true);

            BuildingController controller = new("", lightManager, fireAlarmManager, doorManager, webService, emailService);

            controller.SetCurrentState(sourceState);
            controller.SetCurrentState(BuildingState.fireAlarm);

            webService.Received(1).LogFireAlarm(BuildingState.fireAlarm);
        }

        // L4R3

        /// <summary>
        /// Test that <see cref="BuildingController.GetStatusReport"/>
        /// calls the <see cref="IWebService.LogEngineerRequired"/>
        /// method if a fault was detected.
        /// For L4R3.
        /// </summary>
        [TestCase(ManagerStatus.twoDevicesOkOneDeviceFaulty, ManagerStatus.twoDevicesOkOneDeviceFaulty, ManagerStatus.twoDevicesOkOneDeviceFaulty)]
        [TestCase(ManagerStatus.fiftyDevicesOk, ManagerStatus.fiftyDevicesOk, ManagerStatus.oneDeviceFaulty)]
        [TestCase(ManagerStatus.oneDeviceFaulty, ManagerStatus.fiftyDevicesOk, ManagerStatus.oneDeviceOk)]
        [TestCase(ManagerStatus.tenDevicesFaulty, ManagerStatus.tenDevicesFaulty, ManagerStatus.tenDevicesFaulty)]
        [TestCase(ManagerStatus.oneDeviceFaulty, ManagerStatus.oneDeviceFaulty, ManagerStatus.oneDeviceFaulty)]
        public void GetStatusReport_WhenFindsFaults_CallsLogEngineerRequired(
            string lightStatus, string doorStatus, string alarmStatus)
        {
            ILightManager lightManager = Substitute.For<ILightManager>();
            IFireAlarmManager fireAlarmManager = Substitute.For<IFireAlarmManager>();
            IDoorManager doorManager = Substitute.For<IDoorManager>();
            IWebService webService = Substitute.For<IWebService>();
            IEmailService emailService = Substitute.For<IEmailService>();
            lightManager.GetStatus().Returns(ManagerStatus.lightsWithComma + lightStatus);
            doorManager.GetStatus().Returns(ManagerStatus.doorsWithComma + doorStatus);
            fireAlarmManager.GetStatus().Returns(ManagerStatus.alarmWithComma + alarmStatus);

            BuildingController controller = new("", lightManager, fireAlarmManager, doorManager, webService, emailService);

            controller.GetStatusReport();

            // Test part one of the requirement
            webService.Received(1).LogEngineerRequired(Arg.Any<string>());
        }

        /// <summary>
        /// Test that <see cref="BuildingController.GetStatusReport"/>
        /// does not call the <see cref="IWebService.LogEngineerRequired"/>
        /// method if a fault was not detected.
        /// For L4R3.
        /// </summary>
        [Test]
        public void GetStatusReport_WhenAllOk_DoesNotCallLogEngineerRequired(
            [ValueSource(nameof(OkManagerStatuses))] string lightStatus,
            [ValueSource(nameof(OkManagerStatuses))] string doorStatus,
            [ValueSource(nameof(OkManagerStatuses))] string alarmStatus)
        {
            ILightManager lightManager = Substitute.For<ILightManager>();
            IFireAlarmManager fireAlarmManager = Substitute.For<IFireAlarmManager>();
            IDoorManager doorManager = Substitute.For<IDoorManager>();
            IWebService webService = Substitute.For<IWebService>();
            IEmailService emailService = Substitute.For<IEmailService>();
            lightManager.GetStatus().Returns(ManagerStatus.lightsWithComma + lightStatus);
            doorManager.GetStatus().Returns(ManagerStatus.doorsWithComma + doorStatus);
            fireAlarmManager.GetStatus().Returns(ManagerStatus.alarmWithComma + alarmStatus);

            BuildingController controller = new("", lightManager, fireAlarmManager, doorManager, webService, emailService);

            controller.GetStatusReport();

            webService.DidNotReceive().LogEngineerRequired(Arg.Any<string>());
        }

        /// <summary>
        /// Test that <see cref="BuildingController.GetStatusReport"/>
        /// calls the <see cref="IWebService.LogEngineerRequired"/>
        /// method if a fault was detected in the lights manager.
        /// For L4R3.
        /// </summary>
        [Test]
        public void GetStatusReport_WhenFindsFaultsSingleManagerInLights_CallsLogEngineerRequired(
            [ValueSource(nameof(FaultyManagerStatuses))] string lightStatus,
            [ValueSource(nameof(OkManagerStatuses))] string doorStatus,
            [ValueSource(nameof(OkManagerStatuses))] string alarmStatus)
        {
            ILightManager lightManager = Substitute.For<ILightManager>();
            IFireAlarmManager fireAlarmManager = Substitute.For<IFireAlarmManager>();
            IDoorManager doorManager = Substitute.For<IDoorManager>();
            IWebService webService = Substitute.For<IWebService>();
            IEmailService emailService = Substitute.For<IEmailService>();
            lightManager.GetStatus().Returns(ManagerStatus.lightsWithComma + lightStatus);
            doorManager.GetStatus().Returns(ManagerStatus.doorsWithComma + doorStatus);
            fireAlarmManager.GetStatus().Returns(ManagerStatus.alarmWithComma + alarmStatus);

            BuildingController controller = new("", lightManager, fireAlarmManager, doorManager, webService, emailService);

            controller.GetStatusReport();

            webService.Received(1).LogEngineerRequired(ManagerStatus.lights);
        }

        /// <summary>
        /// Test that <see cref="BuildingController.GetStatusReport"/>
        /// calls the <see cref="IWebService.LogEngineerRequired"/>
        /// method if a fault was detected in the doors manager.
        /// For L4R3.
        /// </summary>
        [Test]
        public void GetStatusReport_WhenFindsFaultsSingleManagerInDoors_CallsLogEngineerRequired(
            [ValueSource(nameof(OkManagerStatuses))] string lightStatus,
            [ValueSource(nameof(FaultyManagerStatuses))] string doorStatus,
            [ValueSource(nameof(OkManagerStatuses))] string alarmStatus)
        {
            ILightManager lightManager = Substitute.For<ILightManager>();
            IFireAlarmManager fireAlarmManager = Substitute.For<IFireAlarmManager>();
            IDoorManager doorManager = Substitute.For<IDoorManager>();
            IWebService webService = Substitute.For<IWebService>();
            IEmailService emailService = Substitute.For<IEmailService>();
            lightManager.GetStatus().Returns(ManagerStatus.lightsWithComma + lightStatus);
            doorManager.GetStatus().Returns(ManagerStatus.doorsWithComma + doorStatus);
            fireAlarmManager.GetStatus().Returns(ManagerStatus.alarmWithComma + alarmStatus);

            BuildingController controller = new("", lightManager, fireAlarmManager, doorManager, webService, emailService);

            controller.GetStatusReport();

            webService.Received(1).LogEngineerRequired(ManagerStatus.doors);
        }

        /// <summary>
        /// Test that <see cref="BuildingController.GetStatusReport"/>
        /// calls the <see cref="IWebService.LogEngineerRequired"/>
        /// method if a fault was detected in the fire alarm manager.
        /// For L4R3.
        /// </summary>
        [Test]
        public void GetStatusReport_WhenFindsFaultsSingleManagerInAlarm_CallsLogEngineerRequired(
            [ValueSource(nameof(OkManagerStatuses))] string lightStatus,
            [ValueSource(nameof(OkManagerStatuses))] string doorStatus,
            [ValueSource(nameof(FaultyManagerStatuses))] string alarmStatus)
        {
            ILightManager lightManager = Substitute.For<ILightManager>();
            IFireAlarmManager fireAlarmManager = Substitute.For<IFireAlarmManager>();
            IDoorManager doorManager = Substitute.For<IDoorManager>();
            IWebService webService = Substitute.For<IWebService>();
            IEmailService emailService = Substitute.For<IEmailService>();
            lightManager.GetStatus().Returns(ManagerStatus.lightsWithComma + lightStatus);
            doorManager.GetStatus().Returns(ManagerStatus.doorsWithComma + doorStatus);
            fireAlarmManager.GetStatus().Returns(ManagerStatus.alarmWithComma + alarmStatus);

            BuildingController controller = new("", lightManager, fireAlarmManager, doorManager, webService, emailService);

            controller.GetStatusReport();

            webService.Received(1).LogEngineerRequired(ManagerStatus.alarm);
        }

        /// <summary>
        /// Test the <see cref="BuildingController.GetStatusReport"/> method using stubs.
        /// For L4R3.
        /// </summary>
        [Test]
        public void GetStatusReport_WhenFindsFaultsAllManagers_CallsLogEngineerRequired(
            [ValueSource(nameof(FaultyManagerStatuses))] string lightStatus,
            [ValueSource(nameof(FaultyManagerStatuses))] string doorStatus,
            [ValueSource(nameof(FaultyManagerStatuses))] string alarmStatus)
        {
            ILightManager lightManager = Substitute.For<ILightManager>();
            IFireAlarmManager fireAlarmManager = Substitute.For<IFireAlarmManager>();
            IDoorManager doorManager = Substitute.For<IDoorManager>();
            IWebService webService = Substitute.For<IWebService>();
            IEmailService emailService = Substitute.For<IEmailService>();
            lightManager.GetStatus().Returns(ManagerStatus.lightsWithComma + lightStatus);
            doorManager.GetStatus().Returns(ManagerStatus.doorsWithComma + doorStatus);
            fireAlarmManager.GetStatus().Returns(ManagerStatus.alarmWithComma + alarmStatus);

            BuildingController controller = new("", lightManager, fireAlarmManager, doorManager, webService, emailService);

            controller.GetStatusReport();

            webService.Received(1).LogEngineerRequired(string.Format("{0}{1}{2}",
                ManagerStatus.lightsWithComma, ManagerStatus.doorsWithComma, ManagerStatus.alarmWithComma));
        }

        /// <summary>
        /// Test the <see cref="BuildingController.GetStatusReport"/> method using stubs.
        /// For L4R3.
        /// </summary>
        [Test]
        public void GetStatusReport_WhenFindsFaultsLightsAndDoors_CallsLogEngineerRequired(
            [ValueSource(nameof(FaultyManagerStatuses))] string lightStatus,
            [ValueSource(nameof(FaultyManagerStatuses))] string doorStatus,
            [ValueSource(nameof(OkManagerStatuses))] string alarmStatus)
        {
            ILightManager lightManager = Substitute.For<ILightManager>();
            IFireAlarmManager fireAlarmManager = Substitute.For<IFireAlarmManager>();
            IDoorManager doorManager = Substitute.For<IDoorManager>();
            IWebService webService = Substitute.For<IWebService>();
            IEmailService emailService = Substitute.For<IEmailService>();
            lightManager.GetStatus().Returns(ManagerStatus.lightsWithComma + lightStatus);
            doorManager.GetStatus().Returns(ManagerStatus.doorsWithComma + doorStatus);
            fireAlarmManager.GetStatus().Returns(ManagerStatus.alarmWithComma + alarmStatus);

            BuildingController controller = new("", lightManager, fireAlarmManager, doorManager, webService, emailService);

            controller.GetStatusReport();

            webService.Received(1).LogEngineerRequired(string.Format("{0}{1}",
                ManagerStatus.lightsWithComma, ManagerStatus.doorsWithComma));
        }

        /// <summary>
        /// Test the <see cref="BuildingController.GetStatusReport"/> method using stubs.
        /// For L4R3.
        /// </summary>
        [Test]
        public void GetStatusReport_WhenFindsFaultsLightsAndAlarm_CallsLogEngineerRequired(
            [ValueSource(nameof(FaultyManagerStatuses))] string lightStatus,
            [ValueSource(nameof(OkManagerStatuses))] string doorStatus,
            [ValueSource(nameof(FaultyManagerStatuses))] string alarmStatus)
        {
            ILightManager lightManager = Substitute.For<ILightManager>();
            IFireAlarmManager fireAlarmManager = Substitute.For<IFireAlarmManager>();
            IDoorManager doorManager = Substitute.For<IDoorManager>();
            IWebService webService = Substitute.For<IWebService>();
            IEmailService emailService = Substitute.For<IEmailService>();
            lightManager.GetStatus().Returns(ManagerStatus.lightsWithComma + lightStatus);
            doorManager.GetStatus().Returns(ManagerStatus.doorsWithComma + doorStatus);
            fireAlarmManager.GetStatus().Returns(ManagerStatus.alarmWithComma + alarmStatus);

            BuildingController controller = new("", lightManager, fireAlarmManager, doorManager, webService, emailService);

            controller.GetStatusReport();

            webService.Received(1).LogEngineerRequired(string.Format("{0}{1}",
                ManagerStatus.lightsWithComma, ManagerStatus.alarmWithComma));
        }

        /// <summary>
        /// Test the <see cref="BuildingController.GetStatusReport"/> method using stubs.
        /// For L4R3.
        /// </summary>
        [Test]
        public void GetStatusReport_WhenFindsFaultsDoorsAndAlarm_CallsLogEngineerRequired(
            [ValueSource(nameof(OkManagerStatuses))] string lightStatus,
            [ValueSource(nameof(FaultyManagerStatuses))] string doorStatus,
            [ValueSource(nameof(FaultyManagerStatuses))] string alarmStatus)
        {
            ILightManager lightManager = Substitute.For<ILightManager>();
            IFireAlarmManager fireAlarmManager = Substitute.For<IFireAlarmManager>();
            IDoorManager doorManager = Substitute.For<IDoorManager>();
            IWebService webService = Substitute.For<IWebService>();
            IEmailService emailService = Substitute.For<IEmailService>();
            lightManager.GetStatus().Returns(ManagerStatus.lightsWithComma + lightStatus);
            doorManager.GetStatus().Returns(ManagerStatus.doorsWithComma + doorStatus);
            fireAlarmManager.GetStatus().Returns(ManagerStatus.alarmWithComma + alarmStatus);

            BuildingController controller = new("", lightManager, fireAlarmManager, doorManager, webService, emailService);

            controller.GetStatusReport();

            webService.Received(1).LogEngineerRequired(string.Format("{0}{1}",
               ManagerStatus.doorsWithComma, ManagerStatus.alarmWithComma));
        }

        // L4R4 

        /// <summary>
        /// Test that <see cref="BuildingController.SetCurrentState"/>
        /// calls the <see cref="IEmailService.SendEmail"/> method
        /// when moving to <c>fire alarm</c> state.
        /// For L4R4.
        /// </summary>
        [Test]
        public void SetCurrentState_WhenMovingToAlarmState_CallsSendEmail(
            [ValueSource(nameof(normalStates))] string sourceState,
            [ValueSource(nameof(TestStrings))] string errorMessage)
        {
            ILightManager lightManager = Substitute.For<ILightManager>();
            IFireAlarmManager fireAlarmManager = Substitute.For<IFireAlarmManager>();
            IDoorManager doorManager = Substitute.For<IDoorManager>();
            IWebService webService = Substitute.For<IWebService>();
            IEmailService emailService = Substitute.For<IEmailService>();
            doorManager.OpenAllDoors().Returns(true);
            doorManager.LockAllDoors().Returns(true);

            // Set mock to throw exception if method is called
            Exception e = new Exception(errorMessage);
            webService.WhenForAnyArgs(x => x.LogFireAlarm(BuildingState.fireAlarm)).Do(x => { throw e; });

            BuildingController controller = new("", lightManager, fireAlarmManager, doorManager, webService, emailService);

            controller.SetCurrentState(sourceState);
            controller.SetCurrentState(BuildingState.fireAlarm);

            // Assert method call with exception message
            emailService.Received(1).SendMail(
                ExpectedTexts.emailAddress,
                ExpectedTexts.emailSubject,
                Arg.Is<string>(x => x.Contains(e.Message))
            );
        }
    }
}