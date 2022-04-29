namespace Hypnonema.Client.Players
{
    using System;
    using System.Threading.Tasks;

    using CitizenFX.Core;

    using Hypnonema.Client.Dui;
    using Hypnonema.Client.Graphics;
    using Hypnonema.Shared.Models;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public sealed class MediaPlayer3D : MediaPlayerBase, IDisposable
    {
        private readonly ScaleformRenderer scaleform;

        public MediaPlayer3D(
            DuiBrowser duiBrowser,
            ScaleformRenderer scaleform,
            Screen screen,
            float globalVolume,
            float soundAttenuation,
            float soundMaxDistance,
            float soundMinDistance)
            : base(duiBrowser, screen, globalVolume, soundAttenuation, soundMaxDistance, soundMinDistance)
        {
            this.scaleform = scaleform;
        }

        ~MediaPlayer3D()
        {
            this.Dispose();
        }

        public override async Task CalculateVolume()
        {
            this.duiBrowser.SetVolume(this.GetSoundFactor() * this.GlobalVolume);

            await BaseScript.Delay(2300);
        }

        public new void Dispose()
        {
            base.Dispose();

            ScaleformRendererPool.Instance.ReleaseScaleformRenderer(this.scaleform);

            GC.SuppressFinalize(this);
        }

        public override async Task Draw()
        {
            this.scaleform.Draw();
        }
    }
}