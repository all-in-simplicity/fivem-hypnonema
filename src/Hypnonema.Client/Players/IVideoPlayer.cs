namespace Hypnonema.Client.Players
{
    using System;
    using System.Threading.Tasks;

    using Hypnonema.Client.Graphics;

    public interface IVideoPlayer : IDisposable
    {
        DuiBrowser Browser { get; }

        float GlobalVolume { get; set; }

        string ScreenName { get; }

        float SoundAttenuation { get; set; }

        float SoundMinDistance { get; set; }

        void CalculateVolume();

        void Draw();

        Task OnTick();

        void Pause();

        void Play(string url);

        void Resume();

        void Seek(float time);

        void Stop();

        void SynchronizeState(bool paused, float currentTime, string currentSource, bool repeat);

        void ToggleRepeat();
    }
}