namespace AssociationRegistry.Test.Common.Scenarios.EventsInDb;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using EventStore;
using global::AutoFixture;

public class V075_AdresWerdGewijzigdInAdressenregister : IEventsInDbScenario
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly List<AdresWerdOvergenomenUitAdressenregister> AdresWerdOvergenomenUitAdressenregisterList;
    public readonly AdresWerdGewijzigdInAdressenregister AdresWerdGewijzigdInAdressenregister;
    public readonly CommandMetadata Metadata;

    public V075_AdresWerdGewijzigdInAdressenregister()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VCode = "V9999075";
        Naam = "Dee sjiekste club";

        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            VCode = VCode,
            Naam = Naam,
            Locaties = new List<Registratiedata.Locatie>
            {
                fixture.Create<Registratiedata.Locatie>(),
            }.ToArray(),
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

        AdresWerdGewijzigdInAdressenregister = fixture.Create<AdresWerdGewijzigdInAdressenregister>() with
        {
            LocatieId = FeitelijkeVerenigingWerdGeregistreerd.Locaties.First().LocatieId,
            VCode = VCode,
        };

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;
    public string Naam { get; set; }
    public AdresWerdOvergenomenUitAdressenregister[] OvergenomenLocaties { get; set; }

    public IEvent[] GetEvents()
        => new IEvent[]
            {
                FeitelijkeVerenigingWerdGeregistreerd,
            }.Concat(AdresWerdOvergenomenUitAdressenregisterList.ToArray())
             .Concat(new IEvent[]
              {
                  AdresWerdGewijzigdInAdressenregister,
              })
             .ToArray();

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
