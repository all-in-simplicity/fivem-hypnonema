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

        public RenderTarget RenderTarget { get; }

        public string ScreenName { get; }

        public float SoundAttenuation { get; set; } = 5f;

        public float SoundMaxDistance { get; set; } = 300f;

        public float SoundMinDistance { get; set; } = 10f;

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

            var entity = GetClosestObjectOfType(this.SoundMaxDistance, (uint)this.RenderTarget.Hash);
            var distance = World.GetDistance(Game.PlayerPed.Position, entity.Position);

            if (distance >= this.SoundMaxDistance || entity == null)
            {
                this.Browser.SendVolume(0f);
            }
            else
            {
                if (entity.IsOccluded)
                {
                    this.Browser.SendVolume(this.GlobalVolume / 2);
                }
                else
                {
                    this.Browser.SendVolume(this.GlobalVolume);
                }
                
                var tickData = new AudioTickData
                                   {
                                       ListenerForward = Game.PlayerPed.ForwardVector,
                                       ListenerUp = Game.PlayerPed.UpVector,
                                       PositionListener = Game.PlayerPed.Position,
                                       PositionPanner = entity.Position - Game.PlayerPed.Position, // relative to player
                                       OrientationPanner = entity.ForwardVector
                                   };

                this.Browser.SendTick(tickData);
            }

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
    }
}