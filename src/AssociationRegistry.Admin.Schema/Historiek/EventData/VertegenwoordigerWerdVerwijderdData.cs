namespace AssociationRegistry.Admin.Schema.Historiek.EventData;

using AssociationRegistry.Persoonsgegevens;
using Events;

public record VertegenwoordigerWerdVerwijderdData(int VertegenwoordigerId, string Voornaam, string Achternaam)
    : IVertegenwoordigerData<VertegenwoordigerWerdVerwijderdData>
{
    public static VertegenwoordigerWerdVerwijderdData Create(VertegenwoordigerWerdVerwijderd e) =>
        new(e.VertegenwoordigerId, e.Voornaam, e.Achternaam);

    public static VertegenwoordigerWerdVerwijderdData Create(KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend e) =>
        new(e.VertegenwoordigerId, e.Voornaam, e.Achternaam);

    public static VertegenwoordigerWerdVerwijderdData Create(KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden e) =>
        new(e.VertegenwoordigerId, e.Voornaam, e.Achternaam);

    public VertegenwoordigerWerdVerwijderdData MakeAnonymous() =>
        this with
        {
            Voornaam = WellKnownAnonymousFields.Geanonimiseerd,
            Achternaam = WellKnownAnonymousFields.Geanonimiseerd,
        };
}
