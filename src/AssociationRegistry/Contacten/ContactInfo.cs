namespace AssociationRegistry.Contacten;

public class ContactInfo
{
    private ContactInfo(string? contactnaam, string? email, string? telefoon, string? website, string? socialMedia)
    {
        Contactnaam = contactnaam;
        Email = email;
        Telefoon = telefoon;
        Website = website;
        SocialMedia = socialMedia;
    }

    public static ContactInfo CreateInstance(string? contactnaam, string? email, string? telefoonNummer, string? website, string? socialMedia)
        => new(contactnaam, email, telefoonNummer, website, socialMedia);

    public string? Contactnaam { get; }
    public string? Email { get; }
    public string? Telefoon { get; }
    public string? Website { get; }
    public string? SocialMedia { get; }
}
