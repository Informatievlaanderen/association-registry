namespace AssociationRegistry.ContactInfo.Websites;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Exceptions;
using Framework;

public class Website : StringValueObject<Website>
{
    private Website(string @string) : base(@string)
    {
    }

    public static Website Create(string? websiteString)
    {
        if (string.IsNullOrEmpty(websiteString))
            return null!;

        Throw<InvalidWebsiteStart>.IfNot(WebsiteHasCorrectStartingCharacters(websiteString));
        Throw<WebsiteMissingPeriod>.IfNot(WebsiteContainsAPoint(websiteString));
        return new Website(websiteString);
    }

    public static implicit operator string?(Website? website)
        => website?.ToString();

    public static implicit operator Website(string? website)
        => Create(website);

    public override string ToString()
        => Value;

    private static bool WebsiteContainsAPoint(string websiteString)
        => websiteString.Contains('.');

    private static bool WebsiteHasCorrectStartingCharacters(string websiteString)
        => websiteString.StartsWith("http://") || websiteString.StartsWith("https://");
}
