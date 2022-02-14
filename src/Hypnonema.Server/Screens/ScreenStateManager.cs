namespace Hypnonema.Server.Screens
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading.Tasks;

    using CitizenFX.Core;

    using Hypnonema.Shared.Models;

    using Newtonsoft.Json;

    public sealed class ScreenStateManager
    {
        private readonly ConcurrentDictionary<string, DuiState> _state = new ConcurrentDictionary<string, DuiState>();

        public ScreenStateManager()
        {
            BaseServer.Self.AddTick(this.OnTick);
        }

        public void OnPause(string screenName)
        {
            var key = string.Concat(screenName.Where(c => !char.IsWhiteSpace(c)));

            var exists = this._state.TryGetValue(key, out var oldState);
            if (!exists) return;

            var newState = oldState;
            newState.IsPaused = true;

            this._state.TryUpdate(key, newState, oldState);
            this.SetGlobalState();
        }

        public void OnPlay(Screen screen, string url)
        {
            var duiState = new DuiState(screen, url);

            // whitespace removal from screenName is necessary 
            // otherwise screen names "screen test" and "screen test1" would be considered equal.
            var key = string.Concat(screen.Name.Where(c => !char.IsWhiteSpace(c)));

            this._state.AddOrUpdate(key, duiState, (k, _) => duiState);
            this.SetGlobalState();
        }

        public void OnResume(string screenName)
        {
            var key = string.Concat(screenName.Where(c => !char.IsWhiteSpace(c)));

            var exists = this._state.TryGetValue(key, out var oldState);
            if (!exists) return;

            var newState = oldState;
            newState.IsPaused = false;

            this._state.TryUpdate(key, newState, oldState);
            this.SetGlobalState();
        }

        public void OnSeek(string screenName, float time)
        {
            var key = string.Concat(screenName.Where(c => !char.IsWhiteSpace(c)));

            var exists = this._state.TryGetValue(key, out var oldState);
            if (!exists) return;

            var newState = oldState;
            var diff = time - newState.CurrentTime;
            newState.StartedAt = newState.StartedAt.Subtract(new TimeSpan(0, 0, (int)diff));

            this._state.TryUpdate(key, newState, oldState);
            this.SetGlobalState();
        }

        public void OnStop(string screenName)
        {
            var key = string.Concat(screenName.Where(c => !char.IsWhiteSpace(c)));

            var exists = this._state.TryGetValue(key, out var _);
            if (!exists) return;

            this._state.TryRemove(key, out var _);
            this.SetGlobalState();
        }

        public void OnUpdateDuration(string screenName, float duration)
        {
            var key = string.Concat(screenName.Where(c => !char.IsWhiteSpace(c)));

            var exists = this._state.TryGetValue(key, out var oldState);
            if (!exists) return;

            var newState = oldState;
            newState.Duration = duration;

            this._state.TryUpdate(key, newState, oldState);
            this.SetGlobalState();
        }

        // flush out old states at regular interval
        private async Task OnTick()
        {
            if (this._state.IsEmpty) return;

            foreach (var duiState in this._state.Values)
            {
                if (duiState.Repeat || !duiState.Ended) continue;

                try
                {
                    this._state.TryRemove(duiState.ScreenName, out _);
                }
                catch (Exception)
                {
                }
            }

            await BaseScript.Delay(60000);
        }

        private void SetGlobalState()
        {
            if (this._state.IsEmpty)
            {
                BaseServer.Self.SetGlobalState("hypnonema", null, true);
                return;
            }

            var state = this._state.Values.ToList();
            
            BaseServer.Self.SetGlobalState("hypnonema", JsonConvert.SerializeObject(state), true);
        }
    }
}