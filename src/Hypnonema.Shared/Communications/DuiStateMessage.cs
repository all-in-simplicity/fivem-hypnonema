using System;
using System.Collections.Generic;

namespace Hypnonema.Shared.Communications
{
    public class DuiStateMessage
    {
        public Guid RequestId { get; set; }

        public List<DuiState> DuiStates { get; set; } = new List<DuiState>();

        public DuiStateMessage()
        {
            this.RequestId = new Guid();
        }
    }
}