using System;
using System.Collections.Generic;

namespace TheRocketTree.Data
{
    /// <summary>
    /// Data Transfer Object for User profile data.
    /// Holds identity and long-term context only.
    /// No progression, no counters, no growth logic.
    /// </summary>
    [Serializable]
    public sealed class UserDTO
    {
        /// <summary>
        /// Firebase Auth user identifier.
        /// </summary>
        public string userId;

        /// <summary>
        /// ISO-8601 timestamp when the user was created.
        /// </summary>
        public string createdAt;

        /// <summary>
        /// ISO-8601 timestamp of last activity.
        /// Updated by backend.
        /// </summary>
        public string lastActiveAt;

        /// <summary>
        /// IANA timezone string (e.g., Europe/Berlin).
        /// </summary>
        public string timezone;

        /// <summary>
        /// Locale identifier (e.g., en-US).
        /// </summary>
        public string locale;

        /// <summary>
        /// Account lifecycle state.
        /// Expected values: active | paused | deleted
        /// </summary>
        public string accountState;

        /// <summary>
        /// Current subscription tier.
        /// Expected values: free | plus | lifetime
        /// </summary>
        public string subscriptionTier;

        /// <summary>
        /// Feature and onboarding flags.
        /// </summary>
        public Dictionary<string, bool> flags;
    }
}
