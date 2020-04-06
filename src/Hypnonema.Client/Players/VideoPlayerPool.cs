namespace Hypnonema.Client.Players
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CitizenFX.Core;
    using CitizenFX.Core.Native;
    using CitizenFX.Core.UI;

    using Hypnonema.Client.Graphics;
    using Hypnonema.Shared;

    public class VideoPlayerPool : IDisposable
    {
        private readonly int duiHeight = (int)Screen.Height;

        private readonly string duiUrl = "http://localhost:9414";

        private readonly int duiWidth = (int)Screen.Width;

        private readonly string posterUrl = "";

        public VideoPlayerPool(string duiUrl, int duiWidth = 1280, int duiHeight = 720)
        {
            this.duiUrl = duiUrl;
            this.duiWidth = duiWidth;
            this.duiHeight = duiHeight;
            this.posterUrl = API.GetResourceMetadata(API.GetCurrentResourceName(), "hypnonema_poster_url", 0);
        }

        ~VideoPlayerPool()
        {
            this.Dispose();
        }

        public IList<IVideoPlayer> VideoPlayers { get; set; } = new List<IVideoPlayer>();

        public void CloseScreen(string screenName)
        {
            var player = this.VideoPlayers.FirstOrDefault(s => s.ScreenName == screenName);
            if (player == null) return;

            try
            {
                this.VideoPlayers.Remove(player);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            player.Dispose();
            Debug.WriteLine($"Closed screen: {screenName}");
        }

        // TODO: Still needs more refactoring => currently too long
        public async Task<IVideoPlayer> CreateVideoPlayerAsync(Shared.Models.Screen screen)
        {
            RenderTarget renderTarget;
            TextureRenderer textureRenderer;

            var browser = new DuiBrowser(this.duiUrl, this.duiWidth, this.duiHeight);
            while (!API.IsDuiAvailable(browser.NativeValue)) await BaseScript.Delay(1);

            browser.CreateRuntimeTexture();
            await BaseScript.Delay(750);

            Debug.WriteLine("sending init..");
            browser.SendInit(screen.Name, this.posterUrl);

            if (!screen.Is3DRendered)
            {
                renderTarget = new RenderTarget(
                    screen.TargetSettings.ModelName,
                    screen.TargetSettings.RenderTargetName);
                var player = new VideoPlayer2D(browser, renderTarget, screen.Name)
                                 {
                                     GlobalVolume = screen.BrowserSettings.GlobalVolume,
                                     SoundMinDistance = screen.BrowserSettings.SoundMinDistance,
                                     SoundAttenuation = screen.BrowserSettings.SoundAttenuation,
                                     SoundMaxDistance = screen.BrowserSettings.SoundMaxDistance
                                 };
                return player;
            }
            else
            {
                var position = new Vector3(
                    screen.PositionalSettings.PositionX,
                    screen.PositionalSettings.PositionY,
                    screen.PositionalSettings.PositionZ);
                var rotation = new Vector3(
                    screen.PositionalSettings.RotationX,
                    screen.PositionalSettings.RotationY,
                    screen.PositionalSettings.RotationZ);
                var scale = new Vector3(
                    screen.PositionalSettings.ScaleX,
                    screen.PositionalSettings.ScaleY,
                    screen.PositionalSettings.ScaleZ);

                textureRenderer = await TextureRendererPool.Instance.CreateTextureRenderer(position, rotation, scale);
                if (textureRenderer == null)
                {
                    Debug.WriteLine("failed to create scaleform.");
                    return null;
                }

                textureRenderer.SetTexture(browser.TxdName, browser.TxnName);

                var player = new VideoPlayer3D(browser, textureRenderer, screen.Name)
                                 {
                                     GlobalVolume = screen.BrowserSettings.GlobalVolume,
                                     SoundMinDistance = screen.BrowserSettings.SoundMinDistance,
                                     SoundAttenuation = screen.BrowserSettings.SoundAttenuation,
                                     SoundMaxDistance = screen.BrowserSettings.SoundMaxDistance
                                 };
                return player;
            }
        }

        public void Dispose()
        {
            foreach (var videoPlayer in this.VideoPlayers) videoPlayer.Dispose();

            GC.SuppressFinalize(this);
        }

        public async Task OnTick()
        {
            foreach (var player in this.VideoPlayers) await player.OnTick();
        }

        public void PauseVideo(string screenName)
        {
            var player = this.VideoPlayers?.FirstOrDefault(s => s.ScreenName == screenName);

            player?.Pause();
        }

        public async Task Play(string url, Shared.Models.Screen screen)
        {
            var player = this.VideoPlayers?.FirstOrDefault(p => p.ScreenName == screen.Name);

            if (player == null)
            {
                player = await this.CreateVideoPlayerAsync(screen);
                if (player == null)
                {
                    Debug.WriteLine("playback failed. couldn't create player");
                    return;
                }

                this.VideoPlayers?.Add(player);
            }

            player.Play(url);
        }

        public void ResumeVideo(string screenName)
        {
            var player = this.VideoPlayers?.FirstOrDefault(s => s.ScreenName == screenName);

            player?.Resume();
        }

        public void Seek(string screenName, float time)
        {
            var player = this.VideoPlayers?.FirstOrDefault(s => s.ScreenName == screenName);
            player?.Seek(time);
        }

        public void SetVolume(string screenName, float volume)
        {
            var screen = this.VideoPlayers?.FirstOrDefault(s => s.ScreenName == screenName);
            if (screen == null) return;

            screen.GlobalVolume = volume;
        }

        public void StopVideo(string screenName)
        {
            var screen = this.VideoPlayers?.FirstOrDefault(s => s.ScreenName == screenName);

            screen?.Browser.SendMessage(new { type = "stop" });
        }

        public async Task SynchronizeState(DuiState state, Shared.Models.Screen screen)
        {
            var player = this.VideoPlayers?.FirstOrDefault(p => p.ScreenName == screen.Name);
            if (player != null)
            {
                // Player exists. No need to synchronize.
                return;
            }

            player = await this.CreateVideoPlayerAsync(screen);
            if (player == null)
            {
                // Failed to create player.
                // TODO: Needs improvement.
                Debug.WriteLine("Failed to create player");
                return;
            }

            Debug.WriteLine("Synchronizing..");
            player.SynchronizeState(state.IsPaused, state.CurrentTime, state.CurrentSource);
            this.VideoPlayers.Add(player);
        }
    }
}