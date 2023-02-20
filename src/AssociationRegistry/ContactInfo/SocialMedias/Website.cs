namespace AssociationRegistry.ContactInfo.SocialMedias;

using Framework;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Exceptions;

public class SocialMedia : StringValueObject<SocialMedia>
{
    private SocialMedia(string @string) : base(@string)
    {
    }

    public static SocialMedia Create(string? socialMediaString)
    {
        if (string.IsNullOrEmpty(socialMediaString))
            return null!;

        Throw<InvalidSocialMediaStart>.IfNot(SocialMediaHasCorrectStartingCharacters(socialMediaString));
        Throw<SocialMediaMissingPeriod>.IfNot(SocialMediaContainsAPoint(socialMediaString));
        return new SocialMedia(socialMediaString);
    }

    public static implicit operator string?(SocialMedia? socialMedia)
        => socialMedia?.ToString();

    public static implicit operator SocialMedia(string? socialMedia)
        => Create(socialMedia);

    public override string ToString()
        => Value;

    private static bool SocialMediaContainsAPoint(string socialMediaString)
        => socialMediaString.Contains('.');

    private static bool SocialMediaHasCorrectStartingCharacters(string socialMediaString)
        => socialMediaString.StartsWith("http://") || socialMediaString.StartsWith("https://");
}
