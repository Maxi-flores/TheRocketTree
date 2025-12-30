using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using TheRocketTree.Network;
using TheRocketTree.Data;

namespace TheRocketTree.Core
{
    /// <summary>
    /// SessionManager orchestrates the lifecycle of a user session.
    ///
    /// RESPONSIBILITIES:
    /// - Detect app start / resume / pause
    /// - Fetch authoritative progression events
    /// - Dispatch events to rendering & UI systems
    ///
    /// HARD RULES:
    /// - This class NEVER computes growth
    /// - It NEVER interprets meaning
    /// - It NEVER mutates TreeState
    /// </summary>
    public class SessionManager : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private GameManager gameManager;
        [SerializeField] private TreeRenderer treeRenderer;
        [SerializeField] private WeatherSystem weatherSystem;

        private ProgressionService _progressionService;
        private CancellationTokenSource _sessionCts;

        private bool _sessionStarted;

        private void Awake()
        {
            _sessionCts = new CancellationTokenSource();
        }

        private async void Start()
        {
            await StartSessionAsync();
        }

        private async void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
            {
                await ResumeSessionAsync();
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                PauseSession();
            }
        }

        private void OnDestroy()
        {
            _sessionCts?.Cancel();
            _sessionCts?.Dispose();
        }

        /// <summary>
        /// Initializes a fresh session.
        /// Safe to call only once per app launch.
        /// </summary>
        private async Task StartSessionAsync()
        {
            if (_sessionStarted)
                return;

            _sessionStarted = true;

            _progressionService = gameManager.ProgressionService;

            await FetchAndDispatchEventsAsync();
        }

        /// <summary>
        /// Called when app regains focus.
        /// </summary>
        private async Task ResumeSessionAsync()
        {
            if (!_sessionStarted)
                return;

            await FetchAndDispatchEventsAsync();
        }

        /// <summary>
        /// Called when app is backgrounded.
        /// </summary>
        private void PauseSession()
        {
            // Intentionally minimal.
            // We do NOT end sessions aggressively.
        }

        /// <summary>
        /// Fetches new progression events and dispatches them
        /// to all interested systems.
        /// </summary>
        private async Task FetchAndDispatchEventsAsync()
        {
            IReadOnlyList<ProgressionEventDTO> events;

            try
            {
                events = await _progressionService.FetchNewEventsAsync(_sessionCts.Token);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SessionManager] Failed to fetch progression events: {ex}");
                return;
            }

            if (events == null || events.Count == 0)
                return;

            foreach (var evt in events)
            {
                DispatchEvent(evt);
            }
        }

        /// <summary>
        /// Dispatches a single progression event to visual systems.
        /// No interpretation, no scoring, no logic branching beyond routing.
        /// </summary>
        private void DispatchEvent(ProgressionEventDTO evt)
        {
            if (evt == null)
                return;

            // Tree reacts to structural / vitality changes
            treeRenderer?.ApplyProgressionEvent(evt);

            // Weather reacts to mood & time-based events
            weatherSystem?.ApplyProgressionEvent(evt);
        }
    }
}

/*
 * ─────────────────────────────────────────────
 * Rocket Tree: Backend-driven world wiring (Phase 1)
 * ─────────────────────────────────────────────
 * NOTE: Unity is a pure renderer. All meaning comes from backend.
 */
#if UNITY_EDITOR || DEVELOPMENT_BUILD
using TheRocketTree.Network;
using TheRocketTree.Rendering;
#endif

