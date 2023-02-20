namespace AssociationRegistry.ContactInfo;

using Framework;
using Exceptions;

public class ContactInfo
{
    private ContactInfo(string? contactnaam, string? email, string? telefoon, string? website, string? socialMedia, bool primairContactInfo)
    {
        Contactnaam = contactnaam;
        Email = email;
        Telefoon = telefoon;
        Website = website;
        SocialMedia = socialMedia;
        PrimairContactInfo = primairContactInfo;
    }

    public static ContactInfo CreateInstance(string? contactnaam, string? email, string? telefoonNummer, string? website, string? socialMedia, bool primairContactInfo)
    {
        Throw<NoContactInfo>.If(NoValuesForAll(email, telefoonNummer, website, socialMedia));
        return new ContactInfo(contactnaam, email, telefoonNummer, website, socialMedia, primairContactInfo);
    }

    private static bool NoValuesForAll(params string?[] args)
        => args.All(string.IsNullOrEmpty);

    public string? Contactnaam { get; }
    public string? Email { get; }
    public string? Telefoon { get; }
    public string? Website { get; }
    public string? SocialMedia { get; }
    public bool PrimairContactInfo { get; }
}
