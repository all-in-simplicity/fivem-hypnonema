namespace Hypnonema.Client.Dui
{
    using System;
    using System.Threading.Tasks;

    using CitizenFX.Core;
    using CitizenFX.Core.Native;

    using Hypnonema.Client.Utils;
    using Hypnonema.Shared.Models;

    public class DuiBrowser : BaseBrowser, IDisposable
    {
        private DuiBrowser(string url, int width = 1280, int height = 720)
            : base(url, width, height)
        {
        }

        public static async Task<DuiBrowser> CreateDuiBrowser(Screen screen, int width = 1280, int height = 720)
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
                "https://i.imgur.com/dPaIjEW.jpg");

            var duiBrowser = new DuiBrowser(url, width, height);

            while (!duiBrowser.IsDuiAvailable)
            {
                await BaseScript.Delay(250);
            }

            duiBrowser.CreateRuntimeTexture();

            // wait again for dui browser to be fully initialized
            // TODO: Is this additional delay necessary?
            await BaseScript.Delay(650);

            duiBrowser.Init(screen.Name, posterUrl, resourceName);

            return duiBrowser;
        }

        public new void Dispose()
        {
            base.Dispose();
        }

        public void Init(string screenName, string posterUrl, string resourceName)
        {
            var payload = new { screenName, posterUrl, resourceName };
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

        public void ShowPlayer(bool show)
        {
            this.SendMessage("showPlayer", show);
        }

        public void Stop()
        {
            this.SendMessage("stop");
        }

        public void SynchronizeState(bool paused, float currentTime, string url, bool looped)
        {
            var payload = new { paused, currentTime, url, looped };
            this.SendMessage("synchronizeState", payload);
        }

        public void ToggleRepeat()
        {
            this.SendMessage("loop");
        }
    }
}