using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using TheRocketTree.Network;

namespace TheRocketTree.Intelligence
{
    /// <summary>
    /// ReflectionService submits user-authored reflections to the backend.
    ///
    /// Reflections are signals of awareness, not effort.
    /// They do NOT directly add growth.
    ///
    /// Backend responsibilities:
    /// - validate reflection
    /// - emit progression events
    /// - modulate vitality / expression
    /// </summary>
    public sealed class ReflectionService
    {
        private readonly ApiClient _apiClient;

        public ReflectionService(ApiClient apiClient)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        /// <summary>
        /// Submits a reflection to the backend.
        /// </summary>
        /// <param name="text">User reflection text</param>
        /// <param name="relatedTaskIds">Optional related task IDs</param>
        /// <param name="tags">Optional descriptive tags</param>
        public async Task<bool> SubmitReflectionAsync(
            string text,
            IReadOnlyList<string> relatedTaskIds = null,
            IReadOnlyList<string> tags = null,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                Debug.LogWarning("[ReflectionService] Cannot submit empty reflection.");
                return false;
            }

            try
            {
                var payload = new
                {
                    text = text,
                    relatedTaskIds = relatedTaskIds,
                    tags = tags,
                    createdAt = DateTime.UtcNow.ToString("o")
                };

                // Example endpoint:
                // POST /reflections
                await _apiClient.PostAsync<object>(
                    "/reflections",
                    payload,
                    ct
                );

                return true;
            }
            catch (OperationCanceledException)
            {
                return false;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[ReflectionService] Failed to submit reflection: {ex}");
                return false;
            }
        }
    }
}
