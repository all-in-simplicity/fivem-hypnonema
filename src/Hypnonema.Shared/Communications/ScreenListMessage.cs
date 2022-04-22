using System.Collections.Generic;
using Hypnonema.Shared.Models;

namespace Hypnonema.Shared.Communications
{
    public class ScreenListMessage
    {
        public List<Screen> Screens { get; set; }

        public ScreenListMessage(List<Screen> screens)
        {
            this.Screens = screens;
        }
    }
}