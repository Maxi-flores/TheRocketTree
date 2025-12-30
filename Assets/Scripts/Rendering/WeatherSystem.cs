using System;
using UnityEngine;
using TheRocketTree.Data;

namespace TheRocketTree.Rendering
{
    /// <summary>
    /// WeatherSystem is a PURE atmospheric responder.
    ///
    /// RESPONSIBILITIES:
    /// - React to authoritative ProgressionEventDTOs
    /// - Transition between calm "mood weather" states
    ///
    /// HARD RULES:
    /// - NO growth math
    /// - NO punitive decay
    /// - NO state authority (backend owns meaning)
    ///
    /// Weather is used to make time and engagement FEEL tangible:
    /// - Inactivity => quieter, muted atmosphere (not negative)
    /// - Return => gentle clearing / warmth (welcoming)
    /// - Reflection => restorative rain / softness
    /// </summary>
    public class WeatherSystem : MonoBehaviour
    {
        public enum MoodWeather
        {
            Neutral = 0,
            ClearWarm = 1,
            SoftOvercast = 2,
            StillMuted = 3,
            GentleRain = 4,
            BreakInClouds = 5,
            FocusedGlow = 6,
            HeavyStillness = 7
        }

        [Header("Optional Scene Hooks")]
        [SerializeField] private Light directionalLight;
        [SerializeField] private ParticleSystem rainParticles;
        [SerializeField] private Animator skyAnimator;

        [Header("Transition Settings")]
        [SerializeField] private float transitionDurationSeconds = 3.0f;

        private MoodWeather _current = MoodWeather.Neutral;
        private MoodWeather _target = MoodWeather.Neutral;

        private float _transitionT;
        private bool _transitioning;

        private void Awake()
        {
            // Ensure rain starts off
            if (rainParticles != null && rainParticles.isPlaying)
                rainParticles.Stop();
        }

        private void Update()
        {
            if (!_transitioning)
                return;

            _transitionT += Time.deltaTime / Mathf.Max(0.01f, transitionDurationSeconds);
            if (_transitionT >= 1f)
            {
                _transitionT = 1f;
                _transitioning = false;
                _current = _target;
            }

            ApplyInterpolatedAtmosphere(_current, _target, _transitionT);
        }

        /// <summary>
        /// Entry point for authoritative events.
        /// Called by SessionManager.
        /// </summary>
        public void ApplyProgressionEvent(ProgressionEventDTO evt)
        {
            if (evt == null)
                return;

            switch (evt.eventType)
            {
                case ProgressionEventType.TaskCompleted:
                    HandleTask(evt);
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
                    break;
            }
        }

        // ─────────────────────────────
        // Event handlers (atmosphere only)
        // ─────────────────────────────

        private void HandleTask(ProgressionEventDTO evt)
        {
            // Tasks generally brighten slightly, but without fireworks.
            // Deep work can feel clearer; small tasks remain subtle.
            switch (evt.eventSubtype)
            {
                case ProgressionEventSubtype.DeepTask:
                    TransitionTo(MoodWeather.ClearWarm);
                    TriggerSky("ClearWarm");
                    break;

                case ProgressionEventSubtype.MediumTask:
                case ProgressionEventSubtype.SmallTask:
                default:
                    TransitionTo(MoodWeather.SoftOvercast);
                    TriggerSky("SoftOvercast");
                    break;
            }
        }

        private void HandleReflection(ProgressionEventDTO evt)
        {
            // Reflection is restorative — gentle rain / softness.
            TransitionTo(MoodWeather.GentleRain);
            TriggerSky("GentleRain");
            SetRain(true);
        }

        private void HandleReturn(ProgressionEventDTO evt)
        {
            // Returning is welcomed: break in clouds.
            TransitionTo(MoodWeather.BreakInClouds);
            TriggerSky("BreakInClouds");
            SetRain(false);
        }

        private void HandleSessionTone(ProgressionEventDTO evt)
        {
            // Backend may tag the day's feel. We render it softly.
            switch (evt.eventSubtype)
            {
                case ProgressionEventSubtype.CalmSession:
                    TransitionTo(MoodWeather.Neutral);
                    TriggerSky("Neutral");
                    SetRain(false);
                    break;

                case ProgressionEventSubtype.FocusedSession:
                    TransitionTo(MoodWeather.FocusedGlow);
                    TriggerSky("FocusedGlow");
                    SetRain(false);
                    break;

                case ProgressionEventSubtype.HeavySession:
                    TransitionTo(MoodWeather.HeavyStillness);
                    TriggerSky("HeavyStillness");
                    SetRain(false);
                    break;
            }
        }

        private void HandleTimeTick(ProgressionEventDTO evt)
        {
            // Time passing without progress should feel patient, not punishing.
            // We nudge toward still/muted, but never "bad".
            TransitionTo(MoodWeather.StillMuted);
            TriggerSky("StillMuted");
            // Don't force rain off/on here.
        }

        // ─────────────────────────────
        // Transition engine
        // ─────────────────────────────

        private void TransitionTo(MoodWeather target)
        {
            if (_target == target && (_transitioning || _current == target))
                return;

            _target = target;
            _transitionT = 0f;
            _transitioning = true;

            Debug.Log($"[WeatherSystem] Transition {_current} -> {_target}");
        }

        private void ApplyInterpolatedAtmosphere(MoodWeather from, MoodWeather to, float t)
        {
            // This is intentionally conservative:
            // - We avoid hard-coded artistic values until art direction is ready.
            // - We keep hooks for light intensity, angle, fog, etc.

            if (directionalLight != null)
            {
                // Basic brightness shaping without forcing a palette.
                // These values are placeholders and safe to tune later.
                float fromIntensity = GetLightIntensity(from);
                float toIntensity = GetLightIntensity(to);
                directionalLight.intensity = Mathf.Lerp(fromIntensity, toIntensity, t);
            }

            // Rain handled separately via SetRain.
        }

        private float GetLightIntensity(MoodWeather mood)
        {
            // Placeholder mapping. Keep calm and subtle.
            // Do NOT treat lower intensity as "punishment".
            switch (mood)
            {
                case MoodWeather.ClearWarm: return 1.2f;
                case MoodWeather.BreakInClouds: return 1.1f;
                case MoodWeather.FocusedGlow: return 1.15f;
                case MoodWeather.SoftOvercast: return 0.95f;
                case MoodWeather.GentleRain: return 0.9f;
                case MoodWeather.StillMuted: return 0.85f;
                case MoodWeather.HeavyStillness: return 0.8f;
                case MoodWeather.Neutral:
                default: return 1.0f;
            }
        }

        private void SetRain(bool on)
        {
            if (rainParticles == null)
                return;

            if (on)
            {
                if (!rainParticles.isPlaying)
                    rainParticles.Play();
            }
            else
            {
                if (rainParticles.isPlaying)
                    rainParticles.Stop();
            }
        }

        private void TriggerSky(string triggerName)
        {
            if (skyAnimator == null || string.IsNullOrWhiteSpace(triggerName))
                return;

            skyAnimator.SetTrigger(triggerName);
        }
    }
}
