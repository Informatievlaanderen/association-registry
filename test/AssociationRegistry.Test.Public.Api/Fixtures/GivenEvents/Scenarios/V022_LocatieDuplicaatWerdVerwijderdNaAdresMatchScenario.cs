namespace AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents.Scenarios;

using AssociationRegistry.Framework;
using Events;
using NodaTime.Extensions;
using Vereniging;

public class V022_LocatieDuplicaatWerdVerwijderdNaAdresMatchScenario : IScenario
{
    public readonly LocatieDuplicaatWerdVerwijderdNaAdresMatch LocatieDuplicaatWerdVerwijderdNaAdresMatch = new(
        "V0001022",
        teVerwijderenLocatie.LocatieId,
        TeBehoudenLocatie.LocatieId,
        TeBehoudenLocatie.Naam,
        TeBehoudenLocatie.AdresId);

    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd = new(
        VCode: "V0001022",
        Naam: "Feesten Hulste",
        string.Empty,
        string.Empty,
        Startdatum: null,
        Registratiedata.Doelgroep.With(Doelgroep.Null),
        IsUitgeschrevenUitPubliekeDatastroom: false,
        Array.Empty<Registratiedata.Contactgegeven>(),
        new[] { TeBehoudenLocatie, teVerwijderenLocatie },
        Array.Empty<Registratiedata.Vertegenwoordiger>(),
        Array.Empty<Registratiedata.HoofdactiviteitVerenigingsloket>());

    public static readonly Registratiedata.Locatie TeBehoudenLocatie = new(
        LocatieId: 1,
        Locatietype.Activiteiten,
        IsPrimair: true,
        Naam: "Te behouden locatie",
        Adres: new Registratiedata.Adres("Testlaan", "22", "A", "8800", "Oekene", "België"),
        new Registratiedata.AdresId(Adresbron.AR.Code, AdresId.DataVlaanderenAdresPrefix+"1"));

    private static readonly Registratiedata.Locatie teVerwijderenLocatie = new(
        LocatieId: 2,
        Locatietype.Activiteiten,
        IsPrimair: false,
        Naam: "Te verwijderen locatie",
        Adres: null,
        new Registratiedata.AdresId(Adresbron.AR.Code, AdresId.DataVlaanderenAdresPrefix+"1"));

    public VCode VCode
        => VCode.Create(FeitelijkeVerenigingWerdGeregistreerd.VCode);

    public IEvent[] GetEvents()
    {
        return new IEvent[]
        {
            FeitelijkeVerenigingWerdGeregistreerd,
            LocatieDuplicaatWerdVerwijderdNaAdresMatch,
        };
    }

    public CommandMetadata GetCommandMetadata()
        => new(Initiator: "OVO000001",
               new DateTimeOffset(year: 2023, month: 01, day: 25, hour: 0, minute: 0, second: 0, TimeSpan.Zero).ToInstant(),
               Guid.NewGuid());
}
