namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Builders;

using AutoFixture;
using Common.Scenarios.CommandHandling;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Events;

public class VzerScenarioBuilder
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario VzerWerdGeregistreerdScenario = new();
    private IList<IEvent> Events = new List<IEvent>();
    public CommandhandlerScenarioBase Vzer { get; private set; } = null!;

    public VzerScenarioBuilder() { }

    public CommandhandlerScenarioBase Build()
    {
        VzerWerdGeregistreerdScenario.additionalEvents.AddRange(Events);

        return VzerWerdGeregistreerdScenario;
    }
}
