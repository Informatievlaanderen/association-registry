namespace AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents.Scenarios;

using AssociationRegistry.Framework;
using DecentraalBeheer.Vereniging;
using Events;
using Events.Factories;
using NodaTime.Extensions;
using Vereniging;

public class V003_BasisgegevensWerdenGewijzigdScenario : IScenario
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd = new(
        VCode: "V0001003",
        Naam: "Foudenaarder feest",
        string.Empty,
        string.Empty,
        Startdatum: null,
        EventFactory.Doelgroep(Doelgroep.Null),
        IsUitgeschrevenUitPubliekeDatastroom: false,
        Array.Empty<Registratiedata.Contactgegeven>(),
        Array.Empty<Registratiedata.Locatie>(),
        Array.Empty<Registratiedata.Vertegenwoordiger>(),
        new Registratiedata.HoofdactiviteitVerenigingsloket[]
        {
            new(Code: "BLA", Naam: "Buitengewoon Leuke Afkortingen"),
        });

    public readonly KorteBeschrijvingWerdGewijzigd KorteBeschrijvingWerdGewijzigd = new(VCode: "V0001003", KorteBeschrijving: "Harelbeke");
    public readonly NaamWerdGewijzigd NaamWerdGewijzigd = new(VCode: "V0001003", Naam: "Oarelbeke Weireldstad");
    public readonly KorteNaamWerdGewijzigd KorteNaamWerdGewijzigd = new(VCode: "V0001003", KorteNaam: "OW");
    public readonly StartdatumWerdGewijzigd StartdatumWerdGewijzigd = new(VCode: "V0001003", new DateOnly(year: 2023, month: 6, day: 3));

    public readonly DoelgroepWerdGewijzigd DoelgroepWerdGewijzigd =
        new(new Registratiedata.Doelgroep(Minimumleeftijd: 12, Maximumleeftijd: 18));

    public VCode VCode
        => VCode.Create(FeitelijkeVerenigingWerdGeregistreerd.VCode);

    public IEvent[] GetEvents()
    {
        return new IEvent[]
        {
            FeitelijkeVerenigingWerdGeregistreerd,
            new FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(VCode),
            KorteBeschrijvingWerdGewijzigd,
            NaamWerdGewijzigd,
            KorteNaamWerdGewijzigd,
            StartdatumWerdGewijzigd,
            DoelgroepWerdGewijzigd,
        };
    }

    public CommandMetadata GetCommandMetadata()
        => new(Initiator: "OVO000001",
               new DateTimeOffset(year: 2023, month: 01, day: 25, hour: 0, minute: 0, second: 0, TimeSpan.Zero).ToInstant(),
               Guid.NewGuid());
}
