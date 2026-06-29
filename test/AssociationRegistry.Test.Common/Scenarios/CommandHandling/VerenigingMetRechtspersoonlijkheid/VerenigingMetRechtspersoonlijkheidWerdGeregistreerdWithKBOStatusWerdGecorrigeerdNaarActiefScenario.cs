namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;

using global::AutoFixture;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;

public class VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithKBOStatusWerdGecorrigeerdNaarActiefScenario
    : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009011");

    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;

    public readonly VerenigingWerdGestoptInKBO VerenigingWerdGestoptInKBO;
    public readonly KBOStatusWerdGecorrigeerdNaarActief KBOStatusWerdGecorrigeerdNaarActief;

    private IEvent[] _events;

    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithKBOStatusWerdGecorrigeerdNaarActiefScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = VCode,
            };
        var einddatum = DateOnly.FromDateTime(DateTime.Now.AddDays(-fixture.Create<int>()));

        VerenigingWerdGestoptInKBO = fixture.Create<VerenigingWerdGestoptInKBO>() with { Einddatum = einddatum };

        KBOStatusWerdGecorrigeerdNaarActief = fixture.Create<KBOStatusWerdGecorrigeerdNaarActief>() with
        {
            VCode = VCode,
        };

        _events =
        [
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            VerenigingWerdGestoptInKBO,
            KBOStatusWerdGecorrigeerdNaarActief,
        ];
    }

    public override IEnumerable<IEvent> Events() => _events;
}
