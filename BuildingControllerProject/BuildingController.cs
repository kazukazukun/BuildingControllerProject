using System.Security.Claims;

namespace BuildingControllerProject
{
    public class BuildingController : IBuildingController
    {
        /// <summary>
        /// Class to store possible states of the Building.
        /// </summary>
        private class State
        {
            /// <summary>
            /// Class to store error messages.
            /// </summary>
            private class ErrorMessages
            {
                public static readonly string abnormalInitialState = "Argument Exception: BuildingController can " +
                    "only be initialised to the following states " +
                    "'open', 'closed', 'out of hours'";
            }

            public const string open = "open";
            public const string outOfHours = "out of hours";
            public const string closed = "closed";
            public const string fireAlarm = "fire alarm";
            public const string fireDrill = "fire drill";
            public const string initialState = outOfHours;

            /// <summary>
            /// Array to hold valid states.
            /// </summary>
            public static readonly string[] validStates = { open, outOfHours, closed, fireAlarm, fireDrill };

            /// <summary>
            /// Array to hold normal operation states.
            /// </summary>
            public static readonly string[] normalStates = { open, outOfHours, closed };

            /// <summary>
            /// Array to hold abnormal operation states.
            /// </summary>
            public static readonly string[] abnormalStates = { fireAlarm, fireDrill };

            /// <summary>
            /// 2D array to hold invalid state transitions.
            /// </summary>
            public static readonly string[][] invalidTransitions = {
                                                                    new string[] { open, closed },
                                                                    new string[] { closed, open }
            };

            /// <summary>
            /// Checks if the given state is valid.
            /// </summary>
            /// <param name="state"></param>
            /// <returns>True if the state is valid, else false</returns>
            public static bool IsValid(string state)
            {
                for (int i = 0; i < validStates.Length; i++)
                {
                    if (validStates[i] == state)
                        return true;
                }
                return false;
            }

            /// <summary>
            /// Checks if the given state transition is valid.
            /// </summary>
            /// <param name="stateTransition"></param>
            /// <returns>True if the state transition is valid, else false</returns>
            public static bool IsValidTransition(string[] stateTransition)
            {
                for (int i = 0; i < invalidTransitions.Length; i++)
                {
                    // Comparing given state transition with invalid transitions
                    if (stateTransition.SequenceEqual(invalidTransitions[i]))
                        return false;
                }
                return true;
            }

            /// <summary>
            /// Checks if the given state is normal.
            /// </summary>
            /// <param name="state"></param>
            /// <returns>True if the state is normal, else false</returns>
            public static bool IsNormal(string state)
            {
                for (int i = 0; i < normalStates.Length; i++)
                {
                    if (normalStates[i] == state)
                        return true;
                }
                return false;
            }

            /// <summary>
            /// Checks if the given state is abnormal.
            /// </summary>
            /// <param name="state"></param>
            /// <returns>True if the state is abnormal, else false</returns>
            public static bool IsAbnormal(string state)
            {
                for (int i = 0; i < abnormalStates.Length; i++)
                {
                    if (abnormalStates[i] == state)
                        return true;
                }
                return false;
            }

            /// <summary>
            /// If a start state is given, validates it, else assigns initialState as startState.
            /// </summary>
            /// <param name="startState"></param>
            /// <returns>The initialState if nothing is given, else startState</returns>
            /// <exception cref="ArgumentException">Thrown if the given state is not a normal state.</exception>
            public static string InitializeState(string startState = initialState)
            {
                if (startState == initialState)
                    return initialState;
                if(!string.IsNullOrEmpty(startState))
                    startState = startState.ToLower();
                if (startState == null || !State.IsNormal(startState))
                {
                    throw new ArgumentException(ErrorMessages.abnormalInitialState);
                }
                return startState;
            }

        }

        /// <summary>
        /// Class to store manager names.
        /// </summary>
        public class Managers
        {
            public const string doors = "Doors";
            public const string lights = "Lights";
            public const string fireAlarm = "FireAlarm";
        }

        /// <summary>
        /// Class to store texts for EmailService parameters.
        /// </summary>
        private class EmailServiceParams
        {
            public const string address = "smartbuilding@uclan.ac.uk";
            public const string subject = "failed to log alarm";
        }

        /// <summary>
        /// Stores the buildingID.
        /// </summary>
        private string buildingID;
        /// <summary>
        /// Stores the current building state.
        /// </summary>
        private string currentState;
        /// <summary>
        /// Stores the previous building state.
        /// </summary>
        private string previousState = State.outOfHours;

        // Managers
        private IFireAlarmManager? fireAlarmManager;
        private ILightManager? lightManager;
        private IDoorManager? doorManager;
        // Services
        private IEmailService? emailService;
        private IWebService? webService;

        // Text returned by a device functioning properly
        const string deviceOK = "OK";
        // Text returned by a device malfunctioning
        const string deviceFaulty = "FAULT";

        /// <summary>
        /// Create a BuildingController with the given id.
        /// Any uppercase letters will be converted to lower case.
        /// </summary>
        /// <param name="id">The buildingID</param>
        public BuildingController(string id)
        {
            if (!string.IsNullOrEmpty(id))
                id = id.ToLower();
            buildingID = id;
            currentState = State.InitializeState();
        }

        /// <summary>
        /// Create a BuildingController with the given id and startState.
        /// </summary>
        /// <param name="id">The buildingID</param>
        /// <param name="startState">The currentState</param>
        public BuildingController(string id, string startState)
        {
            if(!string.IsNullOrEmpty(id))
                id = id.ToLower();
            currentState = State.InitializeState(startState);
            buildingID = id;
        }

        /// <summary>
        /// Create a BuildingController with given id, managers and services.
        /// </summary>
        /// <param name="id">The buildingID</param>
        /// <param name="iLightManager"></param>
        /// <param name="iFireAlarmManager"></param>
        /// <param name="iDoorManager"></param>
        /// <param name="iWebService"></param>
        /// <param name="iEmailService"></param>
        public BuildingController(string id, ILightManager iLightManager, IFireAlarmManager iFireAlarmManager,
                            IDoorManager iDoorManager, IWebService iWebService, IEmailService iEmailService)
        {
            currentState = State.InitializeState();
            buildingID = id.ToLower();
            fireAlarmManager = iFireAlarmManager;
            lightManager = iLightManager;
            doorManager = iDoorManager;
            emailService = iEmailService;
            webService = iWebService;
        }

        /// <summary>
        /// Get the buildingID.
        /// </summary>
        /// <returns>The buildingID</returns>
        public string GetBuildingID()
        {
            return buildingID;
        }

        /// <summary>
        /// Sets the value of buildingID.
        /// Any uppercase letters will be converted to lower case.
        /// </summary>
        /// <param name="id">The buildingID</param>
        public void SetBuildingID(string id)
        {
            if(!string.IsNullOrEmpty(id))
                id = id.ToLower();
            buildingID = id;
        }

        /// <summary>
        /// Gets current state of the building.
        /// </summary>
        /// <returns>State of the building</returns>
        public string? GetCurrentState()
        {
            return currentState;
        }

        /// <summary>
        /// Sets given state as current state of the building.
        /// Checks if the given state is valid and
        /// is a possible transition from current state,
        /// if true, the state is set,
        /// else, the state is unchanged.
        /// </summary>
        /// <param name="state"></param>
        /// <returns>If the state was changed</returns>
        public bool SetCurrentState(string state)
        {
            if (!State.IsValid(state))
                return false;
            if (!State.IsValidTransition(new string[] { currentState, state }))
                return false;
            if (currentState == state)
                return true;
            if (State.IsAbnormal(currentState) && previousState != state)
                return false;
            if (!AffectStateTransition(state))
                return false;
            previousState = currentState;
            currentState = state;
            return true;
        }

        /// <summary>
        /// According to the given state,
        /// fire alarms, doors, lights, web service and email service are affected.
        /// </summary>
        /// <param name="state"></param>
        /// <returns>True if affected properly, else false</returns>
        private bool AffectStateTransition(string state)
        {
            if (state == State.open)
            {
                if (doorManager == null)
                    return false;
                if (!doorManager.OpenAllDoors())
                    return false;
            }
            else if (state == State.closed)
            {
                if (doorManager == null || lightManager == null)
                    return false;
                if (!doorManager.LockAllDoors())
                    return false;
                lightManager.SetAllLights(false);
            }
            else if (state == State.fireAlarm)
            {
                if (fireAlarmManager == null)
                    return false;
                fireAlarmManager.SetAlarm(true);
                if (doorManager == null)
                    return false;
                doorManager.OpenAllDoors();
                if (lightManager == null)
                    return false;
                lightManager.SetAllLights(true);
                if (webService == null)
                    return false;
                try
                {
                    webService.LogFireAlarm(State.fireAlarm);
                }
                catch (Exception exception)
                {
                    emailService?.SendMail(EmailServiceParams.address, EmailServiceParams.subject, exception.ToString());
                }
            }
            return true;
        }

        /// <summary>
        /// Get status reports of all three managers.
        /// Logs an engineer inquery if any of the devices are malfunctioning.
        /// </summary>
        /// <returns>The light manager status, door manager status and fire alarm manager status</returns>
        public string GetStatusReport()
        {
            string lightStatus = "";
            string doorStatus = "";
            string fireAlarmStatus = "";
            List<string> faultyDevices = new List<string>();
            if (lightManager != null)
            {
                lightStatus = lightManager.GetStatus();
                if (IsFaulty(lightStatus, Managers.lights))
                    faultyDevices.Add(Managers.lights);
            }
            if (doorManager != null)
            {
                doorStatus = doorManager.GetStatus();
                if (IsFaulty(doorStatus, Managers.doors))
                    faultyDevices.Add(Managers.doors);
            }
            if (fireAlarmManager != null)
            {
                fireAlarmStatus = fireAlarmManager.GetStatus();
                if (IsFaulty(fireAlarmStatus, Managers.fireAlarm))
                    faultyDevices.Add(Managers.fireAlarm);
            }
            ReportFaultyDevices(faultyDevices);
            return lightStatus + doorStatus + fireAlarmStatus;
        }

        /// <summary>
        /// Checks if the given status report contains about any faulty devices.
        /// </summary>
        /// <param name="statusReport">The status report</param>
        /// <param name="managerType">The manager type</param>
        /// <returns>True if the report contains about any faulty devices, else false</returns>
        private bool IsFaulty(string statusReport, string managerType)
        {
            List<string> eachStatus = new List<string>(statusReport.Split(','));
            if (eachStatus[0] == managerType && eachStatus[eachStatus.Count - 1] == "")
            {
                for (int i = 1; i < eachStatus.Count - 1; i++)
                {
                    if (eachStatus[i] != deviceOK)
                        return true;
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// Logs an engineer inquery based on the given malfunctioning devices list.
        /// </summary>
        /// <param name="faultDevices">The list containing faulty device types</param>
        private void ReportFaultyDevices(List<string> faultDevices)
        {
            if (webService == null)
                return;
            if (faultDevices.Count > 0)
            {
                string log = "";
                if (faultDevices.Count == 1)
                    log += faultDevices[0];
                else
                {
                    for (int i = 0; i < faultDevices.Count; i++)
                    {
                        log += faultDevices[i] + ",";
                    }
                }
                webService.LogEngineerRequired(log);
            }
        }
    }
}
