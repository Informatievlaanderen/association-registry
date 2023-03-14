namespace AssociationRegistry.ContactInfo;

using Emails;
using Framework;
using Exceptions;
using TelefoonNummers;
using Urls;

public class ContactInfo
{
    private ContactInfo(string contactnaam, Email? email, TelefoonNummer? telefoon, Url? website, Url? socialMedia, bool primairContactInfo)
    {
        Contactnaam = contactnaam;
        Email = email;
        Telefoon = telefoon;
        Website = website;
        SocialMedia = socialMedia;
        PrimairContactInfo = primairContactInfo;
    }

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

    public string Contactnaam { get; }
    public Email? Email { get; }
    public TelefoonNummer? Telefoon { get; }
    public Url? Website { get; }
    public Url? SocialMedia { get; }
    public bool PrimairContactInfo { get; }
}
