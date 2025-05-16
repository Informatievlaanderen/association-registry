namespace AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents.Scenarios;

using AssociationRegistry.Framework;
using EventFactories;
using Events;
using NodaTime.Extensions;
using Vereniging;

public class V022_LocatieDuplicaatWerdVerwijderdNaAdresMatchScenario : IScenario
{
    private static readonly string _adresId = "12345";

    public LocatieDuplicaatWerdVerwijderdNaAdresMatch LocatieDuplicaatWerdVerwijderdNaAdresMatch => new(
        FeitelijkeVerenigingWerdGeregistreerd.VCode,
        TeVerwijderenLocatie.LocatieId,
        TeBehoudenLocatie.LocatieId,
        TeBehoudenLocatie.Naam,
        TeBehoudenLocatie.AdresId);

    public FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd => new(
        VCode: "V0001022",
        Naam: "Feesten Hulste",
        string.Empty,
        string.Empty,
        Startdatum: null,
        EventFactory.Doelgroep(Doelgroep.Null),
        IsUitgeschrevenUitPubliekeDatastroom: false,
        Array.Empty<Registratiedata.Contactgegeven>(),
        new[] { TeBehoudenLocatie, TeVerwijderenLocatie },
        Array.Empty<Registratiedata.Vertegenwoordiger>(),
        Array.Empty<Registratiedata.HoofdactiviteitVerenigingsloket>());

    public static Registratiedata.Locatie TeBehoudenLocatie => new(
        LocatieId: 1,
        Locatietype.Activiteiten,
        IsPrimair: false,
        Naam: "Naam locatie",
        new Registratiedata.Adres(Straatnaam: "Testlaan", Huisnummer: "22", Busnummer: "A", Postcode: "8800", Gemeente: "Oekene",
                                  Land: "België"),
        new Registratiedata.AdresId(Adresbron.AR.Code, $"{AdresId.DataVlaanderenAdresPrefix}{_adresId}"));

    public static Registratiedata.Locatie TeVerwijderenLocatie => TeBehoudenLocatie with { LocatieId = 2 };
    public VCode VCode => VCode.Create(FeitelijkeVerenigingWerdGeregistreerd.VCode);

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
