namespace Hypnonema.Shared.Communications
{
    using Hypnonema.Shared.Models;

    public class CreateScreenMessage
    {
        public CreateScreenMessage(Screen screen)
        {
            this.Screen = screen;
        }

        public Screen Screen { get; set; }
    }
}