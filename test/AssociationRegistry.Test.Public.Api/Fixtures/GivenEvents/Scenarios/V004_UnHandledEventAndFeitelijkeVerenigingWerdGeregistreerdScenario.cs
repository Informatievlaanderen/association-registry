namespace AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents.Scenarios;

using AssociationRegistry.Framework;
using Events;
using NodaTime;
using Vereniging;

public class V004_UnHandledEventAndFeitelijkeVerenigingWerdGeregistreerdScenario : IScenario
{
    public const string Naam = "Oostende voor anker";
    private const string KorteNaam = "OVA";

    public static readonly Registratiedata.HoofdactiviteitVerenigingsloket[] Hoofdactiviteiten =
    {
        new(Code: "BLA", Naam: "Buitengewoon Leuke Afkortingen"),
    };

    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd = CreateFeitelijkeVerenigingWerdGeregistreerd(
        "V0001004",
        Naam,
        KorteNaam);

    public VCode VCode
        => VCode.Create("V0001004");

    public IEvent[] GetEvents()
    {
        return new IEvent[]
        {
            new EenEvent(),
            FeitelijkeVerenigingWerdGeregistreerd,
        };
    }

    public CommandMetadata GetCommandMetadata()
        => new(Initiator: "OVO000001", new Instant(), Guid.NewGuid());

    private static FeitelijkeVerenigingWerdGeregistreerd CreateFeitelijkeVerenigingWerdGeregistreerd(
        string vCode,
        string naam,
        string korteNaam)
        => new(
            vCode,
            naam,
            korteNaam,
            string.Empty,
            Startdatum: null,
            Registratiedata.Doelgroep.With(Doelgroep.Null),
            IsUitgeschrevenUitPubliekeDatastroom: false,
            Array.Empty<Registratiedata.Contactgegeven>(),
            Array.Empty<Registratiedata.Locatie>(),
            Array.Empty<Registratiedata.Vertegenwoordiger>(),
            Hoofdactiviteiten);
}
