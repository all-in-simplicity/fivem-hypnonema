namespace Hypnonema.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CitizenFX.Core;

    using Hypnonema.Client.Communications;
    using Hypnonema.Client.Dui;
    using Hypnonema.Client.Graphics;
    using Hypnonema.Client.Players;
    using Hypnonema.Client.Utils;
    using Hypnonema.Shared;
    using Hypnonema.Shared.Models;

    using Newtonsoft.Json;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

    public sealed class ScreenPlaybackManager : IDisposable
    {
        private readonly IList<IMediaPlayer> videoPlayers = new List<IMediaPlayer>();

        ~ScreenPlaybackManager()
        {
            this.Dispose();
        }

        public NetworkMethod<string> DeleteScreen { get; private set; }

        public NetworkMethod<Screen> EditScreen { get; private set; }

        public bool IsInitialized { get; private set; }

        public NetworkMethod<string> Pause { get; private set; }

        public NetworkMethod<PlayEvent> Play { get; private set; }

        public NetworkMethod<string> PlaybackEnded { get; private set; }

        public NetworkMethod<string> Resume { get; private set; }

        public NetworkMethod<string, float> Seek { get; private set; }

        public NetworkMethod<string, float> StateDuration { get; private set; }

        public NetworkMethod<string> Stop { get; private set; }

        public void Dispose()
        {
            ClientScript.Self.UnregisterNuiCallback(Events.Play, this.OnPlay);
            ClientScript.Self.UnregisterNuiCallback(Events.Pause, this.OnPause);
            ClientScript.Self.UnregisterNuiCallback(Events.Resume, this.OnResume);
            ClientScript.Self.UnregisterNuiCallback(Events.Stop, this.OnStop);

            foreach (var player in this.videoPlayers)
            {
                player.Dispose();
            }

            GC.SuppressFinalize(this);
        }

        public void Initialize()
        {
            if (this.IsInitialized) return;

            this.Play = new NetworkMethod<PlayEvent>(Events.Play, this.OnPlay);
            this.Pause = new NetworkMethod<string>(Events.Pause, this.OnPause);
            this.Resume = new NetworkMethod<string>(Events.Resume, this.OnResume);
            this.Stop = new NetworkMethod<string>(Events.Stop, this.OnStop);
            this.Seek = new NetworkMethod<string, float>(Events.Seek, this.OnSeek);
            this.StateDuration = new NetworkMethod<string, float>(
                Events.UpdateStateDuration,
                this.OnUpdateStateDuration);
            this.EditScreen = new NetworkMethod<Screen>(Events.EditScreen, this.OnEditScreen);
            this.DeleteScreen = new NetworkMethod<string>(Events.DeleteScreen, this.OnDeleteScreen);
            this.PlaybackEnded = new NetworkMethod<string>(Events.PlaybackEnded, this.OnPlaybackEnded);

            ClientScript.Self.RegisterNuiCallback(Events.Play, this.OnPlay);
            ClientScript.Self.RegisterNuiCallback(Events.Pause, this.OnPause);
            ClientScript.Self.RegisterNuiCallback(Events.Resume, this.OnResume);
            ClientScript.Self.RegisterNuiCallback(Events.Stop, this.OnStop);
            ClientScript.Self.RegisterNuiCallback(Events.Seek, this.OnSeek);

            ClientScript.Self.RegisterNuiCallback(Events.UpdateStateDuration, this.OnUpdateStateDuration);
            ClientScript.Self.RegisterNuiCallback(Events.RequestState, this.OnRequestState);
            ClientScript.Self.RegisterNuiCallback(Events.PlaybackEnded, this.OnPlaybackEnded);

            this.IsInitialized = true;
        }

        public async Task SynchronizeState(DuiState state)
        {
            var player = this.videoPlayers?.FirstOrDefault(s => s.PlayerName == state.ScreenName);
            if (player != null)
            {
                // no need to synchronize. player already exists.
                return;
            }

            player = await this.CreateVideoPlayer(state.Screen);

            player?.SynchronizeState(state.IsPaused, state.CurrentTime, state.CurrentSource, state.Repeat);

            this.videoPlayers?.Add(player);
        }

        private async Task<IMediaPlayer> CreateVideoPlayer(Screen screen)
        {
            IMediaPlayer videoPlayer;

            var duiBrowser = await DuiBrowserPool.Instance.AcquireDuiBrowser(screen);

            if (screen.Is3DRendered)
            {
                var scaleform = await ScaleformRendererPool.Instance.AcquireScaleformRenderer(
                                    screen.PositionalSettings,
                                    duiBrowser.TxdName,
                                    duiBrowser.TxnName);

                videoPlayer = new MediaPlayer3D(
                    scaleform,
                    duiBrowser,
                    screen.Name,
                    screen.BrowserSettings.GlobalVolume,
                    screen.BrowserSettings.SoundAttenuation,
                    screen.BrowserSettings.SoundMaxDistance,
                    screen.BrowserSettings.SoundMinDistance);
            }
            else
            {
                var renderTarget = new RenderTargetRenderer(
                    screen.TargetSettings.ModelName,
                    screen.TargetSettings.RenderTargetName,
                    duiBrowser.TxdName,
                    duiBrowser.TxnName);

                videoPlayer = new MediaPlayer2D(
                    renderTarget,
                    duiBrowser,
                    screen.Name,
                    screen.BrowserSettings.GlobalVolume,
                    screen.BrowserSettings.SoundAttenuation,
                    screen.BrowserSettings.SoundMaxDistance,
                    screen.BrowserSettings.SoundMinDistance);
            }

            return videoPlayer;
        }

        private void OnDeleteScreen(string screenName)
        {
            var player = this.videoPlayers?.FirstOrDefault(p => p.PlayerName == screenName);
            if (player == null) return;

            this.videoPlayers.Remove(player);
            player.Dispose();
        }

        private async void OnEditScreen(Screen screen)
        {
            var currentPlayer = this.videoPlayers?.FirstOrDefault(p => p.PlayerName == screen.Name);
            if (currentPlayer == null)
            {
                return;
            }

            this.videoPlayers?.Remove(currentPlayer);
            currentPlayer.Dispose();

            var newPlayer = await this.CreateVideoPlayer(screen);

            this.videoPlayers?.Add(newPlayer);
        }

        private CallbackDelegate OnPause(IDictionary<string, object> args, CallbackDelegate callback)
        {
            var screenName = ArgsReader.GetArgKeyValue<string>(args, "screenName");
            if (string.IsNullOrEmpty(screenName))
            {
                callback("ERROR", "screenName is empty");
                return callback;
            }

            this.Pause.Invoke(screenName);

            callback("OK");
            return callback;
        }

        private void OnPause(string screenName)
        {
            var player = this.videoPlayers.FirstOrDefault(p => p.PlayerName == screenName);

            player?.Pause();
        }

        private async void OnPlay(PlayEvent playEvent)
        {
            var player = this.videoPlayers.FirstOrDefault(p => p.PlayerName == playEvent.Screen.Name);

            if (player == null)
            {
                player = await this.CreateVideoPlayer(playEvent.Screen);
                if (player == null)
                {
                    ClientScript.AddChatMessage("Playback failed. Check logs for more information.");
                    return;
                }

                player.InitDuiBrowser();
                player.Play(playEvent.Url);

                await BaseScript.Delay(500);

                this.videoPlayers.Add(player);

                Logger.Debug($"created a new player for screen: \"{playEvent.Screen.Name}\"");
                return;
            }

            player.Play(playEvent.Url);
        }

        private CallbackDelegate OnPlay(IDictionary<string, object> args, CallbackDelegate callback)
        {
            var videoUrl = ArgsReader.GetArgKeyValue<string>(args, "videoUrl");
            var screen = ArgsReader.GetArgKeyValue<Screen>(args, "screen");

            if (string.IsNullOrEmpty(videoUrl))
            {
                callback("ERROR", "missing videoUrl");
                return callback;
            }

            if (screen == null)
            {
                callback("ERROR", "missing screen");
                return callback;
            }

            var playEvent = new PlayEvent() { Screen = screen, Url = videoUrl };
            this.Play.Invoke(playEvent);

            callback("OK");
            return callback;
        }

        private void OnPlaybackEnded(string screenName)
        {
            // Nothing to do on client side
        }

        private CallbackDelegate OnPlaybackEnded(IDictionary<string, object> args, CallbackDelegate callback)
        {
            callback("OK");

            var screenName = ArgsReader.GetArgKeyValue<string>(args, "screenName");
            if (string.IsNullOrEmpty(screenName))
            {
                return callback;
            }

            var player = this.videoPlayers?.FirstOrDefault(p => p.PlayerName == screenName);
            if (player == null)
            {
                return callback;
            }

            this.videoPlayers.Remove(player);
            player.Dispose();

            this.PlaybackEnded.Invoke(screenName);

            return callback;
        }

        private async Task<CallbackDelegate> OnRequestState(IDictionary<string, object> args, CallbackDelegate callback)
        {
            var state = await ClientScript.Self.DuiStateHelper.RequestDuiStateAsync();

            callback(JsonConvert.SerializeObject(state, Nui.NuiSerializerSettings));
            return callback;
        }

        private void OnResume(string screenName)
        {
            var player = this.videoPlayers.FirstOrDefault(p => p.PlayerName == screenName);

            player?.Resume();
        }

        private CallbackDelegate OnResume(IDictionary<string, object> args, CallbackDelegate callback)
        {
            var screenName = ArgsReader.GetArgKeyValue<string>(args, "screenName");
            if (string.IsNullOrEmpty(screenName))
            {
                callback("ERROR", "screenName is empty");
                return callback;
            }

            this.Resume.Invoke(screenName);

            callback("OK");
            return callback;
        }

        private CallbackDelegate OnSeek(IDictionary<string, object> args, CallbackDelegate callback)
        {
            var screenName = ArgsReader.GetArgKeyValue<string>(args, "screenName");
            var time = ArgsReader.GetArgKeyValue<float>(args, "time");

            this.Seek.Invoke(screenName, time);

            callback("OK");
            return callback;
        }

        private void OnSeek(string screenName, float time)
        {
            var player = this.videoPlayers.FirstOrDefault(s => s.PlayerName == screenName);

            player?.Seek(time);
        }

        private CallbackDelegate OnStop(IDictionary<string, object> args, CallbackDelegate callback)
        {
            var screenName = ArgsReader.GetArgKeyValue<string>(args, "screenName");
            if (string.IsNullOrEmpty(screenName))
            {
                callback("ERROR", "screenName is empty");
                return callback;
            }

            this.Stop.Invoke(screenName);

            callback("OK");
            return callback;
        }

        private void OnStop(string screenName)
        {
            var player = this.videoPlayers.FirstOrDefault(p => p.PlayerName == screenName);
            if (player == null) return;

            player.Stop();

            this.videoPlayers.Remove(player);

            player.Dispose();
        }

        private void OnUpdateStateDuration(string screenName, float duration)
        {
            // nothing to do on client side
        }

        private CallbackDelegate OnUpdateStateDuration(IDictionary<string, object> args, CallbackDelegate callback)
        {
            var duration = ArgsReader.GetArgKeyValue<float>(args, "duration");
            var screenName = ArgsReader.GetArgKeyValue<string>(args, "screenName");

            if (string.IsNullOrEmpty(screenName))
            {
                callback("ERROR");
                return callback;
            }

            this.StateDuration.Invoke(screenName, duration);
            callback("OK");
            return callback;
        }
    }
}