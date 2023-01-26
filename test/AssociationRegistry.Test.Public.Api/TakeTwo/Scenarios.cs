namespace AssociationRegistry.Test.Public.Api.TakeTwo;

using AssociationRegistry.Framework;
using Events;
using NodaTime;
using NodaTime.Extensions;
using VCodes;

public interface IScenario
{
    public VCode VCode { get; }
    public IEvent[] GetEvents();
    public CommandMetadata GetCommandMetadata();
}

public class VerenigingWerdGeregistreerdScenario : IScenario
{
    public VCode VCode => VCode.Create("V0001001");
    private readonly string _naam = "Feestcommittee Oudenaarde";
    private readonly string? _korteBeschrijving = "Het feestcommittee van Oudenaarde";
    private readonly string? _korteNaam = "FOud";
    private readonly string? _kboNummer = "0123456789";


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


    public IEvent[] GetEvents()
    {
        return new IEvent[]
        {
            new VerenigingWerdGeregistreerd(
                VCode,
                _naam,
                _korteNaam,
                _korteBeschrijving,
                _startdatum,
                _kboNummer,
                new[] { _contactInfo },
                new[] { _locatie }),
        };
    }

    public CommandMetadata GetCommandMetadata()
        => new("OVO000001", new Instant());
}

public class VerenigingWerdGeregistreerdWithMinimalFieldsScenario : IScenario
{
    public VCode VCode
        => VCode.Create("V0001002");

    private readonly string Naam = "Feestcommittee Oudenaarde";

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
                Array.Empty<VerenigingWerdGeregistreerd.Locatie>()),
        };
    }

    public CommandMetadata GetCommandMetadata()
        => new("OVO000001", new Instant());
}

public class KorteBeschrijvingWerdGewijzigdScenario : IScenario
{
    public VCode VCode => VCode.Create("V0001003");
    private const string KorteBeschrijving = "het feestcomite van Oudenaarde";
    private const string KorteNaam = "FO";
    private const string Naam = "Foudenaarde";

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
                Array.Empty<VerenigingWerdGeregistreerd.Locatie>()),
            new KorteBeschrijvingWerdGewijzigd(VCode, KorteBeschrijving),
            new NaamWerdGewijzigd(VCode, Naam),
            new KorteNaamWerdGewijzigd(VCode, KorteNaam),
        };
    }

    public CommandMetadata GetCommandMetadata()
        => new("OVO000001", new DateTimeOffset(2023, 01, 25, 0, 0, 0, TimeSpan.Zero).ToInstant());
}
