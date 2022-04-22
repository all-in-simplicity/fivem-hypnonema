namespace Hypnonema.Shared.Communications
{
    public class StopMessage
    {
        public string ScreenName { get; set; }

        public StopMessage(string screenName)
        {
            this.ScreenName = screenName;
        }
    }
}