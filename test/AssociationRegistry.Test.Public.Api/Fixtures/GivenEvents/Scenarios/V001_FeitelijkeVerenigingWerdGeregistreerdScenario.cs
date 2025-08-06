namespace AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents.Scenarios;

using AssociationRegistry.Framework;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Adressen;
using Events;
using NodaTime;
using Vereniging;

public class V001_FeitelijkeVerenigingWerdGeregistreerdScenario : IScenario
{
    private const string _vCode = "V0001001";

    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd = new(
        VCode: _vCode,
        Naam: "Feestcommittee Oudenaarde",
        KorteNaam: "FOud",
        KorteBeschrijving: "Het feestcommittee van Oudenaarde",
        Startdatum: DateOnly.FromDateTime(dateTime: new DateTime(year: 2022, month: 11, day: 9)),
        Doelgroep: new Registratiedata.Doelgroep(Minimumleeftijd: 20, Maximumleeftijd: 71),
        IsUitgeschrevenUitPubliekeDatastroom: false,
        Contactgegevens:
        [
            new Registratiedata.Contactgegeven(
                ContactgegevenId: 1,
                Contactgegeventype: Contactgegeventype.Email,
                Waarde: "info@FOud.be",
                Beschrijving: "Algemeen",
                IsPrimair: true),
        ],
        Locaties:
        [
            new(
                LocatieId: 1,
                Locatietype: "Correspondentie",
                IsPrimair: true,
                Naam: "Correspondentie",
                Adres: new Registratiedata.Adres(
                    Straatnaam: "Stationsstraat",
                    Huisnummer: "1",
                    Busnummer: "B",
                    Postcode: "1790",
                    Gemeente: "Affligem",
                    Land: "België"),
                AdresId: new Registratiedata.AdresId(
                    Broncode: Adresbron.AR,
                    Bronwaarde: "https://data.vlaanderen.be/id/adres/0")),
            new(
                LocatieId: 2,
                Locatietype: "Activiteiten",
                IsPrimair: false,
                Naam: "Activiteiten",
                Adres: null,
                AdresId: new Registratiedata.AdresId(
                    Broncode: Adresbron.AR,
                    Bronwaarde: "https://data.vlaanderen.be/id/adres/0")),
            new Registratiedata.Locatie(
                LocatieId: 3,
                Locatietype: "Activiteiten",
                IsPrimair: false,
                Naam: "Activiteiten",
                Adres: new Registratiedata.Adres(
                    Straatnaam: "Dorpstraat",
                    Huisnummer: "1",
                    Busnummer: "B",
                    Postcode: "1790",
                    Gemeente: "Affligem",
                    Land: "België"),
                AdresId: null),
        ],
        Vertegenwoordigers:
        [
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
        ],
        HoofdactiviteitenVerenigingsloket:
        [
            new(Code: "BLA", Naam: "Buitengewoon Leuke Afkortingen"),
        ]);

    public readonly WerkingsgebiedenWerdenBepaald WerkingsgebiedenWerdenBepaald = new(
        _vCode,
        [
            new(Code: "BE25", Naam: "Provincie West-Vlaanderen"),
            new("BE25535003", "Bredene"),
        ]);

    public VCode VCode
        => VCode.Create(_vCode);

    public IEvent[] GetEvents()
    {
        return
        [
            FeitelijkeVerenigingWerdGeregistreerd,
            WerkingsgebiedenWerdenBepaald,
            new FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(VCode)
        ];
    }

    public CommandMetadata GetCommandMetadata()
        => new(Initiator: "OVO000001",
               Tijdstip: new Instant(),
               CorrelationId: Guid.NewGuid());
}
