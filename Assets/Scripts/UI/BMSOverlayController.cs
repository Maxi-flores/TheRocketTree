using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TheRocketTree.Intelligence;
using TheRocketTree.Network;

namespace TheRocketTree.UI
{
    /// <summary>
    /// Controls the BMS overlay (tasks, reflections, insights).
    ///
    /// Responsibilities:
    /// - Wire UI actions to services
    /// - Show / hide panels
    /// - Never compute meaning or growth
    ///
    /// This controller is intentionally thin.
    /// </summary>
    public sealed class BMSOverlayController : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject taskPanel;
        [SerializeField] private GameObject reflectionPanel;
        [SerializeField] private GameObject insightPanel;

        [Header("Buttons")]
        [SerializeField] private Button openTasksButton;
        [SerializeField] private Button openReflectionButton;
        [SerializeField] private Button openInsightButton;
        [SerializeField] private Button closeAllButton;

        private CancellationTokenSource _cts;

        // Services (injected or created upstream)
        private TaskService _taskService;
        private ReflectionService _reflectionService;
        private InsightService _insightService;

        /// <summary>
        /// Call during scene initialization to inject services.
        /// </summary>
        public void Initialize(
            TaskService taskService,
            ReflectionService reflectionService,
            InsightService insightService)
        {
            _taskService = taskService;
            _reflectionService = reflectionService;
            _insightService = insightService;
        }

        private void Awake()
        {
            _cts = new CancellationTokenSource();

            if (openTasksButton != null)
                openTasksButton.onClick.AddListener(ShowTasks);

            if (openReflectionButton != null)
                openReflectionButton.onClick.AddListener(ShowReflection);

            if (openInsightButton != null)
                openInsightButton.onClick.AddListener(ShowInsight);

            if (closeAllButton != null)
                closeAllButton.onClick.AddListener(HideAll);
        }

        private void OnDestroy()
        {
            _cts.Cancel();
            _cts.Dispose();
        }

        // ─────────────────────────────
        // Panel control
        // ─────────────────────────────

        private void ShowTasks()
        {
            HideAll();
            if (taskPanel != null)
                taskPanel.SetActive(true);
        }

        private void ShowReflection()
        {
            HideAll();
            if (reflectionPanel != null)
                reflectionPanel.SetActive(true);
        }

        private async void ShowInsight()
        {
            HideAll();
            if (insightPanel != null)
                insightPanel.SetActive(true);

            if (_insightService == null)
                return;

            // Example: request last 7 days
            var toUtc = DateTime.UtcNow;
            var fromUtc = toUtc.AddDays(-7);

            try
            {
                await _insightService.GetWeeklyInsightAsync(
                    fromUtc,
                    toUtc,
                    _cts.Token
                );
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[BMSOverlayController] Insight load failed: {ex}");
            }
        }

        private void HideAll()
        {
            if (taskPanel != null) taskPanel.SetActive(false);
            if (reflectionPanel != null) reflectionPanel.SetActive(false);
            if (insightPanel != null) insightPanel.SetActive(false);
        }
    }
}
