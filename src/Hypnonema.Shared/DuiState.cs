namespace Hypnonema.Shared
{
    using System;

    using Hypnonema.Shared.Models;

    public class DuiState
    {
        public DuiState(Screen screen, string currentSource)
        {
            this.Screen = screen;
            this.ScreenName = screen.Name;
            this.StartedAt = DateTime.UtcNow;
            this.CurrentSource = currentSource;
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