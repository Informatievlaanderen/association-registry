namespace AssociationRegistry.Vereniging.SocialMedias;

using Framework;
using Exceptions;

public record SocialMedia(string Waarde):IContactWaarde
{
    public static readonly SocialMedia Leeg = new(string.Empty);

    public static SocialMedia Create(string? socialMedia)
    {
        if (string.IsNullOrEmpty(socialMedia))
            return Leeg;

        Throw<InvalidSocialMediaStart>.IfNot(UrlHasCorrectStartingCharacters(socialMedia));
        Throw<SocialMediaMissingPeriod>.IfNot(UrlContainsAPeriod(socialMedia));
        return new SocialMedia(socialMedia);
    }

    public static SocialMedia Hydrate(string socialMedia)
        => new(socialMedia);

    private static bool UrlContainsAPeriod(string urlString)
        => urlString.Contains('.');

    private static bool UrlHasCorrectStartingCharacters(string urlString)
        => urlString.StartsWith("http://") || urlString.StartsWith("https://");
    public virtual bool Equals(IContactWaarde? other)
        => other?.Waarde == Waarde;
}
