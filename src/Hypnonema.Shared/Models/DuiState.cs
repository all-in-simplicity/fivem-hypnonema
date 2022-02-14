namespace Hypnonema.Shared.Models
{
    using System;

    public class DuiState
    {
        public DuiState(Screen screen, string currentSource)
        {
            this.ScreenName = screen.Name;
            this.CurrentSource = currentSource;
            this.StartedAt = DateTime.UtcNow;
            this.Screen = screen;
        }

        public string CurrentSource { get; set; }

        public float CurrentTime => (float)(DateTime.UtcNow - this.StartedAt).TotalSeconds;

        public float Duration { get; set; }

        public bool Ended => this.CurrentTime >= this.Duration;

        public bool IsPaused { get; set; }

        public bool Repeat { get; set; }

        public Screen Screen { get; set; }

        public string ScreenName { get; set; }

        public DateTime StartedAt { get; set; }
    }
}