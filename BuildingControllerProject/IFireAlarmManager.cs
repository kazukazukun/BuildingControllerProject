namespace BuildingControllerProject
{
    internal interface IFireAlarmManager : IManager
    {
        /// <summary>
        /// Sets alarm to Active/Inactive.
        /// </summary>
        /// <param name="isActive">Active or Inactive</param>
        public void SetAlarm(bool isActive);
    }
}
