namespace AssociationRegistry.ContactGegevens.Websites;

using Framework;
using Exceptions;

public record Website(string Waarde, string Omschrijving, bool IsPrimair)
    : Contactgegeven(ContactgegevenType.Website, Waarde, Omschrijving, IsPrimair)
{
    public static Website Create(string? website)
        => Create(website, string.Empty,false);

    public static Website Create(string? website, string omschrijving,bool isPrimair)
    {
        if (string.IsNullOrEmpty(website))
            return null!;

        Throw<InvalidWebsiteStart>.IfNot(UrlHasCorrectStartingCharacters(website));
        Throw<WebsiteMissingPeriod>.IfNot(UrlContainsAPeriod(website));
        return new Website(website, omschrijving, isPrimair);
    }

    private static bool UrlContainsAPeriod(string urlString)
        => urlString.Contains('.');

    private static bool UrlHasCorrectStartingCharacters(string urlString)
        => urlString.StartsWith("http://") || urlString.StartsWith("https://");
}
