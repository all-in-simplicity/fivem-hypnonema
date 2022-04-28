namespace Hypnonema.Shared.Communications
{
    using System;

    using Hypnonema.Shared.Models;

    public class PlayMessage
    {
        public bool IsValid =>
            Uri.TryCreate(this.Url, UriKind.Absolute, out var uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

        public Screen Screen { get; set; }

        public string Url { get; set; } = string.Empty;
    }
}