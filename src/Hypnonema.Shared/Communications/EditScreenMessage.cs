using Hypnonema.Shared.Models;

namespace Hypnonema.Shared.Communications
{
    public class EditScreenMessage
    {
        public Screen Screen { get; set; }

        public EditScreenMessage(Screen screen)
        {
            this.Screen = screen;
        }
    }
}