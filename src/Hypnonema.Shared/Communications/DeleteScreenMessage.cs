namespace Hypnonema.Shared.Communications
{
    public class DeleteScreenMessage
    {
        public string ScreenName { get; set; }

        public DeleteScreenMessage(string screenName)
        {
            this.ScreenName = screenName;
        }
    }
}