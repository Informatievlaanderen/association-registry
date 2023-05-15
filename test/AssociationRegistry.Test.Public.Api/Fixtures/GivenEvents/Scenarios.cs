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

public class V001_VerenigingWerdGeregistreerdScenario : IScenario
{
    private readonly FeitelijkeVerenigingWerdGeregistreerd.Contactgegeven _contactgegeven = new(
        ContactgegevenId: 1,
        ContactgegevenType.Email,
        "info@FOud.be",
        "Algemeen",
        IsPrimair: true);

    private readonly FeitelijkeVerenigingWerdGeregistreerd.Locatie _locatie = new(
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

    private readonly FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordiger _vertegenwoordiger = new(
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

    public readonly FeitelijkeVerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket[] Hoofdactiviteiten =
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
                VerenigingsType.FeitelijkeVereniging.Code,
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

public class V002_VerenigingWerdGeregistreerdWithMinimalFieldsScenario : IScenario
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
                VerenigingsType.FeitelijkeVereniging.Code,
                Naam,
                string.Empty,
                string.Empty,
                Startdatum: null,
                Array.Empty<FeitelijkeVerenigingWerdGeregistreerd.Contactgegeven>(),
                Array.Empty<FeitelijkeVerenigingWerdGeregistreerd.Locatie>(),
                Array.Empty<FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordiger>(),
                Array.Empty<FeitelijkeVerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket>()),
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
        VerenigingsType.FeitelijkeVereniging.Code,
        "Feesten Hulste",
        string.Empty,
        string.Empty,
        Startdatum: null,
        Array.Empty<FeitelijkeVerenigingWerdGeregistreerd.Contactgegeven>(),
        Array.Empty<FeitelijkeVerenigingWerdGeregistreerd.Locatie>(),
        Array.Empty<FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordiger>(),
        Array.Empty<FeitelijkeVerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket>());

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

public class V003_BasisgegevensWerdenGewijzigdScenario : IScenario
{
    public const string KorteBeschrijving = "Harelbeke";
    public const string KorteNaam = "OW";
    public const string Naam = "Oarelbeke Weireldstad";

    public readonly FeitelijkeVerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket[] Hoofdactiviteiten =
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
                VerenigingsType.FeitelijkeVereniging.Code,
                "Foudenaarder feest",
                string.Empty,
                string.Empty,
                Startdatum: null,
                Array.Empty<FeitelijkeVerenigingWerdGeregistreerd.Contactgegeven>(),
                Array.Empty<FeitelijkeVerenigingWerdGeregistreerd.Locatie>(),
                Array.Empty<FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordiger>(),
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

public class V004_UnHandledEventAndVerenigingWerdGeregistreerdScenario : IScenario
{
    public const string Naam = "Oostende voor anker";
    private const string KorteNaam = "OVA";

    public static readonly FeitelijkeVerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket[] Hoofdactiviteiten =
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
            VerenigingWerdGeregistreerd(
                VCode,
                Naam,
                KorteNaam),
        };
    }

    public CommandMetadata GetCommandMetadata()
        => new("OVO000001", new Instant());

    private static FeitelijkeVerenigingWerdGeregistreerd VerenigingWerdGeregistreerd(string vCode, string naam, string korteNaam)
        => new(
            vCode,
            VerenigingsType.FeitelijkeVereniging.Code,
            naam,
            korteNaam,
            string.Empty,
            Startdatum: null,
            Array.Empty<FeitelijkeVerenigingWerdGeregistreerd.Contactgegeven>(),
            Array.Empty<FeitelijkeVerenigingWerdGeregistreerd.Locatie>(),
            Array.Empty<FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordiger>(),
            Hoofdactiviteiten);
}

public record EenEvent : IEvent;
