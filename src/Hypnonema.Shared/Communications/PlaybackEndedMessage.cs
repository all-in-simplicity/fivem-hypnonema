namespace Hypnonema.Shared.Communications
{
    public class PlaybackEndedMessage
    {
        public string ScreenName { get; set; }

        public PlaybackEndedMessage(string screenName)
        {
            this.ScreenName = screenName;
        }
    }
}