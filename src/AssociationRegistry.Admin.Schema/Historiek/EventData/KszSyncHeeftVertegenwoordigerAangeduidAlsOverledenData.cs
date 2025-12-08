namespace AssociationRegistry.Admin.Schema.Historiek.EventData;

using Events;

public record KszSyncHeeftVertegenwoordigerAangeduidAlsOverledenData
(
    int VertegenwoordigerId,
    string Voornaam,
    string Achternaam)
{
    public static KszSyncHeeftVertegenwoordigerAangeduidAlsOverledenData Create(KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden e)
        => new(
            e.VertegenwoordigerId,
            e.Voornaam,
            e.Achternaam);
}
