namespace AssociationRegistry.Contacten;

public class ContactInfo
{
    private ContactInfo(string contactnaam, string? email, string? telefoonNummer, string? website)
    {
        Contactnaam = contactnaam;
        Email = email;
        TelefoonNummer = telefoonNummer;
        Website = website;
    }

    public static ContactInfo CreateInstance(string contactnaam, string? email, string? telefoonNummer, string? website)
        => new(contactnaam, email, telefoonNummer, website);

    public string Contactnaam { get; }
    public string? Email { get; }
    public string? TelefoonNummer { get; }
    public string? Website { get; }
}
