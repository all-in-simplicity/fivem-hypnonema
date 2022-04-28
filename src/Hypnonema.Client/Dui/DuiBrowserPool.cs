namespace Hypnonema.Client.Dui
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Hypnonema.Shared.Models;

    public class DuiBrowserPool : IDisposable
    {
        private static DuiBrowserPool instance;

        private readonly Stack<DuiBrowser> duiBrowsers = new Stack<DuiBrowser>();

        private DuiBrowserPool()
        {
        }

        ~DuiBrowserPool()
        {
            this.Dispose();
        }

        public static DuiBrowserPool Instance => instance ?? (instance = new DuiBrowserPool());

        public async Task<DuiBrowser> AcquireDuiBrowser(Screen screen, int width = 1280, int height = 720)
        {
            DuiBrowser browser;

            try
            {
                browser = this.duiBrowsers.Pop();
            }
            catch (Exception)
            {
                // no browser left. create one
                browser = await DuiBrowser.CreateDuiBrowser(screen.Name, width, height);
                browser.Init();
                return browser;
            }

            browser.ScreenName = screen.Name;
            browser.Init();

            return browser;
        }

        public void Dispose()
        {
            foreach (var duiBrowser in this.duiBrowsers) duiBrowser.Dispose();

            GC.SuppressFinalize(this);
        }

        public void ReleaseDuiBrowser(DuiBrowser browser)
        {
            this.duiBrowsers.Push(browser);
        }
    }
}