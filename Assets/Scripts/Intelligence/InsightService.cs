using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace TheRocketTree.Intelligence
{
    /// <summary>
    /// InsightService coordinates the retrieval and delivery of
    /// reflective insights to the UI layer.
    ///
    /// Responsibilities:
    /// - Request insights via GrokClient
    /// - Cache last insight in-memory for session
    /// - Expose a clean async API to UI
    ///
    /// This service NEVER:
    /// - writes to Firestore
    /// - emits progression events
    /// - modifies TreeState
    /// </summary>
    public sealed class InsightService
    {
        private readonly GrokClient _grokClient;

        private InsightDTO _cachedInsight;

        public InsightService(GrokClient grokClient)
        {
            _grokClient = grokClient ?? throw new ArgumentNullException(nameof(grokClient));
        }

        /// <summary>
        /// Requests an insight for a given time window.
        /// Results are cached for the lifetime of the session.
        /// </summary>
        public async Task<InsightDTO> GetWeeklyInsightAsync(
            DateTime fromUtc,
            DateTime toUtc,
            CancellationToken ct = default)
        {
            if (_cachedInsight != null)
                return _cachedInsight;

            try
            {
                var text = await _grokClient.RequestWeeklyInsightAsync(
                    fromUtc,
                    toUtc,
                    ct
                );

                if (string.IsNullOrEmpty(text))
                    return null;

                _cachedInsight = new InsightDTO
                {
                    text = text,
                    fromUtc = fromUtc.ToString("o"),
                    toUtc = toUtc.ToString("o"),
                    title = null,
                    tags = null
                };

                return _cachedInsight;
            }
            catch (OperationCanceledException)
            {
                return null;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[InsightService] Failed to retrieve insight: {ex}");
                return null;
            }
        }

        /// <summary>
        /// Clears cached insight.
        /// Intended for logout or hard session reset.
        /// </summary>
        public void Reset()
        {
            _cachedInsight = null;
        }
    }
}
