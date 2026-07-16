namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;

using global::AutoFixture;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;
using Magda.Kbo;

public class VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0019002");
    public KboNummer KboNummer => KboNummer.Create(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.KboNummer);
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;

    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = VCode,
            };
    }

    public readonly List<IEvent> additionalEvents = new();

    public override IEnumerable<IEvent> Events() =>
        new IEvent[] { VerenigingMetRechtspersoonlijkheidWerdGeregistreerd }.Concat(additionalEvents);

    public VerenigingVolgensKbo VerenigingVolgensKbo =>
        new()
        {
            Naam = VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.Naam,
            KboNummer = KboNummer.Create(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.KboNummer),
            Startdatum = VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.Startdatum,
            Type = Verenigingstype.Parse(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.Rechtsvorm),
            KorteNaam = VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.KorteNaam,
            Adres = new AdresVolgensKbo(),
            Contactgegevens = new ContactgegevensVolgensKbo(),
            Vertegenwoordigers = Array.Empty<VertegenwoordigerVolgensKbo>(),
            IsActief = true,
            EindDatum = null,
        };
}

public class VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithAdresScenario : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009002");
    public KboNummer KboNummer => KboNummer.Create(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.KboNummer);
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;
    public readonly MaatschappelijkeZetelWerdOvergenomenUitKbo MaatschappelijkeZetelWerdOvergenomenUitKbo;

    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithAdresScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = VCode,
            };

        MaatschappelijkeZetelWerdOvergenomenUitKbo = new MaatschappelijkeZetelWerdOvergenomenUitKbo(
            Locatie: new Registratiedata.Locatie(
                LocatieId: 1,
                Naam: string.Empty,
                IsPrimair: false,
                AdresId: null,
                Locatietype: Locatietype.MaatschappelijkeZetelVolgensKbo.Waarde,
                Adres: fixture.Create<Registratiedata.Adres>()
            )
        );
    }

    public override IEnumerable<IEvent> Events() =>
        new IEvent[]
        {
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            MaatschappelijkeZetelWerdOvergenomenUitKbo,
        };

    public VerenigingVolgensKbo VerenigingVolgensKbo =>
        new()
        {
            Naam = VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.Naam,
            KboNummer = KboNummer.Create(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.KboNummer),
            Startdatum = VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.Startdatum,
            Type = Verenigingstype.Parse(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.Rechtsvorm),
            KorteNaam = VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.KorteNaam,
            Adres = AdresVolgensKbo,
            Contactgegevens = new ContactgegevensVolgensKbo(),
            Vertegenwoordigers = Array.Empty<VertegenwoordigerVolgensKbo>(),
            IsActief = true,
            EindDatum = null,
        };

    public AdresVolgensKbo AdresVolgensKbo =>
        new()
        {
            Straatnaam = MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres.Straatnaam,
            Huisnummer = MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres.Huisnummer,
            Busnummer = MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres.Busnummer,
            Postcode = MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres.Postcode,
            Gemeente = MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres.Gemeente,
            Land = MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres.Land,
        };
}
