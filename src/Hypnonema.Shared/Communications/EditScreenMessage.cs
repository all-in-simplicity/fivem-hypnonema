namespace Hypnonema.Shared.Communications
{
    using Hypnonema.Shared.Models;

    public class EditScreenMessage
    {
        public EditScreenMessage(Screen screen)
        {
            this.Screen = screen;
        }

        public Screen Screen { get; set; }
    }
}