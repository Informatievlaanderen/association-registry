namespace AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents;

using AssociationRegistry.Framework;
using Events;
using NodaTime;
using NodaTime.Extensions;
using Vereniging;

public interface IScenario
{
    public VCode VCode { get; }
    public IEvent[] GetEvents();
    public CommandMetadata GetCommandMetadata();
}

public class V001_FeitelijkeVerenigingWerdGeregistreerdScenario : IScenario
{
    private readonly Registratiedata.Contactgegeven _contactgegeven = new(
        ContactgegevenId: 1,
        ContactgegevenType.Email,
        "info@FOud.be",
        "Algemeen",
        IsPrimair: true);

    private readonly  Registratiedata.Locatie _locatie = new(
        "Correspondentie",
        "Stationsstraat",
        "1",
        "B",
        "1790",
        "Affligem",
        "België",
        Hoofdlocatie: true,
        "Correspondentie");

    private readonly DateOnly? _startdatum = DateOnly.FromDateTime(new DateTime(year: 2022, month: 11, day: 9));

    private readonly Registratiedata.Vertegenwoordiger _vertegenwoordiger = new(
        VertegenwoordigerId: 1,
        "01234567890",
        IsPrimair: true,
        "father",
        "Leader",
        "Odin",
        "Allfather",
        "asgard@world.tree",
        "",
        "",
        "");

    public readonly Registratiedata.HoofdactiviteitVerenigingsloket[] Hoofdactiviteiten =
    {
        new("BLA", "Buitengewoon Leuke Afkortingen"),
    };

    public readonly string? KorteBeschrijving = "Het feestcommittee van Oudenaarde";
    public readonly string? KorteNaam = "FOud";

    public readonly string Naam = "Feestcommittee Oudenaarde";

    public VCode VCode
        => VCode.Create("V0001001");


    public IEvent[] GetEvents()
    {
        return new IEvent[]
        {
            new FeitelijkeVerenigingWerdGeregistreerd(
                VCode,
                Naam,
                KorteNaam ?? string.Empty,
                KorteBeschrijving ?? string.Empty,
                _startdatum,
                new[] { _contactgegeven },
                new[] { _locatie },
                new[] { _vertegenwoordiger },
                Hoofdactiviteiten),
        };
    }

    public CommandMetadata GetCommandMetadata()
        => new("OVO000001", new Instant());
}

public class V002_FeitelijkeVerenigingWerdGeregistreerdWithMinimalFieldsScenario : IScenario
{
    private readonly string Naam = "Feesten Hulste";

    public VCode VCode
        => VCode.Create("V0001002");

    public IEvent[] GetEvents()
    {
        return new IEvent[]
        {
            new FeitelijkeVerenigingWerdGeregistreerd(
                VCode,
                Naam,
                string.Empty,
                string.Empty,
                Startdatum: null,
                Array.Empty<Registratiedata.Contactgegeven>(),
                Array.Empty<Registratiedata.Locatie>(),
                Array.Empty<Registratiedata.Vertegenwoordiger>(),
                Array.Empty<Registratiedata.HoofdactiviteitVerenigingsloket>()),
        };
    }

    public CommandMetadata GetCommandMetadata()
        => new("OVO000001", new Instant());
}

public class V005_ContactgegevenWerdToegevoegdScenario : IScenario
{
    public readonly ContactgegevenWerdToegevoegd ContactgegevenWerdToegevoegd = new(ContactgegevenId: 1, ContactgegevenType.Email, "test@example.org", "de email om naar te sturen", IsPrimair: false);

    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd = new(
        "V0001005",
        "Feesten Hulste",
        string.Empty,
        string.Empty,
        Startdatum: null,
        Array.Empty<Registratiedata.Contactgegeven>(),
        Array.Empty<Registratiedata.Locatie>(),
        Array.Empty<Registratiedata.Vertegenwoordiger>(),
        Array.Empty<Registratiedata.HoofdactiviteitVerenigingsloket>());

    public VCode VCode
        => VCode.Create(FeitelijkeVerenigingWerdGeregistreerd.VCode);

    public IEvent[] GetEvents()
    {
        return new IEvent[]
        {
            FeitelijkeVerenigingWerdGeregistreerd,
            ContactgegevenWerdToegevoegd,
        };
    }

    public CommandMetadata GetCommandMetadata()
        => new("OVO000001", new DateTimeOffset(year: 2023, month: 01, day: 25, hour: 0, minute: 0, second: 0, TimeSpan.Zero).ToInstant());
}

public class V006_VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario : IScenario
{
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd = new(
        "V0001006",
        "0000000000",
        "VZW",
        "VZW 0000000000",
        string.Empty,
        null);

    public VCode VCode
        => VCode.Create(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode);

    public IEvent[] GetEvents()
    {
        return new IEvent[]
        {
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
        };
    }

    public CommandMetadata GetCommandMetadata()
        => new("OVO000001", new DateTimeOffset(year: 2023, month: 01, day: 25, hour: 0, minute: 0, second: 0, TimeSpan.Zero).ToInstant());
}

public class V003_BasisgegevensWerdenGewijzigdScenario : IScenario
{
    public const string KorteBeschrijving = "Harelbeke";
    public const string KorteNaam = "OW";
    public const string Naam = "Oarelbeke Weireldstad";

    public readonly Registratiedata.HoofdactiviteitVerenigingsloket[] Hoofdactiviteiten =
    {
        new("BLA", "Buitengewoon Leuke Afkortingen"),
    };

    public readonly DateOnly Startdatum = new(year: 2023, month: 6, day: 3);

    public VCode VCode
        => VCode.Create("V0001003");


    public IEvent[] GetEvents()
    {
        return new IEvent[]
        {
            new FeitelijkeVerenigingWerdGeregistreerd(
                VCode,
                "Foudenaarder feest",
                string.Empty,
                string.Empty,
                Startdatum: null,
                Array.Empty<Registratiedata.Contactgegeven>(),
                Array.Empty<Registratiedata.Locatie>(),
                Array.Empty<Registratiedata.Vertegenwoordiger>(),
                Hoofdactiviteiten),
            new KorteBeschrijvingWerdGewijzigd(VCode, KorteBeschrijving),
            new NaamWerdGewijzigd(VCode, Naam),
            new KorteNaamWerdGewijzigd(VCode, KorteNaam),
            new StartdatumWerdGewijzigd(VCode, Startdatum),
        };
    }

    public CommandMetadata GetCommandMetadata()
        => new("OVO000001", new DateTimeOffset(year: 2023, month: 01, day: 25, hour: 0, minute: 0, second: 0, TimeSpan.Zero).ToInstant());
}

public class V004_UnHandledEventAndFeitelijkeVerenigingWerdGeregistreerdScenario : IScenario
{
    public const string Naam = "Oostende voor anker";
    private const string KorteNaam = "OVA";

    public static readonly Registratiedata.HoofdactiviteitVerenigingsloket[] Hoofdactiviteiten =
    {
        new("BLA", "Buitengewoon Leuke Afkortingen"),
    };

    public VCode VCode
        => VCode.Create("V0001004");

    public IEvent[] GetEvents()
    {
        return new IEvent[]
        {
            new EenEvent(),
            FeitelijkeVerenigingWerdGeregistreerd(
                VCode,
                Naam,
                KorteNaam),
        };
    }

    public CommandMetadata GetCommandMetadata()
        => new("OVO000001", new Instant());

    private static FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd(string vCode, string naam, string korteNaam)
        => new(
            vCode,
            naam,
            korteNaam,
            string.Empty,
            Startdatum: null,
            Array.Empty<Registratiedata.Contactgegeven>(),
            Array.Empty<Registratiedata.Locatie>(),
            Array.Empty<Registratiedata.Vertegenwoordiger>(),
            Hoofdactiviteiten);
}

public class V007_AfdelingWerdGeregistreerdScenario : IScenario
{
    private readonly Registratiedata.Contactgegeven _contactgegeven = new(
        ContactgegevenId: 1,
        ContactgegevenType.Email,
        "info@FOud.be",
        "Algemeen",
        IsPrimair: true);

    private readonly  Registratiedata.Locatie _locatie = new(
        "Correspondentie",
        "berglaan",
        "12",
        "B",
        "2000",
        "Antwerpen",
        "België",
        Hoofdlocatie: true,
        "Correspondentie");

    private readonly DateOnly? _startdatum = DateOnly.FromDateTime(new DateTime(year: 2022, month: 11, day: 9));

    private readonly Registratiedata.Vertegenwoordiger _vertegenwoordiger = new(
        VertegenwoordigerId: 1,
        "01234567890",
        IsPrimair: true,
        "father",
        "Leader",
        "Odin",
        "Allfather",
        "asgard@world.tree",
        "",
        "",
        "");

    public readonly Registratiedata.HoofdactiviteitVerenigingsloket[] Hoofdactiviteiten =
    {
        new("BLA", "Buitengewoon Leuke Afkortingen"),
    };

    public readonly string? KorteBeschrijving = "balpenverzamelaars van antwerpse bijscholingen";
    public readonly string? KorteNaam = "ABC";

    public readonly string Naam = "Antwerpse Bijscholing Clickers";

    public VCode VCode
        => VCode.Create("V0001007");


    public IEvent[] GetEvents()
    {
        return new IEvent[]
        {
            new AfdelingWerdGeregistreerd(
                VCode,
                Naam,
                "KBO Nummer",
                KorteNaam ?? string.Empty,
                KorteBeschrijving ?? string.Empty,
                _startdatum,
                new[] { _contactgegeven },
                new[] { _locatie },
                new[] { _vertegenwoordiger },
                Hoofdactiviteiten),
        };
    }

    public CommandMetadata GetCommandMetadata()
        => new("OVO000001", new Instant());
}

public record EenEvent : IEvent;
