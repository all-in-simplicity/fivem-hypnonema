namespace Hypnonema.Server.Utils
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    using Hypnonema.Shared;

    public class State
    {
        private readonly ConcurrentDictionary<string, DuiState> _state = new ConcurrentDictionary<string, DuiState>();

        public void Add(string key, DuiState duiState)
        {
            // whitespace removal from key is necessary 
            // otherwise screen names "screen test" and "screen test1" would be considered equal.
            key = string.Concat(key.Where(c => !char.IsWhiteSpace(c)));

            this._state.AddOrUpdate(key, duiState, (k, _) => duiState);
        }

        public DuiState Get(string key)
        {
            var exists = this._state.TryGetValue(key, out var state);

            return !exists ? null : state;
        }

        public void Remove(string key)
        {
            // whitespace removal from key is necessary 
            // otherwise screen names "screen test" and "screen test1" would be considered equal.
            key = string.Concat(key.Where(c => !char.IsWhiteSpace(c)));

            var existingState = this.Get(key);
            if (existingState == null) return;

            this._state.TryRemove(key, out var _);
        }

        public List<DuiState> ToList()
        {
            return this._state.Values.ToList();
        }

        public void Update(string key, DuiState duiState)
        {
            // whitespace removal from key is necessary 
            // otherwise screen names "screen test" and "screen test1" would be considered equal.
            key = string.Concat(key.Where(c => !char.IsWhiteSpace(c)));

            var oldState = this.Get(key);
            if (oldState == null) return;

            this._state.TryUpdate(key, duiState, oldState);
        }
    }
}