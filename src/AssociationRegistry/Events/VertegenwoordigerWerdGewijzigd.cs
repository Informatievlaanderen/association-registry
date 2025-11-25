namespace AssociationRegistry.Events;

public record VertegenwoordigerWerdGewijzigd(
    int VertegenwoordigerId,
    bool IsPrimair,
    string Roepnaam,
    string Rol,
    string Voornaam,
    string Achternaam,
    string Email,
    string Telefoon,
    string Mobiel,
    string SocialMedia) : IEvent
{ }

public record VertegenwoordigerWerdGewijzigdZonderPersoonsgegevens(
    Guid RefId,
    int VertegenwoordigerId,
    bool IsPrimair) : IEvent
{ }
