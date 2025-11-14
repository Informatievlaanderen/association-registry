namespace AssociationRegistry.Admin.Schema.Historiek.EventData;

using Events;
using Events.Enriched;

public record VertegenwoordigerWerdVerwijderdData
(
    int VertegenwoordigerId,
    string Voornaam,
    string Achternaam)
{
    public static VertegenwoordigerWerdVerwijderdData Create(VertegenwoordigerWerdVerwijderdMetPersoonsgegevens e)
        => new(
            e.VertegenwoordigerId,
            e.Voornaam,
            e.Achternaam);
}
