namespace AssociationRegistry.Vereniging.SocialMedias;

using Framework;
using Exceptions;

public record SocialMedia(string Waarde, string Omschrijving, bool IsPrimair)
    : Contactgegeven(ContactgegevenType.SocialMedia, Waarde, Omschrijving, IsPrimair)
{
    public static SocialMedia Create(string? socialMedia)
        => Create(socialMedia, string.Empty, false);

    public static SocialMedia Create(string? socialMedia, string omschrijving, bool isPrimair)
    {
        if (string.IsNullOrEmpty(socialMedia))
            return null!;

        Throw<InvalidSocialMediaStart>.IfNot(UrlHasCorrectStartingCharacters(socialMedia));
        Throw<SocialMediaMissingPeriod>.IfNot(UrlContainsAPeriod(socialMedia));
        return new SocialMedia(socialMedia, omschrijving, isPrimair);
    }

    private static bool UrlContainsAPeriod(string urlString)
        => urlString.Contains('.');

    private static bool UrlHasCorrectStartingCharacters(string urlString)
        => urlString.StartsWith("http://") || urlString.StartsWith("https://");
}
