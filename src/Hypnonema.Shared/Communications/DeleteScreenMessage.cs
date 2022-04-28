namespace Hypnonema.Shared.Communications
{
    public class DeleteScreenMessage
    {
        public DeleteScreenMessage(string screenName)
        {
            this.ScreenName = screenName;
        }

        public string ScreenName { get; set; }
    }
}