namespace Hypnonema.Shared.Communications
{
    using System;
    using System.Collections.Generic;

    public class DuiStateMessage
    {
        public DuiStateMessage()
        {
            this.RequestId = new Guid();
        }

        public List<DuiState> DuiStates { get; set; } = new List<DuiState>();

        public Guid RequestId { get; set; }
    }
}