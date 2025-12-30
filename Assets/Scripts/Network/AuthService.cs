using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace TheRocketTree.Network
{
    /// <summary>
    /// Handles authentication lifecycle for the client.
    /// This service manages identity only — never progression or meaning.
    /// </summary>
    public sealed class AuthService
    {
        private string _userId;
        private string _accessToken;

        /// <summary>
        /// True when a user is authenticated.
        /// </summary>
        public bool IsAuthenticated => !string.IsNullOrEmpty(_userId);

        /// <summary>
        /// Current authenticated user ID (backend-authoritative).
        /// </summary>
        public string UserId => _userId;

        /// <summary>
        /// Fired when authentication state changes.
        /// </summary>
        public event Action<bool> OnAuthStateChanged;

        /// <summary>
        /// Simulated login.
        /// Replace with Firebase/Auth provider integration.
        /// </summary>
        public async Task LoginAsync(
            string userId,
            string accessToken,
            CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("userId is required");

            // Simulate async boundary (real auth provider goes here)
            await Task.Yield();
            ct.ThrowIfCancellationRequested();

            _userId = userId;
            _accessToken = accessToken;

            Debug.Log($"[AuthService] Logged in as {_userId}");
            OnAuthStateChanged?.Invoke(true);
        }

        /// <summary>
        /// Clears local authentication state.
        /// Backend sessions must be invalidated separately.
        /// </summary>
        public async Task LogoutAsync(CancellationToken ct = default)
        {
            await Task.Yield();
            ct.ThrowIfCancellationRequested();

            Debug.Log($"[AuthService] Logged out user {_userId}");

            _userId = null;
            _accessToken = null;

            OnAuthStateChanged?.Invoke(false);
        }

        /// <summary>
        /// Returns the current access token for backend requests.
        /// </summary>
        public string GetAccessToken()
        {
            return _accessToken;
        }

        /// <summary>
        /// Hard reset — intended for session invalidation.
        /// </summary>
        public void Reset()
        {
            _userId = null;
            _accessToken = null;
            OnAuthStateChanged?.Invoke(false);
        }
    }
}
