namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;

using Websites;
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

        if (!Website.IsValid(url))
            throw new OngeldigUrl();

        return new HernieuwingsUrl(url);
    }

    public static HernieuwingsUrl Hydrate(string url) => new(url);
}
