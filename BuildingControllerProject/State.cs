namespace BuildingControllerProject
{
    /// <summary>
    /// Class to store possible states of the Building.
    /// </summary>
    internal class State
    {     
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


    }
}
