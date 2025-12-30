using System;

namespace TheRocketTree.Data
{
    /// <summary>
    /// Data Transfer Object for the authoritative TreeState.
    /// Represents slow, accumulated physical structure of the tree.
    /// This state is computed and written ONLY by the backend.
    /// </summary>
    [Serializable]
    public sealed class TreeStateDTO
    {
        /// <summary>
        /// Owning user identifier.
        /// TreeState is 1:1 with user.
        /// </summary>
        public string userId;

        /// <summary>
        /// Accumulated mass of the tree.
        /// Interpreted visually as trunk thickness / age proxy.
        /// Never decreases.
        /// </summary>
        public float mass;

        /// <summary>
        /// Structural complexity of the tree.
        /// Interpreted visually as branching density / spread.
        /// Never decreases.
        /// </summary>
        public float structure;

        /// <summary>
        /// Vitality of the tree.
        /// Range: 0..1
        /// Mood-sensitive, slow drift, backend-controlled.
        /// </summary>
        public float vitality;

        /// <summary>
        /// ISO-8601 timestamp of the last authoritative update.
        /// </summary>
        public string lastUpdatedAt;
    }
}
