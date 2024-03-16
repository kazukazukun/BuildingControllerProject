namespace BuildingControllerProject
{
    internal class BuildingController : IBuildingController
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
        /// <summary>
        /// Stores the buildingID.
        /// </summary>
        private string buildingID;
        /// <summary>
        /// Stores the current building state.
        /// </summary>
        private string currentState;

        /// <summary>
        /// Create a BuildingController with the given id.
        /// Any uppercase letters will be converted to lower case.
        /// </summary>
        /// <param name="id">The buildingID</param>
        public BuildingController(string id)
        {
            buildingID = id.ToLower();
            currentState = State.initialState;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">The buildingID</param>
        /// <param name="startState">The currentState</param>
        /// <exception cref="ArgumentException"></exception>
        public BuildingController(string id, string startState)
        {
            if (!State.IsNormal(startState.ToLower()))
            {
                throw new ArgumentException(ErrorMessages.abnormalInitialState);
            }
            buildingID = id.ToLower();
            currentState = startState.ToLower();
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
            currentState = state;
            return true;
        }
    }
}
