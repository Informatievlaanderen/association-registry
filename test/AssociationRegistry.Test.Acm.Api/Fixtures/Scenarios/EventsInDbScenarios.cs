namespace AssociationRegistry.Test.Acm.Api.Fixtures.Scenarios;

using Events;
using EventStore;
using AssociationRegistry.Framework;
using AutoFixture;
using Framework;
using Marten.Events;
using IEvent = AssociationRegistry.Framework.IEvent;

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
        VCode = "V0001001";
        VerenigingWerdGeregistreerd = fixture.Create<VerenigingWerdGeregistreerd>() with { VCode = VCode };
        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;

    public string Insz
        => VerenigingWerdGeregistreerd.Vertegenwoordigers![0].Insz;

    public IEvent[] GetEvents()
        => new IEvent[]
            { VerenigingWerdGeregistreerd };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}

public class VerenigingWerdGeregistreerd_WithMinimalFields_EventsInDbScenario : IEventsInDbScenario
{
    public readonly VerenigingWerdGeregistreerd VerenigingWerdGeregistreerd;
    public readonly CommandMetadata Metadata;

    public VerenigingWerdGeregistreerd_WithMinimalFields_EventsInDbScenario()
    {
        var fixture = new Fixture().CustomizeAll();
        VCode = "V0001002";
        VerenigingWerdGeregistreerd = fixture.Create<VerenigingWerdGeregistreerd>() with
        {
            VCode = VCode,
            Locaties = Array.Empty<VerenigingWerdGeregistreerd.Locatie>(),
            KorteNaam = null,
            KboNummer = null,
            Startdatum = null,
            KorteBeschrijving = null,
            ContactInfoLijst = Array.Empty<VerenigingWerdGeregistreerd.ContactInfo>(),
            Vertegenwoordigers = Array.Empty<VerenigingWerdGeregistreerd.Vertegenwoordiger>(),
        };
        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;

    public IEvent[] GetEvents()
        => new IEvent[]
            { VerenigingWerdGeregistreerd };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}

public class VerenigingWerdGeregistreerd_ForUseWithNoChanges_EventsInDbScenario : IEventsInDbScenario
{
    public readonly VerenigingWerdGeregistreerd VerenigingWerdGeregistreerd;
    public readonly CommandMetadata Metadata;

    public VerenigingWerdGeregistreerd_ForUseWithNoChanges_EventsInDbScenario()
    {
        var fixture = new Fixture().CustomizeAll();
        VCode = "V0001003";
        VerenigingWerdGeregistreerd = fixture.Create<VerenigingWerdGeregistreerd>() with
        {
            VCode = VCode,
            Locaties = Array.Empty<VerenigingWerdGeregistreerd.Locatie>(),
            KorteNaam = null,
            KboNummer = null,
            Startdatum = null,
            KorteBeschrijving = null,
            ContactInfoLijst = Array.Empty<VerenigingWerdGeregistreerd.ContactInfo>(),
            Vertegenwoordigers = Array.Empty<VerenigingWerdGeregistreerd.Vertegenwoordiger>(),
        };
        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;

    public IEvent[] GetEvents()
        => new IEvent[]
            { VerenigingWerdGeregistreerd };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}

public class AlleBasisGegevensWerdenGewijzigd_EventsInDbScenario : IEventsInDbScenario
{
    public readonly VerenigingWerdGeregistreerd VerenigingWerdGeregistreerd;
    public VerenigingWerdGeregistreerd AndereVerenigingWerdGeregistreerd { get; set; }
    public readonly NaamWerdGewijzigd NaamWerdGewijzigd;
    public NaamWerdGewijzigd NaamAndereVerenigingWerdGewijzigd { get; set; }
    public readonly KorteNaamWerdGewijzigd KorteNaamWerdGewijzigd;
    public readonly KorteBeschrijvingWerdGewijzigd KorteBeschrijvingWerdGewijzigd;
    public readonly CommandMetadata Metadata;
    public NaamWerdGewijzigd NaamWerdOpnieuwGewijzigd { get; set; }


    public AlleBasisGegevensWerdenGewijzigd_EventsInDbScenario()
    {
        var fixture = new Fixture().CustomizeAll();
        VCode = "V0001004";
        VerenigingWerdGeregistreerd = fixture.Create<VerenigingWerdGeregistreerd>() with { VCode = VCode };
        AndereVerenigingWerdGeregistreerd = fixture.Create<VerenigingWerdGeregistreerd>();
        NaamWerdGewijzigd = fixture.Create<NaamWerdGewijzigd>() with { VCode = VCode };
        NaamAndereVerenigingWerdGewijzigd = fixture.Create<NaamWerdGewijzigd>() with { VCode = AndereVerenigingWerdGeregistreerd.VCode };
        KorteNaamWerdGewijzigd = fixture.Create<KorteNaamWerdGewijzigd>() with { VCode = VCode };
        KorteBeschrijvingWerdGewijzigd = fixture.Create<KorteBeschrijvingWerdGewijzigd>() with { VCode = VCode };
        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };

        // Second Batch
        NaamWerdOpnieuwGewijzigd = fixture.Create<NaamWerdGewijzigd>() with { VCode = VCode };
    }

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;

    public List<string> Inszs
        => VerenigingWerdGeregistreerd.Vertegenwoordigers.Select(x => x.Insz).ToList();

    public List<string> InszsAndereVereniging
        => AndereVerenigingWerdGeregistreerd.Vertegenwoordigers.Select(x => x.Insz).ToList();

    public IEvent[] GetEvents()
        => new IEvent[]
        {
            VerenigingWerdGeregistreerd,
            AndereVerenigingWerdGeregistreerd,
            NaamWerdGewijzigd,
            NaamAndereVerenigingWerdGewijzigd,
            KorteNaamWerdGewijzigd,
            KorteBeschrijvingWerdGewijzigd
        };

    public IEvent[] GetSecondBatchOfEvents()
        => new[] { NaamWerdOpnieuwGewijzigd };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}

public class VerenigingWerdGeregistreerd_ForUseWithETagMatching_EventsInDbScenario : IEventsInDbScenario
{
    public readonly VerenigingWerdGeregistreerd VerenigingWerdGeregistreerd;
    public readonly CommandMetadata Metadata;

    public VerenigingWerdGeregistreerd_ForUseWithETagMatching_EventsInDbScenario()
    {
        var fixture = new Fixture().CustomizeAll();
        VCode = "V0001005";
        VerenigingWerdGeregistreerd = fixture.Create<VerenigingWerdGeregistreerd>() with
        {
            VCode = VCode,
            Locaties = Array.Empty<VerenigingWerdGeregistreerd.Locatie>(),
            KorteNaam = null,
            KboNummer = null,
            Startdatum = null,
            KorteBeschrijving = null,
            ContactInfoLijst = Array.Empty<VerenigingWerdGeregistreerd.ContactInfo>(),
            Vertegenwoordigers = Array.Empty<VerenigingWerdGeregistreerd.Vertegenwoordiger>(),
        };
        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;

    public IEvent[] GetEvents()
        => new IEvent[]
            { VerenigingWerdGeregistreerd };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
