namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;

using Exceptions;
using Websites.Exceptions;

public record HernieuwingsUrl
{
    public string Value { get; }

    private HernieuwingsUrl(string value)
    {
        Value = value;
    }

    public static HernieuwingsUrl Create(string? url)
    {
        if (string.IsNullOrEmpty(url))
            return new HernieuwingsUrl(string.Empty);

        if (!IsValidScheme(url))
            throw new WebsiteMoetStartenMetHttps();

        if (!IsValid(url))
            throw new OngeldigUrl();

        return new HernieuwingsUrl(url);
    }

    private static bool IsValidScheme(string url) =>
        url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
        url.StartsWith("https://", StringComparison.OrdinalIgnoreCase);

    public static HernieuwingsUrl Hydrate(string url) => new(url);

    private static bool IsValid(string url) =>
        Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var uri)
     && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
     && !string.IsNullOrWhiteSpace(uri.Host);
}
