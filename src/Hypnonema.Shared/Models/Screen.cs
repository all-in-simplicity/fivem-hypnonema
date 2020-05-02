namespace Hypnonema.Shared.Models
{
    public class Screen
    {
        public bool AlwaysOn { get; set; }

        public DuiBrowserSettings BrowserSettings { get; set; }

        public int Id { get; set; }

        public bool Is3DRendered { get; set; }

        public bool IsValid =>
            this.TargetSettings != null && this.BrowserSettings != null && this.Id != 0
            && !string.IsNullOrEmpty(this.Name);

        public string Name { get; set; }

        public PositionalSettings PositionalSettings { get; set; }

        public RenderTargetSettings TargetSettings { get; set; }
    }
}