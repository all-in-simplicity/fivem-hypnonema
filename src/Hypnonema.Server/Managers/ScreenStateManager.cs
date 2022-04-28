namespace Hypnonema.Server.Managers
{
    using System;

    using CitizenFX.Core;

    using Hypnonema.Server.Communications;
    using Hypnonema.Server.Utils;
    using Hypnonema.Shared;
    using Hypnonema.Shared.Communications;
    using Hypnonema.Shared.Models;

    using Newtonsoft.Json;

    public sealed class ScreenStateManager
    {
        private readonly State _state = new State();

        private readonly NetworkMethod<DuiStateMessage> duiState;

        public ScreenStateManager()
        {
            this.duiState = new NetworkMethod<DuiStateMessage>(Events.DuiState, this.OnDuiState);

            BaseServer.Self.AddExport(Events.DuiState, new Func<string>(this.OnDuiState));
        }

        public void OnEnded(string screenName)
        {
            this._state.Remove(screenName);
        }

        public void OnPause(string screenName)
        {
            var screenState = this._state.Get(screenName);
            if (screenState == null) return;

            screenState.IsPaused = true;

            this._state.Update(screenName, screenState);
        }

        public void OnPlay(Screen screen, string url)
        {
            var duiState = new DuiState(screen, url);

            this._state.Add(screen.Name, duiState);
        }

        public void OnRepeat(string screenName, bool repeat)
        {
            var screenState = this._state.Get(screenName);
            if (screenState == null) return;

            screenState.Repeat = repeat;

            this._state.Update(screenName, screenState);
        }

        public void OnResume(string screenName)
        {
            var screenState = this._state.Get(screenName);
            if (screenState == null) return;

            screenState.IsPaused = false;

            this._state.Update(screenName, screenState);
        }

        public void OnSeek(string screenName, float time)
        {
            var screenState = this._state.Get(screenName);
            if (screenState == null) return;

            var diff = time - screenState.CurrentTime;
            screenState.StartedAt = screenState.StartedAt.Subtract(new TimeSpan(0, 0, (int) diff));

            this._state.Update(screenName, screenState);
        }

        public void OnStop(string screenName)
        {
            this._state.Remove(screenName);
        }

        public void OnUpdateDuration(string screenName, float duration)
        {
            var screenState = this._state.Get(screenName);
            if (screenState == null) return;

            screenState.Duration = duration;

            this._state.Update(screenName, screenState);
        }

        private string OnDuiState()
        {
            var state = this._state.ToList();

            return state == null ? string.Empty : JsonConvert.SerializeObject(state);
        }

        private void OnDuiState(Player p, DuiStateMessage duiStateMessage)
        {
            var states = this._state.ToList();
            if (states == null) return;

            duiStateMessage.DuiStates = states;
            this.duiState.Invoke(p, duiStateMessage);
        }
    }
}