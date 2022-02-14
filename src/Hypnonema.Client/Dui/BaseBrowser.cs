namespace Hypnonema.Client.Dui
{
    using System;

    using CitizenFX.Core.Native;

    using Newtonsoft.Json;

    public abstract class BaseBrowser : IDisposable
    {
        protected BaseBrowser(string url, int width = 1280, int height = 720)
        {
            this.Url = url;
            this.Width = width;
            this.Height = height;

            this.NativeValue = API.CreateDui(url, this.Width, this.Height);
            this.DuiHandle = API.GetDuiHandle(this.NativeValue);
        }

        ~BaseBrowser()
        {
            this.Dispose();
        }

        public string DuiHandle { get; }

        public int Height { get; }

        public bool IsDuiAvailable => API.IsDuiAvailable(this.NativeValue);

        public bool IsRuntimeTextureCreated => this.RuntimeTextureHandle != 0;

        public bool IsValid => this.NativeValue != 0;

        public long RuntimeTextureHandle { get; private set; }

        public long Txd { get; private set; }

        public string TxdName { get; private set; }

        public string TxnName { get; private set; }

        public string Url { get; }

        public int Width { get; }

        private long NativeValue { get; }

        public long CreateRuntimeTexture()
        {
            if (this.IsRuntimeTextureCreated)
            {
                return this.RuntimeTextureHandle;
            }

            this.TxdName = $"hy_txd_{this.DuiHandle}";
            this.TxnName = "video";
            this.Txd = API.CreateRuntimeTxd(this.TxdName);

            this.RuntimeTextureHandle = API.CreateRuntimeTextureFromDuiHandle(this.Txd, this.TxnName, this.DuiHandle);

            return this.RuntimeTextureHandle;
        }

        public void Delete()
        {
            if (this.Exists()) API.DestroyDui(this.NativeValue);
        }

        public void Dispose()
        {
            this.Delete();

            GC.SuppressFinalize(this);
        }

        public bool Exists()
        {
            return this.IsValid;
        }

        protected void SendMessage(string type, object payload = null)
        {
            var message = new { type, payload = new { payload } };

            API.SendDuiMessage(this.NativeValue, JsonConvert.SerializeObject(message));
        }
    }
}