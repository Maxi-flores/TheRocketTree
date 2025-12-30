using UnityEngine;
using TheRocketTree.Network;

namespace TheRocketTree.Core
{
    /// <summary>
    /// GameManager is the top-level composition root.
    ///
    /// RESPONSIBILITIES:
    /// - Initialize core services
    /// - Expose them to session-level systems
    ///
    /// HARD RULES:
    /// - No gameplay logic
    /// - No progression logic
    /// - No rendering logic
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public ProgressionService ProgressionService { get; private set; }

        private ApiClient _apiClient;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeServices();
        }

        private void InitializeServices()
        {
            _apiClient = new ApiClient();
            ProgressionService = new ProgressionService(_apiClient);
        }
    }
}
