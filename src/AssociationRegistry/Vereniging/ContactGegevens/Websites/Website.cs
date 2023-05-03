namespace AssociationRegistry.Vereniging.Websites;

using Framework;
using Exceptions;

public record Website(string Waarde)
{
    public static readonly Website Leeg = new(string.Empty);

    public static Website Create(string? website)
    {
        if (string.IsNullOrEmpty(website))
            return Leeg;

        Throw<InvalidWebsiteStart>.IfNot(UrlHasCorrectStartingCharacters(website));
        Throw<WebsiteMissingPeriod>.IfNot(UrlContainsAPeriod(website));
        return new Website(website);
    }

    public static Website Hydrate(string website)
        => new(website);

    public override string ToString()
        => Waarde;

    private static bool UrlContainsAPeriod(string urlString)
        => urlString.Contains('.');

    private static bool UrlHasCorrectStartingCharacters(string urlString)
        => urlString.StartsWith("http://") || urlString.StartsWith("https://");
}
