namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.EventsInDb;

using Events;
using EventStore;
using AssociationRegistry.Framework;
using Framework;
using AutoFixture;

public class V004_AlleBasisGegevensWerdenGewijzigd : IEventsInDbScenario
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly NaamWerdGewijzigd NaamWerdGewijzigd;
    public readonly KorteNaamWerdGewijzigd KorteNaamWerdGewijzigd;
    public readonly KorteBeschrijvingWerdGewijzigd KorteBeschrijvingWerdGewijzigd;
    public readonly StartdatumWerdGewijzigd StartdatumWerdGewijzigd;
    public readonly VerenigingWerdUitgeschrevenUitPubliekeDatastroom VerenigingWerdUitgeschrevenUitPubliekeDatastroom;
    public readonly VerenigingWerdIngeschrevenInPubliekeDatastroom VerenigingWerdIngeschrevenInPubliekeDatastroom;
    public readonly CommandMetadata Metadata;

    public V004_AlleBasisGegevensWerdenGewijzigd()
    {
        var fixture = new Fixture().CustomizeAll();
        VCode = "V9999004";
        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with { VCode = VCode };
        NaamWerdGewijzigd = fixture.Create<NaamWerdGewijzigd>() with { VCode = VCode };
        KorteNaamWerdGewijzigd = fixture.Create<KorteNaamWerdGewijzigd>() with { VCode = VCode };
        KorteBeschrijvingWerdGewijzigd = fixture.Create<KorteBeschrijvingWerdGewijzigd>() with { VCode = VCode };
        StartdatumWerdGewijzigd = fixture.Create<StartdatumWerdGewijzigd>() with { VCode = VCode };
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
        };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
