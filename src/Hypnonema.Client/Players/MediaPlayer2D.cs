namespace Hypnonema.Client.Players
{
    using System.Threading.Tasks;

    using CitizenFX.Core;

    using Hypnonema.Client.Dui;
    using Hypnonema.Client.Graphics;
    using Hypnonema.Shared.Models;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

    public sealed class MediaPlayer2D : MediaPlayerBase
    {
        public MediaPlayer2D(
            RenderTargetRenderer renderer,
            DuiBrowser duiBrowser,
            Screen screen,
            float globalVolume,
            float soundAttenuation,
            float soundMaxDistance,
            float soundMinDistance)
            : base(duiBrowser, screen, globalVolume, soundAttenuation, soundMaxDistance, soundMinDistance)
        {
            this.renderTarget = renderer;
        }

        private RenderTargetRenderer renderTarget { get; }

        public override async Task CalculateVolume()
        {
            if (this.IsOccluded)
            {
                this.duiBrowser.SetVolume((this.GetSoundFactor() / 2) * this.GlobalVolume);
            }
            else
            {
                this.duiBrowser.SetVolume(this.GetSoundFactor() * this.GlobalVolume);
            }

            await BaseScript.Delay(2200);
        }

        public override async Task Draw()
        {
            this.renderTarget.Draw();
        }
    }
}