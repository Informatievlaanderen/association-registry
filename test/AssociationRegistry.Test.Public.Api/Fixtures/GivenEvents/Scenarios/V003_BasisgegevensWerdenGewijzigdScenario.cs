namespace AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents.Scenarios;

using Events;
using AssociationRegistry.Framework;
using Vereniging;
using NodaTime.Extensions;

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

    public VCode AfdelingVCode
        => VCode.Create("V0001003");


    public IEvent[] GetEvents()
    {
        return new IEvent[]
        {
            new FeitelijkeVerenigingWerdGeregistreerd(
                AfdelingVCode,
                "Foudenaarder feest",
                string.Empty,
                string.Empty,
                Startdatum: null,
                Array.Empty<Registratiedata.Contactgegeven>(),
                Array.Empty<Registratiedata.Locatie>(),
                Array.Empty<Registratiedata.Vertegenwoordiger>(),
                Hoofdactiviteiten),
            new KorteBeschrijvingWerdGewijzigd(AfdelingVCode, KorteBeschrijving),
            new NaamWerdGewijzigd(AfdelingVCode, Naam),
            new KorteNaamWerdGewijzigd(AfdelingVCode, KorteNaam),
            new StartdatumWerdGewijzigd(AfdelingVCode, Startdatum),
        };
    }

    public CommandMetadata GetCommandMetadata()
        => new("OVO000001", new DateTimeOffset(year: 2023, month: 01, day: 25, hour: 0, minute: 0, second: 0, TimeSpan.Zero).ToInstant());
}