using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace TheRocketTree.Network
{
    /// <summary>
    /// Provides read-only access to subscription and entitlement state.
    /// Subscriptions unlock features — they never modify progression.
    /// </summary>
    public sealed class SubscriptionService
    {
        private readonly ApiClient _api;

        public SubscriptionService(ApiClient apiClient)
        {
            _api = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        /// <summary>
        /// Fetch the current user's subscription tier.
        /// Example values: free | plus | lifetime
        /// </summary>
        public async Task<string> GetSubscriptionTierAsync(
            CancellationToken ct = default)
        {
            try
            {
                return await _api.GetAsync<string>(
                    "/subscription/tier",
                    queryParams: null,
                    cancellationToken: ct
                );
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SubscriptionService] Failed to fetch subscription tier: {ex}");
                return "free";
            }
        }

        /// <summary>
        /// Check whether the current user has access to a named feature.
        /// Feature evaluation is fully backend-driven.
        /// </summary>
        public async Task<bool> HasFeatureAsync(
            string featureKey,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(featureKey))
                throw new ArgumentException("Feature key cannot be empty.", nameof(featureKey));

            try
            {
                return await _api.GetAsync<bool>(
                    "/subscription/feature",
                    queryParams: new System.Collections.Generic.Dictionary<string, string>
                    {
                        { "key", featureKey }
                    },
                    cancellationToken: ct
                );
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SubscriptionService] Failed to check feature '{featureKey}': {ex}");
                return false;
            }
        }

        /// <summary>
        /// Returns whether the user is allowed to exceed a soft limit
        /// (e.g., max active projects, reflections per day).
        /// </summary>
        public async Task<bool> CanExceedLimitAsync(
            string limitKey,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(limitKey))
                throw new ArgumentException("Limit key cannot be empty.", nameof(limitKey));

            try
            {
                return await _api.GetAsync<bool>(
                    "/subscription/limit",
                    queryParams: new System.Collections.Generic.Dictionary<string, string>
                    {
                        { "key", limitKey }
                    },
                    cancellationToken: ct
                );
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SubscriptionService] Failed to check limit '{limitKey}': {ex}");
                return false;
            }
        }
    }
}
