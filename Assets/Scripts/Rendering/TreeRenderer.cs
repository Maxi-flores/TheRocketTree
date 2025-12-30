using System;
using UnityEngine;
using TheRocketTree.Data;

namespace TheRocketTree.Rendering
{
    /// <summary>
    /// TreeRenderer is a PURE visual responder.
    ///
    /// RESPONSIBILITIES:
    /// - React to authoritative ProgressionEventDTOs
    /// - Trigger animations, blends, and visual states
    ///
    /// HARD RULES:
    /// - NO growth math
    /// - NO scoring
    /// - NO interpretation of meaning
    /// - NO persistence
    ///
    /// The backend decides WHAT happened.
    /// This class decides only HOW it feels visually.
    /// </summary>
    public class TreeRenderer : MonoBehaviour
    {
        [Header("Tree Visual Roots")]
        [SerializeField] private Transform trunkRoot;
        [SerializeField] private Animator treeAnimator;

        [Header("Visual Parameters")]
        [SerializeField] private float gentleGrowthScale = 0.02f;
        [SerializeField] private float deepGrowthScale = 0.05f;

        [Header("Branching")]
        [SerializeField] private GameObject branchPrefab;
        [SerializeField] private Transform branchAnchor;

        private void Awake()
        {
            if (trunkRoot == null)
            {
                Debug.LogWarning("[TreeRenderer] Trunk root not assigned.");
            }
        }

        /// <summary>
        /// Entry point for all progression-related visual updates.
        /// Called by SessionManager.
        /// </summary>
        public void ApplyProgressionEvent(ProgressionEventDTO evt)
        {
            if (evt == null)
                return;

            switch (evt.eventType)
            {
                case ProgressionEventType.TaskCompleted:
                    HandleTaskCompleted(evt);
                    break;

                case ProgressionEventType.ReflectionLogged:
                    HandleReflection(evt);
                    break;

                case ProgressionEventType.ReturnAfterAbsence:
                    HandleReturn(evt);
                    break;

                case ProgressionEventType.SessionInterpreted:
                    HandleSessionTone(evt);
                    break;

                case ProgressionEventType.TimeTick:
                    HandleTimeTick(evt);
                    break;

                default:
                    // Unknown or unsupported event — ignore safely
                    break;
            }
        }

        // ─────────────────────────────
        // Event Handlers (visual only)
        // ─────────────────────────────

        private void HandleTaskCompleted(ProgressionEventDTO evt)
        {
            switch (evt.eventSubtype)
            {
                case ProgressionEventSubtype.DeepTask:
                    ApplyGentleScale(deepGrowthScale);
                    TriggerBranchGrowth();
                    TriggerAnimation("DeepGrowth");
                    break;

                case ProgressionEventSubtype.MediumTask:
                case ProgressionEventSubtype.SmallTask:
                default:
                    ApplyGentleScale(gentleGrowthScale);
                    TriggerAnimation("GentleGrowth");
                    break;
            }
        }

        private void HandleReflection(ProgressionEventDTO evt)
        {
            // Reflection stabilizes and enriches visuals
            TriggerAnimation("ReflectionPulse");
        }

        private void HandleReturn(ProgressionEventDTO evt)
        {
            // Returns are honored, never punished
            TriggerAnimation("GentleReturn");
        }

        private void HandleSessionTone(ProgressionEventDTO evt)
        {
            // Session tone affects subtle sway / vitality
            switch (evt.eventSubtype)
            {
                case ProgressionEventSubtype.CalmSession:
                    TriggerAnimation("CalmSway");
                    break;

                case ProgressionEventSubtype.FocusedSession:
                    TriggerAnimation("FocusedGlow");
                    break;

                case ProgressionEventSubtype.HeavySession:
                    TriggerAnimation("HeavyStillness");
                    break;
            }
        }

        private void HandleTimeTick(ProgressionEventDTO evt)
        {
            // Time passing without progress should feel patient
            TriggerAnimation("IdleBreath");
        }

        // ─────────────────────────────
        // Visual Helpers
        // ─────────────────────────────

        private void ApplyGentleScale(float delta)
        {
            if (trunkRoot == null)
                return;

            var currentScale = trunkRoot.localScale;
            trunkRoot.localScale = currentScale + Vector3.up * delta;
        }

        private void TriggerBranchGrowth()
        {
            if (branchPrefab == null || branchAnchor == null)
                return;

            Instantiate(branchPrefab, branchAnchor.position, branchAnchor.rotation, branchAnchor);
        }

        private void TriggerAnimation(string triggerName)
        {
            if (treeAnimator == null)
                return;

            treeAnimator.SetTrigger(triggerName);
        }
    }
}
