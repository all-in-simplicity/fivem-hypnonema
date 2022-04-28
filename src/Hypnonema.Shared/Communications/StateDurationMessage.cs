namespace Hypnonema.Shared.Communications
{
    public class StateDurationMessage
    {
        public StateDurationMessage(string screenName, float duration)
        {
            this.ScreenName = screenName;
            this.Duration = duration;
        }

        public float Duration { get; set; }

        public string ScreenName { get; set; }
    }
}