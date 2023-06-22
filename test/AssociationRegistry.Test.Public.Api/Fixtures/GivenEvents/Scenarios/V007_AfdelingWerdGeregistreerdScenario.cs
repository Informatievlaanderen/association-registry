namespace AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents.Scenarios;

using Events;
using AssociationRegistry.Framework;
using Vereniging;
using NodaTime;

public class V007_AfdelingWerdGeregistreerdScenario : IScenario
{
    private readonly Registratiedata.Contactgegeven _contactgegeven = new(
        ContactgegevenId: 1,
        ContactgegevenType.Email,
        "info@FOud.be",
        "Algemeen",
        IsPrimair: true);

    private readonly  Registratiedata.Locatie _locatie = new(
        1,
        "Correspondentie",
        new Registratiedata.Adres("berglaan",
        "12",
        "B",
        "2000",
        "Antwerpen",
        "België"),
        IsPrimair: true,
        "Correspondentie");

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

    public readonly string? KorteBeschrijving = "balpenverzamelaars van antwerpse bijscholingen";
    public readonly string? KorteNaam = "ABC";

    public readonly string Naam = "Antwerpse Bijscholing Clickers";

    public VCode VCode
        => VCode.Create("V0001007");


    public IEvent[] GetEvents()
    {
        return new IEvent[]
        {
            new AfdelingWerdGeregistreerd(
                VCode,
                Naam,
                new AfdelingWerdGeregistreerd.MoederverenigingsData("0123456789", string.Empty, "Moeder 0123456789"),
                KorteNaam ?? string.Empty,
                KorteBeschrijving ?? string.Empty,
                _startdatum,
                new[] { _contactgegeven },
                new[] { _locatie },
                new[] { _vertegenwoordiger },
                Hoofdactiviteiten),
        };
    }

    public CommandMetadata GetCommandMetadata()
        => new("OVO000001", new Instant());
}
