// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable InconsistentNaming
namespace AssociationRegistry.Test.Acm.Api.Fixtures.Scenarios;

using Admin.Schema.Persoonsgegevens;
using AssociationRegistry.Framework;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;
using EventStore;
using Framework;
using MartenDb.Store;
using Vereniging;

public interface IEventsInDbScenario
{
    string VCode { get; set; }
    StreamActionResult Result { get; set; }
    IEvent[] GetEvents();
    CommandMetadata GetCommandMetadata();
    VertegenwoordigerPersoonsgegevensDocument[] GetVertegenwoordigerPersoonsgegevens();
}

public class FeitelijkeVerenigingWerdGeregistreerd_WithAllFields_EventsInDbScenario : IEventsInDbScenario
{
    public readonly FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid
        FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid;

    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly CommandMetadata Metadata;
    public readonly Verenigingstype Verenigingstype = Verenigingstype.FeitelijkeVereniging;
    public List<VertegenwoordigerPersoonsgegevensDocument> VertegenwoordigerPersoonsgegevensDocuments = new();


    public FeitelijkeVerenigingWerdGeregistreerd_WithAllFields_EventsInDbScenario()
    {
        var fixture = new Fixture().CustomizeAcmApi();
        VCode = "V0003001";

        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with { VCode = VCode };

        foreach (var vertegenwoordiger in FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordigers)
        {
            VertegenwoordigerPersoonsgegevensDocuments.Add(fixture.Create<VertegenwoordigerPersoonsgegevensDocument>() with
            {
                RefId = vertegenwoordiger.RefId,
                VertegenwoordigerId = vertegenwoordiger.VertegenwoordigerId,
                VCode = VCode,
            });
        }

        FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid =
            new FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(VCode: VCode);

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

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

    public VertegenwoordigerPersoonsgegevensDocument[] GetVertegenwoordigerPersoonsgegevens()
        => VertegenwoordigerPersoonsgegevensDocuments.ToArray();
}


public class VertegenwoordigerWerdToegevoegd_EventsInDbScenario : IEventsInDbScenario
{
    public readonly FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid
        FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid;

    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly CommandMetadata Metadata;
    public readonly Verenigingstype Verenigingstype = Verenigingstype.FeitelijkeVereniging;
    public readonly VertegenwoordigerWerdToegevoegd VertegenwoordigerWerdToegevoegd;
    public readonly VertegenwoordigerPersoonsgegevensDocument VertegenwoordigerPersoonsgegevensDocument;
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

        var refId = Guid.NewGuid();
        VertegenwoordigerWerdToegevoegd = fixture.Create<VertegenwoordigerWerdToegevoegd>() with
        {
            RefId = refId,
        };

        VertegenwoordigerPersoonsgegevensDocument = fixture.Create<VertegenwoordigerPersoonsgegevensDocument>() with
        {
            RefId = refId,
            VertegenwoordigerId = VertegenwoordigerWerdToegevoegd.VertegenwoordigerId,
            VCode = VCode,
        };
        Insz = VertegenwoordigerPersoonsgegevensDocument.Insz;
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

    public VertegenwoordigerPersoonsgegevensDocument[] GetVertegenwoordigerPersoonsgegevens()
        => [VertegenwoordigerPersoonsgegevensDocument];
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
    public readonly VertegenwoordigerPersoonsgegevensDocument VertegenwoordigerPersoonsgegevensDocument;

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
        var refId = Guid.NewGuid();
        VertegenwoordigerWerdToegevoegd = fixture.Create<VertegenwoordigerWerdToegevoegd>() with
        {
            RefId = refId,
        };

        VertegenwoordigerPersoonsgegevensDocument = fixture.Create<VertegenwoordigerPersoonsgegevensDocument>() with
        {
            RefId = refId,
            VertegenwoordigerId = VertegenwoordigerWerdToegevoegd.VertegenwoordigerId,
            VCode = VCode,
        };
        Insz = VertegenwoordigerPersoonsgegevensDocument.Insz;
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

    public VertegenwoordigerPersoonsgegevensDocument[] GetVertegenwoordigerPersoonsgegevens()
        => [VertegenwoordigerPersoonsgegevensDocument];
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
    public List<VertegenwoordigerPersoonsgegevensDocument> VertegenwoordigerPersoonsgegevensDocuments = new();

    public AlleBasisGegevensWerdenGewijzigd_EventsInDbScenario()
    {
        var fixture = new Fixture().CustomizeAcmApi();
        VCode = "V0003004";
        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with { VCode = VCode };

        foreach (var vertegenwoordiger in FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordigers)
        {
            VertegenwoordigerPersoonsgegevensDocuments.Add(fixture.Create<VertegenwoordigerPersoonsgegevensDocument>() with
            {
                RefId = vertegenwoordiger.RefId,
                VertegenwoordigerId = vertegenwoordiger.VertegenwoordigerId,
                VCode = VCode,
            });
        }

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
        => VertegenwoordigerPersoonsgegevensDocuments[0].Insz;

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

    public VertegenwoordigerPersoonsgegevensDocument[] GetVertegenwoordigerPersoonsgegevens()
        => VertegenwoordigerPersoonsgegevensDocuments.ToArray();
}

public class VertegenwoordigerWerdVerwijderd_EventsInDbScenario : IEventsInDbScenario
{
    public readonly FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid
        FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid;

    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly CommandMetadata Metadata;
    private readonly VertegenwoordigerWerdToegevoegd VertegenwoordigerWerdToegevoegd;
    public readonly VertegenwoordigerWerdVerwijderd VertegenwoordigerWerdVerwijderd;
    public readonly VertegenwoordigerPersoonsgegevensDocument VertegenwoordigerPersoonsgegevensDocument;

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

        var refId = Guid.NewGuid();
        VertegenwoordigerWerdToegevoegd = fixture.Create<VertegenwoordigerWerdToegevoegd>() with
        {
            RefId = refId,
        };

        VertegenwoordigerPersoonsgegevensDocument = fixture.Create<VertegenwoordigerPersoonsgegevensDocument>() with
        {
            RefId = refId,
            VertegenwoordigerId = VertegenwoordigerWerdToegevoegd.VertegenwoordigerId,
            VCode = VCode,
        };
        Insz = VertegenwoordigerPersoonsgegevensDocument.Insz;
        VertegenwoordigerWerdVerwijderd = new VertegenwoordigerWerdVerwijderd(
            refId,
            VertegenwoordigerPersoonsgegevensDocument.VertegenwoordigerId);

        Insz = VertegenwoordigerPersoonsgegevensDocument.Insz;
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

    public VertegenwoordigerPersoonsgegevensDocument[] GetVertegenwoordigerPersoonsgegevens()
        => [VertegenwoordigerPersoonsgegevensDocument];
}

public class FeitelijkeVerenigingWerdGestopt_EventsInDbScenario : IEventsInDbScenario
{
    public readonly FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid
        FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid;

    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly CommandMetadata Metadata;
    public readonly Verenigingstype Verenigingstype = Verenigingstype.FeitelijkeVereniging;
    public readonly VerenigingWerdGestopt VerenigingWerdGestopt;
    public readonly List<VertegenwoordigerPersoonsgegevensDocument> VertegenwoordigerPersoonsgegevensDocuments = new();

    public FeitelijkeVerenigingWerdGestopt_EventsInDbScenario()
    {
        var fixture = new Fixture().CustomizeAcmApi();
        VCode = "V0003008";
        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with { VCode = VCode };

        foreach (var vertegenwoordiger in FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordigers)
        {
            VertegenwoordigerPersoonsgegevensDocuments.Add(fixture.Create<VertegenwoordigerPersoonsgegevensDocument>() with
            {
                RefId = vertegenwoordiger.RefId,
                VertegenwoordigerId = vertegenwoordiger.VertegenwoordigerId,
                VCode = VCode,
            });
        }
        FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid =
            new FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(VCode: VCode);

        VerenigingWerdGestopt = fixture.Create<VerenigingWerdGestopt>();
        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string Insz
        => VertegenwoordigerPersoonsgegevensDocuments[0].Insz;

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

    public VertegenwoordigerPersoonsgegevensDocument[] GetVertegenwoordigerPersoonsgegevens()
        => VertegenwoordigerPersoonsgegevensDocuments.ToArray();
}

public class FeitelijkeVerenigingWerdVerwijderd_EventsInDbScenario : IEventsInDbScenario
{
    public readonly FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid
        FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid;

    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly CommandMetadata Metadata;
    public readonly VerenigingWerdVerwijderd VerenigingWerdVerwijderd;

    public List<VertegenwoordigerPersoonsgegevensDocument> VertegenwoordigerPersoonsgegevensDocuments = new();

    public FeitelijkeVerenigingWerdVerwijderd_EventsInDbScenario()
    {
        var fixture = new Fixture().CustomizeAcmApi();
        VCode = "V0003009";
        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with { VCode = VCode };
        foreach (var vertegenwoordiger in FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordigers)
        {
            VertegenwoordigerPersoonsgegevensDocuments.Add(fixture.Create<VertegenwoordigerPersoonsgegevensDocument>() with
            {
                RefId = vertegenwoordiger.RefId,
                VertegenwoordigerId = vertegenwoordiger.VertegenwoordigerId,
                VCode = VCode,
            });
        }
        FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid =
            new FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(VCode: VCode);

        VerenigingWerdVerwijderd = fixture.Create<VerenigingWerdVerwijderd>();
        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string Insz
        => VertegenwoordigerPersoonsgegevensDocuments[0].Insz;

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

    public VertegenwoordigerPersoonsgegevensDocument[] GetVertegenwoordigerPersoonsgegevens()
        => VertegenwoordigerPersoonsgegevensDocuments.ToArray();
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

    public VertegenwoordigerPersoonsgegevensDocument[] GetVertegenwoordigerPersoonsgegevens()
        => [];
}

public class RechtsvormWerdGewijzigdInKBO_EventsInDbScenario : IEventsInDbScenario
{
    public readonly CommandMetadata Metadata;
    public readonly RechtsvormWerdGewijzigdInKBO RechtsvormWerdGewijzigdInKBO;
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;
    public readonly VertegenwoordigerWerdToegevoegd VertegenwoordigerWerdToegevoegd;
    public readonly VertegenwoordigerPersoonsgegevensDocument VertegenwoordigerPersoonsgegevensDocument;

    public RechtsvormWerdGewijzigdInKBO_EventsInDbScenario()
    {
        var fixture = new Fixture().CustomizeAcmApi();
        VCode = "V0003011";

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with { VCode = VCode, Rechtsvorm = "VZW" };

        RechtsvormWerdGewijzigdInKBO = new RechtsvormWerdGewijzigdInKBO(Rechtsvorm: "IVZW");

        var refId = Guid.NewGuid();
        VertegenwoordigerWerdToegevoegd = fixture.Create<VertegenwoordigerWerdToegevoegd>() with
        {
            RefId = refId,
        };

        VertegenwoordigerPersoonsgegevensDocument = fixture.Create<VertegenwoordigerPersoonsgegevensDocument>() with
        {
            RefId = refId,
            VertegenwoordigerId = VertegenwoordigerWerdToegevoegd.VertegenwoordigerId,
            VCode = VCode,
        };
        Insz = VertegenwoordigerPersoonsgegevensDocument.Insz;
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

    public VertegenwoordigerPersoonsgegevensDocument[] GetVertegenwoordigerPersoonsgegevens()
        => [VertegenwoordigerPersoonsgegevensDocument];
}
