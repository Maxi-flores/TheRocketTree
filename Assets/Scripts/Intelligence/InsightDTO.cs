using System;
using System.Collections.Generic;

namespace TheRocketTree.Intelligence
{
    /// <summary>
    /// Data Transfer Object representing a reflective insight.
    /// Insights help the user understand patterns and meaning
    /// in already-authoritative data.
    ///
    /// Insights never cause growth, progression, or state changes.
    /// </summary>
    [Serializable]
    public sealed class InsightDTO
    {
        /// <summary>
        /// Primary insight text shown to the user.
        /// Calm, non-judgmental, non-directive.
        /// </summary>
        public string text;

        /// <summary>
        /// Optional short title or theme.
        /// </summary>
        public string title;

        /// <summary>
        /// Time window this insight refers to (ISO-8601).
        /// </summary>
        public string fromUtc;
        public string toUtc;

        /// <summary>
        /// Optional tags describing the nature of the insight.
        /// Examples: consistency, return, gentleness, momentum
        /// </summary>
        public List<string> tags;
    }
}
