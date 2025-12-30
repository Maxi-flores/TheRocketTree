using System;
using UnityEngine;
using TheRocketTree.Network;
using TheRocketTree.Data;

namespace TheRocketTree.Rendering
{
    /// <summary>
    /// QuestInteractor translates in-world interaction into
    /// BMS task intent.
    ///
    /// IMPORTANT:
    /// - This class does NOT decide meaning.
    /// - It does NOT trigger growth.
    /// - It ONLY sends user intent to the backend.
    /// </summary>
    public sealed class QuestInteractor : MonoBehaviour
    {
        [Header("Interaction Metadata")]
        [SerializeField] private string projectId;
        [SerializeField] private string defaultEstimatedDepth = "small";

        private TaskService _taskService;

        /// <summary>
        /// Inject TaskService from higher-level bootstrapper.
        /// </summary>
        public void Initialize(TaskService taskService)
        {
            _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
        }

        /// <summary>
        /// Called when a new quest-like interaction begins.
        /// Creates a task intent in the backend.
        /// </summary>
        public async void BeginQuest(string title, string notes = null)
        {
            if (_taskService == null)
            {
                Debug.LogError("[QuestInteractor] TaskService not initialized.");
                return;
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                Debug.LogWarning("[QuestInteractor] Cannot create task without title.");
                return;
            }

            var task = new TaskDTO
            {
                projectId = projectId,
                title = title,
                notes = notes,
                estimatedDepth = defaultEstimatedDepth,
                status = "open",
                createdAt = DateTime.UtcNow.ToString("o")
            };

            try
            {
                await _taskService.CreateTaskAsync(task);
                Debug.Log($"[QuestInteractor] Task created: {title}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[QuestInteractor] Failed to create task: {ex}");
            }
        }

        /// <summary>
        /// Called when a quest-like interaction is completed.
        /// Signals task completion intent to backend.
        /// </summary>
        public async void CompleteQuest(string taskId)
        {
            if (_taskService == null)
            {
                Debug.LogError("[QuestInteractor] TaskService not initialized.");
                return;
            }

            if (string.IsNullOrEmpty(taskId))
            {
                Debug.LogWarning("[QuestInteractor] Cannot complete task without ID.");
                return;
            }

            try
            {
                await _taskService.CompleteTaskAsync(
                    taskId,
                    DateTime.UtcNow.ToString("o")
                );

                Debug.Log($"[QuestInteractor] Task completion sent: {taskId}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[QuestInteractor] Failed to complete task: {ex}");
            }
        }
    }
}
