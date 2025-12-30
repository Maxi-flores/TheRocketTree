using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using TheRocketTree.Network;

namespace TheRocketTree.Intelligence
{
    /// <summary>
    /// GrokClient is a constrained insight generator.
    ///
    /// It may:
    /// - summarize
    /// - reframe
    /// - surface patterns
    ///
    /// It may NOT:
    /// - change progression
    /// - emit events
    /// - modify TreeState
    /// - override backend authority
    ///
    /// Grok helps the user *understand* what already happened.
    /// </summary>
    public sealed class GrokClient
    {
        private readonly ApiClient _apiClient;

        public GrokClient(ApiClient apiClient)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        /// <summary>
        /// Requests a reflective summary for a given time window.
        /// The backend decides what data is used and how it is interpreted.
        /// </summary>
        public async Task<string> RequestWeeklyInsightAsync(
            DateTime fromUtc,
            DateTime toUtc,
            CancellationToken ct = default)
        {
            try
            {
                var query = new
                {
                    fromUtc = fromUtc.ToString("o"),
                    toUtc = toUtc.ToString("o")
                };

                // Example endpoint:
                // POST /insights/weekly
                var response = await _apiClient.PostAsync<InsightResponse>(
                    "/insights/weekly",
                    query,
                    ct
                );

                return response?.text;
            }
            catch (OperationCanceledException)
            {
                return null;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[GrokClient] Insight request failed: {ex}");
                return null;
            }
        }

        /// <summary>
        /// Internal DTO for insight responses.
        /// This is intentionally minimal.
        /// </summary>
        [Serializable]
        private sealed class InsightResponse
        {
            public string text;
        }
    }
}
