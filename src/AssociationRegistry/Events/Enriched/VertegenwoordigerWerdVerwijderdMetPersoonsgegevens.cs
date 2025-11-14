namespace AssociationRegistry.Events.Enriched;

using AssociationRegistry.Vereniging.Bronnen;
using System.Runtime.Serialization;

public record VertegenwoordigerWerdVerwijderdMetPersoonsgegevens(
    int VertegenwoordigerId,
    string? Insz,
    string? Voornaam,
    string? Achternaam) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron
        => Bron.Initiator;
};
