namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Builders;

using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Events;

public class VzerScenarioBuilder
{
    private readonly IFixture _fixture;
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario VzerWerdGeregistreerdScenario = new();
    private IList<IEvent> Events = new List<IEvent>();
    public CommandhandlerScenarioBase Vzer { get; private set; } = null!;

    public VzerScenarioBuilder()
    {
        _fixture = new Fixture().CustomizeAdminApi();
    }

    public CommandhandlerScenarioBase Build()
    {
        VzerWerdGeregistreerdScenario.additionalEvents.AddRange(Events);

        return VzerWerdGeregistreerdScenario;
    }

    public VzerScenarioBuilder GivenStopgezetteVereniging()
    {
        var verenigingWerdGestopt = _fixture.Create<VerenigingWerdGestopt>();

        Events.Add(verenigingWerdGestopt);

        return this;
    }
}
