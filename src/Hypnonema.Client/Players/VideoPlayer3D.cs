namespace Hypnonema.Client.Players
{
    using System;
    using System.Threading.Tasks;

    using CitizenFX.Core;

    using Hypnonema.Client.Graphics;

    public class VideoPlayer3D : IVideoPlayer
    {
        private readonly TextureRenderer textureRenderer;

        static readonly float RENDER_DISTANCE = 300.0f;

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

        public bool Is3DAudioEnabled { get; set; } = true;

        public string ScreenName { get; }

        public float SoundAttenuation { get; set; }

        public float SoundMaxDistance { get; set; }

        public float SoundMinDistance { get; set; }

        public void CalculateVolume()
        {
            var distance = this.textureRenderer.GetDistanceToPlayer();
            if (distance >= this.SoundMaxDistance)
            {
                this.Browser.SetVolume(0f);
                return;
            }

            if (this.Is3DAudioEnabled)
            {
                this.Browser.SetVolume(this.GlobalVolume);
                var tickData = new AudioTickData
                                   {
                                       ListenerForward = Game.PlayerPed.ForwardVector,
                                       ListenerUp = Game.PlayerPed.UpVector,
                                       PositionListener = Game.PlayerPed.Position,
                                       PositionPanner = this.textureRenderer.Position - Game.PlayerPed.Position,
                                       OrientationPanner = GameMath.RotationToDirection(this.textureRenderer.Rotation)
                                   };
                this.Browser.Tick(tickData);
            }
            else
            {
                this.Browser.SetVolume(this.GetSoundFactor(distance) * this.GlobalVolume);
            }
        }

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
            float dist = this.textureRenderer.GetDistanceToPlayer();
            if (dist <= RENDER_DISTANCE)
            {
                this.Draw();
                this.CalculateVolume();
            } else
            {
                this.Browser.SetVolume(0f);
            }

            await Task.FromResult(0);
        }

        public void Pause()
        {
            this.Browser.Pause();
        }

        public void Play(string url)
        {
            this.Browser.Play(url);
        }

        public void Resume()
        {
            this.Browser.Resume();
        }

        public void Seek(float time)
        {
            this.Browser.Seek(time);
        }

        public void Stop()
        {
            this.Browser.Stop();
        }

        public void SynchronizeState(bool paused, float currentTime, string currentSource, bool repeat)
        {
            this.Browser.Update(paused, currentTime, currentSource, repeat);
        }

        public void Toggle3DAudio(bool value)
        {
            this.Browser.Toggle3DAudio(value);
        }

        public void ToggleRepeat()
        {
            this.Browser.ToggleRepeat();
        }

        private float GetSoundFactor(float distance)
        {
            return this.SoundMinDistance / (this.SoundMinDistance + this.SoundAttenuation
                                            * (Math.Max(distance, this.SoundMinDistance) - this.SoundMinDistance));
        }
    }
}