namespace AssociationRegistry.Events.Enriched;

using System.Runtime.Serialization;
using Vereniging.Bronnen;

public record VertegenwoordigerWerdToegevoegdMetPersoonsgegevens(
    int VertegenwoordigerId,
    string Insz,
    bool IsPrimair,
    string Roepnaam,
    string Rol,
    string Voornaam,
    string Achternaam,
    string Email,
    string Telefoon,
    string Mobiel,
    string SocialMedia) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron
        => Bron.Initiator;
};
