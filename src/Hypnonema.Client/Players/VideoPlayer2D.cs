namespace Hypnonema.Client.Players
{
    using System;
    using System.Threading.Tasks;

    using CitizenFX.Core;
    using CitizenFX.Core.Native;

    using Hypnonema.Client.Graphics;

    public sealed class VideoPlayer2D : IVideoPlayer
    {
        public VideoPlayer2D(DuiBrowser browser, RenderTarget renderTarget, string screenName)
        {
            this.Browser = browser;
            this.RenderTarget = renderTarget;
            this.ScreenName = screenName;
        }

        ~VideoPlayer2D()
        {
            this.Dispose();
        }

        public DuiBrowser Browser { get; }

        public float GlobalVolume { get; set; } = 100f;

        public bool Is3DAudioEnabled { get; set; } = true;

        public RenderTarget RenderTarget { get; }

        public string ScreenName { get; }

        public float SoundAttenuation { get; set; } = 5f;

        public float SoundMaxDistance { get; set; } = 300f;

        public float SoundMinDistance { get; set; } = 10f;

        public void CalculateVolume()
        {
            var entity = GetClosestObjectOfType(this.SoundMaxDistance, (uint)this.RenderTarget.Hash);
            if (entity == null)
            {
                this.Browser.SetVolume(0f);
                return;
            }

            var distance = World.GetDistance(Game.PlayerPed.Position, entity.Position);

            if (distance >= this.SoundMaxDistance)
            {
                this.Browser.SetVolume(0f);
            }
            else
            {
                if (entity.IsOccluded) this.Browser.SetVolume(this.GlobalVolume / 2);
                else this.Browser.SetVolume(this.GlobalVolume);

                if (this.Is3DAudioEnabled)
                {
                    var tickData = new AudioTickData
                                       {
                                           ListenerForward = Game.PlayerPed.ForwardVector,
                                           ListenerUp = Game.PlayerPed.UpVector,
                                           PositionListener = Game.PlayerPed.Position,
                                           PositionPanner =
                                               entity.Position - Game.PlayerPed.Position, // relative to player
                                           OrientationPanner = entity.ForwardVector
                                       };

                    this.Browser.Tick(tickData);
                }
                else
                {
                    this.Browser.SetVolume(this.GetSoundFactor(distance) * this.GlobalVolume);
                }
            }
        }

        public void Dispose()
        {
            if (this.Browser.IsValid) this.Browser.Dispose();

            if (this.RenderTarget.IsValid) this.RenderTarget.Dispose();

            GC.SuppressFinalize(this);
        }

        public void Draw()
        {
            this.RenderTarget.Draw(this.Browser.TxdName, this.Browser.TxnName);
        }

        public async Task OnTick()
        {
            this.Draw();

            this.CalculateVolume();

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

        private static Prop GetClosestObjectOfType(float radius, uint modelHash)
        {
            var playerPos = Game.PlayerPed.Position;
            var entity = API.GetClosestObjectOfType(
                playerPos.X,
                playerPos.Y,
                playerPos.Z,
                radius,
                modelHash,
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