namespace AssociationRegistry.Admin.Api.Projections.Historiek.Schema.EventData;

using Events;

public record VertegenwoordigerWerdVerwijderdData
(
    int VertegenwoordigerId,
    string Voornaam,
    string Achternaam)
{
    public static VertegenwoordigerWerdVerwijderdData Create(VertegenwoordigerWerdVerwijderd e)
        => new(
            e.VertegenwoordigerId,
            e.Voornaam,
            e.Achternaam);
};
