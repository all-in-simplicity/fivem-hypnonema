namespace Hypnonema.Client.Graphics
{
    using System;

    using CitizenFX.Core;

    using Hypnonema.Client.Utils;
    using Hypnonema.Shared.Models;

    public class ScaleformRenderer : IDisposable
    {
        private readonly Scaleform scaleform;

        public ScaleformRenderer(
            Scaleform scaleform,
            Screen.PositionSettings positionalSettings,
            string txdName,
            string txnName)
        {
            this.scaleform = scaleform;

            this.SetPosition(positionalSettings);

            this.SetTexture(txdName, txnName);
        }

        ~ScaleformRenderer()
        {
            this.Dispose();
        }

        public bool IsTextureSet { get; protected set; }

        public Vector3 Position { get; set; }

        public Vector3 Rotation { get; set; }

        public Vector3 Scale { get; set; }

        public void Dispose()
        {
            if (this.scaleform.IsLoaded) this.scaleform.Dispose();

            GC.SuppressFinalize(this);
        }

        public void Draw()
        {
            if (this.scaleform.IsLoaded && this.scaleform.IsValid && this.IsTextureSet)
                this.scaleform.Render3D(this.Position, this.Rotation, this.Scale);
        }

        public void SetPosition(Screen.PositionSettings positionalSettings)
        {
            this.Position = new Vector3(
                positionalSettings.PositionX,
                positionalSettings.PositionY,
                positionalSettings.PositionZ);
            this.Rotation = new Vector3(
                positionalSettings.RotationX,
                positionalSettings.RotationY,
                positionalSettings.RotationZ);
            this.Scale = new Vector3(positionalSettings.ScaleX, positionalSettings.ScaleY, positionalSettings.ScaleZ);
        }

        public void SetTexture(
            string txdName,
            string txnName,
            int width = 1280,
            int height = 720,
            int positionX = 0,
            int positionY = 0)
        {
            if (this.IsTextureSet) return;

            Logger.Debug($"setting scaleform texture: txdName: \"{txdName}\"; txnName: \"{txnName}\"");

            this.scaleform.CallFunction("SET_TEXTURE", txdName, txnName, positionX, positionY, width, height);
            this.IsTextureSet = true;
        }

        public void UnsetTexture()
        {
            if (!this.IsTextureSet) return;

            this.scaleform.CallFunction("UNSET_TEXTURE");
            this.IsTextureSet = false;
        }
    }
}