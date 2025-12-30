using System;
using System.Threading;
using System.Threading.Tasks;
using TheRocketTree.Data;
using TheRocketTree.Network;
using TheRocketTree.Rendering;

namespace TheRocketTree.Core
{
    /// <summary>
    /// Coordinates authoritative backend state with Unity render systems.
    /// - Pull TreeState (authoritative)
    /// - Pull ProgressionEvents (authoritative)
    /// - Trigger TreeRenderer / WeatherSystem (visual only)
    /// </summary>
    public sealed class TreeWorldCoordinator
    {
        private readonly TreeStateService _treeStateService;
        private readonly ProgressionEventStream _eventStream;

        private readonly TreeRenderer _treeRenderer;
        private readonly WeatherSystem _weatherSystem;

        public TreeWorldCoordinator(
            TreeStateService treeStateService,
            ProgressionEventStream eventStream,
            TreeRenderer treeRenderer,
            WeatherSystem weatherSystem)
        {
            _treeStateService = treeStateService ?? throw new ArgumentNullException(nameof(treeStateService));
            _eventStream = eventStream ?? throw new ArgumentNullException(nameof(eventStream));
            _treeRenderer = treeRenderer ?? throw new ArgumentNullException(nameof(treeRenderer));
            _weatherSystem = weatherSystem ?? throw new ArgumentNullException(nameof(weatherSystem));
        }

        public async Task InitializeAsync(CancellationToken ct = default)
        {
            var state = await _treeStateService.GetTreeStateAsync(ct);
            if (state != null)
                ApplyTreeState(state);

            _eventStream.OnEvent += HandleProgressionEvent;

            await _eventStream.PollAsync(ct);
        }

        public Task PollAsync(CancellationToken ct = default)
            => _eventStream.PollAsync(ct);

        public void Shutdown()
        {
            _eventStream.OnEvent -= HandleProgressionEvent;
        }

        private void ApplyTreeState(TreeStateDTO state)
        {
            _treeRenderer.SetAuthoritativeState(state);
            _weatherSystem.SetAuthoritativeVitality(state.vitality);
        }

        private void HandleProgressionEvent(ProgressionEventDTO evt)
        {
            _treeRenderer.PlayEventPulse(evt);
            _weatherSystem.ReactToEvent(evt);
        }
    }
}
