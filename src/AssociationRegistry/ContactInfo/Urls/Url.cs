namespace AssociationRegistry.ContactInfo.Urls;

using Framework;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Exceptions;

public class Url : StringValueObject<Url>
{
    private Url(string @string) : base(@string)
    {
    }

    public static Url Create(string? urlString)
    {
        if (string.IsNullOrEmpty(urlString))
            return null!;

        Throw<InvalidUrlStart>.IfNot(UrlHasCorrectStartingCharacters(urlString));
        Throw<UrlMissingPeriod>.IfNot(UrlContainsAPeriod(urlString));
        return new Url(urlString);
    }

    public static implicit operator string?(Url? url)
        => url?.ToString();

    public static implicit operator Url(string? url)
        => Create(url);

    public override string ToString()
        => Value;

    private static bool UrlContainsAPeriod(string urlString)
        => urlString.Contains('.');

    private static bool UrlHasCorrectStartingCharacters(string urlString)
        => urlString.StartsWith("http://") || urlString.StartsWith("https://");
}
