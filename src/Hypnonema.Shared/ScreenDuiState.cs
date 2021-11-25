namespace Hypnonema.Shared.Events
{
    using System;
    using System.Collections.Generic;

    using Hypnonema.Shared.Events.Models;

    public class ScreenDuiState
    {
        public Screen Screen { get; set; }

        public DuiState State { get; set; }
    }

    public class ScreenDuiStateList
    {
        public List<ScreenDuiState> StateList { get; set; } = new List<ScreenDuiState>();

        public DateTime Timestamp { get; set; }
    }
}