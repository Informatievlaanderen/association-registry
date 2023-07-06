namespace AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents.Scenarios;

using AssociationRegistry.Framework;
using Events;
using NodaTime.Extensions;
using Vereniging;

public class V012_LocatieWerdVerwijderdScenario : IScenario
{
    public readonly LocatieWerdVerwijderd LocatieWerdVerwijderd = new(
        Locatie: teVerwijderenLocatie);

    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd = new(
        "V0001012",
        "Feesten Hulste",
        string.Empty,
        string.Empty,
        Startdatum: null,
        Registratiedata.Doelgroep.With(Doelgroep.Null),
        false,
        Array.Empty<Registratiedata.Contactgegeven>(),
        new[] { teVerwijderenLocatie },
        Array.Empty<Registratiedata.Vertegenwoordiger>(),
        Array.Empty<Registratiedata.HoofdactiviteitVerenigingsloket>());

    private static Registratiedata.Locatie teVerwijderenLocatie = new(
        LocatieId: 1,
        Locatietype.Activiteiten,
        false,
        "Naam locatie",
        null,
        new Registratiedata.AdresId(Adresbron.AR.Code, AdresId.DataVlaanderenAdresPrefix));

    public VCode VCode
        => VCode.Create(FeitelijkeVerenigingWerdGeregistreerd.VCode);

    public IEvent[] GetEvents()
    {
        return new IEvent[]
        {
            FeitelijkeVerenigingWerdGeregistreerd,
            LocatieWerdVerwijderd,
        };
    }

    public CommandMetadata GetCommandMetadata()
        => new("OVO000001", new DateTimeOffset(year: 2023, month: 01, day: 25, hour: 0, minute: 0, second: 0, TimeSpan.Zero).ToInstant());
}
