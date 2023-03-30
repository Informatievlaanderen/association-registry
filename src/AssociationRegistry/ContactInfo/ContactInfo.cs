namespace AssociationRegistry.ContactInfo;

using Emails;
using Exceptions;
using Framework;
using TelefoonNummers;
using Urls;

public record ContactInfo(string Contactnaam, Email? Email, TelefoonNummer? Telefoon, Url? Website, Url? SocialMedia, bool PrimairContactInfo)
{
    public static ContactInfo CreateInstance(string contactnaam, Email? email, TelefoonNummer? telefoonNummer, Url? website, Url? socialMedia, bool primairContactInfo)
    {
        Throw<NoContactInfo>.If(NoValuesForAll(email, telefoonNummer, website, socialMedia));
        Throw<NoContactnaam>.If(string.IsNullOrWhiteSpace(contactnaam));
        return new ContactInfo(contactnaam, email, telefoonNummer, website, socialMedia, primairContactInfo);
    }

    public static ContactInfo FromEvent(Events.CommonEventDataTypes.ContactInfo contactInfo)
        => new(contactInfo.Contactnaam, contactInfo.Email, contactInfo.Telefoon, contactInfo.Website, contactInfo.SocialMedia, contactInfo.PrimairContactInfo);

    private static bool NoValuesForAll(params string?[] args)
        => args.All(string.IsNullOrEmpty);
}
