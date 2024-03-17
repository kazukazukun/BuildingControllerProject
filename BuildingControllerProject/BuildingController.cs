namespace BuildingControllerProject
{
    internal class BuildingController : IBuildingController
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
                    "only be initialised to the following states\n" +
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
            /// If a start state is given, validates it, else assigns initialState as startState.
            /// </summary>
            /// <param name="startState"></param>
            /// <returns>The initialState if nothing is given, else startState</returns>
            /// <exception cref="ArgumentException">Thrown if the given state is not a normal state.</exception>
            public static string InitializeState(string startState = initialState)
            {
                if (startState == initialState)
                    return initialState;
                startState = startState.ToLower();
                if (!State.IsNormal(startState))
                {
                    throw new ArgumentException(ErrorMessages.abnormalInitialState);
                }
                return startState;
            }

        }
 
        /// <summary>
        /// Stores the buildingID.
        /// </summary>
        private string buildingID;
        /// <summary>
        /// Stores the current building state.
        /// </summary>
        private string currentState;

        // Managers
        private IFireAlarmManager? fireAlarmManager;
        private ILightManager? lightManager;
        private IDoorManager? doorManager;
        // Services
        private IEmailService? emailService;
        private IWebService? webService;

        /// <summary>
        /// Create a BuildingController with the given id.
        /// Any uppercase letters will be converted to lower case.
        /// </summary>
        /// <param name="id">The buildingID</param>
        public BuildingController(string id)
        {
            buildingID = id.ToLower();
            currentState = State.InitializeState();
        }

        /// <summary>
        /// Create a BuildingController with the given id and startState.
        /// </summary>
        /// <param name="id">The buildingID</param>
        /// <param name="startState">The currentState</param>
        public BuildingController(string id, string startState)
        {
            currentState = State.InitializeState(startState);
            buildingID = id.ToLower();
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
                            IDoorManager iDoorManager, IWebService iWebService,IEmailService iEmailService)
        {
            currentState= State.InitializeState();
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
            buildingID = id.ToLower();
        }

        /// <summary>
        /// Gets current state of the building.
        /// </summary>
        /// <returns>State of the building</returns>
        public string GetCurrentState()
        {
            return currentState;
        }

        /// <summary>
        /// Sets given state as current state of the building.
        /// Checks if the given state is valid,
        /// if true, the state is set,
        /// else, the state is unchanged.
        /// </summary>
        /// <param name="state"></param>
        /// <returns>If the state was changed</returns>
        public bool SetCurrentState(string state)
        {
            if (!State.IsValid(state))
                return false;
            if (!State.IsValidTransition([ currentState, state ]))
                return false;
            if (currentState == state)
                return true;
            if (state == State.open)
            {
                if(!doorManager.OpenAllDoors())
                    return false;
            }
            currentState = state;
            return true;
        }

        /// <summary>
        /// Get status reports of all three managers.
        /// </summary>
        /// <returns>The light manager status, door manager status and fire alarm manager status</returns>
        public string GetStatusReport()
        {
            return lightManager.GetStatus() + doorManager.GetStatus() + fireAlarmManager.GetStatus();
        }
    }
}
