namespace AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents.Scenarios;

using AssociationRegistry.Framework;
using Events;
using NodaTime;
using Vereniging;

public class V019_Vereniging19ForSearchScenario : IScenario
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd = new(
        VCode: "V0001019",
        Naam: "Nieuwe Auto's die Starten Afligem",
        KorteNaam: "  NASA",
        KorteBeschrijving: "Wij verzamelen nieuwe en oude autos.",
        DateOnly.FromDateTime(new DateTime(year: 2024, month: 01, day: 22)),
        new Registratiedata.Doelgroep(Minimumleeftijd: 18, Maximumleeftijd: 99),
        IsUitgeschrevenUitPubliekeDatastroom: false,
        new[]
        {
            new Registratiedata.Contactgegeven(
                ContactgegevenId: 1,
                Contactgegeventype.Website,
                Waarde: "example.org/nasafligem",
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
                    Straatnaam: "wasstraat",
                    Huisnummer: "5",
                    Busnummer: "",
                    Postcode: "1790",
                    Gemeente: "Affligem",
                    Land: "België"),
                new Registratiedata.AdresId(
                    Adresbron.AR,
                    Bronwaarde: "https://data.vlaanderen.be/id/adres/0")
            ),
        },
        new[]
        {
            new Registratiedata.Vertegenwoordiger(
                VertegenwoordigerId: 1,
                Insz: "01234567890",
                IsPrimair: true,
                Roepnaam: "engine",
                Rol: "Leader",
                Voornaam: "Put",
                Achternaam: "Put",
                Email: "putput@broembroem.com",
                Telefoon: "",
                Mobiel: "",
                SocialMedia: ""),
        },
        new Registratiedata.HoofdactiviteitVerenigingsloket[]
        {
            new(Code: "HAHA", Naam: "Hilarische Afkortingen Hebben Afgedaan"),
        });

    public VCode VCode
        => VCode.Create(FeitelijkeVerenigingWerdGeregistreerd.VCode);

    public IEvent[] GetEvents()
    {
        return new IEvent[]
        {
            FeitelijkeVerenigingWerdGeregistreerd,
        };
    }

    public CommandMetadata GetCommandMetadata()
        => new(Initiator: "OVO000001", new Instant(), Guid.NewGuid());
}
