using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TheRocketTree.Data;
using TheRocketTree.Network;

namespace TheRocketTree.Core
{
    /// <summary>
    /// Pulls authoritative progression events and dispatches them to render systems.
    /// NEVER computes meaning or growth; only replays backend-decided events.
    /// </summary>
    public sealed class ProgressionEventStream
    {
        private readonly ProgressionService _progressionService;
        private readonly HashSet<string> _seenEventIds = new HashSet<string>();

        public event Action<ProgressionEventDTO> OnEvent;

        public ProgressionEventStream(ProgressionService progressionService)
        {
            _progressionService = progressionService ?? throw new ArgumentNullException(nameof(progressionService));
        }

        public async Task PollAsync(CancellationToken ct = default)
        {
            var events = await _progressionService.FetchNewEventsAsync(ct);
            if (events == null || events.Count == 0) return;

            foreach (var evt in events)
            {
                if (evt == null || string.IsNullOrEmpty(evt.eventId)) continue;
                if (_seenEventIds.Contains(evt.eventId)) continue;

                _seenEventIds.Add(evt.eventId);
                OnEvent?.Invoke(evt);
            }
        }

        public void Reset()
        {
            _seenEventIds.Clear();
        }
    }
}
