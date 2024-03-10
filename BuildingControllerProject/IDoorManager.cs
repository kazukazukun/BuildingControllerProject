namespace BuildingControllerProject
{
    internal interface IDoorManager
    {
        /// <summary>
        /// Opens the door associated with the given doorID.
        /// </summary>
        /// <param name="doorID"></param>
        /// <returns>If the process was successful</returns>
        public bool OpenDoor(int doorID);

        /// <summary>
        /// Locks the door associated with the given doorID.
        /// </summary>
        /// <param name="doorID"></param>
        /// <returns>If the process was successful</returns>
        public bool LockDoor(int doorID);

        /// <summary>
        /// Opens all the doors.
        /// </summary>
        /// <returns>If the process was successful</returns>
        public bool OpenAllDoors();

        /// <summary>
        /// Locks all the doors.
        /// </summary>
        /// <returns>If the process was successful</returns>
        public bool LockAllDoors();
    }
}
