using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using TheRocketTree.Data;

namespace TheRocketTree.Network
{
    /// <summary>
    /// Fetches authoritative TreeState from backend.
    /// Unity never computes growth; it only renders backend truth.
    /// </summary>
    public sealed class TreeStateService
    {
        private readonly ApiClient _api;

        public TreeStateService(ApiClient apiClient)
        {
            _api = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        /// <summary>
        /// Gets the latest authoritative TreeState for the current user.
        /// Backend infers user from auth token.
        /// </summary>
        public async Task<TreeStateDTO> GetTreeStateAsync(CancellationToken ct = default)
        {
            try
            {
                var state = await _api.GetAsync<TreeStateDTO>(
                    "/tree/state",
                    queryParams: null,
                    cancellationToken: ct
                );

                if (state == null)
                    Debug.LogWarning("[TreeStateService] Backend returned null TreeState.");

                return state;
            }
            catch (OperationCanceledException)
            {
                return null;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[TreeStateService] Failed to fetch TreeState: {ex}");
                return null;
            }
        }
    }
}
