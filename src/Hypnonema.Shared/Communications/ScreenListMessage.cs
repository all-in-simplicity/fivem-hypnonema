namespace Hypnonema.Shared.Communications
{
    using System.Collections.Generic;

    using Hypnonema.Shared.Models;

    public class ScreenListMessage
    {
        public ScreenListMessage(List<Screen> screens)
        {
            this.Screens = screens;
        }

        public List<Screen> Screens { get; set; }
    }
}