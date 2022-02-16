namespace Hypnonema.Client.Players
{
    using System;

    public interface IVideoPlayer : IDisposable
    {
        string ScreenName { get; }
        
        void Pause();

        void Play(string url);

        void Resume();

        void Seek(float time);

        void Stop();

        void SynchronizeState(bool paused, float currentTime, string currentSource, bool repeat);

        void ToggleRepeat();
    }
}