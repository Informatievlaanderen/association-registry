namespace AssociationRegistry.Test.Acm.Api.Fixtures.Scenarios;

using Events;
using EventStore;
using AssociationRegistry.Framework;
using AutoFixture;
using Framework;
using Vereniging;

public interface IEventsInDbScenario
{
    string VCode { get; set; }
    StreamActionResult Result { get; set; }
    IEvent[] GetEvents();
    CommandMetadata GetCommandMetadata();
}

public class VerenigingWerdGeregistreerd_WithAllFields_EventsInDbScenario : IEventsInDbScenario
{
    public readonly VerenigingWerdGeregistreerd VerenigingWerdGeregistreerd;
    public readonly CommandMetadata Metadata;

    public VerenigingWerdGeregistreerd_WithAllFields_EventsInDbScenario()
    {
        var fixture = new Fixture().CustomizeAll();
        VCode = "V0003001";
        VerenigingWerdGeregistreerd = fixture.Create<VerenigingWerdGeregistreerd>() with { VCode = VCode };
        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;

    public string Insz
        => VerenigingWerdGeregistreerd.Vertegenwoordigers[0].Insz;

    public IEvent[] GetEvents()
        => new IEvent[]
            { VerenigingWerdGeregistreerd };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}

public class VertegenwoordigerWerdToegevoegd_EventsInDbScenario : IEventsInDbScenario
{
    public readonly VerenigingWerdGeregistreerd VerenigingWerdGeregistreerd;
    public readonly VertegenwoordigerWerdToegevoegd VertegenwoordigerWerdToegevoegd;
    public readonly CommandMetadata Metadata;

    public VertegenwoordigerWerdToegevoegd_EventsInDbScenario()
    {
        var fixture = new Fixture().CustomizeAll();
        VCode = "V0003002";
        VerenigingWerdGeregistreerd = fixture.Create<VerenigingWerdGeregistreerd>() with
        {
            VCode = VCode,
            Locaties = Array.Empty<VerenigingWerdGeregistreerd.Locatie>(),
            KorteNaam = string.Empty,
            KboNummer = string.Empty,
            Startdatum = null,
            KorteBeschrijving = string.Empty,
            Contactgegevens = Array.Empty<VerenigingWerdGeregistreerd.Contactgegeven>(),
            Vertegenwoordigers = Array.Empty<VerenigingWerdGeregistreerd.Vertegenwoordiger>(),
        };
        VertegenwoordigerWerdToegevoegd = fixture.Create<VertegenwoordigerWerdToegevoegd>();
        Insz = VertegenwoordigerWerdToegevoegd.Insz;
        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; }
    public string Insz { get; set; }
    public StreamActionResult Result { get; set; } = null!;

    public IEvent[] GetEvents()
        => new IEvent[]
        {
            VerenigingWerdGeregistreerd,
            VertegenwoordigerWerdToegevoegd,
        };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}

public class AlleBasisGegevensWerdenGewijzigd_EventsInDbScenario : IEventsInDbScenario
{
    public readonly VerenigingWerdGeregistreerd VerenigingWerdGeregistreerd;
    public readonly NaamWerdGewijzigd NaamWerdGewijzigd;
    public readonly KorteNaamWerdGewijzigd KorteNaamWerdGewijzigd;
    public readonly KorteBeschrijvingWerdGewijzigd KorteBeschrijvingWerdGewijzigd;
    public readonly CommandMetadata Metadata;

    public AlleBasisGegevensWerdenGewijzigd_EventsInDbScenario()
    {
        var fixture = new Fixture().CustomizeAll();
        VCode = "V0003004";
        VerenigingWerdGeregistreerd = fixture.Create<VerenigingWerdGeregistreerd>() with { VCode = VCode };
        VerenigingWerdGeregistreerd = VerenigingWerdGeregistreerd with { Vertegenwoordigers = VerenigingWerdGeregistreerd.Vertegenwoordigers.Take(1).ToArray() };
        NaamWerdGewijzigd = fixture.Create<NaamWerdGewijzigd>() with { VCode = VCode };
        KorteNaamWerdGewijzigd = fixture.Create<KorteNaamWerdGewijzigd>() with { VCode = VCode };
        KorteBeschrijvingWerdGewijzigd = fixture.Create<KorteBeschrijvingWerdGewijzigd>() with { VCode = VCode };
        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;

    public string Insz
        => VerenigingWerdGeregistreerd.Vertegenwoordigers[0].Insz;

    public IEvent[] GetEvents()
        => new IEvent[]
        {
            VerenigingWerdGeregistreerd,
            NaamWerdGewijzigd,
            KorteNaamWerdGewijzigd,
            KorteBeschrijvingWerdGewijzigd,
        };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
