namespace AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents;

using Events;
using AssociationRegistry.Framework;
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

    private readonly VerenigingWerdGeregistreerd.ContactInfo _contactInfo = new(
        "Algemeen",
        "info@FOud.be",
        "1111.11.11.11",
        "www.oudenaarde.be/feest",
        "#FOudenaarde");

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
        "Allfather");


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
                new[] { _vertegenwoordiger }),
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
                Array.Empty<VerenigingWerdGeregistreerd.ContactInfo>(),
                Array.Empty<VerenigingWerdGeregistreerd.Locatie>(),
                Array.Empty<VerenigingWerdGeregistreerd.Vertegenwoordiger>()),
        };
    }

    public CommandMetadata GetCommandMetadata()
        => new("OVO000001", new Instant());
}

public class KorteBeschrijvingWerdGewijzigdScenario : IScenario
{
    public VCode VCode
        => VCode.Create("V0001003");

    public readonly string KorteBeschrijving = "Harelbeke";
    public readonly string KorteNaam = "OW";
    public readonly string Naam = "Oarelbeke Weireldstad";

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
                Array.Empty<VerenigingWerdGeregistreerd.ContactInfo>(),
                Array.Empty<VerenigingWerdGeregistreerd.Locatie>(),
                Array.Empty<VerenigingWerdGeregistreerd.Vertegenwoordiger>()),
            new KorteBeschrijvingWerdGewijzigd(VCode, KorteBeschrijving),
            new NaamWerdGewijzigd(VCode, Naam),
            new KorteNaamWerdGewijzigd(VCode, KorteNaam),
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
            Array.Empty<VerenigingWerdGeregistreerd.ContactInfo>(),
            Array.Empty<VerenigingWerdGeregistreerd.Locatie>(),
            Array.Empty<VerenigingWerdGeregistreerd.Vertegenwoordiger>());
}

public record EenEvent : IEvent;
