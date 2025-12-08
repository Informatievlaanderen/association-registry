namespace AssociationRegistry.Admin.Schema.Historiek.EventData;

using Events;

public record KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekendData
(
    int VertegenwoordigerId,
    string Voornaam,
    string Achternaam)
{
    public static KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekendData Create(KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend e)
        => new(
            e.VertegenwoordigerId,
            e.Voornaam,
            e.Achternaam);
}
