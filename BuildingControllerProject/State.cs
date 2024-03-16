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

    }
}
