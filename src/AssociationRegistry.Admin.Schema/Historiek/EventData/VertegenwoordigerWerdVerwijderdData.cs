namespace AssociationRegistry.Admin.Schema.Historiek.EventData;

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

    public static VertegenwoordigerWerdVerwijderdData Create(KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend e)
        => new(
            e.VertegenwoordigerId,
            e.Voornaam,
            e.Achternaam);

    public static VertegenwoordigerWerdVerwijderdData Create(KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden e)
        => new(
            e.VertegenwoordigerId,
            e.Voornaam,
            e.Achternaam);
}
