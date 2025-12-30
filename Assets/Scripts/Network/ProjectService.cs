using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using TheRocketTree.Data;

namespace TheRocketTree.Network
{
    /// <summary>
    /// Network service for user Projects.
    /// Projects represent intent containers, not progress.
    /// This service performs NO interpretation or scoring.
    /// </summary>
    public sealed class ProjectService
    {
        private readonly ApiClient _api;

        public ProjectService(ApiClient apiClient)
        {
            _api = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        /// <summary>
        /// Fetch all active projects for the current user.
        /// </summary>
        public async Task<IReadOnlyList<ProjectDTO>> GetProjectsAsync(
            CancellationToken ct = default)
        {
            try
            {
                return await _api.GetAsync<List<ProjectDTO>>(
                    "/projects",
                    queryParams: null,
                    cancellationToken: ct
                );
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ProjectService] Failed to fetch projects: {ex}");
                return Array.Empty<ProjectDTO>();
            }
        }

        /// <summary>
        /// Create a new project.
        /// Backend assigns authority fields (timestamps, userId).
        /// </summary>
        public async Task<ProjectDTO> CreateProjectAsync(
            string title,
            string description,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Project title cannot be empty.", nameof(title));

            var payload = new
            {
                title,
                description
            };

            return await _api.PostAsync<ProjectDTO>(
                "/projects",
                payload,
                cancellationToken: ct
            );
        }

        /// <summary>
        /// Update an existing project.
        /// </summary>
        public async Task<ProjectDTO> UpdateProjectAsync(
            string projectId,
            string title,
            string description,
            CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(projectId))
                throw new ArgumentNullException(nameof(projectId));

            var payload = new
            {
                title,
                description
            };

            return await _api.PutAsync<ProjectDTO>(
                $"/projects/{projectId}",
                payload,
                cancellationToken: ct
            );
        }

        /// <summary>
        /// Archive a project (soft delete).
        /// </summary>
        public async Task ArchiveProjectAsync(
            string projectId,
            CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(projectId))
                throw new ArgumentNullException(nameof(projectId));

            await _api.PostAsync<object>(
                $"/projects/{projectId}/archive",
                body: null,
                cancellationToken: ct
            );
        }
    }
}
