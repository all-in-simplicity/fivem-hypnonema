namespace Hypnonema.Server.Screens
{
    using System;

    using CitizenFX.Core;

    using Hypnonema.Server.Communications;
    using Hypnonema.Shared;
    using Hypnonema.Shared.Models;

    using LiteDB;

    using Logger = Hypnonema.Server.Utils.Logger;

    public sealed class ScreenPlaybackManager
    {
        private LiteCollection<Screen> screenCollection;

        private ScreenStateManager screenStateManager;

        public NetworkMethod<string, float> Duration { get; private set; }

        public bool IsInitialized { get; private set; }

        public NetworkMethod<string> Pause { get; private set; }

        public NetworkMethod<PlayEvent> Play { get; private set; }

        public NetworkMethod<string> PlaybackEnded { get; private set; }

        public NetworkMethod<string> Resume { get; private set; }

        public NetworkMethod<string, float> Seek { get; private set; }

        public NetworkMethod<string> Stop { get; private set; }

        public void Initialize(LiteCollection<Screen> screenCollection)
        {
            if (this.IsInitialized) return;

            this.screenCollection = screenCollection;
            this.screenStateManager = new ScreenStateManager();

            this.Play = new NetworkMethod<PlayEvent>(Events.Play, this.OnPlay);
            this.Pause = new NetworkMethod<string>(Events.Pause, this.OnPause);
            this.Stop = new NetworkMethod<string>(Events.Stop, this.OnStop);
            this.Resume = new NetworkMethod<string>(Events.Resume, this.OnResume);
            this.Seek = new NetworkMethod<string, float>(Events.Seek, this.OnSeek);
            this.Duration = new NetworkMethod<string, float>(Events.UpdateStateDuration, this.OnUpdateDuration);
            this.PlaybackEnded = new NetworkMethod<string>(Events.PlaybackEnded, this.OnPlaybackEnded);

            BaseServer.Self.AddExport(Events.Play, new Action<string, string>(this.OnPlay));
            BaseServer.Self.AddExport(Events.Pause, new Action<string>(this.OnPause));
            BaseServer.Self.AddExport(Events.Stop, new Action<string>(this.OnStop));
            BaseServer.Self.AddExport(Events.Resume, new Action<string>(this.OnResume));
            BaseServer.Self.AddExport(Events.Seek, new Action<string, float>(this.OnSeek));

            this.IsInitialized = true;
        }

        // Called through export
        private void OnPause(string screenName)
        {
            var screen = this.screenCollection.FindOne(s => s.Name == screenName);
            if (screen == null)
            {
                Logger.Error($"Pausing failed. Screen \"{screenName}\" not found.");
                return;
            }

            this.Pause.Invoke(null, screenName);
            this.screenStateManager.OnPause(screenName);
        }

        private void OnPause(Player p, string screenName)
        {
            if (!p.IsAceAllowed(Permission.Pause))
            {
                p.AddChatMessage(
                    $"You are not permitted to pause a screen. Missing ace: {Permission.Pause}",
                    new[] { 255, 0, 0 });
                return;
            }

            var screen = this.screenCollection.FindOne(s => s.Name == screenName);
            if (screen == null)
            {
                p.AddChatMessage($"Pausing failed. Screen \"{screenName}\" not found.");
                return;
            }

            this.Pause.Invoke(null, screenName);
            this.screenStateManager.OnPause(screenName);
        }

        // Called through export
        private void OnPlay(string screenName, string videoUrl)
        {
            var screen = this.screenCollection.FindOne(s => s.Name == screenName);
            if (screen == null)
            {
                Logger.Error($"play on screen \"{screenName}\" failed. Screen not found");
                return;
            }

            var playEvent = new PlayEvent() { Screen = screen, Url = videoUrl };

            if (!playEvent.IsValid)
            {
                Logger.Error($"play \"{playEvent.Url}\" on screen \"{playEvent.Url}\" failed. Reason: Invalid url");
                return;
            }

            this.Play.Invoke(null, playEvent);

            this.screenStateManager.OnPlay(screen, videoUrl);

            Logger.Debug($"playing: {playEvent.Url} on \"{playEvent.Screen.Name}\"");
        }

        private void OnPlay(Player p, PlayEvent playEvent)
        {
            if (!p.IsAceAllowed(Permission.Play))
            {
                p.AddChatMessage($"You are not permitted to play on this screen. missing ace: \"{Permission.Play}\"");
                return;
            }

            if (!playEvent.IsValid)
            {
                p.AddChatMessage($"Play \"{playEvent.Url}\" on screen \"{playEvent.Url}\" failed. Reason: Invalid url");
                return;
            }

            var screen = this.screenCollection.FindOne(s => s.Name == playEvent.Screen.Name);
            if (screen == null)
            {
                p.AddChatMessage($"Play on screen \"{playEvent.Screen.Name}\" failed. screen not found");
                return;
            }

            Logger.Debug($"playing: {playEvent.Url} on \"{playEvent.Screen.Name}\"");

            this.Play.Invoke(null, playEvent);

            this.screenStateManager.OnPlay(screen, playEvent.Url);
        }

        private void OnPlaybackEnded(Player p, string screenName)
        {
            this.screenStateManager.OnEnded(screenName);
        }

        // Called through export
        private void OnResume(string screenName)
        {
            var screen = this.screenCollection.FindOne(s => s.Name == screenName);
            if (screen == null)
            {
                Logger.Error($"Resuming failed. screen \"{screenName}\" not found.");
                return;
            }

            this.Resume.Invoke(null, screenName);
            this.screenStateManager.OnResume(screenName);
        }

        private void OnResume(Player p, string screenName)
        {
            if (!p.IsAceAllowed(Permission.Resume))
            {
                p.AddChatMessage($"You are not permitted to resume a screen. missing ace {Permission.Resume}");
                return;
            }

            var screen = this.screenCollection.FindOne(s => s.Name == screenName);
            if (screen == null)
            {
                p.AddChatMessage($"Resuming failed. screen \"{screenName}\" not found.");
                return;
            }

            this.Resume.Invoke(null, screenName);

            this.screenStateManager.OnResume(screenName);
        }

        private void OnSeek(Player p, string screenName, float time)
        {
            if (!p.IsAceAllowed(Permission.Seek))
            {
                p.AddChatMessage($"You are not permitted to seek. missing ace: {Permission.Seek}", new[] { 255, 0, 0 });
                return;
            }

            var screen = this.screenCollection.FindOne(s => s.Name == screenName);
            if (screen == null)
            {
                p.AddChatMessage($"Seek failed. screen \"{screenName}\" not found.");
                return;
            }

            this.Seek.Invoke(null, screenName, time);
            this.screenStateManager.OnSeek(screenName, time);
        }

        // Export
        private void OnSeek(string screenName, float time)
        {
            var screen = this.screenCollection.FindOne(s => s.Name == screenName);
            if (screen == null)
            {
                Logger.Error($"seeking failed. Cant find {screenName}");
                return;
            }

            this.Seek.Invoke(null, screenName, time);
            this.screenStateManager.OnSeek(screenName, time);
        }

        // Called through export
        private void OnStop(string screenName)
        {
            var screen = this.screenCollection.FindOne(s => s.Name == screenName);
            if (screen == null)
            {
                Logger.Error($"Stopping failed. screen \"{screenName}\" not found.");
                return;
            }

            this.Stop.Invoke(null, screenName);

            this.screenStateManager.OnStop(screenName);
        }

        private void OnStop(Player p, string screenName)
        {
            if (!p.IsAceAllowed(Permission.Stop))
            {
                p.AddChatMessage($"You are not permitted to stop a screen. missing ace: {Permission.Stop}");
                return;
            }

            var screen = this.screenCollection.FindOne(s => s.Name == screenName);
            if (screen == null)
            {
                p.AddChatMessage($"Stopping failed. screen \"{screenName}\" not found.");
                return;
            }

            this.Stop.Invoke(null, screenName);

            this.screenStateManager.OnStop(screenName);
        }

        private void OnUpdateDuration(Player p, string screenName, float duration)
        {
            this.screenStateManager.OnUpdateDuration(screenName, duration);
        }
    }
}