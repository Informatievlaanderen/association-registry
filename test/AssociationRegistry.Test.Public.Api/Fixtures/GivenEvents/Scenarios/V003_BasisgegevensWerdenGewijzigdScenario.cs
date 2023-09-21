namespace AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents.Scenarios;

using Events;
using AssociationRegistry.Framework;
using Vereniging;
using NodaTime.Extensions;

public class V003_BasisgegevensWerdenGewijzigdScenario : IScenario
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd = new(
        "V0001003",
        "Foudenaarder feest",
        string.Empty,
        string.Empty,
        Startdatum: null,
        Registratiedata.Doelgroep.With(Doelgroep.Null),
        false,
        Array.Empty<Registratiedata.Contactgegeven>(),
        Array.Empty<Registratiedata.Locatie>(),
        Array.Empty<Registratiedata.Vertegenwoordiger>(),
        new Registratiedata.HoofdactiviteitVerenigingsloket[]
        {
            new("BLA", "Buitengewoon Leuke Afkortingen"),
        });

    public readonly KorteBeschrijvingWerdGewijzigd KorteBeschrijvingWerdGewijzigd = new("V0001003", "Harelbeke");
    public readonly NaamWerdGewijzigd NaamWerdGewijzigd = new("V0001003", "Oarelbeke Weireldstad");
    public readonly KorteNaamWerdGewijzigd KorteNaamWerdGewijzigd = new("V0001003", "OW");
    public readonly StartdatumWerdGewijzigd StartdatumWerdGewijzigd = new("V0001003", new DateOnly(year: 2023, month: 6, day: 3));
    public readonly DoelgroepWerdGewijzigd DoelgroepWerdGewijzigd = new(new Registratiedata.Doelgroep(12, 18));

    public VCode VCode
        => VCode.Create(FeitelijkeVerenigingWerdGeregistreerd.VCode);

    public IEvent[] GetEvents()
    {
        return new IEvent[]
        {
            FeitelijkeVerenigingWerdGeregistreerd,
            KorteBeschrijvingWerdGewijzigd,
            NaamWerdGewijzigd,
            KorteNaamWerdGewijzigd,
            StartdatumWerdGewijzigd,
            DoelgroepWerdGewijzigd,
        };
    }

    public CommandMetadata GetCommandMetadata()
        => new("OVO000001", new DateTimeOffset(year: 2023, month: 01, day: 25, hour: 0, minute: 0, second: 0, TimeSpan.Zero).ToInstant(),
               Guid.NewGuid());
}
