// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable InconsistentNaming
namespace AssociationRegistry.Test.Acm.Api.Fixtures.Scenarios;

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
    public readonly FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid
        FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid;

    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly CommandMetadata Metadata;
    public readonly Verenigingstype Verenigingstype = Verenigingstype.FeitelijkeVereniging;

    public FeitelijkeVerenigingWerdGeregistreerd_WithAllFields_EventsInDbScenario()
    {
        var fixture = new Fixture().CustomizeAcmApi();
        VCode = "V0003001";

        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with { VCode = VCode };

        FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid =
            new FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(VCode: VCode);

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string Insz
        => FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordigers[0].Insz;

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;

    public IEvent[] GetEvents()
        =>
        [
            FeitelijkeVerenigingWerdGeregistreerd,
            FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid,
        ];

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}


public class VertegenwoordigerWerdToegevoegd_EventsInDbScenario : IEventsInDbScenario
{
    public readonly FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid
        FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid;

    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly CommandMetadata Metadata;
    public readonly Verenigingstype Verenigingstype = Verenigingstype.FeitelijkeVereniging;
    public readonly VertegenwoordigerWerdToegevoegd VertegenwoordigerWerdToegevoegd;

    public VertegenwoordigerWerdToegevoegd_EventsInDbScenario()
    {
        var fixture = new Fixture().CustomizeAcmApi();
        VCode = "V0003002";

        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            VCode = VCode,
            Locaties = [],
            KorteNaam = string.Empty,
            Startdatum = null,
            KorteBeschrijving = string.Empty,
            Contactgegevens = [],
            Vertegenwoordigers = [],
        };

        FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid = new(VCode);

        VertegenwoordigerWerdToegevoegd = fixture.Create<VertegenwoordigerWerdToegevoegd>();
        Insz = VertegenwoordigerWerdToegevoegd.Insz;
        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string Insz { get; set; }
    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;

    public IEvent[] GetEvents()
        =>
        [
            FeitelijkeVerenigingWerdGeregistreerd,
            FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid, VertegenwoordigerWerdToegevoegd,
        ];

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}

public class NaamWerdGewijzigd_And_VertegenwoordigerWerdToegevoegd_EventsInDbScenario : IEventsInDbScenario
{
    public readonly FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid
        FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid;

    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly CommandMetadata Metadata;
    public readonly NaamWerdGewijzigd NaamWerdGewijzigd;
    public readonly Verenigingstype Verenigingstype = Verenigingstype.FeitelijkeVereniging;
    public readonly VertegenwoordigerWerdToegevoegd VertegenwoordigerWerdToegevoegd;

    public NaamWerdGewijzigd_And_VertegenwoordigerWerdToegevoegd_EventsInDbScenario()
    {
        var fixture = new Fixture().CustomizeAcmApi();
        VCode = "V0003003";

        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            VCode = VCode,
            Locaties = [],
            KorteNaam = string.Empty,
            Startdatum = null,
            KorteBeschrijving = string.Empty,
            Contactgegevens = [],
            Vertegenwoordigers = [],
        };

        FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid =
            new FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(VCode);

        NaamWerdGewijzigd = fixture.Create<NaamWerdGewijzigd>() with { VCode = VCode };
        VertegenwoordigerWerdToegevoegd = fixture.Create<VertegenwoordigerWerdToegevoegd>();
        Insz = VertegenwoordigerWerdToegevoegd.Insz;
        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string Insz { get; set; }
    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;

    public IEvent[] GetEvents()
        =>
        [
            FeitelijkeVerenigingWerdGeregistreerd,
            FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid, NaamWerdGewijzigd,
            VertegenwoordigerWerdToegevoegd,
        ];

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}

public class AlleBasisGegevensWerdenGewijzigd_EventsInDbScenario : IEventsInDbScenario
{
    public readonly FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid
        FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid;

    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly KorteBeschrijvingWerdGewijzigd KorteBeschrijvingWerdGewijzigd;
    public readonly KorteNaamWerdGewijzigd KorteNaamWerdGewijzigd;
    public readonly CommandMetadata Metadata;
    public readonly NaamWerdGewijzigd NaamWerdGewijzigd;
    public readonly Verenigingstype Verenigingstype = Verenigingstype.FeitelijkeVereniging;

    public AlleBasisGegevensWerdenGewijzigd_EventsInDbScenario()
    {
        var fixture = new Fixture().CustomizeAcmApi();
        VCode = "V0003004";
        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with { VCode = VCode };

        FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid =
            new FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(VCode: VCode);

        FeitelijkeVerenigingWerdGeregistreerd = FeitelijkeVerenigingWerdGeregistreerd with
        {
            Vertegenwoordigers = FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordigers.Take(1).ToArray(),
        };

        NaamWerdGewijzigd = fixture.Create<NaamWerdGewijzigd>() with { VCode = VCode };
        KorteNaamWerdGewijzigd = fixture.Create<KorteNaamWerdGewijzigd>() with { VCode = VCode };
        KorteBeschrijvingWerdGewijzigd = fixture.Create<KorteBeschrijvingWerdGewijzigd>() with { VCode = VCode };
        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string Insz
        => FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordigers[0].Insz;

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;

    public IEvent[] GetEvents()
        =>
        [
            FeitelijkeVerenigingWerdGeregistreerd,
            FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid, NaamWerdGewijzigd,
            KorteNaamWerdGewijzigd,
            KorteBeschrijvingWerdGewijzigd,
        ];

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}

public class VertegenwoordigerWerdVerwijderd_EventsInDbScenario : IEventsInDbScenario
{
    public readonly FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid
        FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid;

    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly CommandMetadata Metadata;
    private readonly VertegenwoordigerWerdToegevoegd VertegenwoordigerWerdToegevoegd;
    public readonly VertegenwoordigerWerdVerwijderd VertegenwoordigerWerdVerwijderd;

    public VertegenwoordigerWerdVerwijderd_EventsInDbScenario()
    {
        var fixture = new Fixture().CustomizeAcmApi();
        VCode = "V0003005";

        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            VCode = VCode,
            Locaties = [],
            KorteNaam = string.Empty,
            Startdatum = null,
            KorteBeschrijving = string.Empty,
            Contactgegevens = [],
            Vertegenwoordigers = [],
        };

        FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid =
            new FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(VCode);

        VertegenwoordigerWerdToegevoegd = fixture.Create<VertegenwoordigerWerdToegevoegd>();

        VertegenwoordigerWerdVerwijderd = new VertegenwoordigerWerdVerwijderd(
            VertegenwoordigerWerdToegevoegd.VertegenwoordigerId,
            VertegenwoordigerWerdToegevoegd.Insz,
            VertegenwoordigerWerdToegevoegd.Voornaam,
            VertegenwoordigerWerdToegevoegd.Achternaam);

        Insz = VertegenwoordigerWerdToegevoegd.Insz;
        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string Insz { get; set; }
    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;

    public IEvent[] GetEvents()
        =>
        [
            FeitelijkeVerenigingWerdGeregistreerd,
            FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid, VertegenwoordigerWerdToegevoegd,
            VertegenwoordigerWerdVerwijderd,
        ];

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}

public class FeitelijkeVerenigingWerdGestopt_EventsInDbScenario : IEventsInDbScenario
{
    public readonly FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid
        FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid;

    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly CommandMetadata Metadata;
    public readonly Verenigingstype Verenigingstype = Verenigingstype.FeitelijkeVereniging;
    public readonly VerenigingWerdGestopt VerenigingWerdGestopt;

    public FeitelijkeVerenigingWerdGestopt_EventsInDbScenario()
    {
        var fixture = new Fixture().CustomizeAcmApi();
        VCode = "V0003008";
        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with { VCode = VCode };

        FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid =
            new FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(VCode: VCode);

        VerenigingWerdGestopt = fixture.Create<VerenigingWerdGestopt>();
        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string Insz
        => FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordigers[0].Insz;

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;

    public IEvent[] GetEvents()
        =>
        [
            FeitelijkeVerenigingWerdGeregistreerd,
            FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid, VerenigingWerdGestopt,
        ];

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}

public class FeitelijkeVerenigingWerdVerwijderd_EventsInDbScenario : IEventsInDbScenario
{
    public readonly FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid
        FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid;

    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly CommandMetadata Metadata;
    public readonly VerenigingWerdVerwijderd VerenigingWerdVerwijderd;

    public FeitelijkeVerenigingWerdVerwijderd_EventsInDbScenario()
    {
        var fixture = new Fixture().CustomizeAcmApi();
        VCode = "V0003009";
        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with { VCode = VCode };

        FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid =
            new FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(VCode: VCode);

        VerenigingWerdVerwijderd = fixture.Create<VerenigingWerdVerwijderd>();
        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string Insz
        => FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordigers[0].Insz;

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;

    public IEvent[] GetEvents()
        =>
        [
            FeitelijkeVerenigingWerdGeregistreerd,
            FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid,
            VerenigingWerdVerwijderd,
        ];

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}

public class VerenigingMetRechtspersoonlijkheid_WithAllFields_EventsInDbScenario : IEventsInDbScenario
{
    public readonly CommandMetadata Metadata;
    public readonly NaamWerdGewijzigdInKbo NaamWerdGewijzigdInKbo;
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;
    public readonly VertegenwoordigerWerdOvergenomenUitKBO VertegenwoordigerWerdOvergenomenUitKBO;

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

    public string Insz { get; set; }
    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;

    public IEvent[] GetEvents()
        =>
        [
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            VertegenwoordigerWerdOvergenomenUitKBO,
            NaamWerdGewijzigdInKbo,
        ];

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}

public class RechtsvormWerdGewijzigdInKBO_EventsInDbScenario : IEventsInDbScenario
{
    public readonly CommandMetadata Metadata;
    public readonly RechtsvormWerdGewijzigdInKBO RechtsvormWerdGewijzigdInKBO;
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;
    public readonly VertegenwoordigerWerdToegevoegd VertegenwoordigerWerdToegevoegd;

    public RechtsvormWerdGewijzigdInKBO_EventsInDbScenario()
    {
        var fixture = new Fixture().CustomizeAcmApi();
        VCode = "V0003011";

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with { VCode = VCode, Rechtsvorm = "VZW" };

        RechtsvormWerdGewijzigdInKBO = new RechtsvormWerdGewijzigdInKBO(Rechtsvorm: "IVZW");

        VertegenwoordigerWerdToegevoegd = fixture.Create<VertegenwoordigerWerdToegevoegd>();
        Insz = VertegenwoordigerWerdToegevoegd.Insz;
        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string Insz { get; set; }
    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;

    public IEvent[] GetEvents()
        =>
        [
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            RechtsvormWerdGewijzigdInKBO,
            VertegenwoordigerWerdToegevoegd,
        ];

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
