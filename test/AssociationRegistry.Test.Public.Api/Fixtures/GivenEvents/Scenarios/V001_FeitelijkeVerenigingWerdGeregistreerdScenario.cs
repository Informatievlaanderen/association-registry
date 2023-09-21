namespace AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents.Scenarios;

using Events;
using AssociationRegistry.Framework;
using Vereniging;
using NodaTime;

public class V001_FeitelijkeVerenigingWerdGeregistreerdScenario : IScenario
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd = new(
        "V0001001",
        "Feestcommittee Oudenaarde",
        "FOud",
        "Het feestcommittee van Oudenaarde",
        DateOnly.FromDateTime(new DateTime(year: 2022, month: 11, day: 9)),
        new Registratiedata.Doelgroep(20, 71),
        false,
        new[]
        {
            new Registratiedata.Contactgegeven(
                ContactgegevenId: 1,
                ContactgegevenType.Email,
                "info@FOud.be",
                "Algemeen",
                IsPrimair: true),
        },
        new[]
        {
            new(
                1,
                "Correspondentie",
                IsPrimair: true,
                Naam: "Correspondentie",
                Adres: new Registratiedata.Adres(
                    "Stationsstraat",
                    "1",
                    "B",
                    "1790",
                    "Affligem",
                    "België"),
                AdresId: new Registratiedata.AdresId(
                    Adresbron.AR,
                    "https://data.vlaanderen.be/id/adres/0")),
            new(
                2,
                "Activiteiten",
                IsPrimair: false,
                Naam: "Activiteiten",
                Adres: null,
                AdresId: new Registratiedata.AdresId(
                    Adresbron.AR,
                    "https://data.vlaanderen.be/id/adres/0")),
            new Registratiedata.Locatie(
                3,
                "Activiteiten",
                IsPrimair: false,
                Naam: "Activiteiten",
                Adres: new Registratiedata.Adres(
                    "Dorpstraat",
                    "1",
                    "B",
                    "1790",
                    "Affligem",
                    "België"),
                AdresId: null),
        },
        new[]
        {
            new Registratiedata.Vertegenwoordiger(
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
                ""),
        },
        new Registratiedata.HoofdactiviteitVerenigingsloket[]
        {
            new("BLA", "Buitengewoon Leuke Afkortingen"),
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
        => new("OVO000001", new Instant(), Guid.NewGuid());
}
