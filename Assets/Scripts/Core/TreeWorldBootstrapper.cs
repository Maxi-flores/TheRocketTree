using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using TheRocketTree.Network;
using TheRocketTree.Rendering;

namespace TheRocketTree.Core
{
    /// <summary>
    /// Scene-level glue that wires backend services to render systems.
    /// Attach to a GameObject and assign TreeRenderer + WeatherSystem in Inspector.
    /// </summary>
    public sealed class TreeWorldBootstrapper : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField] private TreeRenderer treeRenderer;
        [SerializeField] private WeatherSystem weatherSystem;

        private CancellationTokenSource _cts;

        private ApiClient _apiClient;
        private ProgressionService _progressionService;
        private TreeStateService _treeStateService;

        private ProgressionEventStream _eventStream;
        private TreeWorldCoordinator _coordinator;

        private async void Start()
        {
            _cts = new CancellationTokenSource();

            // NOTE: Replace with your existing dependency injection/singletons later.
            _apiClient = new ApiClient();
            _progressionService = new ProgressionService(_apiClient);
            _treeStateService = new TreeStateService(_apiClient);

            _eventStream = new ProgressionEventStream(_progressionService);
            _coordinator = new TreeWorldCoordinator(_treeStateService, _eventStream, treeRenderer, weatherSystem);

            await SafeInitAsync(_cts.Token);

            _ = PollLoopAsync(_cts.Token);
        }

        private async Task SafeInitAsync(CancellationToken ct)
        {
            try { await _coordinator.InitializeAsync(ct); }
            catch (OperationCanceledException) { }
            catch (System.Exception ex)
            {
                Debug.LogError($"[TreeWorldBootstrapper] Init failed: {ex}");
            }
        }

        private async Task PollLoopAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try { await _coordinator.PollAsync(ct); }
                catch (OperationCanceledException) { }
                catch (System.Exception ex)
                {
                    Debug.LogError($"[TreeWorldBootstrapper] Poll failed: {ex}");
                }

                await Task.Delay(3000, ct);
            }
        }

        private void OnDestroy()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }

            _coordinator?.Shutdown();
        }
    }
}
