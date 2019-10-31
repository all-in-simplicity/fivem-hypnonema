namespace Hypnonema.Client.Graphics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CitizenFX.Core;

    public class TextureRendererPool
    {
        public const string ScaleformName = "hypnonema_texture_renderer";

        private static readonly Lazy<TextureRendererPool> lazy =
            new Lazy<TextureRendererPool>(() => new TextureRendererPool());

        private TextureRendererPool()
        {
        }

        public static TextureRendererPool Instance => lazy.Value;

        public static int MaxActiveScaleforms { get; set; } = 1;

        public IList<TextureRenderer> TextureRenderers { get; protected set; } = new List<TextureRenderer>();

        public async Task<TextureRenderer> CreateTextureRenderer(Vector3 position, Vector3 rotation, Vector3 scale)
        {
            if (this.TextureRenderers.Count >= MaxActiveScaleforms)
            {
                Debug.WriteLine($"Warning: Maximal amount of active scaleforms ({MaxActiveScaleforms}) exceeded.");
                return null;
            }

            var length = this.TextureRenderers.Count;
            var scaleformName = $"{ScaleformName}{length + 1:D2}";
            var scaleform = await this.LoadScaleform(scaleformName, 8000);
            if (scaleform == null)
            {
                Debug.WriteLine($"Warning: Attempt to load scaleform \"{scaleformName}\" exceeded 8000 ms. Aborting.");
                return null;
            }

            var renderer = new TextureRenderer(scaleform, length + 1, position, rotation, scale);
            this.TextureRenderers.Add(renderer);
            return renderer;
        }

        public void ReleaseTextureRenderer(int id)
        {
            var renderer = this.TextureRenderers.FirstOrDefault(r => r.Id == id);
            this.TextureRenderers.Remove(renderer);

            try
            {
                renderer.Dispose();
            }
            catch (Exception)
            {
            }
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
    }
}