namespace Hypnonema.Shared.Communications
{
    public class ResumeMessage
    {
        public ResumeMessage(string screenName)
        {
            this.ScreenName = screenName;
        }

        public string ScreenName { get; set; }
    }
}