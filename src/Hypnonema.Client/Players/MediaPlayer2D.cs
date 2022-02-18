namespace Hypnonema.Client.Players
{
    using System.Threading.Tasks;

    using CitizenFX.Core;
    using CitizenFX.Core.Native;

    using Hypnonema.Client.Dui;
    using Hypnonema.Client.Graphics;

    using ClientScript = Hypnonema.Client.ClientScript;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

    public sealed class MediaPlayer2D : MediaPlayerBase
    {
        private Prop closestObject;

        public MediaPlayer2D(
            RenderTargetRenderer renderer,
            DuiBrowser duiBrowser,
            string playerName,
            float globalVolume,
            float soundAttenuation,
            float soundMaxDistance,
            float soundMinDistance)
            : base(duiBrowser, playerName, globalVolume, soundAttenuation, soundMaxDistance, soundMinDistance)
        {
            this.renderTarget = renderer;

            ClientScript.Self.AddTick(this.CalculateVolume);
            ClientScript.Self.AddTick(this.Draw);
            ClientScript.Self.AddTick(this.CalculateDistance);
        }

        private RenderTargetRenderer renderTarget { get; }

        public async Task CalculateDistance()
        {
            this.closestObject = await this.GetClosestObjectOfType();

            await BaseScript.Delay(950);
        }

        public override async Task CalculateVolume()
        {
            if (this.closestObject == null)
            {
                // no object found, so set distance to maxRenderDistance to mute the player
                this.duiBrowser.SetVolume(0f);
                return;
            }

            var distance = World.GetDistance(Game.PlayerPed.Position, this.closestObject.Position);
            if (distance >= this.SoundMaxDistance)
            {
                this.duiBrowser.SetVolume(0f);
            }
            else
            {
                if (this.closestObject.IsOccluded)
                {
                    this.duiBrowser.SetVolume(this.GetSoundFactor(distance) * this.GlobalVolume / 2);
                }
                else
                {
                    this.duiBrowser.SetVolume(this.GetSoundFactor(distance) * this.GlobalVolume);
                }
            }

            await BaseScript.Delay(300);
        }

        public async Task Draw()
        {
            if (this.closestObject == null)
            {
                return;
            }

            this.renderTarget.Draw();
        }

        private async Task<Prop> GetClosestObjectOfType()
        {
            var objects = API.GetGamePool("CObject");
            foreach (int obj in objects)
            {
                var entity = API.GetEntityModel(obj);

                if (entity == this.renderTarget.Hash) return new Prop(obj);
            }

            return null;
        }
    }
}