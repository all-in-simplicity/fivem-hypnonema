namespace Hypnonema.Shared.Models
{
    public class Screen
    {
        public bool AlwaysOn { get; set; }

        public DuiBrowserSettings BrowserSettings { get; set; }

        public int Id { get; set; }

        public bool Is3DRendered { get; set; }

        public string Name { get; set; }

        public PositionalSettings PositionalSettings { get; set; }

        public RenderTargetSettings TargetSettings { get; set; }
    }
}