namespace Hypnonema.Client.Dui
{
    using System;
    using System.Threading.Tasks;

    using CitizenFX.Core;
    using CitizenFX.Core.Native;

    using Hypnonema.Client.Utils;

    public class DuiBrowser : BaseBrowser, IDisposable
    {
        private DuiBrowser(string screenName, string posterUrl, string url, int width = 1280, int height = 720)
            : base(url, width, height)
        {
            this.ScreenName = screenName;
            this.PosterUrl = posterUrl;
        }

        public string PosterUrl { get; set; }

        public string ScreenName { get; set; }

        public static async Task<DuiBrowser> CreateDuiBrowser(string screenName, int width = 1280, int height = 720)
        {
            var resourceName = API.GetCurrentResourceName();

            var url = ConfigReader.GetConfigKeyValue(
                resourceName,
                "hypnonema_url",
                0,
                "https://thiago-dev.github.io/fivem-hypnonema");

            var posterUrl = ConfigReader.GetConfigKeyValue(
                resourceName,
                "hypnonema_poster_url",
                0,
                "https://thiago-dev.github.io/fivem-hypnonema");

            var duiBrowser = new DuiBrowser(screenName, posterUrl, url, width, height);

            while (!duiBrowser.IsDuiAvailable) await BaseScript.Delay(250);

            duiBrowser.CreateRuntimeTexture();

            // wait again for dui browser to be fully initialized
            // TODO: Is this additional delay necessary?
            await BaseScript.Delay(650);

            return duiBrowser;
        }

        public new void Dispose()
        {
            base.Dispose();
        }

        public void Init()
        {
            var resourceName = API.GetCurrentResourceName();

            var payload = new {this.ScreenName, this.PosterUrl, resourceName};

            this.SendMessage("init", payload);
        }

        public void Pause()
        {
            this.SendMessage("pause");
        }

        public void Play(string url)
        {
            this.SendMessage("play", url);
        }

        public void Repeat(bool repeat)
        {
            this.SendMessage("repeat", repeat);
        }

        public void Resume()
        {
            this.SendMessage("resume");
        }

        public void Seek(float time)
        {
            this.SendMessage("seek", time);
        }

        public void SetVolume(float volume)
        {
            this.SendMessage("volume", volume / 100);
        }

        public void Stop()
        {
            this.SendMessage("stop");
        }

        public void SynchronizeState(bool paused, float currentTime, string url, bool repeat)
        {
            var payload = new {paused, currentTime, url, repeat};

            this.SendMessage("synchronizeState", payload);
        }
    }
}