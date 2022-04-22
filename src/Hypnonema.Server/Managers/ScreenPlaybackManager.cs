using Hypnonema.Shared.Communications;

namespace Hypnonema.Server.Managers
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

        public NetworkMethod<StateDurationMessage> Duration { get; private set; }

        public bool IsInitialized { get; private set; }

        public NetworkMethod<PauseMessage> Pause { get; private set; }

        public NetworkMethod<PlayMessage> Play { get; private set; }

        public NetworkMethod<PlaybackEndedMessage> PlaybackEnded { get; private set; }

        public NetworkMethod<ResumeMessage> Resume { get; private set; }

        public NetworkMethod<SeekMessage> Seek { get; private set; }

        public NetworkMethod<StopMessage> Stop { get; private set; }

        public void Initialize(LiteCollection<Screen> screens)
        {
            if (this.IsInitialized) return;

            this.screenCollection = screens;
            this.screenStateManager = new ScreenStateManager();

            this.Play = new NetworkMethod<PlayMessage>(Events.Play, this.OnPlay);
            this.Pause = new NetworkMethod<PauseMessage>(Events.Pause, this.OnPause);
            this.Stop = new NetworkMethod<StopMessage>(Events.Stop, this.OnStop);
            this.Resume = new NetworkMethod<ResumeMessage>(Events.Resume, this.OnResume);
            this.Seek = new NetworkMethod<SeekMessage>(Events.Seek, this.OnSeek);
            this.Duration = new NetworkMethod<StateDurationMessage>(Events.UpdateStateDuration, this.OnUpdateDuration);
            this.PlaybackEnded = new NetworkMethod<PlaybackEndedMessage>(Events.PlaybackEnded, this.OnPlaybackEnded);

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

            var pauseMessage = new PauseMessage(screenName);

            this.Pause.Invoke(null, pauseMessage);

            this.screenStateManager.OnPause(screenName);
        }

        private void OnPause(Player p, PauseMessage pauseMessage)
        {
            if (!p.IsAceAllowed(Permission.Pause))
            {
                p.AddChatMessage(
                    $"You are not permitted to pause a screen. Missing ace: {Permission.Pause}",
                    new[] { 255, 0, 0 });
                return;
            }

            var screen = this.screenCollection.FindOne(s => s.Name == pauseMessage.ScreenName);
            if (screen == null)
            {
                p.AddChatMessage($"Pausing failed. Screen \"{pauseMessage.ScreenName}\" not found.");
                return;
            }

            this.Pause.Invoke(null, pauseMessage);
            this.screenStateManager.OnPause(pauseMessage.ScreenName);
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

            var playMessage = new PlayMessage() { Screen = screen, Url = videoUrl };

            if (!playMessage.IsValid)
            {
                Logger.Error($"play \"{playMessage.Url}\" on screen \"{playMessage.Url}\" failed. Reason: Invalid url");
                return;
            }

            this.Play.Invoke(null, playMessage);

            this.screenStateManager.OnPlay(screen, videoUrl);

            Logger.Debug($"playing: {playMessage.Url} on \"{playMessage.Screen.Name}\"");
        }

        private void OnPlay(Player p, PlayMessage playMessage)
        {
            if (!p.IsAceAllowed(Permission.Play))
            {
                p.AddChatMessage($"You are not permitted to play on this screen. missing ace: \"{Permission.Play}\"");
                return;
            }

            if (!playMessage.IsValid)
            {
                p.AddChatMessage($"Play \"{playMessage.Url}\" on screen \"{playMessage.Url}\" failed. Reason: Invalid url");
                return;
            }

            var screen = this.screenCollection.FindOne(s => s.Name == playMessage.Screen.Name);
            if (screen == null)
            {
                p.AddChatMessage($"Play on screen \"{playMessage.Screen.Name}\" failed. screen not found");
                return;
            }

            Logger.Debug($"playing: {playMessage.Url} on \"{playMessage.Screen.Name}\"");

            this.Play.Invoke(null, playMessage);

            this.screenStateManager.OnPlay(screen, playMessage.Url);
        }

        private void OnPlaybackEnded(Player p, PlaybackEndedMessage playbackEndedMessage)
        {
            this.screenStateManager.OnEnded(playbackEndedMessage.ScreenName);
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

            var resumeMessage = new ResumeMessage(screenName);

            this.Resume.Invoke(null, resumeMessage);
            
            this.screenStateManager.OnResume(screenName);
        }

        private void OnResume(Player p, ResumeMessage resumeMessage)
        {
            if (!p.IsAceAllowed(Permission.Resume))
            {
                p.AddChatMessage($"You are not permitted to resume a screen. missing ace {Permission.Resume}");
                return;
            }

            var screen = this.screenCollection.FindOne(s => s.Name == resumeMessage.ScreenName);
            if (screen == null)
            {
                p.AddChatMessage($"Resuming failed. screen \"{resumeMessage.ScreenName}\" not found.");
                return;
            }

            this.Resume.Invoke(null, resumeMessage);

            this.screenStateManager.OnResume(resumeMessage.ScreenName);
        }

        private void OnSeek(Player p, SeekMessage seekMessage)
        {
            if (!p.IsAceAllowed(Permission.Seek))
            {
                p.AddChatMessage($"You are not permitted to seek. missing ace: {Permission.Seek}", new[] { 255, 0, 0 });
                return;
            }

            var screen = this.screenCollection.FindOne(s => s.Name == seekMessage.ScreenName);
            if (screen == null)
            {
                p.AddChatMessage($"Seek failed. screen \"{seekMessage.ScreenName}\" not found.");
                return;
            }

            this.Seek.Invoke(null, seekMessage);

            this.screenStateManager.OnSeek(seekMessage.ScreenName, seekMessage.Time);
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

            var seekMessage = new SeekMessage(screenName, time);

            this.Seek.Invoke(null, seekMessage);

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

            var stopMessage = new StopMessage(screenName);

            this.Stop.Invoke(null, stopMessage);

            this.screenStateManager.OnStop(screenName);
        }

        private void OnStop(Player p, StopMessage stopMessage)
        {
            if (!p.IsAceAllowed(Permission.Stop))
            {
                p.AddChatMessage($"You are not permitted to stop a screen. missing ace: {Permission.Stop}");
                return;
            }

            var screen = this.screenCollection.FindOne(s => s.Name == stopMessage.ScreenName);
            if (screen == null)
            {
                p.AddChatMessage($"Stopping failed. screen \"{stopMessage.ScreenName}\" not found.");
                return;
            }

            this.Stop.Invoke(null, stopMessage);

            this.screenStateManager.OnStop(stopMessage.ScreenName);
        }

        private void OnUpdateDuration(Player p, StateDurationMessage stateDurationMessage)
        {
            this.screenStateManager.OnUpdateDuration(stateDurationMessage.ScreenName, stateDurationMessage.Duration);
        }
    }
}