namespace AssociationRegistry.ContactInfo;

using Emails;
using Framework;
using Exceptions;
using SocialMedias;
using TelefoonNummers;
using Websites;

public class ContactInfo
{
    private ContactInfo(string contactnaam, Email? email, TelefoonNummer? telefoon, Website? website, SocialMedia? socialMedia, bool primairContactInfo)
    {
        Contactnaam = contactnaam;
        Email = email;
        Telefoon = telefoon;
        Website = website;
        SocialMedia = socialMedia;
        PrimairContactInfo = primairContactInfo;
    }

    public static ContactInfo CreateInstance(string contactnaam, Email? email, TelefoonNummer? telefoonNummer, Website? website, SocialMedia? socialMedia, bool primairContactInfo)
    {
        Throw<NoContactInfo>.If(NoValuesForAll(email, telefoonNummer, website, socialMedia));
        Throw<NoContactnaam>.If(string.IsNullOrWhiteSpace(contactnaam));
        return new ContactInfo(contactnaam, email, telefoonNummer, website, socialMedia, primairContactInfo);
    }

    private static bool NoValuesForAll(params string?[] args)
        => args.All(string.IsNullOrEmpty);

    public string Contactnaam { get; }
    public Email? Email { get; }
    public TelefoonNummer? Telefoon { get; }
    public Website? Website { get; }
    public SocialMedia? SocialMedia { get; }
    public bool PrimairContactInfo { get; }
}
