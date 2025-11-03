namespace AssociationRegistry.Events.Enriched;

public record VertegenwoordigerWerdGewijzigdMetPersoonsdata(
    int VertegenwoordigerId,
    bool IsPrimair,
    string Roepnaam,
    string Rol,
    string Voornaam,
    string Achternaam,
    string Email,
    string Telefoon,
    string Mobiel,
    string SocialMedia
) : IEvent
{

}
