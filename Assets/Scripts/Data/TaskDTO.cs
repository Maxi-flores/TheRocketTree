using System;

namespace TheRocketTree.Data
{
    /// <summary>
    /// Data Transfer Object for a Task.
    /// Tasks represent user-declared effort attempts.
    /// Completion does NOT imply growth — backend decides meaning.
    /// </summary>
    [Serializable]
    public sealed class TaskDTO
    {
        /// <summary>
        /// Unique task identifier.
        /// </summary>
        public string taskId;

        /// <summary>
        /// Owning user identifier.
        /// </summary>
        public string userId;

        /// <summary>
        /// Parent project identifier (intent container).
        /// </summary>
        public string projectId;

        /// <summary>
        /// Human-readable task title.
        /// </summary>
        public string title;

        /// <summary>
        /// Optional user notes.
        /// </summary>
        public string notes;

        /// <summary>
        /// User-estimated depth hint.
        /// This is NOT authoritative.
        /// Expected values: small | medium | deep
        /// </summary>
        public string estimatedDepth;

        /// <summary>
        /// Current task status.
        /// Expected values: open | completed | abandoned
        /// </summary>
        public string status;

        /// <summary>
        /// ISO-8601 timestamp when task was created.
        /// </summary>
        public string createdAt;

        /// <summary>
        /// ISO-8601 timestamp when task was completed (nullable).
        /// </summary>
        public string completedAt;
    }
}
