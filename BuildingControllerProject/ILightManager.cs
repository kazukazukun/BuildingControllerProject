namespace BuildingControllerProject
{
    internal interface ILightManager : IManager
    {
        /// <summary>
        /// Sets the light with given lightID On/Off.
        /// </summary>
        /// <param name="isOn">On or Off</param>
        /// <param name="lightID"></param>
        public void SetLight(bool isOn, int lightID);

        /// <summary>
        /// Sets all the lights On/Off.
        /// </summary>
        /// <param name="isOn">On or Off</param>
        public void SetAllLights(bool isOn);
    }
}
