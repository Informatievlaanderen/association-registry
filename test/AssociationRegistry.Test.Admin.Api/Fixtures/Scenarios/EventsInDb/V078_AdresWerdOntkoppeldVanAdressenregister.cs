namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.EventsInDb;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using EventStore;
using Framework;
using System.Collections;
using Vereniging;

public class V078_AdresWerdOntkoppeldVanAdressenregister : IEventsInDbScenario
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly List<AdresWerdOvergenomenUitAdressenregister> AdresWerdOvergenomenUitAdressenregisterList;
    public readonly AdresWerdOntkoppeldVanAdressenregister AdresWerdOntkoppeldVanAdressenregister;
    public readonly AdresMatchUitAdressenregister AdresMatchUitAdressenregister;
    public readonly CommandMetadata Metadata;

    public V078_AdresWerdOntkoppeldVanAdressenregister()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VCode = "V9999078";
        Naam = "Dee sjiekste club";

        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            VCode = VCode,
            Naam = Naam,
            Locaties = new List<Registratiedata.Locatie>
            {
                fixture.Create<Registratiedata.Locatie>(),
                fixture.Create<Registratiedata.Locatie>(),
                fixture.Create<Registratiedata.Locatie>(),
            }.ToArray(),
        };

        AdresWerdOvergenomenUitAdressenregisterList = new List<AdresWerdOvergenomenUitAdressenregister>();

        AdresMatchUitAdressenregister = new AdresMatchUitAdressenregister
        {
            AdresId = new Registratiedata.AdresId(Adresbron.AR.Code, "https://data.vlaanderen.be/id/adres/12345"),
            Adres = new Registratiedata.Adres("Fosselstraat", "48", "", "1790", "Affligem", "België")
        };

        foreach (var locatie in FeitelijkeVerenigingWerdGeregistreerd.Locaties)
        {
            var adresId = fixture.Create<Registratiedata.AdresId>();
            var @event = fixture.Create<AdresWerdOvergenomenUitAdressenregister>()
                with
                {
                    VCode = VCode,
                    LocatieId = locatie.LocatieId,
                    OvergenomenAdresUitAdressenregister = AdresMatchUitAdressenregister with
                    {
                        AdresId = adresId
                    }
                };

            AdresWerdOvergenomenUitAdressenregisterList.Add(@event);
            ExpectedAdresIds.Add(adresId);
        }

        AdresWerdOntkoppeldVanAdressenregister = fixture.Create<AdresWerdOntkoppeldVanAdressenregister>() with
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
    public HashSet<Registratiedata.AdresId> ExpectedAdresIds { get; set; } = new();

    public IEvent[] GetEvents()
        => new IEvent[]
            {
                FeitelijkeVerenigingWerdGeregistreerd,
            }.Concat(AdresWerdOvergenomenUitAdressenregisterList.ToArray())
             .Concat(new IEvent[]
              {
                  AdresWerdOntkoppeldVanAdressenregister,
              })
             .ToArray();

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
