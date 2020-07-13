namespace Hypnonema.Shared
{
    public class DuiState
    {
        public string CurrentSource { get; set; }

        public float CurrentTime { get; set; }

        public float Duration { get; set; }

        public bool Ended { get; set; }

        public bool IsPaused { get; set; }

        public string ScreenName { get; set; }

        public bool Repeat { get; set; }
    }
}