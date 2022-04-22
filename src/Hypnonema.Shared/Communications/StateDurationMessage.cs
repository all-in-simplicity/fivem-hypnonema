namespace Hypnonema.Shared.Communications
{
    public class StateDurationMessage
    {
        public string ScreenName { get; set; }

        public float Duration { get; set; }

        public StateDurationMessage(string screenName, float duration)
        {
            this.ScreenName = screenName;
            this.Duration = duration;
        }
    }
}