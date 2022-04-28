namespace Hypnonema.Shared.Communications
{
    public class RepeatMessage
    {
        public RepeatMessage(string screenName, bool repeat)
        {
            this.ScreenName = screenName;
            this.Repeat = repeat;
        }

        public bool Repeat { get; set; }

        public string ScreenName { get; set; }
    }
}