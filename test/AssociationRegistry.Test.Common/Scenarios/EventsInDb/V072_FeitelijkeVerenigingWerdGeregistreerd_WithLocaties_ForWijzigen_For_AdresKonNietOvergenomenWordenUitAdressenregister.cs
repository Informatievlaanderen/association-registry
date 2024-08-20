namespace AssociationRegistry.Test.Common.Scenarios.EventsInDb;

using AssociationRegistry.Events;
using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AutoFixture;
using global::AutoFixture;

public class V072_FeitelijkeVerenigingWerdGeregistreerd_WithLocaties_ForWijzigen_For_AdresKonNietOvergenomenWordenUitAdressenregister : IEventsInDbScenario
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly AdresWerdOvergenomenUitAdressenregister AdresWerdOvergenomenUitAdressenregister;
    public readonly AdresNietUniekInAdressenregister AdresNietUniekInAdressenregister;
    public readonly AdresWerdNietGevondenInAdressenregister AdresWerdNietGevondenInAdressenregister;
    public readonly AdresKonNietOvergenomenWordenUitAdressenregister AdresKonNietOvergenomenWordenUitAdressenregister;
    public readonly CommandMetadata Metadata;

    public V072_FeitelijkeVerenigingWerdGeregistreerd_WithLocaties_ForWijzigen_For_AdresKonNietOvergenomenWordenUitAdressenregister()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VCode = "V9999072";
        Naam = "Dee sjiekste club";

        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            VCode = VCode,
            Naam = Naam,
            Locaties = fixture.CreateMany<Registratiedata.Locatie>().Select(
                (locatie, w) => locatie with
                {
                    IsPrimair = w == 0,
                    LocatieId = locatie.LocatieId,
                }
            ).ToArray(),
        };

        AdresWerdOvergenomenUitAdressenregister = fixture.Create<AdresWerdOvergenomenUitAdressenregister>()
            with

            {
                LocatieId = FeitelijkeVerenigingWerdGeregistreerd.Locaties.First().LocatieId,
            };
        AdresNietUniekInAdressenregister = fixture.Create<AdresNietUniekInAdressenregister>() with

        {
            LocatieId = FeitelijkeVerenigingWerdGeregistreerd.Locaties.First().LocatieId,
        };;
        AdresWerdNietGevondenInAdressenregister = fixture.Create<AdresWerdNietGevondenInAdressenregister>() with

        {
            LocatieId = FeitelijkeVerenigingWerdGeregistreerd.Locaties.First().LocatieId,
        };;
        AdresKonNietOvergenomenWordenUitAdressenregister = fixture.Create<AdresKonNietOvergenomenWordenUitAdressenregister>() with

        {
            LocatieId = FeitelijkeVerenigingWerdGeregistreerd.Locaties.First().LocatieId,
        };;


        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;
    public string Naam { get; set; }

    public DateOnly? Startdatum
        => FeitelijkeVerenigingWerdGeregistreerd.Startdatum;

    public IEvent[] GetEvents()
        => new IEvent[]
        {
            FeitelijkeVerenigingWerdGeregistreerd,
            AdresWerdOvergenomenUitAdressenregister,
            AdresNietUniekInAdressenregister,
            AdresWerdNietGevondenInAdressenregister,
            AdresKonNietOvergenomenWordenUitAdressenregister,
        };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
