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

        unsafe public Task<List<DuiState>> RequestDuiStateAsync()
        {
            TaskCompletionSource<List<DuiState>> tcs = new TaskCompletionSource<List<DuiState>>();

            this.pendingRequests.TryAdd(this.RequestDuiState(), tcs);

            return tcs.Task;
        }

        unsafe private void OnDuiState(Guid requestId, List<DuiState> duiState)
        {
            this.pendingRequests.TryRemove(requestId, out var tcs);

            tcs?.SetResult(duiState);
        }

        unsafe private Guid RequestDuiState()
        {
            Guid requestId = Guid.NewGuid();
            List<DuiState> myList = new List<DuiState>();
            this.duiStateMethod.Invoke(requestId, myList);

            return requestId;
        }
    }
}