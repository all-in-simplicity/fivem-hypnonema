namespace Hypnonema.Shared.Communications
{
    public class ResumeMessage
    {
        public string ScreenName { get; set; }

        public ResumeMessage(string screenName)
        {
            this.ScreenName = screenName;
        }
    }
}