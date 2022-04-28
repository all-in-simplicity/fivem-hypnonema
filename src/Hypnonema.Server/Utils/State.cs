namespace Hypnonema.Server.Utils
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    using CitizenFX.Core;

    using Hypnonema.Server.Communications;
    using Hypnonema.Shared;
    using Hypnonema.Shared.Communications;

    public class State
    {
        private readonly ConcurrentDictionary<string, DuiState> _state = new ConcurrentDictionary<string, DuiState>();

        private readonly NetworkMethod<DuiStateChangedMessage> duiStateChanged;

        public State()
        {
            this.duiStateChanged = new NetworkMethod<DuiStateChangedMessage>(
                Events.DuiStateChanged,
                this.OnDuiStateChanged);
        }

        public void Add(string key, DuiState duiState)
        {
            // whitespace removal from key is necessary 
            // otherwise screen names "screen test" and "screen test1" would be considered equal.
            this._state.AddOrUpdate(string.Concat(key.Where(c => !char.IsWhiteSpace(c))), duiState, (k, _) => duiState);

            var duiStateChangedMessage = new DuiStateChangedMessage(
                key,
                duiState,
                DuiStateChangedMessage.ChangeTypeEnum.Created);

            this.duiStateChanged.Invoke(null, duiStateChangedMessage);
        }

        public DuiState Get(string key)
        {
            // whitespace removal from key is necessary 
            // otherwise screen names "screen test" and "screen test1" would be considered equal.
            key = string.Concat(key.Where(c => !char.IsWhiteSpace(c)));

            var exists = this._state.TryGetValue(key, out var state);

            return !exists ? null : state;
        }

        public void Remove(string key)
        {
            // whitespace removal from key is necessary 
            // otherwise screen names "screen test" and "screen test1" would be considered equal.
            var existingState = this.Get(string.Concat(key.Where(c => !char.IsWhiteSpace(c))));
            if (existingState == null) return;

            var duiStateChangedMessage = new DuiStateChangedMessage(
                key,
                existingState,
                DuiStateChangedMessage.ChangeTypeEnum.Deleted);

            this._state.TryRemove(string.Concat(key.Where(c => !char.IsWhiteSpace(c))), out var _);

            this.duiStateChanged.Invoke(null, duiStateChangedMessage);
        }

        public List<DuiState> ToList()
        {
            return this._state.Values.ToList();
        }

        public void Update(string key, DuiState duiState)
        {
            var duiStateChangedMessage = new DuiStateChangedMessage(
                key,
                duiState,
                DuiStateChangedMessage.ChangeTypeEnum.Updated);

            // whitespace removal from key is necessary 
            // otherwise screen names "screen test" and "screen test1" would be considered equal.
            key = string.Concat(key.Where(c => !char.IsWhiteSpace(c)));

            var oldState = this.Get(key);
            if (oldState == null) return;

            this._state.TryUpdate(key, duiState, oldState);

            this.duiStateChanged.Invoke(null, duiStateChangedMessage);
        }

        private void OnDuiStateChanged(Player p, DuiStateChangedMessage duiStateChangedMessage)
        {
            // Nothing to do when invoked from client side!
        }
    }
}