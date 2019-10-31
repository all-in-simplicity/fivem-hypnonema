namespace Hypnonema.Client.Graphics
{
    using System;

    using CitizenFX.Core;

    public class TextureRenderer : IDisposable
    {
        private readonly Scaleform scaleform;

        public TextureRenderer(Scaleform scaleform, int id, Vector3 position, Vector3 rotation, Vector3 scale)
        {
            this.scaleform = scaleform;
            this.Id = id;
            this.Position = position;
            this.Rotation = rotation;
            this.Scale = scale;
        }

        ~TextureRenderer()
        {
            this.Dispose();
        }

        public int Id { get; set; }

        public bool IsTextureSet { get; protected set; }

        public Vector3 Position { get; set; }

        public Vector3 Rotation { get; set; }

        public Vector3 Scale { get; set; }

        public void Dispose()
        {
            if (this.scaleform.IsLoaded) this.scaleform.Dispose();

            GC.SuppressFinalize(this);
        }

        public float GetDistanceToPlayer()
        {
            return World.GetDistance(this.Position, Game.PlayerPed.Position);
        }

        public void Render3D()
        {
            if (this.scaleform.IsLoaded && this.scaleform.IsValid && this.IsTextureSet)
                this.scaleform.Render3D(this.Position, this.Rotation, this.Scale);
        }

        public void SetTexture(
            string txdName,
            string txnName,
            int width = 1280,
            int height = 720,
            int positionX = 0,
            int positionY = 0)
        {
            if (this.IsTextureSet)
            {
                Debug.WriteLine("Warning: Texture for TextureRenderer already set");
                return;
            }

            this.scaleform.CallFunction("SET_TEXTURE", txdName, txnName, positionX, positionY, width, height);
            this.IsTextureSet = true;
        }
    }
}