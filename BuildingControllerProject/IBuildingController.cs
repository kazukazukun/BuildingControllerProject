namespace BuildingControllerProject
{
    /// <summary>
    /// Interface for Building Controller
    /// </summary>
    internal interface IBuildingController
    {
        /// <summary>
        /// Gets current state of the building.
        /// </summary>
        /// <returns>State of the building</returns>
        public string? GetCurrentState();

        /// <summary>
        /// Sets given state as current state of the building.
        /// </summary>
        /// <param name="state"></param>
        /// <returns>If the process was successful</returns>
        public bool SetCurrentState(string state);

        /// <summary>
        /// Gets buildingID.
        /// </summary>
        /// <returns>The buildingID</returns>
        public string? GetBuildingID();

        /// <summary>
        /// Sets buildingID to the given id.
        /// </summary>
        /// <param name="id">The buildingID</param>
        public void SetBuildingID(string id);

        /// <summary>
        /// Gets status report.
        /// </summary>
        /// <returns>Status report</returns>
        public string GetStatusReport();
    }
}
