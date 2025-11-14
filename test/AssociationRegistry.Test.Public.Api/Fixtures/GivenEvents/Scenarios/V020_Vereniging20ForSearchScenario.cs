namespace AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents.Scenarios;

using AssociationRegistry.Framework;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Adressen;
using Events;
using NodaTime;
using Vereniging;

public class V020_Vereniging20ForSearchScenario : IScenario
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd = new(
        VCode: "V0001020",
        Naam: "  nooit oma's zonder wafels",
        KorteNaam: "nozw",
        KorteBeschrijving: "Oma's die houden van bakken",
        DateOnly.FromDateTime(new DateTime(year: 2024, month: 01, day: 22)),
        new Registratiedata.Doelgroep(Minimumleeftijd: 60, Maximumleeftijd: 99),
        IsUitgeschrevenUitPubliekeDatastroom: false,
        new[]
        {
            new Registratiedata.Contactgegeven(
                ContactgegevenId: 1,
                Contactgegeventype.Telefoon,
                Waarde: "0000112233",
                Beschrijving: "Algemeen",
                IsPrimair: true),
        },
        new[]
        {
            new Registratiedata.Locatie(
                LocatieId: 1,
                Locatietype: "Correspondentie",
                IsPrimair: true,
                Naam: "Correspondentie",
                new Registratiedata.Adres(
                    Straatnaam: "bakkershof",
                    Huisnummer: "60",
                    Busnummer: "B",
                    Postcode: "1840",
                    Gemeente: "Londerzeel",
                    Land: "België"),
                new Registratiedata.AdresId(
                    Adresbron.AR,
                    Bronwaarde: "https://data.vlaanderen.be/id/adres/0")
            ),
        },
        [],
        new Registratiedata.HoofdactiviteitVerenigingsloket[]
        {
            new(Code: "BLA", Naam: "Buitengewoon Leuke Afkortingen"),
        });

    public VCode VCode
        => VCode.Create(FeitelijkeVerenigingWerdGeregistreerd.VCode);

    public IEvent[] GetEvents()
    {
        return new IEvent[]
        {
            FeitelijkeVerenigingWerdGeregistreerd,
            new FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(VCode),
        };
    }

    public CommandMetadata GetCommandMetadata()
        => new(Initiator: "OVO000001", new Instant(), Guid.NewGuid());
}
