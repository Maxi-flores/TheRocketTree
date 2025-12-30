using System;

namespace TheRocketTree.Data
{
    /// <summary>
    /// Data Transfer Object for a Project (intent container).
    /// Projects define direction, not progress.
    /// </summary>
    [Serializable]
    public sealed class ProjectDTO
    {
        /// <summary>
        /// Unique project identifier.
        /// </summary>
        public string projectId;

        /// <summary>
        /// Owning user identifier.
        /// </summary>
        public string userId;

        /// <summary>
        /// Human-readable project title.
        /// </summary>
        public string title;

        /// <summary>
        /// Optional description provided by the user.
        /// </summary>
        public string description;

        /// <summary>
        /// ISO-8601 timestamp when the project was created.
        /// </summary>
        public string createdAt;

        /// <summary>
        /// ISO-8601 timestamp when the project was archived (nullable).
        /// </summary>
        public string archivedAt;

        /// <summary>
        /// True if the project is archived and no longer active.
        /// </summary>
        public bool isArchived;
    }
}
