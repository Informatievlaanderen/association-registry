namespace AssociationRegistry.Events;

using System.Runtime.Serialization;
using Vereniging.Bronnen;

public record VertegenwoordigerPersoonsgegevens(
    string? Insz,
    string? Roepnaam,
    string? Rol,
    string? Voornaam,
    string? Achternaam,
    string? Email,
    string? Telefoon,
    string? Mobiel,
    string? SocialMedia);

public record VertegenwoordigerWerdToegevoegdMetPersoonsgegevens(
    int VertegenwoordigerId,
    bool IsPrimair,
    VertegenwoordigerPersoonsgegevens? VertegenwoordigerPersoonsgegevens) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron
        => Bron.Initiator;
};
