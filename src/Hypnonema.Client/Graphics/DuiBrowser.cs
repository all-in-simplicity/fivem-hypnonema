namespace Hypnonema.Client.Graphics
{
    using System;

    using CitizenFX.Core.Native;

    using Newtonsoft.Json;

    public sealed class DuiBrowser : IDisposable
    {
        private long runtimeTextureHandle;

        public DuiBrowser(string url, int width = 1280, int height = 720)
        {
            this.Url = url;
            this.Width = width;
            this.Height = height;

            this.NativeValue = API.CreateDui(url, this.Width, this.Height);
            this.Handle = API.GetDuiHandle(this.NativeValue);
        }

        ~DuiBrowser()
        {
            this.Dispose();
        }

        public string Handle { get; }

        public int Height { get; }

        public bool IsValid => this.NativeValue != 0;

        public long NativeValue { get; }

        public long Txd { get; private set; }

        public string TxdName { get; private set; }

        public string TxnName { get; private set; }

        public string Url { get; }

        public int Width { get; }

        public long CreateRuntimeTexture()
        {
            this.TxdName = Guid.NewGuid().ToString();
            this.TxnName = "hy_txn";
            this.Txd = API.CreateRuntimeTxd(this.TxdName);

            this.runtimeTextureHandle = Function.Call<long>(
                Hash.CREATE_RUNTIME_TEXTURE_FROM_DUI_HANDLE,
                this.Txd,
                this.TxnName,
                this.Handle);

            return this.runtimeTextureHandle;
        }

        public void Dispose()
        {
            if (this.IsValid) API.DestroyDui(this.NativeValue);

            GC.SuppressFinalize(this);
        }

        public void SendGetState()
        {
            this.SendMessage(new { type = "getState" });
        }

        public void SendInit(string screenName, string posterUrl)
        {
            this.SendMessage(new { type = "init", screenName, posterUrl });
        }

        public void SendMessage(object obj)
        {
            API.SendDuiMessage(this.NativeValue, JsonConvert.SerializeObject(obj));
        }

        public void SendPause()
        {
            this.SendMessage(new { type = "pause" });
        }

        public void SendPlay(string url)
        {
            this.SendMessage(new { type = "play", src = new { url } });
        }

        public void SendResume()
        {
            this.SendMessage(new { type = "resume" });
        }

        public void SendSeek(float time)
        {
            this.SendMessage(new { type = "seek", time });
        }

        public void SendStop()
        {
            this.SendMessage(new { type = "stop" });
        }

        public void SendTick(AudioTickData tickData)
        {
            this.SendMessage(
                new
                    {
                        type = "tick",
                        listenerObj =
                            new
                                {
                                    positionX = tickData.PositionListener.X,
                                    positionY = tickData.PositionListener.Y,
                                    positionZ = tickData.PositionListener.Z,
                                    forwardX = tickData.ListenerForward.X,
                                    forwardY = tickData.ListenerForward.Y,
                                    forwardZ = tickData.ListenerForward.Z,
                                    upX = tickData.ListenerUp.X,
                                    upY = tickData.ListenerUp.Y,
                                    upZ = tickData.ListenerUp.Z
                                },
                        pannerObj = new
                                        {
                                            positionX = tickData.PositionPanner.X,
                                            positionY = tickData.PositionPanner.Y,
                                            positionZ = tickData.PositionPanner.Z,
                                            orientationX = tickData.OrientationPanner.X,
                                            orientationY = tickData.OrientationPanner.Y,
                                            orientationZ = tickData.OrientationPanner.Z
                                        }
                    });
        }

        public void SendToggleReplay(bool toggle)
        {
            this.SendMessage(new { type = "toggleReplay", value = toggle });
        }

        public void SendUpdate(bool paused, float currentTime, string currentSource)
        {
            this.SendMessage(
                new { type = "update", paused, currentTime, src = currentSource });
        }

        public void SendVolume(float volume)
        {
            this.SendMessage(new { type = "volume", volume = volume / 100 });
        }
    }
}