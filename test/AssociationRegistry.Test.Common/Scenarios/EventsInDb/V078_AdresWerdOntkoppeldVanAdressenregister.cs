namespace AssociationRegistry.Test.Common.Scenarios.EventsInDb;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using EventStore;
using global::AutoFixture;
using MartenDb.Store;
using Vereniging;

public class V078_AdresWerdOntkoppeldVanAdressenregister : IEventsInDbScenario
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly List<AdresWerdOvergenomenUitAdressenregister> AdresWerdOvergenomenUitAdressenregisterList;
    public readonly AdresWerdOntkoppeldVanAdressenregister AdresWerdOntkoppeldVanAdressenregister;
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

        AdresId = new Registratiedata.AdresId(Adresbron.AR.Code, Bronwaarde: "https://data.vlaanderen.be/id/adres/12345");

        Adres = new Registratiedata.AdresUitAdressenregister(Straatnaam: "Fosselstraat", Huisnummer: "48", Busnummer: "", Postcode: "1790",
                                                             Gemeente: "Affligem");

        foreach (var locatie in FeitelijkeVerenigingWerdGeregistreerd.Locaties)
        {
            var adresId = fixture.Create<Registratiedata.AdresId>();

            var @event = fixture.Create<AdresWerdOvergenomenUitAdressenregister>()
                with
                {
                    VCode = VCode,
                    LocatieId = locatie.LocatieId,
                    AdresId = adresId,
                    Adres = Adres,
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

    public Registratiedata.AdresId AdresId { get; set; }
    public Registratiedata.AdresUitAdressenregister Adres { get; set; }
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
