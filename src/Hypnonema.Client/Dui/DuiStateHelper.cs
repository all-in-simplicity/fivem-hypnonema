namespace Hypnonema.Client.Dui
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Hypnonema.Client.Communications;
    using Hypnonema.Shared;

    public class DuiStateHelper
    {
        private readonly NetworkMethod<Guid, List<DuiState>> duiStateMethod;

        private readonly ConcurrentDictionary<Guid, TaskCompletionSource<List<DuiState>>> pendingRequests =
            new ConcurrentDictionary<Guid, TaskCompletionSource<List<DuiState>>>();

        public DuiStateHelper()
        {
            this.duiStateMethod = new NetworkMethod<Guid, List<DuiState>>(Events.DuiState, this.OnDuiState);
        }

        public Task<List<DuiState>> RequestDuiStateAsync()
        {
            var tcs = new TaskCompletionSource<List<DuiState>>();

            this.pendingRequests.TryAdd(this.RequestDuiState(), tcs);

            return tcs.Task;
        }

        private void OnDuiState(Guid requestId, List<DuiState> duiState)
        {
            this.pendingRequests.TryRemove(requestId, out var tcs);

            tcs?.SetResult(duiState);
        }

        private Guid RequestDuiState()
        {
            var requestId = Guid.NewGuid();
            this.duiStateMethod.Invoke(requestId, new List<DuiState>());

            return requestId;
        }
    }
}