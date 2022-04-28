namespace Hypnonema.Shared.Communications
{
    public class PauseMessage
    {
        public PauseMessage(string screenName)
        {
            this.ScreenName = screenName;
        }

        public string ScreenName { get; set; }
    }
}