namespace Hypnonema.Client.Graphics
{
    using System;

    using CitizenFX.Core.Native;

    public class RenderTargetRenderer : IDisposable
    {
        public RenderTargetRenderer(string objectName, string targetName, string txdName, string txnName)
        {
            this.ObjectName = objectName;
            this.TargetName = targetName;
            this.Hash = API.GetHashKey(this.ObjectName);
            this.TargetHandle = CreateNamedRenderTargetForModel(targetName, this.Hash);

            this.TxdName = txdName;
            this.TxnName = txnName;
        }

        ~RenderTargetRenderer()
        {
            this.Dispose();
        }

        public int Hash { get; protected set; }

        public bool IsValid => this.TargetHandle != 0;

        public string ObjectName { get; protected set; }

        public int TargetHandle { get; protected set; }

        public string TargetName { get; protected set; }

        public string TxdName { get; protected set; }

        public string TxnName { get; protected set; }

        public void Dispose()
        {
            if (this.IsValid) API.ReleaseNamedRendertarget(this.TargetName);

            GC.SuppressFinalize(this);
        }

        public void Draw()
        {
            API.SetTextRenderId(this.TargetHandle);
            API.Set_2dLayer(4);
            API.SetScriptGfxDrawBehindPausemenu(true);

            API.DrawSprite(this.TxdName, this.TxnName, 0.5f, 0.5f, 1.0f, 1.0f, 0.0f, 255, 255, 255, 255);
            API.SetTextRenderId(API.GetDefaultScriptRendertargetRenderId());
            API.SetScriptGfxDrawBehindPausemenu(false);
        }

        private static int CreateNamedRenderTargetForModel(string targetName, int hash)
        {
            var handle = 0;
            if (!API.IsNamedRendertargetRegistered(targetName)) API.RegisterNamedRendertarget(targetName, false);

            if (!API.IsNamedRendertargetLinked((uint) hash)) API.LinkNamedRendertarget((uint) hash);

            if (API.IsNamedRendertargetRegistered(targetName)) handle = API.GetNamedRendertargetRenderId(targetName);

            return handle;
        }
    }
}