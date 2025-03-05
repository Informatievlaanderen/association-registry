namespace AssociationRegistry.Test.Acm.Api.Fixtures.Scenarios;

using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using EventStore;
using Framework;
using Vereniging;


public interface IEventsInDbScenario
{
    string VCode { get; set; }
    StreamActionResult Result { get; set; }
    IEvent[] GetEvents();
    CommandMetadata GetCommandMetadata();
}

public class FeitelijkeVerenigingWerdGeregistreerd_WithAllFields_EventsInDbScenario : IEventsInDbScenario
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly Verenigingstype Verenigingstype = Verenigingstype.FeitelijkeVereniging;
    public readonly CommandMetadata Metadata;

    public FeitelijkeVerenigingWerdGeregistreerd_WithAllFields_EventsInDbScenario()
    {
        var fixture = new Fixture().CustomizeAcmApi();
        VCode = "V0003001";
        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with { VCode = VCode };
        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;

    public string Insz
        => FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordigers[0].Insz;

    public IEvent[] GetEvents()
        => new IEvent[]
            { FeitelijkeVerenigingWerdGeregistreerd };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}

public class VertegenwoordigerWerdToegevoegd_EventsInDbScenario : IEventsInDbScenario
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly VertegenwoordigerWerdToegevoegd VertegenwoordigerWerdToegevoegd;
    public readonly Verenigingstype Verenigingstype = Verenigingstype.FeitelijkeVereniging;
    public readonly CommandMetadata Metadata;

    public VertegenwoordigerWerdToegevoegd_EventsInDbScenario()
    {
        var fixture = new Fixture().CustomizeAcmApi();
        VCode = "V0003002";

        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            VCode = VCode,
            Locaties = Array.Empty<Registratiedata.Locatie>(),
            KorteNaam = string.Empty,
            Startdatum = null,
            KorteBeschrijving = string.Empty,
            Contactgegevens = Array.Empty<Registratiedata.Contactgegeven>(),
            Vertegenwoordigers = Array.Empty<Registratiedata.Vertegenwoordiger>(),
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
            FeitelijkeVerenigingWerdGeregistreerd,
            VertegenwoordigerWerdToegevoegd,
        };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}

public class NaamWerdGewijzigd_And_VertegenwoordigerWerdToegevoegd_EventsInDbScenario : IEventsInDbScenario
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly VertegenwoordigerWerdToegevoegd VertegenwoordigerWerdToegevoegd;
    public readonly NaamWerdGewijzigd NaamWerdGewijzigd;
    public readonly Verenigingstype Verenigingstype = Verenigingstype.FeitelijkeVereniging;
    public readonly CommandMetadata Metadata;

    public NaamWerdGewijzigd_And_VertegenwoordigerWerdToegevoegd_EventsInDbScenario()
    {
        var fixture = new Fixture().CustomizeAcmApi();
        VCode = "V0003003";

        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            VCode = VCode,
            Locaties = Array.Empty<Registratiedata.Locatie>(),
            KorteNaam = string.Empty,
            Startdatum = null,
            KorteBeschrijving = string.Empty,
            Contactgegevens = Array.Empty<Registratiedata.Contactgegeven>(),
            Vertegenwoordigers = Array.Empty<Registratiedata.Vertegenwoordiger>(),
        };

        NaamWerdGewijzigd = fixture.Create<NaamWerdGewijzigd>() with { VCode = VCode };
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
            FeitelijkeVerenigingWerdGeregistreerd,
            NaamWerdGewijzigd,
            VertegenwoordigerWerdToegevoegd,
        };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}

public class AlleBasisGegevensWerdenGewijzigd_EventsInDbScenario : IEventsInDbScenario
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly NaamWerdGewijzigd NaamWerdGewijzigd;
    public readonly KorteNaamWerdGewijzigd KorteNaamWerdGewijzigd;
    public readonly KorteBeschrijvingWerdGewijzigd KorteBeschrijvingWerdGewijzigd;
    public readonly Verenigingstype Verenigingstype = Verenigingstype.FeitelijkeVereniging;
    public readonly CommandMetadata Metadata;

    public AlleBasisGegevensWerdenGewijzigd_EventsInDbScenario()
    {
        var fixture = new Fixture().CustomizeAcmApi();
        VCode = "V0003004";
        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with { VCode = VCode };

        FeitelijkeVerenigingWerdGeregistreerd = FeitelijkeVerenigingWerdGeregistreerd with
        {
            Vertegenwoordigers = FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordigers.Take(1).ToArray(),
        };

        NaamWerdGewijzigd = fixture.Create<NaamWerdGewijzigd>() with { VCode = VCode };
        KorteNaamWerdGewijzigd = fixture.Create<KorteNaamWerdGewijzigd>() with { VCode = VCode };
        KorteBeschrijvingWerdGewijzigd = fixture.Create<KorteBeschrijvingWerdGewijzigd>() with { VCode = VCode };
        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;

    public string Insz
        => FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordigers[0].Insz;

    public IEvent[] GetEvents()
        => new IEvent[]
        {
            FeitelijkeVerenigingWerdGeregistreerd,
            NaamWerdGewijzigd,
            KorteNaamWerdGewijzigd,
            KorteBeschrijvingWerdGewijzigd,
        };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}

public class VertegenwoordigerWerdVerwijderd_EventsInDbScenario : IEventsInDbScenario
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    private readonly VertegenwoordigerWerdToegevoegd VertegenwoordigerWerdToegevoegd;
    public readonly VertegenwoordigerWerdVerwijderd VertegenwoordigerWerdVerwijderd;
    public readonly CommandMetadata Metadata;

    public VertegenwoordigerWerdVerwijderd_EventsInDbScenario()
    {
        var fixture = new Fixture().CustomizeAcmApi();
        VCode = "V0003005";

        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            VCode = VCode,
            Locaties = Array.Empty<Registratiedata.Locatie>(),
            KorteNaam = string.Empty,
            Startdatum = null,
            KorteBeschrijving = string.Empty,
            Contactgegevens = Array.Empty<Registratiedata.Contactgegeven>(),
            Vertegenwoordigers = Array.Empty<Registratiedata.Vertegenwoordiger>(),
        };

        VertegenwoordigerWerdToegevoegd = fixture.Create<VertegenwoordigerWerdToegevoegd>();

        VertegenwoordigerWerdVerwijderd = new VertegenwoordigerWerdVerwijderd(
            VertegenwoordigerWerdToegevoegd.VertegenwoordigerId,
            VertegenwoordigerWerdToegevoegd.Insz,
            VertegenwoordigerWerdToegevoegd.Voornaam,
            VertegenwoordigerWerdToegevoegd.Achternaam);

        Insz = VertegenwoordigerWerdToegevoegd.Insz;
        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; }
    public string Insz { get; set; }
    public StreamActionResult Result { get; set; } = null!;

    public IEvent[] GetEvents()
        => new IEvent[]
        {
            FeitelijkeVerenigingWerdGeregistreerd,
            VertegenwoordigerWerdToegevoegd,
            VertegenwoordigerWerdVerwijderd,
        };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}

public class FeitelijkeVerenigingWerdGestopt_EventsInDbScenario : IEventsInDbScenario
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly VerenigingWerdGestopt VerenigingWerdGestopt;
    public readonly Verenigingstype Verenigingstype = Verenigingstype.FeitelijkeVereniging;
    public readonly CommandMetadata Metadata;

    public FeitelijkeVerenigingWerdGestopt_EventsInDbScenario()
    {
        var fixture = new Fixture().CustomizeAcmApi();
        VCode = "V0003008";
        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with { VCode = VCode };
        VerenigingWerdGestopt = fixture.Create<VerenigingWerdGestopt>();
        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;

    public string Insz
        => FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordigers[0].Insz;

    public IEvent[] GetEvents()
        => new IEvent[]
        {
            FeitelijkeVerenigingWerdGeregistreerd,
            VerenigingWerdGestopt,
        };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}

public class FeitelijkeVerenigingWerdVerwijderd_EventsInDbScenario : IEventsInDbScenario
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly VerenigingWerdVerwijderd VerenigingWerdVerwijderd;
    public readonly CommandMetadata Metadata;

    public FeitelijkeVerenigingWerdVerwijderd_EventsInDbScenario()
    {
        var fixture = new Fixture().CustomizeAcmApi();
        VCode = "V0003009";
        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with { VCode = VCode };
        VerenigingWerdVerwijderd = fixture.Create<VerenigingWerdVerwijderd>();
        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;

    public string Insz
        => FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordigers[0].Insz;

    public IEvent[] GetEvents()
        => new IEvent[]
        {
            FeitelijkeVerenigingWerdGeregistreerd,
            VerenigingWerdVerwijderd,
        };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}

public class VerenigingMetRechtspersoonlijkheid_WithAllFields_EventsInDbScenario : IEventsInDbScenario
{
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;
    public readonly VertegenwoordigerWerdOvergenomenUitKBO VertegenwoordigerWerdOvergenomenUitKBO;
    public readonly NaamWerdGewijzigdInKbo NaamWerdGewijzigdInKbo;
    public readonly CommandMetadata Metadata;

    public VerenigingMetRechtspersoonlijkheid_WithAllFields_EventsInDbScenario()
    {
        var fixture = new Fixture().CustomizeAcmApi();
        VCode = "V0003010";
        Insz = fixture.Create<Insz>().Value;

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with { VCode = VCode };

        VertegenwoordigerWerdOvergenomenUitKBO = fixture.Create<VertegenwoordigerWerdOvergenomenUitKBO>() with
        {
            Insz = Insz,
            VertegenwoordigerId = 12,
        };

        NaamWerdGewijzigdInKbo = fixture.Create<NaamWerdGewijzigdInKbo>();
        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;
    public string Insz { get; set; }

    public IEvent[] GetEvents()
        => new IEvent[]
        {
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            VertegenwoordigerWerdOvergenomenUitKBO,
            NaamWerdGewijzigdInKbo,
        };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}

public class RechtsvormWerdGewijzigdInKBO_EventsInDbScenario : IEventsInDbScenario
{
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;
    public readonly RechtsvormWerdGewijzigdInKBO RechtsvormWerdGewijzigdInKBO;
    public readonly VertegenwoordigerWerdToegevoegd VertegenwoordigerWerdToegevoegd;
    public readonly CommandMetadata Metadata;

    public RechtsvormWerdGewijzigdInKBO_EventsInDbScenario()
    {
        var fixture = new Fixture().CustomizeAcmApi();
        VCode = "V0003011";

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with { VCode = VCode, Rechtsvorm = "VZW" };

        RechtsvormWerdGewijzigdInKBO = fixture.Create<RechtsvormWerdGewijzigdInKBO>() with
        {
            Rechtsvorm = "IVZW",
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
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            RechtsvormWerdGewijzigdInKBO,
            VertegenwoordigerWerdToegevoegd,
        };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
