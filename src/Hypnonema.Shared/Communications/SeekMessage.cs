namespace Hypnonema.Shared.Communications
{
    public class SeekMessage
    {
        public SeekMessage(string screenName, float time)
        {
            this.ScreenName = screenName;
            this.Time = time;
        }

        public string ScreenName { get; set; }

        public float Time { get; set; }
    }
}