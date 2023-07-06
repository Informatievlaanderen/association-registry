namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.EventsInDb;

using Events;
using EventStore;
using AssociationRegistry.Framework;
using Framework;
using AutoFixture;
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
    public readonly CommandMetadata Metadata;

    public V004_AlleBasisGegevensWerdenGewijzigd()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VCode = "V9999004";
        FeitelijkeVerenigingWerdGeregistreerd = new FeitelijkeVerenigingWerdGeregistreerd(
            VCode,
            "Vereniging voor verwaarloosde vogels",
            "VVVV",
            string.Empty,
            null,
            Registratiedata.Doelgroep.With(Doelgroep.Null),
            false,
            Array.Empty<Registratiedata.Contactgegeven>(),
            Array.Empty<Registratiedata.Locatie>(),
            Array.Empty<Registratiedata.Vertegenwoordiger>(),
            new []
            {
                new Registratiedata.HoofdactiviteitVerenigingsloket("BLA", "blabla"),
            }
        );
        NaamWerdGewijzigd = new NaamWerdGewijzigd(VCode, "Vrije Vogels van Verre Vertrekken");
        KorteNaamWerdGewijzigd = new KorteNaamWerdGewijzigd(VCode, "VVVVV");
        KorteBeschrijvingWerdGewijzigd = new KorteBeschrijvingWerdGewijzigd(VCode, "Een vereniging voor vrij vogels");
        StartdatumWerdGewijzigd = new StartdatumWerdGewijzigd(VCode, new DateOnly(2023, 6, 14));
        DoelgroepWerdGewijzigd = new DoelgroepWerdGewijzigd(new Registratiedata.Doelgroep(12,18));
        VerenigingWerdUitgeschrevenUitPubliekeDatastroom = new VerenigingWerdUitgeschrevenUitPubliekeDatastroom();
        VerenigingWerdIngeschrevenInPubliekeDatastroom = new VerenigingWerdIngeschrevenInPubliekeDatastroom();
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
        };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
