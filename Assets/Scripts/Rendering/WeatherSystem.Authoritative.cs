using UnityEngine;
using TheRocketTree.Data;

namespace TheRocketTree.Rendering
{
    /// <summary>
    /// Authoritative backend-driven hooks for WeatherSystem.
    /// Weather reflects state — it never computes it.
    /// </summary>
    public partial class WeatherSystem
    {
        /// <summary>
        /// Apply long-term vitality as baseline atmosphere.
        /// </summary>
        public void SetAuthoritativeVitality(float vitality)
        {
            // Clamp defensively; backend already guarantees bounds
            vitality = Mathf.Clamp01(vitality);

            ApplyBaselineWeather(vitality);
        }

        /// <summary>
        /// React visually to a single progression event.
        /// </summary>
        public void ReactToEvent(ProgressionEventDTO evt)
        {
            if (evt == null) return;

            switch (evt.type)
            {
                case "TASK_COMPLETED":
                    GentleSunbreak();
                    break;

                case "REFLECTION_LOGGED":
                    CalmWind();
                    break;
            }
        }

        // ─────────────────────────────
        // Internal weather helpers
        // ─────────────────────────────

        private void ApplyBaselineWeather(float vitality)
        {
            // Example:
            // - cloud density
            // - light warmth
            // - ambient audio
        }

        private void GentleSunbreak()
        {
            // Brief light increase / cloud opening
        }

        private void CalmWind()
        {
            // Soft wind, leaf motion, audio cue
        }
    }
}
