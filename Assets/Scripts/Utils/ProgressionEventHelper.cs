using System;
using System.Collections.Generic;
using System.Linq;
using TheRocketTree.Data;

namespace TheRocketTree.Utils
{
    /// <summary>
    /// Utility helpers for working with backend-issued ProgressionEvents.
    ///
    /// IMPORTANT:
    /// - This class NEVER computes growth or meaning.
    /// - It NEVER mutates state.
    /// - It ONLY helps Unity classify and organize events.
    /// </summary>
    public static class ProgressionEventHelper
    {
        // ─────────────────────────────────────────────
        // Type checks
        // ─────────────────────────────────────────────

        public static bool IsTaskCompleted(ProgressionEventDTO evt)
            => evt != null && evt.type == "TASK_COMPLETED";

        public static bool IsReflectionLogged(ProgressionEventDTO evt)
            => evt != null && evt.type == "REFLECTION_LOGGED";

        public static bool IsReturnAfterAbsence(ProgressionEventDTO evt)
            => evt != null && evt.type == "RETURN_AFTER_ABSENCE";

        public static bool IsTimeTick(ProgressionEventDTO evt)
            => evt != null && evt.type == "TIME_TICK";

        // ─────────────────────────────────────────────
        // Ordering & filtering
        // ─────────────────────────────────────────────

        /// <summary>
        /// Returns events ordered by occurredAt (ascending).
        /// Safe against nulls and malformed timestamps.
        /// </summary>
        public static IReadOnlyList<ProgressionEventDTO> OrderChronologically(
            IEnumerable<ProgressionEventDTO> events)
        {
            if (events == null)
                return Array.Empty<ProgressionEventDTO>();

            return events
                .Where(e => e != null)
                .OrderBy(e => ParseUtcSafe(e.occurredAt))
                .ToList();
        }

        /// <summary>
        /// Filters events by type.
        /// </summary>
        public static IReadOnlyList<ProgressionEventDTO> FilterByType(
            IEnumerable<ProgressionEventDTO> events,
            string type)
        {
            if (events == null || string.IsNullOrEmpty(type))
                return Array.Empty<ProgressionEventDTO>();

            return events
                .Where(e => e != null && e.type == type)
                .ToList();
        }

        // ─────────────────────────────────────────────
        // Grouping helpers
        // ─────────────────────────────────────────────

        /// <summary>
        /// Groups events by calendar day (UTC).
        /// Useful for session-based rendering.
        /// </summary>
        public static IReadOnlyDictionary<DateTime, List<ProgressionEventDTO>>
            GroupByUtcDay(IEnumerable<ProgressionEventDTO> events)
        {
            if (events == null)
                return new Dictionary<DateTime, List<ProgressionEventDTO>>();

            return events
                .Where(e => e != null)
                .GroupBy(e => ParseUtcSafe(e.occurredAt).Date)
                .ToDictionary(
                    g => g.Key,
                    g => g.ToList()
                );
        }

        // ─────────────────────────────────────────────
        // Metadata helpers
        // ─────────────────────────────────────────────

        /// <summary>
        /// Safely gets a metadata value by key.
        /// Returns null if missing.
        /// </summary>
        public static string GetMetadata(
            ProgressionEventDTO evt,
            string key)
        {
            if (evt?.metadata == null || string.IsNullOrEmpty(key))
                return null;

            return evt.metadata.TryGetValue(key, out var value)
                ? value
                : null;
        }

        // ─────────────────────────────────────────────
        // Internal helpers
        // ─────────────────────────────────────────────

        private static DateTime ParseUtcSafe(string isoUtc)
        {
            if (string.IsNullOrEmpty(isoUtc))
                return DateTime.MinValue;

            if (DateTime.TryParse(
                    isoUtc,
                    null,
                    System.Globalization.DateTimeStyles.AdjustToUniversal,
                    out var parsed))
            {
                return parsed;
            }

            return DateTime.MinValue;
        }
    }
}
