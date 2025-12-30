using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using TheRocketTree.Data;

namespace TheRocketTree.Network
{
    /// <summary>
    /// Provides read-only access to social context.
    /// Social data is observational — never competitive.
    /// </summary>
    public sealed class SocialService
    {
        private readonly ApiClient _api;

        public SocialService(ApiClient apiClient)
        {
            _api = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        /// <summary>
        /// Fetch a lightweight list of users the current user follows.
        /// </summary>
        public async Task<IReadOnlyList<UserDTO>> GetFollowingAsync(
            CancellationToken ct = default)
        {
            try
            {
                return await _api.GetAsync<List<UserDTO>>(
                    "/social/following",
                    queryParams: null,
                    cancellationToken: ct
                );
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SocialService] Failed to fetch following: {ex}");
                return Array.Empty<UserDTO>();
            }
        }

        /// <summary>
        /// Fetch a lightweight list of users following the current user.
        /// </summary>
        public async Task<IReadOnlyList<UserDTO>> GetFollowersAsync(
            CancellationToken ct = default)
        {
            try
            {
                return await _api.GetAsync<List<UserDTO>>(
                    "/social/followers",
                    queryParams: null,
                    cancellationToken: ct
                );
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SocialService] Failed to fetch followers: {ex}");
                return Array.Empty<UserDTO>();
            }
        }

        /// <summary>
        /// Fetch recent public reflections from followed users.
        /// These are contextual signals — not feeds, not scores.
        /// </summary>
        public async Task<IReadOnlyList<ReflectionDTO>> GetCommunityReflectionsAsync(
            int limit = 20,
            CancellationToken ct = default)
        {
            try
            {
                var query = new Dictionary<string, string>
                {
                    { "limit", limit.ToString() }
                };

                return await _api.GetAsync<List<ReflectionDTO>>(
                    "/social/reflections",
                    query,
                    ct
                );
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SocialService] Failed to fetch community reflections: {ex}");
                return Array.Empty<ReflectionDTO>();
            }
        }
    }
}
