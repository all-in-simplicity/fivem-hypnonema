namespace Hypnonema.Client.Players
{
    using System;
    using System.Threading.Tasks;

    using CitizenFX.Core;
    using CitizenFX.Core.Native;

    using Hypnonema.Client.Dui;
    using Hypnonema.Client.Utils;
    using Hypnonema.Shared;
    using Hypnonema.Shared.Models;

    using ClientScript = Hypnonema.Client.ClientScript;
    
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

    public abstract class MediaPlayerBase : IMediaPlayer
    {
        protected DuiBrowser duiBrowser;

        protected MediaPlayerBase(
            DuiBrowser duiBrowser,
            Screen screen,
            float globalVolume,
            float soundAttenuation,
            float soundMaxDistance,
            float soundMinDistance)
        {
            this.duiBrowser = duiBrowser;

            this.Screen = screen;

            this.GlobalVolume = globalVolume;
            this.SoundAttenuation = soundAttenuation;
            this.SoundMaxDistance = soundMaxDistance;
            this.SoundMinDistance = soundMinDistance;

            ClientScript.Self.AddEvent(Events.ClientVolume, new Action<float>(this.OnClientVolumeChange));
            ClientScript.Self.AddTick(this.CalculateDistance);
        }

        ~MediaPlayerBase()
        {
            this.Dispose();
        }

        public float GlobalVolume { get; set; } = 100f;

        public string PlayerName => this.Screen?.Name;

        public Screen Screen { get; set; }

        public float SoundAttenuation { get; set; } = 5f;

        public float SoundMaxDistance { get; set; } = 300f;

        public float SoundMinDistance { get; set; } = 10f;
        
        protected bool IsOccluded { get; set; }

        private bool IsDrawTickRegistered { get; set; }

        private bool IsCalculateVolumeTickRegistered { get; set; }

        public float DistanceToPlayer { get; protected set; }

        private async Task CalculateDistance()
        {
            if (this.Screen.Is3DRendered)
            {
                var screenPosition = new Vector3(
                    this.Screen.PositionalSettings.PositionX,
                    this.Screen.PositionalSettings.PositionY,
                    this.Screen.PositionalSettings.PositionZ);

                this.DistanceToPlayer = World.GetDistance(screenPosition, Game.PlayerPed.Position);
            }
            else
            {
                var hash = API.GetHashKey(this.Screen.TargetSettings.ModelName);

                var closestObject = await GetClosestObjectOfType(hash);
                
                this.IsOccluded = closestObject != null && closestObject.IsOccluded;

                this.DistanceToPlayer = closestObject == null ? this.MaxRenderDistance : World.GetDistance(closestObject.Position, Game.PlayerPed.Position);
            }

            if (this.DistanceToPlayer < this.MaxRenderDistance)
            {
                if (!this.IsCalculateVolumeTickRegistered)
                {
                    ClientScript.Self.AddTick(this.CalculateVolume);
                    this.IsCalculateVolumeTickRegistered = true;
                }

                if (!this.IsDrawTickRegistered)
                {
                    ClientScript.Self.AddTick(this.Draw);
                    this.IsDrawTickRegistered = true;
                }
            }
            else
            {
                if (this.IsCalculateVolumeTickRegistered)
                {
                    ClientScript.Self.RemoveTick(this.CalculateVolume);
                    this.IsCalculateVolumeTickRegistered = false;
                }

                if (this.IsDrawTickRegistered)
                {
                    ClientScript.Self.RemoveTick(this.Draw);
                    this.IsDrawTickRegistered = false;
                }

                this.duiBrowser.SetVolume(0f);
            }

            await BaseScript.Delay(2000);
        }

        public static async Task<Prop> GetClosestObjectOfType(int hash)
        {
            var objects = API.GetGamePool("CObject");
            foreach (int obj in objects)
            {
                var entity = API.GetEntityModel(obj);

                if (entity == hash) return new Prop(obj);
            }

            return null;
        }

        public float MaxRenderDistance { get; set; } = ConfigReader.GetConfigKeyValue(
            API.GetCurrentResourceName(),
            "hypnonema_max_render_distance",
            0,
            400f);

        public abstract Task CalculateVolume();

        public abstract Task Draw();

        public void Dispose()
        {
            if (this.duiBrowser.IsValid) DuiBrowserPool.Instance.ReleaseDuiBrowser(this.duiBrowser);

            if (this.IsCalculateVolumeTickRegistered)
            {
                ClientScript.Self.RemoveTick(this.CalculateVolume);
            }

            if (this.IsDrawTickRegistered)
            {
                ClientScript.Self.RemoveTick(this.Draw);
            }

            GC.SuppressFinalize(this);
        }

        public void InitDuiBrowser()
        {
            this.duiBrowser.Init();
        }

        public void Pause()
        {
            this.duiBrowser.Pause();
        }

        public void Play(string url)
        {
            this.duiBrowser.Play(url);
        }

        public void Repeat(bool repeat)
        {
            this.duiBrowser.Repeat(repeat);
        }

        public void Resume()
        {
            this.duiBrowser.Resume();
        }

        public void Seek(float time)
        {
            this.duiBrowser.Seek(time);
        }

        public void Stop()
        {
            this.duiBrowser.Stop();
        }

        public void SynchronizeState(bool paused, float currentTime, string currentSource, bool looped)
        {
            this.duiBrowser.SynchronizeState(paused, currentTime, currentSource, looped);
        }

        protected float GetSoundFactor()
        {
            return this.SoundMinDistance / (this.SoundMinDistance + this.SoundAttenuation
                                            * (Math.Max(this.DistanceToPlayer, this.SoundMinDistance) - this.SoundMinDistance));
        }

        private void OnClientVolumeChange(float volume)
        {
            ClientScript.AddChatMessage($"Setting volume to: {volume}");

            this.GlobalVolume = volume;
        }
    }
}