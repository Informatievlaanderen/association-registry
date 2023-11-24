namespace AssociationRegistry.Vereniging.Websites;

using Exceptions;
using Framework;

public record Website(string Waarde, string Beschrijving, bool IsPrimair)
    : Contactgegeven(Contactgegeventype.Website, Waarde, Beschrijving, IsPrimair)
{
    public static readonly Website Leeg = new(string.Empty, string.Empty, IsPrimair: false);

    public static Website Create(string? website)
        => Create(website, string.Empty, isPrimair: false);

    public static Website Create(string? website, string beschrijving, bool isPrimair)
    {
        if (string.IsNullOrEmpty(website))
            return Leeg;

        Throw<WebsiteMoetStartenMetHttps>.IfNot(UrlHasCorrectStartingCharacters(website));
        Throw<WebsiteMoetMinstensEenPuntBevatten>.IfNot(UrlContainsAPeriod(website));

        return new Website(website, beschrijving, isPrimair);
    }

    private static bool UrlContainsAPeriod(string urlString)
        => urlString.Contains('.');

    private static bool UrlHasCorrectStartingCharacters(string urlString)
        => urlString.StartsWith("http://") || urlString.StartsWith("https://");
}
