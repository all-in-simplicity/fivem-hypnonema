namespace Hypnonema.Shared.Communications
{
    public class PlaybackEndedMessage
    {
        public PlaybackEndedMessage(string screenName)
        {
            this.ScreenName = screenName;
        }

        public string ScreenName { get; set; }
    }
}