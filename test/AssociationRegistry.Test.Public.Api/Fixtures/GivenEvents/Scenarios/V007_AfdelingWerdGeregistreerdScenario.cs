namespace AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents.Scenarios;

using AssociationRegistry.Framework;
using Events;
using NodaTime;
using Vereniging;

public class V007_AfdelingWerdGeregistreerdScenario : IScenario
{
    public readonly AfdelingWerdGeregistreerd AfdelingWerdGeregistreerd = new(
        VCode: "V0001007",
        Naam: "Antwerpse Bijscholing Clickers",
        new AfdelingWerdGeregistreerd.MoederverenigingsData(KboNummer: "0123456789", string.Empty, Naam: "Moeder 0123456789"),
        KorteNaam: "ABC",
        KorteBeschrijving: "balpenverzamelaars van antwerpse bijscholingen",
        DateOnly.FromDateTime(new DateTime(year: 2022, month: 11, day: 9)),
        Registratiedata.Doelgroep.With(Doelgroep.Null),
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
                new Registratiedata.Adres(Straatnaam: "berglaan",
                                          Huisnummer: "12",
                                          Busnummer: "B",
                                          Postcode: "2000",
                                          Gemeente: "Antwerpen",
                                          Land: "België"),
                AdresId: null),
        },
        new[]
        {
            (Registratiedata.Vertegenwoordiger)new Registratiedata.Vertegenwoordiger(
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
            new(Code: "BLA", Beschrijving: "Buitengewoon Leuke Afkortingen"),
        });

    public VCode VCode
        => VCode.Create(AfdelingWerdGeregistreerd.VCode);

    public IEvent[] GetEvents()
    {
        return new IEvent[]
        {
            AfdelingWerdGeregistreerd,
        };
    }

    public CommandMetadata GetCommandMetadata()
        => new(Initiator: "OVO000001", new Instant(), Guid.NewGuid());
}
