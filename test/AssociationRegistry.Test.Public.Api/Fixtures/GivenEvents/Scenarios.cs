namespace AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents;

using Events;
using AssociationRegistry.Framework;
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
    public VCode VCode
        => VCode.Create("V0001001");

    public readonly string Naam = "Feestcommittee Oudenaarde";
    public readonly string? KorteBeschrijving = "Het feestcommittee van Oudenaarde";
    public readonly string? KorteNaam = "FOud";
    public readonly string? KboNummer = "0123456789";

    public readonly VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket[] Hoofdactiviteiten =
    {
        new("BLA", "Buitengewoon Leuke Afkortingen"),
    };

    private readonly VerenigingWerdGeregistreerd.Contactgegeven _contactgegeven = new(
        1,
        ContactgegevenType.Email,
        "info@FOud.be",
        "Algemeen",
        true);

    private readonly VerenigingWerdGeregistreerd.Locatie _locatie = new(
        "Correspondentie",
        "Stationsstraat",
        "1",
        "B",
        "1790",
        "Affligem",
        "België",
        true,
        "Correspondentie");

    private readonly DateOnly? _startdatum = DateOnly.FromDateTime(new DateTime(2022, 11, 9));

    private readonly VerenigingWerdGeregistreerd.Vertegenwoordiger _vertegenwoordiger = new(
        "01234567890",
        true,
        "father",
        "Leader",
        "Odin",
        "Allfather",
        "asgard@world.tree",
        "",
        "",
        "");


    public IEvent[] GetEvents()
    {
        return new IEvent[]
        {
            new VerenigingWerdGeregistreerd(
                VCode,
                Naam,
                KorteNaam,
                KorteBeschrijving,
                _startdatum,
                KboNummer,
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
    public VCode VCode
        => VCode.Create("V0001002");

    private readonly string Naam = "Feesten Hulste";

    public IEvent[] GetEvents()
    {
        return new IEvent[]
        {
            new VerenigingWerdGeregistreerd(
                VCode,
                Naam,
                null,
                null,
                null,
                null,
                Array.Empty<VerenigingWerdGeregistreerd.Contactgegeven>(),
                Array.Empty<VerenigingWerdGeregistreerd.Locatie>(),
                Array.Empty<VerenigingWerdGeregistreerd.Vertegenwoordiger>(),
                Array.Empty<VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket>()),
        };
    }

    public CommandMetadata GetCommandMetadata()
        => new("OVO000001", new Instant());
}

public class V005_ContactgegevenWerdToegevoegdScenario : IScenario
{
    public VerenigingWerdGeregistreerd VerenigingWerdGeregistreerd = new(
        "V0001005",
        "Feesten Hulste",
        null,
        null,
        null,
        null,
        Array.Empty<VerenigingWerdGeregistreerd.Contactgegeven>(),
        Array.Empty<VerenigingWerdGeregistreerd.Locatie>(),
        Array.Empty<VerenigingWerdGeregistreerd.Vertegenwoordiger>(),
        Array.Empty<VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket>());

    public ContactgegevenWerdToegevoegd ContactgegevenWerdToegevoegd = new(1, ContactgegevenType.Email, "test@example.org", "de email om naar te sturen", false);

    public VCode VCode
        => VCode.Create(VerenigingWerdGeregistreerd.VCode);

    public IEvent[] GetEvents()
    {
        return new IEvent[]
        {
            VerenigingWerdGeregistreerd,
            ContactgegevenWerdToegevoegd,
        };
    }

    public CommandMetadata GetCommandMetadata()
        => new("OVO000001", new DateTimeOffset(2023, 01, 25, 0, 0, 0, TimeSpan.Zero).ToInstant());
}

public class V003_BasisgegevensWerdenGewijzigdScenario : IScenario
{
    public VCode VCode
        => VCode.Create("V0001003");

    public readonly string KorteBeschrijving = "Harelbeke";
    public readonly string KorteNaam = "OW";
    public readonly string Naam = "Oarelbeke Weireldstad";
    public readonly DateOnly Startdatum = new(2023, 6, 3);

    public readonly VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket[] Hoofdactiviteiten =
    {
        new("BLA", "Buitengewoon Leuke Afkortingen"),
    };


    public IEvent[] GetEvents()
    {
        return new IEvent[]
        {
            new VerenigingWerdGeregistreerd(
                VCode,
                "Foudenaarder feest",
                null,
                null,
                null,
                null,
                Array.Empty<VerenigingWerdGeregistreerd.Contactgegeven>(),
                Array.Empty<VerenigingWerdGeregistreerd.Locatie>(),
                Array.Empty<VerenigingWerdGeregistreerd.Vertegenwoordiger>(),
                Hoofdactiviteiten),
            new KorteBeschrijvingWerdGewijzigd(VCode, KorteBeschrijving),
            new NaamWerdGewijzigd(VCode, Naam),
            new KorteNaamWerdGewijzigd(VCode, KorteNaam),
            new StartdatumWerdGewijzigd(VCode, Startdatum),
        };
    }

    public CommandMetadata GetCommandMetadata()
        => new("OVO000001", new DateTimeOffset(2023, 01, 25, 0, 0, 0, TimeSpan.Zero).ToInstant());
}

public class V004_UnHandledEventAndVerenigingWerdGeregistreerdScenario : IScenario
{
    public VCode VCode
        => VCode.Create("V0001004");

    public readonly string Naam = "Oostende voor anker";
    private readonly string KorteNaam = "OVA";

    public static readonly VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket[] Hoofdactiviteiten =
    {
        new("BLA", "Buitengewoon Leuke Afkortingen"),
    };

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

    private static VerenigingWerdGeregistreerd VerenigingWerdGeregistreerd(string vCode, string naam, string? korteNaam)
        => new(
            vCode,
            naam,
            korteNaam,
            null,
            null,
            null,
            Array.Empty<VerenigingWerdGeregistreerd.Contactgegeven>(),
            Array.Empty<VerenigingWerdGeregistreerd.Locatie>(),
            Array.Empty<VerenigingWerdGeregistreerd.Vertegenwoordiger>(),
            Hoofdactiviteiten);
}

public record EenEvent : IEvent;
