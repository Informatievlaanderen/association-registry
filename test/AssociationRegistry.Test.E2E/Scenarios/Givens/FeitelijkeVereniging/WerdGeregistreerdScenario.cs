namespace AssociationRegistry.Test.E2E.Scenarios.Givens.FeitelijkeVereniging;

using Events;
using EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using Vereniging;
using AutoFixture;
using Requests.FeitelijkeVereniging;

public class FeitelijkeVerenigingWerdGeregistreerdScenario : IFeitelijkeVerenigingWerdGeregistreerdScenario, Framework.TestClasses.IScenario
{
    private readonly bool _isUitgeschreven;
    public FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd { get; set; }
    public FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid { get; set; }
    private CommandMetadata Metadata;
    public VCode VCode { get; private set; }

    public FeitelijkeVerenigingWerdGeregistreerdScenario(bool isUitgeschreven = false)
    {
        _isUitgeschreven = isUitgeschreven;
    }

    public async Task<KeyValuePair<string, IEvent[]>[]> GivenEvents(IVCodeService service)
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VCode = await service.GetNext();

        FeitelijkeVerenigingWerdGeregistreerd = new FeitelijkeVerenigingWerdGeregistreerd(
            VCode,
            Naam: fixture.Create<string>(),
            KorteNaam: "FOud",
            KorteBeschrijving: fixture.Create<string>(),
            DateOnly.FromDateTime(new DateTime(year: 2022, month: 11, day: 9)),
            new Registratiedata.Doelgroep(Minimumleeftijd: 18, Maximumleeftijd: 90),
            IsUitgeschrevenUitPubliekeDatastroom: _isUitgeschreven,
            new[]
            {
                new Registratiedata.Contactgegeven(
                    ContactgegevenId: 1,
                    Contactgegeventype.Email,
                    Waarde: "info@FOud.be",
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
                        Straatnaam: "Stationsstraat",
                        Huisnummer: "1",
                        Busnummer: "B",
                        Postcode: "1790",
                        Gemeente: "Affligem",
                        Land: "België"),
                    new Registratiedata.AdresId(Adresbron.AR.Code, Bronwaarde: "https://data.vlaanderen.be/id/adres/0")),
                new Registratiedata.Locatie(
                    LocatieId: 2,
                    Locatietype: "Activiteiten",
                    IsPrimair: false,
                    Naam: "Activiteiten",
                    Adres: null,
                    new Registratiedata.AdresId(Adresbron.AR.Code, Bronwaarde: "https://data.vlaanderen.be/id/adres/0")),
                new Registratiedata.Locatie(
                    LocatieId: 3,
                    Locatietype: "Activiteiten",
                    IsPrimair: false,
                    Naam: "Activiteiten",
                    new Registratiedata.Adres(
                        Straatnaam: "Dorpstraat",
                        Huisnummer: "1",
                        Busnummer: "B",
                        Postcode: "1790",
                        Gemeente: "Affligem",
                        Land: "België"),
                    AdresId: null),
            },
            new[]
            {
                new Registratiedata.Vertegenwoordiger(
                    VertegenwoordigerId: 1,
                    Insz: "01234567890",
                    IsPrimair: true,
                    Roepnaam: "father",
                    Rol: "Leader",
                    Voornaam: "Odin",
                    Achternaam: "Allfather",
                    Email: "asgard@world.tree",
                    Telefoon: "",
                    Mobiel: "",
                    SocialMedia: ""),
            },
            new Registratiedata.HoofdactiviteitVerenigingsloket[]
            {
                new(HoofdactiviteitVerenigingsloket.All()[0].Code, Naam: "Buitengewoon Leuke Afkortingen"),
            });

        FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid = new(VCode);

                Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };

        return [
           new(VCode, [FeitelijkeVerenigingWerdGeregistreerd, FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid]),
        ];
    }

    public StreamActionResult Result { get; set; } = null!;

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
