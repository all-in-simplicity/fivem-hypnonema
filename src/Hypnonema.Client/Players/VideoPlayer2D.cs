namespace Hypnonema.Client.Players
{
    using System;
    using System.Threading.Tasks;

    using CitizenFX.Core;
    using CitizenFX.Core.Native;

    using Hypnonema.Client.Dui;
    using Hypnonema.Client.Graphics;
    using Hypnonema.Client.Utils;
    using Hypnonema.Shared.Models;

    public sealed class VideoPlayer2D : IVideoPlayer
    {
        public VideoPlayer2D(Screen screen, RenderTargetRenderer renderTarget, DuiBrowser duiBrowser)
        {
            this.DuiBrowser = duiBrowser;
            this.RenderTarget = renderTarget;
            this.ScreenName = screen.Name;

            this.SoundAttenuation = screen.BrowserSettings.SoundAttenuation;
            this.GlobalVolume = screen.BrowserSettings.GlobalVolume;
            this.SoundMinDistance = screen.BrowserSettings.SoundMinDistance;
            this.SoundMaxDistance = screen.BrowserSettings.SoundMaxDistance;
        }

        ~VideoPlayer2D()
        {
            this.Dispose();
        }

        public DuiBrowser DuiBrowser { get; }

        public float GlobalVolume { get; set; } = 100f;

        public bool Is3DAudioEnabled { get; set; } = false;

        public float MaxRenderDistance { get; set; } = ConfigReader.GetConfigKeyValue(
            API.GetCurrentResourceName(),
            "hypnonema_max_render_distance",
            0,
            400f);

        public RenderTargetRenderer RenderTarget { get; }

        public string ScreenName { get; }

        public float SoundAttenuation { get; set; } = 5f;

        public float SoundMaxDistance { get; set; } = 300f;

        public float SoundMinDistance { get; set; } = 10f;

        public static IVideoPlayer CreateVideoPlayer(Screen screen, DuiBrowser duiBrowser)
        {
            var renderTarget = new RenderTargetRenderer(
                screen.TargetSettings.ModelName,
                screen.TargetSettings.RenderTargetName,
                duiBrowser.TxdName,
                duiBrowser.TxnName);

            return new VideoPlayer2D(screen, renderTarget, duiBrowser)
                       {
                           GlobalVolume = screen.BrowserSettings.GlobalVolume,
                           Is3DAudioEnabled = screen.BrowserSettings.Is3DAudioEnabled,
                           SoundAttenuation = screen.BrowserSettings.SoundAttenuation,
                           SoundMaxDistance = screen.BrowserSettings.SoundMaxDistance,
                           SoundMinDistance = screen.BrowserSettings.SoundMinDistance
                       };
        }

        public void CalculateVolume(float distance, bool isOccluded)
        {
            if (distance >= this.SoundMaxDistance)
            {
                this.DuiBrowser.SetVolume(0f);
            }
            else
            {
                if (isOccluded) this.DuiBrowser.SetVolume(this.GlobalVolume / 2);
                else this.DuiBrowser.SetVolume(this.GlobalVolume);

                this.DuiBrowser.SetVolume(this.GetSoundFactor(distance) * this.GlobalVolume);
            }
        }

        public void Dispose()
        {
            if (this.DuiBrowser.IsValid) DuiBrowserPool.Instance.ReleaseDuiBrowser(this.DuiBrowser);

            if (this.RenderTarget.IsValid) this.RenderTarget.Dispose();

            GC.SuppressFinalize(this);
        }

        public void Draw()
        {
            this.RenderTarget.Draw();
        }

        public async void OnTick()
        {
            var entity = await this.GetClosestObjectOfType();
            if (entity == null)
            {
                this.DuiBrowser.ShowPlayer(false);
                return;
            }

            var distance = World.GetDistance(Game.PlayerPed.Position, entity.Position);
            if (distance > this.MaxRenderDistance)
            {
                this.DuiBrowser.ShowPlayer(false);
                return;
            }

            this.CalculateVolume(distance, entity.IsOccluded);
            this.DuiBrowser.ShowPlayer(true);
            this.Draw();
        }

        public void Pause()
        {
            this.DuiBrowser.Pause();
        }

        public void Play(string url)
        {
            this.DuiBrowser.Play(url);
        }

        public void Resume()
        {
            this.DuiBrowser.Resume();
        }

        public void Seek(float time)
        {
            this.DuiBrowser.Seek(time);
        }

        public void Stop()
        {
            this.DuiBrowser.Stop();
        }

        public void SynchronizeState(bool paused, float currentTime, string currentSource, bool repeat)
        {
            this.DuiBrowser.SynchronizeState(paused, currentTime, currentSource, repeat);
        }

        public void ToggleRepeat()
        {
            this.DuiBrowser.ToggleRepeat();
        }

        private async Task<Prop> GetClosestObjectOfType()
        {
            var objects = API.GetGamePool("CObject");
            foreach (int obj in objects)
            {
                var entity = API.GetEntityModel(obj);

                if (entity == this.RenderTarget.Hash) return new Prop(obj);
            }

            return null;
        }

        private Prop GetClosestObjectOfType(float radius)
        {
            var playerPos = Game.PlayerPed.Position;
            var entity = API.GetClosestObjectOfType(
                playerPos.X,
                playerPos.Y,
                playerPos.Z,
                radius,
                (uint)this.RenderTarget.Hash,
                false,
                false,
                false);

            return entity == 0 ? null : new Prop(entity);
        }

        private float GetSoundFactor(float distance)
        {
            return this.SoundMinDistance / (this.SoundMinDistance + this.SoundAttenuation
                                            * (Math.Max(distance, this.SoundMinDistance) - this.SoundMinDistance));
        }
    }
}