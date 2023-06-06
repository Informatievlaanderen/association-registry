namespace AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents.Scenarios;

using Events;
using AssociationRegistry.Framework;
using Vereniging;
using NodaTime;

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