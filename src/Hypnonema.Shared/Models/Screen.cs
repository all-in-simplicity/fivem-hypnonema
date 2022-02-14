namespace Hypnonema.Shared.Models
{
    public class Screen
    {
        public bool AlwaysOn { get; set; }

        public DuiBrowserSettings BrowserSettings { get; set; }

        public int Id { get; set; }

        public bool Is3DRendered { get; set; }

        public string Name { get; set; }

        public PositionalSettings PositionalSettings { get; set; }

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
                                       Is3DAudioEnabled = true,
                                       SoundAttenuation = 10f,
                                       SoundMaxDistance = 200f,
                                       SoundMinDistance = 10f
                                   },
                           Is3DRendered = true,
                           Name = "Hypnonema Example Screen",
                           PositionalSettings = new PositionalSettings()
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
    }
}