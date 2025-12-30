using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using TheRocketTree.Data;

namespace TheRocketTree.Network
{
    /// <summary>
    /// ProgressionService is responsible for retrieving
    /// authoritative progression events from the backend.
    ///
    /// HARD RULES:
    /// - This service NEVER computes or infers growth.
    /// - It NEVER mutates TreeState locally.
    /// - It ONLY fetches backend-decided progression meaning.
    ///
    /// Unity systems (SessionManager, TreeRenderer, WeatherSystem)
    /// consume these events reactively.
    /// </summary>
    public class ProgressionService
    {
        private readonly ApiClient _apiClient;

        /// <summary>
        /// Local cache of processed event IDs for idempotency.
        /// Prevents duplicate visual application on retries.
        /// </summary>
        private readonly HashSet<string> _processedEventIds = new HashSet<string>();

        /// <summary>
        /// Last known event timestamp (UTC).
        /// Used for incremental fetching.
        /// </summary>
        private DateTime? _lastFetchedUtc;

        public ProgressionService(ApiClient apiClient)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        /// <summary>
        /// Fetch progression events from backend.
        /// Returns ONLY new, unprocessed events in chronological order.
        ///
        /// This method is safe to call:
        /// - on app start
        /// - on resume
        /// - after task completion
        /// - after reflection submission
        /// </summary>
        public async Task<IReadOnlyList<ProgressionEventDTO>> FetchNewEventsAsync(
            CancellationToken cancellationToken = default)
        {
            var fetchedEvents = await FetchFromBackendAsync(cancellationToken);

            if (fetchedEvents == null || fetchedEvents.Count == 0)
                return Array.Empty<ProgressionEventDTO>();

            var newEvents = new List<ProgressionEventDTO>();

            foreach (var evt in fetchedEvents)
            {
                if (evt == null || string.IsNullOrEmpty(evt.eventId))
                    continue;

                if (_processedEventIds.Contains(evt.eventId))
                    continue;

                _processedEventIds.Add(evt.eventId);
                newEvents.Add(evt);

                // Track latest timestamp for incremental fetches
                if (!_lastFetchedUtc.HasValue || evt.occurredAtUtc > _lastFetchedUtc.Value)
                {
                    _lastFetchedUtc = evt.occurredAtUtc;
                }
            }

            // Ensure deterministic ordering for rendering systems
            newEvents.Sort((a, b) => a.occurredAtUtc.CompareTo(b.occurredAtUtc));

            return newEvents;
        }

        /// <summary>
        /// Clears local idempotency cache.
        /// Intended ONLY for logout or full session reset.
        /// </summary>
        public void ResetSession()
        {
            _processedEventIds.Clear();
            _lastFetchedUtc = null;
        }

        /// <summary>
        /// Low-level backend call.
        /// This is the ONLY place where endpoint details exist.
        /// </summary>
        private async Task<List<ProgressionEventDTO>> FetchFromBackendAsync(
            CancellationToken cancellationToken)
        {
            try
            {
                var queryParams = new Dictionary<string, string>();

                if (_lastFetchedUtc.HasValue)
                {
                    queryParams["sinceUtc"] = _lastFetchedUtc.Value.ToString("o");
                }

                // Example endpoint:
                // GET /progression/events?sinceUtc=...
                var response = await _apiClient.GetAsync<List<ProgressionEventDTO>>(
                    "/progression/events",
                    queryParams,
                    cancellationToken
                );

                return response ?? new List<ProgressionEventDTO>();
            }
            catch (OperationCanceledException)
            {
                // Expected during shutdown / scene change
                return new List<ProgressionEventDTO>();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ProgressionService] Failed to fetch events: {ex}");
                return new List<ProgressionEventDTO>();
            }
        }
    }
}
