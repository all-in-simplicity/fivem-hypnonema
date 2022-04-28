namespace Hypnonema.Client.Dui
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Hypnonema.Client.Communications;
    using Hypnonema.Shared;
    using Hypnonema.Shared.Communications;

    public class DuiStateHelper
    {
        private readonly NetworkMethod<DuiStateMessage> duiStateMethod;

        private readonly ConcurrentDictionary<Guid, TaskCompletionSource<List<DuiState>>> pendingRequests =
            new ConcurrentDictionary<Guid, TaskCompletionSource<List<DuiState>>>();

        public DuiStateHelper()
        {
            this.duiStateMethod = new NetworkMethod<DuiStateMessage>(Events.DuiState, this.OnDuiState);
        }

        public Task<List<DuiState>> RequestDuiStateAsync()
        {
            var tcs = new TaskCompletionSource<List<DuiState>>();

            this.pendingRequests.TryAdd(this.RequestDuiState(), tcs);

            return tcs.Task;
        }

        private void OnDuiState(DuiStateMessage duiStateMessage)
        {
            this.pendingRequests.TryRemove(duiStateMessage.RequestId, out var tcs);

            tcs?.SetResult(duiStateMessage.DuiStates);
        }

        private Guid RequestDuiState()
        {
            var duiStateMessage = new DuiStateMessage();
            this.duiStateMethod.Invoke(duiStateMessage);

            return duiStateMessage.RequestId;
        }
    }
}