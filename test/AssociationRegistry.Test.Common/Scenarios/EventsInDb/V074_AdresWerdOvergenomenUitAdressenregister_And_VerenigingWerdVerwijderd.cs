namespace AssociationRegistry.Test.Common.Scenarios.EventsInDb;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using EventStore;
using global::AutoFixture;
using MartenDb.Store;

public class V074_AdresWerdOvergenomenUitAdressenregister_And_VerenigingWerdVerwijderd : IEventsInDbScenario
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly List<AdresWerdOvergenomenUitAdressenregister> AdresWerdOvergenomenUitAdressenregisterList;
    public readonly VerenigingWerdVerwijderd VerenigingWerdVerwijderd;
    public readonly CommandMetadata Metadata;

    public V074_AdresWerdOvergenomenUitAdressenregister_And_VerenigingWerdVerwijderd()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VCode = "V9999074";
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

        AdresWerdOvergenomenUitAdressenregisterList = new List<AdresWerdOvergenomenUitAdressenregister>();

        foreach (var locatie in FeitelijkeVerenigingWerdGeregistreerd.Locaties)
        {
            AdresWerdOvergenomenUitAdressenregisterList.Add(
                fixture.Create<AdresWerdOvergenomenUitAdressenregister>()
                    with
                    {
                        VCode = VCode,
                        LocatieId = locatie.LocatieId,
                    });
        }

        VerenigingWerdVerwijderd = fixture.Create<VerenigingWerdVerwijderd>() with
        {
            VCode = VCode,
        };

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;
    public string Naam { get; set; }
    public AdresWerdOvergenomenUitAdressenregister[] ExpectedLocaties { get; set; }

    public IEvent[] GetEvents()
        => new IEvent[]
            {
                FeitelijkeVerenigingWerdGeregistreerd,
            }.Concat(AdresWerdOvergenomenUitAdressenregisterList.ToArray())
             .Concat(new IEvent[]
              {
                  VerenigingWerdVerwijderd,
              })
             .ToArray();

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
