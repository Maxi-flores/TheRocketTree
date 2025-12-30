using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace TheRocketTree.Network
{
    /// <summary>
    /// ApiClient is a minimal async HTTP abstraction.
    ///
    /// This is a STUB implementation.
    /// Real networking (Firebase / HTTPS) will replace internals later
    /// without changing any service contracts.
    /// </summary>
    public class ApiClient
    {
        public ApiClient()
        {
            // Placeholder constructor
        }

        /// <summary>
        /// Generic GET request.
        /// Currently returns default(T) to allow client compilation.
        /// </summary>
        public async Task<T> GetAsync<T>(
            string endpoint,
            Dictionary<string, string> queryParams = null,
            CancellationToken cancellationToken = default)
        {
            await Task.Yield();

            Debug.Log($"[ApiClient] GET {endpoint} (stub)");

            return default;
        }
    }
}
