namespace Hypnonema.Shared.Models
{
    using System.Collections.Generic;

    public class Screen
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool Is3DRendered { get; set; }

        public RenderTargetSettings TargetSettings { get; set; }

        public DuiBrowserSettings BrowserSettings { get; set; }

        public PositionalSettings PositionalSettings { get; set; }

        public bool AlwaysOn { get; set; }

        public bool IsValid => this.TargetSettings != null && this.BrowserSettings != null && this.Id != 0 && !string.IsNullOrEmpty(this.Name);
    }
}