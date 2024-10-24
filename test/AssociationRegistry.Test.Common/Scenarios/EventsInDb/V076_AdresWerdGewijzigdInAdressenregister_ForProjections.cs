namespace AssociationRegistry.Test.Common.Scenarios.EventsInDb;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using EventStore;
using global::AutoFixture;
using Vereniging;

public class V076_AdresWerdGewijzigdInAdressenregister : IEventsInDbScenario
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public Registratiedata.Locatie Locatie;
    public readonly AdresWerdGewijzigdInAdressenregister AdresWerdGewijzigdInAdressenregister;
    public readonly CommandMetadata Metadata;

    public V076_AdresWerdGewijzigdInAdressenregister()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VCode = "V9999076";
        Naam = "Dee sjiekste club";

        Locatie = fixture.Create<Registratiedata.Locatie>();

        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            VCode = VCode,
            Naam = Naam,
            Locaties = new List<Registratiedata.Locatie>
            {
                Locatie,
            }.ToArray(),
            HoofdactiviteitenVerenigingsloket = Array.Empty<Registratiedata.HoofdactiviteitVerenigingsloket>(),
            Werkingsgebieden = Array.Empty<Registratiedata.Werkingsgebied>(),
            Doelgroep = Registratiedata.Doelgroep.With(Doelgroep.Null),
            Vertegenwoordigers = Array.Empty<Registratiedata.Vertegenwoordiger>(),
            Contactgegevens = Array.Empty<Registratiedata.Contactgegeven>(),
        };

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

    public IEvent[] GetEvents()
        => new IEvent[]
           {
               FeitelijkeVerenigingWerdGeregistreerd,
           }
          .Concat(new IEvent[]
           {
               AdresWerdGewijzigdInAdressenregister,
           })
          .ToArray();

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
