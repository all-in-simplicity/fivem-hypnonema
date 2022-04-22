namespace Hypnonema.Shared.Models
{
    public class Screen
    {
        public bool AlwaysOn { get; set; }

        public DuiBrowserSettings BrowserSettings { get; set; }

        public int Id { get; set; }

        public bool Is3DRendered { get; set; }

        public string Name { get; set; }

        public PositionSettings PositionalSettings { get; set; }

        public RenderTargetSettings TargetSettings { get; set; }

        public static Screen CreateExampleScreen()
        {
            return new Screen()
                       {
                           AlwaysOn = false,
                           BrowserSettings =
                               new DuiBrowserSettings()
                                   {
                                       GlobalVolume = 100f,
                                       Is3DAudioEnabled = false,
                                       SoundAttenuation = 5f,
                                       SoundMaxDistance = 200f,
                                       SoundMinDistance = 30f
                                   },
                           Is3DRendered = true,
                           Name = "Hypnonema Example Screen",
                           PositionalSettings = new PositionSettings()
                                                    {
                                                        PositionX = -1678.949f,
                                                        PositionY = -928.3431f,
                                                        PositionZ = 20.6290932f,
                                                        RotationX = 0f,
                                                        RotationY = 0f,
                                                        RotationZ = -140f,
                                                        ScaleX = 0.969999969f,
                                                        ScaleY = 0.484999985f,
                                                        ScaleZ = -0.1f
                                                    }
                       };
        }

        public bool IsValid =>
            string.IsNullOrEmpty(this.Name) || (this.Is3DRendered || this.TargetSettings.IsValid);
       

        public class DuiBrowserSettings
        {
            public float GlobalVolume { get; set; }

            public bool Is3DAudioEnabled { get; set; }

            public float SoundAttenuation { get; set; }

            public float SoundMaxDistance { get; set; }

            public float SoundMinDistance { get; set; }
        }

        public class PositionSettings
        {
            public float PositionX { get; set; }

            public float PositionY { get; set; }

            public float PositionZ { get; set; }

            public float RotationX { get; set; }

            public float RotationY { get; set; }

            public float RotationZ { get; set; }

            public float ScaleX { get; set; }

            public float ScaleY { get; set; }

            public float ScaleZ { get; set; }
        }

        public class RenderTargetSettings
        {
            public bool IsValid =>
                !string.IsNullOrEmpty(this.ModelName) && !string.IsNullOrEmpty(this.RenderTargetName);

            public string ModelName { get; set; }

            public string RenderTargetName { get; set; }
        }
    }
}