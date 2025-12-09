namespace AssociationRegistry.Events;

[Obsolete("These are the upcasted events, you might be looking for <EventName>+ZonderPersoonsgegevens")]
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
