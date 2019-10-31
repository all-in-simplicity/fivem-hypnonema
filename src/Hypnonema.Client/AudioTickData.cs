namespace Hypnonema.Client
{
    using CitizenFX.Core;

    public class AudioTickData
    {
        public Vector3 ListenerForward { get; set; }

        public Vector3 ListenerUp { get; set; }

        public Vector3 OrientationPanner { get; set; }

        public Vector3 PositionListener { get; set; }

        public Vector3 PositionPanner { get; set; }
    }
}