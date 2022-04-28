namespace Hypnonema.Shared.Communications
{
    public class DuiStateChangedMessage
    {
        public DuiStateChangedMessage(string screenName, DuiState duiState, ChangeTypeEnum changeType)
        {
            this.ScreenName = screenName;
            this.DuiState = duiState;
            this.ChangeType = changeType;
        }

        public enum ChangeTypeEnum
        {
            Created = 0,

            Updated = 1,

            Deleted = 2,
        }

        public ChangeTypeEnum ChangeType { get; set; }

        public DuiState DuiState { get; set; }

        public string ScreenName { get; set; }
    }
}