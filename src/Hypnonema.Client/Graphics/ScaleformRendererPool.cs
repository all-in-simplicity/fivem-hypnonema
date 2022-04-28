namespace Hypnonema.Client.Graphics
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using CitizenFX.Core;

    using Hypnonema.Client.Communications;
    using Hypnonema.Client.Utils;
    using Hypnonema.Shared;
    using Hypnonema.Shared.Models;

    public class ScaleformRendererPool : IDisposable
    {
        private static ScaleformRendererPool instance;

        private readonly NetworkMethod<int> getMaxActiveScaleforms;

        private readonly Stack<ScaleformRenderer> scaleformRenderers = new Stack<ScaleformRenderer>();

        private int maxActiveScaleforms = 1;

        private int usageCount;

        private ScaleformRendererPool()
        {
            this.getMaxActiveScaleforms = new NetworkMethod<int>(
                Events.GetMaxActiveScaleforms,
                this.OnGetMaxActiveScaleforms);

            this.getMaxActiveScaleforms.Invoke(0);
        }

        ~ScaleformRendererPool()
        {
            this.Dispose();
        }

        public static ScaleformRendererPool Instance => instance ?? (instance = new ScaleformRendererPool());

        public async Task<ScaleformRenderer> AcquireScaleformRenderer(
            Screen.PositionSettings positionalSettings,
            string txdName,
            string txnName)
        {
            if (this.usageCount >= this.maxActiveScaleforms)
            {
                Logger.Error($"scaleformRenderers exceeded max usage limit of {this.maxActiveScaleforms}");
                return null;
            }

            ScaleformRenderer renderer;

            try
            {
                renderer = this.scaleformRenderers.Pop();

                renderer.UnsetTexture();
                renderer.SetTexture(txdName, txnName);

                renderer.SetPosition(positionalSettings);
            }
            catch (Exception)
            {
                // no scaleformRenderer in stack. create one
                renderer = await this.CreateScaleformRenderer(positionalSettings, txdName, txnName);
            }

            this.usageCount += 1;

            return renderer;
        }

        public void Dispose()
        {
            foreach (var renderer in this.scaleformRenderers) renderer.Dispose();

            GC.SuppressFinalize(this);
        }

        public void ReleaseScaleformRenderer(ScaleformRenderer renderer)
        {
            this.scaleformRenderers.Push(renderer);

            this.usageCount -= 1;
        }

        private async Task<ScaleformRenderer> CreateScaleformRenderer(
            Screen.PositionSettings positionalSettings,
            string txdName,
            string txnName)
        {
            var scaleformId = $"hypnonema_texture_renderer{this.usageCount + 1:D2}";
            var scaleform = await this.LoadScaleform(scaleformId, 3000);

            if (scaleform != null) return new ScaleformRenderer(scaleform, positionalSettings, txdName, txnName);

            Logger.Error($"Failed to load scaleform. Timeout exceeded. Scaleform-ID: \"{scaleformId}\"");
            return null;
        }

        private async Task<Scaleform> LoadScaleform(string scaleformId, int timeout)
        {
            var endTime = DateTime.UtcNow + new TimeSpan(0, 0, 0, 0, timeout);
            var scaleform = new Scaleform(scaleformId);

            while (!scaleform.IsLoaded)
            {
                await BaseScript.Delay(5);

                if (DateTime.UtcNow >= endTime) return null;
            }

            return scaleform;
        }

        private void OnGetMaxActiveScaleforms(int amount)
        {
            this.maxActiveScaleforms = amount;
        }
    }
}