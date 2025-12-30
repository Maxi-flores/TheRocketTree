using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using TheRocketTree.Data;

namespace TheRocketTree.Network
{
    /// <summary>
    /// Handles user-declared task intent.
    /// Tasks represent effort attempts — not growth.
    /// Backend decides meaning and progression.
    /// </summary>
    public sealed class TaskService
    {
        private readonly ApiClient _api;

        public TaskService(ApiClient apiClient)
        {
            _api = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        /// <summary>
        /// Create a new task under a project.
        /// </summary>
        public async Task<TaskDTO> CreateTaskAsync(
            TaskDTO task,
            CancellationToken ct = default)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            try
            {
                return await _api.PostAsync<TaskDTO>(
                    "/tasks",
                    task,
                    cancellationToken: ct
                );
            }
            catch (Exception ex)
            {
                Debug.LogError($"[TaskService] Failed to create task: {ex}");
                return null;
            }
        }

        /// <summary>
        /// Fetch all tasks for the current user.
        /// </summary>
        public async Task<IReadOnlyList<TaskDTO>> GetTasksAsync(
            CancellationToken ct = default)
        {
            try
            {
                return await _api.GetAsync<List<TaskDTO>>(
                    "/tasks",
                    queryParams: null,
                    cancellationToken: ct
                );
            }
            catch (Exception ex)
            {
                Debug.LogError($"[TaskService] Failed to fetch tasks: {ex}");
                return Array.Empty<TaskDTO>();
            }
        }

        /// <summary>
        /// Mark a task as completed.
        /// This only declares completion — it does not imply growth.
        /// </summary>
        public async Task<bool> CompleteTaskAsync(
            string taskId,
            DateTime completedAt,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(taskId))
                throw new ArgumentException("Task ID cannot be empty.", nameof(taskId));

            try
            {
                await _api.PostAsync<object>(
                    $"/tasks/{taskId}/complete",
                    new
                    {
                        completedAt = completedAt.ToUniversalTime().ToString("o")
                    },
                    cancellationToken: ct
                );

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[TaskService] Failed to complete task {taskId}: {ex}");
                return false;
            }
        }

        /// <summary>
        /// Abandon a task without completion.
        /// </summary>
        public async Task<bool> AbandonTaskAsync(
            string taskId,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(taskId))
                throw new ArgumentException("Task ID cannot be empty.", nameof(taskId));

            try
            {
                await _api.PostAsync<object>(
                    $"/tasks/{taskId}/abandon",
                    body: null,
                    cancellationToken: ct
                );

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[TaskService] Failed to abandon task {taskId}: {ex}");
                return false;
            }
        }
    }
}
