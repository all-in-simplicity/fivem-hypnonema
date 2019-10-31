namespace Hypnonema.Client.Players
{
    using System;
    using System.Threading.Tasks;

    using CitizenFX.Core;

    using Hypnonema.Client.Graphics;

    public class VideoPlayer3D : IVideoPlayer
    {
        private readonly TextureRenderer textureRenderer;

        public VideoPlayer3D(DuiBrowser browser, TextureRenderer textureRenderer, string screenName)
        {
            this.textureRenderer = textureRenderer;
            this.ScreenName = screenName;
            this.Browser = browser;
        }

        ~VideoPlayer3D()
        {
            this.Dispose();
        }

        public DuiBrowser Browser { get; }

        public float GlobalVolume { get; set; }

        public string ScreenName { get; }

        public float SoundAttenuation { get; set; }

        public float SoundMaxDistance { get; set; }

        public float SoundMinDistance { get; set; }

        public void Dispose()
        {
            if (this.Browser.IsValid) this.Browser.Dispose();

            TextureRendererPool.Instance.ReleaseTextureRenderer(this.textureRenderer.Id);
            GC.SuppressFinalize(this);
        }

        public void Draw()
        {
            this.textureRenderer.Render3D();
        }

        public async Task OnTick()
        {
            this.Draw();

            if (this.textureRenderer.GetDistanceToPlayer() >= this.SoundMaxDistance)
            {
                this.Browser.SendVolume(0f);
                return;
            }
            
            // TODO: Implement better Mute/Unmute if out of range
            this.Browser.SendVolume(this.GlobalVolume);

            // still needs improvement
            // Orientation needs to be calculated or user specified!
            var tickData = new AudioTickData
                               {
                                   ListenerForward = Game.PlayerPed.ForwardVector,
                                   ListenerUp = Game.PlayerPed.UpVector,
                                   PositionListener = Game.PlayerPed.Position,
                                   PositionPanner = this.textureRenderer.Position - Game.PlayerPed.Position,
                                   OrientationPanner = Vector3.ForwardRH
                               };
            this.Browser.SendTick(tickData);
     
            await Task.FromResult(0);
        }

        public void Pause()
        {
            this.Browser.SendPause();
        }

        public void Play(string url)
        {
            this.Browser.SendPlay(url);
        }

        public void Resume()
        {
            this.Browser.SendResume();
        }

        public void Seek(float time)
        {
            this.Browser.SendSeek(time);
        }

        public void Stop()
        {
            this.Browser.SendStop();
        }

        public void SynchronizeState(bool paused, float currentTime, string currentType, string currentSource)
        {
            this.Browser.SendUpdate(paused, currentTime, currentType, currentSource);
        }

        public void ToggleReplay(bool replay)
        {
            this.Browser.SendToggleReplay(replay);
        }

    }
}