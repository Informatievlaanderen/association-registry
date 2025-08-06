namespace AssociationRegistry.DecentraalBeheer.Vereniging.SocialMedias;

using Framework;
using Exceptions;

public record SocialMedia(string Waarde, string Beschrijving, bool IsPrimair)
    : Contactgegeven(Contactgegeventype.SocialMedia, Waarde, Beschrijving, IsPrimair)
{
    public static readonly SocialMedia Leeg = new(string.Empty, string.Empty, IsPrimair: false);

    public static SocialMedia Create(string? socialMedia)
        => Create(socialMedia, string.Empty, isPrimair: false);

    public static SocialMedia Create(string? socialMedia, string beschrijving, bool isPrimair)
    {
        if (string.IsNullOrEmpty(socialMedia))
            return Leeg;

        Throw<SocialMediaMoetStartenMetHttp>.IfNot(UrlHasCorrectStartingCharacters(socialMedia));
        Throw<SocialMoetMinstensEenPuntBevatten>.IfNot(UrlContainsAPeriod(socialMedia));

        return new SocialMedia(socialMedia, beschrijving, isPrimair);
    }

    public static SocialMedia Hydrate(string socialMedia)
        => new(socialMedia, string.Empty, IsPrimair: false);

    private static bool UrlContainsAPeriod(string urlString)
        => urlString.Contains('.');

    private static bool UrlHasCorrectStartingCharacters(string urlString)
        => urlString.ToLower().StartsWith("http://") || urlString.ToLower().StartsWith("https://");
}
