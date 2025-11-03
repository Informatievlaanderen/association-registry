namespace AssociationRegistry.Persoonsdata;

public record Vertegenwoordigerspersoonsdata(
    Guid RefId,
    string VertegenwoordigerId,
    string VCode,
    string Insz,
    string Roepnaam,
    string Rol,
    string Voornaam,
    string Achternaam,
    string Email,
    string Telefoon,
    string Mobiel,
    string SocialMedia);
