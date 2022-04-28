namespace Hypnonema.Shared.Communications
{
    public class StopMessage
    {
        public StopMessage(string screenName)
        {
            this.ScreenName = screenName;
        }

        public string ScreenName { get; set; }
    }
}