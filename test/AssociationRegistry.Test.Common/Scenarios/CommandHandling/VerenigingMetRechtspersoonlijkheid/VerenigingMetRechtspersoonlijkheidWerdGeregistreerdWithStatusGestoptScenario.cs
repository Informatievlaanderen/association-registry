namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;

using global::AutoFixture;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;

public class VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithStatusGestoptScenario : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009011");

    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;

    public readonly VerenigingWerdGestoptInKBO VerenigingWerdGestoptInKBO;

    private IEvent[] _events;

    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithStatusGestoptScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = VCode,
            };
        var einddatum = DateOnly.FromDateTime(DateTime.Now.AddDays(-fixture.Create<int>()));

        VerenigingWerdGestoptInKBO = fixture.Create<VerenigingWerdGestoptInKBO>() with { Einddatum = einddatum };

        _events = [VerenigingMetRechtspersoonlijkheidWerdGeregistreerd, VerenigingWerdGestoptInKBO];
    }

    public override IEnumerable<IEvent> Events() => _events;
}
