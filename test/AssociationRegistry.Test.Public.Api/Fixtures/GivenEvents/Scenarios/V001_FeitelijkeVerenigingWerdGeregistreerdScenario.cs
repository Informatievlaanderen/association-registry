namespace AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents.Scenarios;

using Events;
using AssociationRegistry.Framework;
using Vereniging;
using NodaTime;

public class V001_FeitelijkeVerenigingWerdGeregistreerdScenario : IScenario
{
    private readonly Registratiedata.Contactgegeven _contactgegeven = new(
        ContactgegevenId: 1,
        ContactgegevenType.Email,
        "info@FOud.be",
        "Algemeen",
        IsPrimair: true);

    private readonly Registratiedata.Locatie _locatie = new(
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
            "https://data.vlaanderen.be/id/adres/0"));

    private readonly Registratiedata.Locatie _locatie2 = new(
        2,
        "Activiteiten",
        IsPrimair: false,
        Naam: "Activiteiten",
        Adres: null,
        AdresId: new Registratiedata.AdresId(
            Adresbron.AR,
            "https://data.vlaanderen.be/id/adres/0"));

    private readonly Registratiedata.Locatie _locatie3 = new(
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
        AdresId: null);

    private readonly DateOnly? _startdatum = DateOnly.FromDateTime(new DateTime(year: 2022, month: 11, day: 9));

    private readonly Registratiedata.Vertegenwoordiger _vertegenwoordiger = new(
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
        "");

    public readonly Registratiedata.HoofdactiviteitVerenigingsloket[] Hoofdactiviteiten =
    {
        new("BLA", "Buitengewoon Leuke Afkortingen"),
    };

    public readonly string? KorteBeschrijving = "Het feestcommittee van Oudenaarde";
    public readonly string? KorteNaam = "FOud";

    public readonly string Naam = "Feestcommittee Oudenaarde";

    public VCode VCode
        => VCode.Create("V0001001");


    public IEvent[] GetEvents()
    {
        return new IEvent[]
        {
            new FeitelijkeVerenigingWerdGeregistreerd(
                VCode,
                Naam,
                KorteNaam ?? string.Empty,
                KorteBeschrijving ?? string.Empty,
                _startdatum,
                new Registratiedata.Doelgroep(20,71),
                false,
                new[] { _contactgegeven },
                new[] { _locatie, _locatie2, _locatie3 },
                new[] { _vertegenwoordiger },
                Hoofdactiviteiten),
        };
    }

    public CommandMetadata GetCommandMetadata()
        => new("OVO000001", new Instant(), Guid.NewGuid());
}
