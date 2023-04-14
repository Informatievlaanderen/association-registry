namespace AssociationRegistry.Vereniging.SocialMedias;

using Framework;
using Exceptions;

public record SocialMedia(string Waarde, string Beschrijving, bool IsPrimair)
    : Contactgegeven(ContactgegevenType.SocialMedia, Waarde, Beschrijving, IsPrimair)
{
    public static SocialMedia Create(string? socialMedia)
        => Create(socialMedia, string.Empty, false);

    public static SocialMedia Create(string? socialMedia, string beschrijving, bool isPrimair)
    {
        if (string.IsNullOrEmpty(socialMedia))
            return null!;

        Throw<InvalidSocialMediaStart>.IfNot(UrlHasCorrectStartingCharacters(socialMedia));
        Throw<SocialMediaMissingPeriod>.IfNot(UrlContainsAPeriod(socialMedia));
        return new SocialMedia(socialMedia, beschrijving, isPrimair);
    }

    private static bool UrlContainsAPeriod(string urlString)
        => urlString.Contains('.');

    private static bool UrlHasCorrectStartingCharacters(string urlString)
        => urlString.StartsWith("http://") || urlString.StartsWith("https://");
}
