namespace Hypnonema.Client.Players
{
    using System;
    using System.Threading.Tasks;

    using Hypnonema.Client.Dui;
    using Hypnonema.Shared;
    using Hypnonema.Shared.Models;

    public abstract class MediaPlayerBase : IMediaPlayer
    {
        protected DuiBrowser duiBrowser;

        protected MediaPlayerBase(
            DuiBrowser duiBrowser,
            string playerName,
            float globalVolume,
            float soundAttenuation,
            float soundMaxDistance,
            float soundMinDistance)
        {
            this.duiBrowser = duiBrowser;

            this.PlayerName = playerName;

            this.GlobalVolume = globalVolume;
            this.SoundAttenuation = soundAttenuation;
            this.SoundMaxDistance = soundMaxDistance;
            this.SoundMinDistance = soundMinDistance;

            ClientScript.Self.AddEvent(Events.ClientVolume, new Action<float>(this.OnClientVolumeChange));
        }

        ~MediaPlayerBase()
        {
            this.Dispose();
        }

        public float GlobalVolume { get; set; } = 100f;

        public string PlayerName { get; protected set; }

        public Screen Screen { get; set; }

        public float SoundAttenuation { get; set; } = 5f;

        public float SoundMaxDistance { get; set; } = 300f;

        public float SoundMinDistance { get; set; } = 10f;

        public DuiState State { get; set; }

        public abstract Task CalculateVolume();

        public void Dispose()
        {
            if (this.duiBrowser.IsValid) DuiBrowserPool.Instance.ReleaseDuiBrowser(this.duiBrowser);

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

        protected float GetSoundFactor(float distance)
        {
            return this.SoundMinDistance / (this.SoundMinDistance + this.SoundAttenuation
                                            * (Math.Max(distance, this.SoundMinDistance) - this.SoundMinDistance));
        }

        private void OnClientVolumeChange(float volume)
        {
            ClientScript.AddChatMessage($"Setting volume to: {volume}");

            this.GlobalVolume = volume;
        }
    }
}