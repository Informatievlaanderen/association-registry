namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.CommandHandling;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using Framework;
using Kbo;
using Vereniging;

public class VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009002");
    public KboNummer KboNummer => KboNummer.Create(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.KboNummer);
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;

    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with { VCode = VCode };
    }

    public override IEnumerable<IEvent> Events()
        => new IEvent[]
        {
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
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
        };
}
