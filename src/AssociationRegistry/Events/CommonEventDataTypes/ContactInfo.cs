namespace AssociationRegistry.Events.CommonEventDataTypes;

using AssociationRegistry.ContactInfo;

public record ContactInfo(
    string Contactnaam,
    string? Email,
    string? Telefoon,
    string? Website,
    string? SocialMedia,
    bool PrimairContactInfo)
{
    public static ContactInfo[] FromContactInfoLijst(ContactLijst contactLijst)
        => contactLijst.Select(FromDomain).ToArray();

    public static ContactInfo FromDomain(AssociationRegistry.ContactInfo.ContactInfo c)
        => new(c.Contactnaam, c.Email, c.Telefoon, c.Website, c.SocialMedia, c.PrimairContactInfo);
}
