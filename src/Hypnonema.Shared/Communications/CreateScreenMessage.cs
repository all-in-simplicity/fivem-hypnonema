using Hypnonema.Shared.Models;

namespace Hypnonema.Shared.Communications
{
    public class CreateScreenMessage
    {
        public Screen Screen { get; set; }

        public CreateScreenMessage(Screen screen)
        {
            this.Screen = screen;
        }
    }
}