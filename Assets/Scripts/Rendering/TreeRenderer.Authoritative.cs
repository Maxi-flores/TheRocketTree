using UnityEngine;
using TheRocketTree.Data;

namespace TheRocketTree.Rendering
{
    /// <summary>
    /// Authoritative backend-driven hooks for TreeRenderer.
    /// This file contains NO growth math — only visual interpretation.
    /// </summary>
    public partial class TreeRenderer
    {
        /// <summary>
        /// Apply the full authoritative TreeState.
        /// Called on session start or hard refresh.
        /// </summary>
        public void SetAuthoritativeState(TreeStateDTO state)
        {
            if (state == null)
            {
                Debug.LogWarning("[TreeRenderer] Null TreeState received.");
                return;
            }

            // These values are already computed by backend.
            // Unity ONLY renders them.
            ApplyMass(state.mass);
            ApplyStructure(state.structure);
            ApplyVitality(state.vitality);
        }

        /// <summary>
        /// Play a small, local visual reaction to a progression event.
        /// No state is mutated here.
        /// </summary>
        public void PlayEventPulse(ProgressionEventDTO evt)
        {
            if (evt == null) return;

            switch (evt.type)
            {
                case "TASK_COMPLETED":
                    PulseGrowth();
                    break;

                case "REFLECTION_LOGGED":
                    PulseGlow();
                    break;
            }
        }

        // ─────────────────────────────
        // Internal visual helpers
        // (pure rendering, safe to tweak)
        // ─────────────────────────────

        private void ApplyMass(float mass)
        {
            // Example: scale trunk thickness
            // Implement using your existing mesh/material system
        }

        private void ApplyStructure(float structure)
        {
            // Example: branch density / spread
        }

        private void ApplyVitality(float vitality)
        {
            // Example: color saturation / sway amplitude
        }

        private void PulseGrowth()
        {
            // Short animation, particles, or shader pulse
        }

        private void PulseGlow()
        {
            // Gentle glow or breathing effect
        }
    }
}
