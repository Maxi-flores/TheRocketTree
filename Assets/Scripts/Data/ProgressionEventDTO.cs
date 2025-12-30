using System;
using System.Collections.Generic;

namespace TheRocketTree.Data
{
    /// <summary>
    /// ProgressionEventDTO is the PRIMARY shared language between
    /// backend progression logic and Unity's rendering / UX systems.
    ///
    /// ARCHITECTURAL RULES:
    /// - These events are AUTHORITATIVE and backend-generated.
    /// - Unity must NEVER infer, calculate, or alter progression.
    /// - Unity only reacts to these events visually and emotionally.
    /// - AI/Grok insights may reference these events, but never modify them.
    /// </summary>
    [Serializable]
    public class ProgressionEventDTO
    {
        /// <summary>
        /// Unique event identifier (generated backend-side).
        /// Used for idempotency and safe replays.
        /// </summary>
        public string eventId;

        /// <summary>
        /// User this event belongs to.
        /// Included for clarity; Unity should already be scoped per-user.
        /// </summary>
        public string userId;

        /// <summary>
        /// When the event occurred (UTC).
        /// Used for sequencing and mood/weather evolution.
        /// </summary>
        public DateTime occurredAtUtc;

        /// <summary>
        /// High-level category of progression.
        /// Defines how Unity should interpret the event visually,
        /// NOT how valuable it was.
        /// </summary>
        public ProgressionEventType eventType;

        /// <summary>
        /// Optional subtype for finer expression.
        /// Unity may branch visuals on this, but must not assign value.
        /// </summary>
        public ProgressionEventSubtype eventSubtype;

        /// <summary>
        /// Contextual metadata attached by the backend.
        /// This data enriches meaning but must never be re-scored in Unity.
        ///
        /// Examples:
        /// - "taskDepth" : "small" | "medium" | "deep"
        /// - "reflectionPresent" : "true"
        /// - "returnAfterGapDays" : "2"
        /// - "sessionTone" : "calm" | "focused" | "tired"
        /// </summary>
        public Dictionary<string, string> metadata;

        /// <summary>
        /// Optional human-readable summary.
        /// Intended for UI display or debugging only.
        /// Never used for logic.
        /// </summary>
        public string summary;
    }

    /// <summary>
    /// Broad classification of progression events.
    /// These are stable and should evolve VERY slowly.
    /// </summary>
    public enum ProgressionEventType
    {
        Unknown = 0,

        /// <summary>
        /// A task was completed and validated by the backend.
        /// </summary>
        TaskCompleted = 1,

        /// <summary>
        /// A reflection was logged (with or without tasks).
        /// </summary>
        ReflectionLogged = 2,

        /// <summary>
        /// User returned after a gap in activity.
        /// This is NEVER punitive.
        /// </summary>
        ReturnAfterAbsence = 3,

        /// <summary>
        /// A derived session-level interpretation
        /// (e.g. "gentle progress", "deep focus day").
        /// </summary>
        SessionInterpreted = 4,

        /// <summary>
        /// Time-based state update (e.g. day tick).
        /// Used for mood/weather evolution only.
        /// </summary>
        TimeTick = 5
    }

    /// <summary>
    /// Optional finer-grained descriptor.
    /// These help Unity choose expressive visuals.
    /// </summary>
    public enum ProgressionEventSubtype
    {
        None = 0,

        // Task-related
        SmallTask = 10,
        MediumTask = 11,
        DeepTask = 12,

        // Reflection-related
        ReflectionWithTasks = 20,
        ReflectionOnly = 21,

        // Return-related
        GentleReturn = 30,
        LongAbsenceReturn = 31,

        // Session tone
        CalmSession = 40,
        FocusedSession = 41,
        HeavySession = 42
    }
}
