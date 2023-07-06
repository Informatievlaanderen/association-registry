namespace AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents.Scenarios;

using AssociationRegistry.Framework;
using Events;
using NodaTime.Extensions;
using Vereniging;

public class V013_LocatieWerdGewijzigdScenario : IScenario
{
    public readonly LocatieWerdGewijzigd LocatieWerdGewijzigd = new(
        Locatie: new Registratiedata.Locatie(
            LocatieId: 1,
            Locatietype.Activiteiten,
            false,
            "Naam locatie",
            null,
            new Registratiedata.AdresId(Adresbron.AR.Code, AdresId.DataVlaanderenAdresPrefix)));

    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd = new(
        "V0001013",
        "Feesten Hulste",
        string.Empty,
        string.Empty,
        Startdatum: null,
        Registratiedata.Doelgroep.With(Doelgroep.Null),
        false,
        Array.Empty<Registratiedata.Contactgegeven>(),
        new [] { new Registratiedata.Locatie(
            LocatieId: 1,
            Locatietype.Activiteiten,
            false,
            "Foute naam",
            null,
            new Registratiedata.AdresId(Adresbron.AR.Code, AdresId.DataVlaanderenAdresPrefix)) },
        Array.Empty<Registratiedata.Vertegenwoordiger>(),
        Array.Empty<Registratiedata.HoofdactiviteitVerenigingsloket>());

    public VCode VCode
        => VCode.Create(FeitelijkeVerenigingWerdGeregistreerd.VCode);

    public IEvent[] GetEvents()
    {
        return new IEvent[]
        {
            FeitelijkeVerenigingWerdGeregistreerd,
            LocatieWerdGewijzigd,
        };
    }

    public CommandMetadata GetCommandMetadata()
        => new("OVO000001", new DateTimeOffset(year: 2023, month: 01, day: 25, hour: 0, minute: 0, second: 0, TimeSpan.Zero).ToInstant());
}
