namespace BuildingControllerProject
{
    internal class BuildingController : IBuildingController
    {
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
            currentState = state;
            return true;
        }
    }
}
