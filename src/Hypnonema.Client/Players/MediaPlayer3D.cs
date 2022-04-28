namespace Hypnonema.Client.Players
{
    using System;
    using System.Threading.Tasks;

    using CitizenFX.Core;
    using CitizenFX.Core.Native;

    using Hypnonema.Client.Dui;
    using Hypnonema.Client.Graphics;
    using Hypnonema.Client.Utils;

    using ClientScript = ClientScript;

    public sealed class MediaPlayer3D : MediaPlayerBase, IDisposable
    {
        private ScaleformRenderer scaleform;

        public MediaPlayer3D(
            DuiBrowser duiBrowser,
            ScaleformRenderer scaleform,
            string playerName,
            float globalVolume,
            float soundAttenuation,
            float soundMaxDistance,
            float soundMinDistance)
            : base(duiBrowser, playerName, globalVolume, soundAttenuation, soundMaxDistance, soundMinDistance)
        {
            this.scaleform = scaleform;

            ClientScript.Self.AddTick(this.CalculateVolume);
            ClientScript.Self.AddTick(this.Draw);
        }

        ~MediaPlayer3D()
        {
            this.Dispose();
        }

        public bool IsOutOfRange => this.scaleform.GetDistanceToPlayer() > this.MaxRenderDistance;

        public float MaxRenderDistance { get; set; } = ConfigReader.GetConfigKeyValue(
            API.GetCurrentResourceName(),
            "hypnonema_max_render_distance",
            0,
            400f);

        public override async Task CalculateVolume()
        {
            await BaseScript.Delay(1000);

            var distance = this.scaleform.GetDistanceToPlayer();

            if (distance >= this.SoundMaxDistance || this.IsOutOfRange)
            {
                this.duiBrowser.SetVolume(0f);
                return;
            }

            this.duiBrowser.SetVolume(this.GetSoundFactor(distance) * this.GlobalVolume);
        }

        public new void Dispose()
        {
            base.Dispose();

            ClientScript.Self.RemoveTick(this.CalculateVolume);
            ClientScript.Self.RemoveTick(this.Draw);

            ScaleformRendererPool.Instance.ReleaseScaleformRenderer(this.scaleform);

            GC.SuppressFinalize(this);
        }

        public async Task Draw()
        {
            if (this.IsOutOfRange) return;

            this.scaleform.Draw();
        }
    }
}