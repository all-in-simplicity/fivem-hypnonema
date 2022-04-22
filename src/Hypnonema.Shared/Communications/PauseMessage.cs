namespace Hypnonema.Shared.Communications
{
    public class PauseMessage
    {
        public string ScreenName { get; set; }

        public PauseMessage(string screenName)
        {
            this.ScreenName = screenName;
        }
    }
}