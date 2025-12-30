using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using TheRocketTree.Data;

namespace TheRocketTree.Network
{
    /// <summary>
    /// Read-only access to the authoritative Tree world state.
    /// Unity never mutates the tree — it only renders backend truth.
    /// </summary>
    public sealed class TreeService
    {
        private readonly ApiClient _api;

        public TreeService(ApiClient apiClient)
        {
            _api = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        /// <summary>
        /// Fetch the latest authoritative TreeState.
        /// Used on session start and hard refresh.
        /// </summary>
        public async Task<TreeStateDTO> GetTreeStateAsync(
            CancellationToken ct = default)
        {
            try
            {
                // Example endpoint:
                // GET /tree/state
                return await _api.GetAsync<TreeStateDTO>(
                    "/tree/state",
                    queryParams: null,
                    cancellationToken: ct
                );
            }
            catch (OperationCanceledException)
            {
                return null;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[TreeService] Failed to fetch TreeState: {ex}");
                return null;
            }
        }

        /// <summary>
        /// Fetch a single authoritative snapshot of the tree for inspection.
        /// Useful for debug tools or editor-only panels.
        /// </summary>
        public async Task<TreeStateDTO> RefreshTreeStateAsync(
            CancellationToken ct = default)
        {
            return await GetTreeStateAsync(ct);
        }
    }
}
