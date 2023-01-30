namespace AssociationRegistry.Test.Admin.Api.TakeTwo;

using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using Framework;

public interface IScenario
{
    string VCode { get; set; }
    StreamActionResult Result { get; set; }
    IEvent[] GetEvents();
    CommandMetadata GetCommandMetadata();
}

public class VerenigingWerdGeregistreerdScenario : IScenario
{
    public readonly VerenigingWerdGeregistreerd VerenigingWerdGeregistreerd;
    public readonly CommandMetadata Metadata;

    public VerenigingWerdGeregistreerdScenario()
    {
        var fixture = new Fixture().CustomizeAll();
        VCode = "V0001001";
        VerenigingWerdGeregistreerd = fixture.Create<VerenigingWerdGeregistreerd>() with { VCode = VCode };
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

public class VerenigingWerdGeregistreerdWithMinimalFieldsScenario : IScenario
{
    public readonly VerenigingWerdGeregistreerd VerenigingWerdGeregistreerd;
    public readonly CommandMetadata Metadata;

    public VerenigingWerdGeregistreerdWithMinimalFieldsScenario()
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

public class AlleBasisGegevensWerdenGewijzigdScenario : IScenario
{
    public readonly VerenigingWerdGeregistreerd VerenigingWerdGeregistreerd;
    public readonly NaamWerdGewijzigd NaamWerdGewijzigd;
    public readonly KorteNaamWerdGewijzigd KorteNaamWerdGewijzigd;
    public readonly KorteBeschrijvingWerdGewijzigd KorteBeschrijvingWerdGewijzigd;
    public readonly CommandMetadata Metadata;

    public AlleBasisGegevensWerdenGewijzigdScenario()
    {
        var fixture = new Fixture().CustomizeAll();
        VCode = "V0001003";
        VerenigingWerdGeregistreerd = fixture.Create<VerenigingWerdGeregistreerd>() with { VCode = VCode };
        NaamWerdGewijzigd = fixture.Create<NaamWerdGewijzigd>() with { VCode = VCode };
        KorteNaamWerdGewijzigd = fixture.Create<KorteNaamWerdGewijzigd>() with { VCode = VCode };
        KorteBeschrijvingWerdGewijzigd = fixture.Create<KorteBeschrijvingWerdGewijzigd>() with { VCode = VCode };
        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;

    public IEvent[] GetEvents()
        => new IEvent[] { VerenigingWerdGeregistreerd, NaamWerdGewijzigd, KorteNaamWerdGewijzigd, KorteBeschrijvingWerdGewijzigd };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
