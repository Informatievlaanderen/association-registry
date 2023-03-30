namespace AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents;

using Events;
using AssociationRegistry.Framework;
using Events.CommonEventDataTypes;
using VCodes;
using NodaTime;
using NodaTime.Extensions;

public interface IScenario
{
    public VCode VCode { get; }
    public IEvent[] GetEvents();
    public CommandMetadata GetCommandMetadata();
}

public class VerenigingWerdGeregistreerdScenario : IScenario
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

    private readonly ContactInfo _contactInfo = new(
        "Algemeen",
        "info@FOud.be",
        "1111.11.11.11",
        "www.oudenaarde.be/feest",
        "#FOudenaarde",
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
        new[]
        {
            new ContactInfo(
                "Asgard",
                "asgard@world.tree",
                "0000000001",
                "www.asgard.tree",
                "#Asgard",
                false),
        });


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
                new[] { _contactInfo },
                new[] { _locatie },
                new[] { _vertegenwoordiger },
                Hoofdactiviteiten),
        };
    }

    public CommandMetadata GetCommandMetadata()
        => new("OVO000001", new Instant());
}

public class VerenigingWerdGeregistreerdWithMinimalFieldsScenario : IScenario
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
                Array.Empty<ContactInfo>(),
                Array.Empty<VerenigingWerdGeregistreerd.Locatie>(),
                Array.Empty<VerenigingWerdGeregistreerd.Vertegenwoordiger>(),
                Array.Empty<VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket>()),
        };
    }

    public CommandMetadata GetCommandMetadata()
        => new("OVO000001", new Instant());
}

public class ContactgegevenWerdToegevoegdScenario : IScenario
{
    public VerenigingWerdGeregistreerd VerenigingWerdGeregistreerd = new(
        "V0001005",
        "Feesten Hulste",
        null,
        null,
        null,
        null,
        Array.Empty<ContactInfo>(),
        Array.Empty<VerenigingWerdGeregistreerd.Locatie>(),
        Array.Empty<VerenigingWerdGeregistreerd.Vertegenwoordiger>(),
        Array.Empty<VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket>());

    public ContactgegevenWerdToegevoegd ContactgegevenWerdToegevoegd = new(1, "Email", "test@example.org", "de email om naar te sturen", false);

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

public class BasisgegevensWerdenGewijzigdScenario : IScenario
{
    private const string ContactInfoDieGewijzigdZalWordenNaam = "ContactNaamDieGewijzigdZalWorden";

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

    public readonly ContactInfo VerenigingContactInfo = new("Initiële waarde", "email@example.org", "9876543210", "http://example.org", "https://example.org/social", true);
    public readonly ContactInfo VerenigingContactInfoDieGewijzigdZalWorden = new(ContactInfoDieGewijzigdZalWordenNaam, "email@example.org", "9876543210", "http://example.org", "https://example.org/social", false);
    public readonly ContactInfo VerwijderdeContactInfo = new("Verwijderde waarde", "email3@example.org", "0246813579", "http://example.org/3", "https://example.org/social/3", false);
    public readonly ContactInfo ToegevoegdeContactInfo = new("Toegevoegde waarde", "email2@example.org", "0123456789", "http://example.org/2", "https://example.org/social/2", false);
    public readonly ContactInfo VerenigingContactInfoDieGewijzigdIs = new(ContactInfoDieGewijzigdZalWordenNaam, "email2@example.org", "+9876543210", "http://example2.org", "https://example2.org/social", false);

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
                new[]
                {
                    VerenigingContactInfo,
                    VerenigingContactInfoDieGewijzigdZalWorden,
                    VerwijderdeContactInfo,
                },
                Array.Empty<VerenigingWerdGeregistreerd.Locatie>(),
                Array.Empty<VerenigingWerdGeregistreerd.Vertegenwoordiger>(),
                Hoofdactiviteiten),
            new KorteBeschrijvingWerdGewijzigd(VCode, KorteBeschrijving),
            new NaamWerdGewijzigd(VCode, Naam),
            new KorteNaamWerdGewijzigd(VCode, KorteNaam),
            new StartdatumWerdGewijzigd(VCode, Startdatum),
            new ContactInfoLijstWerdGewijzigd(
                VCode,
                new[] { ToegevoegdeContactInfo },
                new[] { VerwijderdeContactInfo },
                new[] { VerenigingContactInfoDieGewijzigdIs }),
        };
    }

    public CommandMetadata GetCommandMetadata()
        => new("OVO000001", new DateTimeOffset(2023, 01, 25, 0, 0, 0, TimeSpan.Zero).ToInstant());
}

public class UnHandledEventAndVerenigingWerdGeregistreerdScenario : IScenario
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
            Array.Empty<ContactInfo>(),
            Array.Empty<VerenigingWerdGeregistreerd.Locatie>(),
            Array.Empty<VerenigingWerdGeregistreerd.Vertegenwoordiger>(),
            Hoofdactiviteiten);
}

public record EenEvent : IEvent;
