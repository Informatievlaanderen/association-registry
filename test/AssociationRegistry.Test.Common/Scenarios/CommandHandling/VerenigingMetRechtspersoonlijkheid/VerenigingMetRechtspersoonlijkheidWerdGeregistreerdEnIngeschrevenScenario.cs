namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;

using AssociationRegistry.Events;
using AssociationRegistry.Kbo;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using global::AutoFixture;
using Vereniging.Verenigingstype;

public class VerenigingMetRechtspersoonlijkheidWerdGeregistreerdEnIngeschrevenScenario : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009003");
    public KboNummer KboNummer => KboNummer.Create(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.KboNummer);
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;

    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerdEnIngeschrevenScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with { VCode = VCode };
    }

    public override IEnumerable<IEvent> Events()
        => new IEvent[]
        {
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            new VerenigingWerdIngeschrevenOpWijzigingenUitKbo(KboNummer),
        };

    public VerenigingVolgensKbo VerenigingVolgensKbo
        => new()
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
