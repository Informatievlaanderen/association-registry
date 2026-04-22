namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;

using Exceptions;

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

        if (!IsValid(url))
            throw new OngeldigUrl();

        return new HernieuwingsUrl(url);
    }

    public static HernieuwingsUrl Hydrate(string url) => new(url);

    private static bool IsValid(string url) =>
        Uri.TryCreate(url, UriKind.Absolute, out var uri)
        && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
        && !string.IsNullOrWhiteSpace(uri.Host);
}
