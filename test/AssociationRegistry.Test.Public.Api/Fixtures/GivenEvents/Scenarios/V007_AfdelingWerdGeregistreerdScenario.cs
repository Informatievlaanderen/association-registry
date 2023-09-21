namespace AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents.Scenarios;

using Events;
using AssociationRegistry.Framework;
using Vereniging;
using NodaTime;

public class V007_AfdelingWerdGeregistreerdScenario : IScenario
{
    public VCode VCode
        => VCode.Create(AfdelingWerdGeregistreerd.VCode);

    public readonly AfdelingWerdGeregistreerd AfdelingWerdGeregistreerd = new(
        "V0001007",
        "Antwerpse Bijscholing Clickers",
        new AfdelingWerdGeregistreerd.MoederverenigingsData("0123456789", string.Empty, "Moeder 0123456789"),
        "ABC",
        "balpenverzamelaars van antwerpse bijscholingen",
        DateOnly.FromDateTime(new DateTime(year: 2022, month: 11, day: 9)),
        Registratiedata.Doelgroep.With(Doelgroep.Null),
        new[]
        {
            (Registratiedata.Contactgegeven)new(
                ContactgegevenId: 1,
                ContactgegevenType.Email,
                "info@FOud.be",
                "Algemeen",
                IsPrimair: true)
        },
        new[]
        {
            (Registratiedata.Locatie)new(
                1,
                "Correspondentie",
                IsPrimair: true,
                Naam: "Correspondentie",
                Adres: new Registratiedata.Adres("berglaan",
                                                 "12",
                                                 "B",
                                                 "2000",
                                                 "Antwerpen",
                                                 "België"),
                null)
        },
        new[]
        {
            (Registratiedata.Vertegenwoordiger)new(
                VertegenwoordigerId: 1,
                "01234567890",
                IsPrimair: true,
                "father",
                "Leader",
                "Odin",
                "Allfather",
                "asgard@world.tree",
                "",
                "",
                "")
        },
        new Registratiedata.HoofdactiviteitVerenigingsloket[]
        {
            new("BLA", "Buitengewoon Leuke Afkortingen"),
        });

    public IEvent[] GetEvents()
    {
        return new IEvent[]
        {
            AfdelingWerdGeregistreerd,
        };
    }

    public CommandMetadata GetCommandMetadata()
        => new("OVO000001", new Instant(), Guid.NewGuid());
}
