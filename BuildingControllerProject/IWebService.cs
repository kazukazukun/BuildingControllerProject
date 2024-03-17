namespace BuildingControllerProject
{
    public interface IWebService
    {
        /// <summary>
        /// Changes log details with given logDetails.
        /// </summary>
        /// <param name="logDetails"></param>
        public void LogStateChange(string logDetails);

        /// <summary>
        /// Logs the details of engineer requirement.
        /// </summary>
        /// <param name="logDetails"></param>
        public void LogEngineerRequired(string logDetails);

        /// <summary>
        /// Logs details related to a fire alarm event.
        /// </summary>
        /// <param name="logDetails"></param>
        public void LogFireAlarm(string logDetails);
    }
}
