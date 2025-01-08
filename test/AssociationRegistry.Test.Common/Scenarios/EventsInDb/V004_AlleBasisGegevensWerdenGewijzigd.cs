namespace AssociationRegistry.Test.Common.Scenarios.EventsInDb;

using AssociationRegistry.Framework;
using AutoFixture;
using EventFactories;
using Events;
using EventStore;
using global::AutoFixture;
using Vereniging;

public class V004_AlleBasisGegevensWerdenGewijzigd : IEventsInDbScenario
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly NaamWerdGewijzigd NaamWerdGewijzigd;
    public readonly KorteNaamWerdGewijzigd KorteNaamWerdGewijzigd;
    public readonly KorteBeschrijvingWerdGewijzigd KorteBeschrijvingWerdGewijzigd;
    public readonly StartdatumWerdGewijzigd StartdatumWerdGewijzigd;
    public readonly VerenigingWerdUitgeschrevenUitPubliekeDatastroom VerenigingWerdUitgeschrevenUitPubliekeDatastroom;
    public readonly VerenigingWerdIngeschrevenInPubliekeDatastroom VerenigingWerdIngeschrevenInPubliekeDatastroom;
    public readonly DoelgroepWerdGewijzigd DoelgroepWerdGewijzigd;
    public readonly WerkingsgebiedenWerdenGewijzigd WerkingsgebiedenWerdenGewijzigd;
    public readonly CommandMetadata Metadata;

    public V004_AlleBasisGegevensWerdenGewijzigd()
    {
        var startdatum = new DateOnly(year: 2023, month: 6, day: 14);
        var fixture = new Fixture().CustomizeAdminApi();
        VCode = "V9999004";

        FeitelijkeVerenigingWerdGeregistreerd = new FeitelijkeVerenigingWerdGeregistreerd(
            VCode,
            Naam: "Vereniging voor verwaarloosde vogels",
            KorteNaam: "VVVV",
            string.Empty,
            startdatum,
            EventFactory.Doelgroep(Doelgroep.Null),
            IsUitgeschrevenUitPubliekeDatastroom: false,
            Array.Empty<Registratiedata.Contactgegeven>(),
            Array.Empty<Registratiedata.Locatie>(),
            Array.Empty<Registratiedata.Vertegenwoordiger>(),
            new[]
            {
                new Registratiedata.HoofdactiviteitVerenigingsloket(Code: "BLA", Naam: "blabla"),
            }
        );

        NaamWerdGewijzigd = new NaamWerdGewijzigd(VCode, Naam: "Vrije Vogels van Verre Vertrekken");
        KorteNaamWerdGewijzigd = new KorteNaamWerdGewijzigd(VCode, KorteNaam: "VVVVV");
        KorteBeschrijvingWerdGewijzigd = new KorteBeschrijvingWerdGewijzigd(VCode, KorteBeschrijving: "Een vereniging voor vrij vogels");
        StartdatumWerdGewijzigd = new StartdatumWerdGewijzigd(VCode, startdatum);
        DoelgroepWerdGewijzigd = new DoelgroepWerdGewijzigd(new Registratiedata.Doelgroep(Minimumleeftijd: 12, Maximumleeftijd: 18));
        VerenigingWerdUitgeschrevenUitPubliekeDatastroom = new VerenigingWerdUitgeschrevenUitPubliekeDatastroom();
        VerenigingWerdIngeschrevenInPubliekeDatastroom = new VerenigingWerdIngeschrevenInPubliekeDatastroom();

        WerkingsgebiedenWerdenGewijzigd =
            new WerkingsgebiedenWerdenGewijzigd(VCode, fixture.CreateMany<Registratiedata.Werkingsgebied>().ToArray());

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;

    public IEvent[] GetEvents()
        => new IEvent[]
        {
            FeitelijkeVerenigingWerdGeregistreerd,
            NaamWerdGewijzigd,
            KorteNaamWerdGewijzigd,
            KorteBeschrijvingWerdGewijzigd,
            StartdatumWerdGewijzigd,
            VerenigingWerdUitgeschrevenUitPubliekeDatastroom,
            VerenigingWerdIngeschrevenInPubliekeDatastroom,
            DoelgroepWerdGewijzigd,
            WerkingsgebiedenWerdenGewijzigd,
        };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
