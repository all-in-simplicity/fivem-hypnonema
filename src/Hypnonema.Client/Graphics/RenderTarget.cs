namespace Hypnonema.Client.Graphics
{
    using System;

    using CitizenFX.Core.Native;

    public class RenderTarget : IDisposable
    {
        public RenderTarget(string objectName, string targetName)
        {
            this.ObjectName = objectName;
            this.TargetName = targetName;
            this.TargetHandle = CreateNamedRenderTargetForModel(targetName, this.Hash);
        }

        ~RenderTarget()
        {
            this.Dispose();
        }

        public int Hash => API.GetHashKey(this.ObjectName);

        public bool IsValid => this.TargetHandle != 0;

        public string ObjectName { get; protected set; }

        public int TargetHandle { get; protected set; }

        public string TargetName { get; protected set; }

        public void Dispose()
        {
            if (this.IsValid) API.ReleaseNamedRendertarget(this.TargetName);

            GC.SuppressFinalize(this);
        }

        public void Draw(string txdName, string txnName)
        {
            API.SetTextRenderId(this.TargetHandle);
            API.Set_2dLayer(4);
            API.SetScriptGfxDrawBehindPausemenu(true);

            API.DrawSprite(txdName, txnName, 0.5f, 0.5f, 1.0f, 1.0f, 0.0f, 255, 255, 255, 255);
            API.SetTextRenderId(API.GetDefaultScriptRendertargetRenderId());
            API.SetScriptGfxDrawBehindPausemenu(false);
        }

        private static int CreateNamedRenderTargetForModel(string targetName, int hash)
        {
            var handle = 0;
            if (!API.IsNamedRendertargetRegistered(targetName)) API.RegisterNamedRendertarget(targetName, false);

            if (!API.IsNamedRendertargetLinked((uint)hash)) API.LinkNamedRendertarget((uint)hash);

            if (API.IsNamedRendertargetRegistered(targetName)) handle = API.GetNamedRendertargetRenderId(targetName);

            return handle;
        }
    }
}