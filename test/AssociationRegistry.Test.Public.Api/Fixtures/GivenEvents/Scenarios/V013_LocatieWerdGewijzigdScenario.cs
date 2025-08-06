namespace AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents.Scenarios;

using AssociationRegistry.Framework;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Adressen;
using Events;
using Events.Factories;
using NodaTime.Extensions;
using Vereniging;

public class V013_LocatieWerdGewijzigdScenario : IScenario
{
    public readonly LocatieWerdGewijzigd LocatieWerdGewijzigd = new(
        Locatie: new Registratiedata.Locatie(
            LocatieId: 1,
            Locatietype.Activiteiten,
            IsPrimair: false,
            Naam: "Naam locatie",
            Adres: null,
            new Registratiedata.AdresId(Adresbron.AR.Code, AdresId.DataVlaanderenAdresPrefix + "1")));

    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd = new(
        VCode: "V0001013",
        Naam: "Feesten Hulste",
        string.Empty,
        string.Empty,
        Startdatum: null,
        EventFactory.Doelgroep(Doelgroep.Null),
        IsUitgeschrevenUitPubliekeDatastroom: false,
        Array.Empty<Registratiedata.Contactgegeven>(),
        new[]
        {
            new Registratiedata.Locatie(
                LocatieId: 1,
                Locatietype.Activiteiten,
                IsPrimair: false,
                Naam: "Foute naam",
                Adres: null,
                new Registratiedata.AdresId(Adresbron.AR.Code, AdresId.DataVlaanderenAdresPrefix + "1")),
        },
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
        => new(Initiator: "OVO000001",
               new DateTimeOffset(year: 2023, month: 01, day: 25, hour: 0, minute: 0, second: 0, TimeSpan.Zero).ToInstant(),
               Guid.NewGuid());
}
