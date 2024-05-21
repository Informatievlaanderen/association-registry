namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.EventsInDb;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using EventStore;
using Framework;

public class V073_AdresWerdOvergenomenUitAdressenregister : IEventsInDbScenario
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly AdresWerdOvergenomenUitAdressenregister AdresWerdOvergenomenUitAdressenregister;
    // public readonly AdresNietUniekInAdressenregister AdresNietUniekInAdressenregister;
    // public readonly AdresWerdNietGevondenInAdressenregister AdresWerdNietGevondenInAdressenregister;
    // public readonly LocatieWerdVerwijderd LocatieWerdVerwijderd;
    // public readonly VerenigingWerdVerwijderd VerenigingWerdVerwijderd;

    public readonly CommandMetadata Metadata;

    public V073_AdresWerdOvergenomenUitAdressenregister()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VCode = "V9999073";
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
                VCode = VCode,
                LocatieId = FeitelijkeVerenigingWerdGeregistreerd.Locaties.First().LocatieId
            };
        // AdresNietUniekInAdressenregister = fixture.Create<AdresNietUniekInAdressenregister>() with
        //
        // {
        //     LocatieId = FeitelijkeVerenigingWerdGeregistreerd.Locaties.First().LocatieId
        // };
        // AdresWerdNietGevondenInAdressenregister = fixture.Create<AdresWerdNietGevondenInAdressenregister>() with
        //
        // {
        //     LocatieId = FeitelijkeVerenigingWerdGeregistreerd.Locaties.First().LocatieId
        // };
        //
        // LocatieWerdVerwijderd = fixture.Create<AdresWerdNietGevondenInAdressenregister>() with
        //
        // {
        //     LocatieId = FeitelijkeVerenigingWerdGeregistreerd.Locaties.First().LocatieId
        // };

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;
    public string Naam { get; set; }

    public IEvent[] GetEvents()
        => new IEvent[]
        {
            FeitelijkeVerenigingWerdGeregistreerd,
            AdresWerdOvergenomenUitAdressenregister,
        };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
