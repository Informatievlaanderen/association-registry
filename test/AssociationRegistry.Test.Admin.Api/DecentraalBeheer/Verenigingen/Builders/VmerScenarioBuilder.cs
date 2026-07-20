namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Builders;

using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Events;

public class VmerScenarioBuilder
{
    private readonly IFixture _fixture;
    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario VmerWerdGeregistreerdScenario = new();
    private IList<IEvent> Events = new List<IEvent>();
    public CommandhandlerScenarioBase Vzer { get; private set; } = null!;

    public VmerScenarioBuilder()
    {
        _fixture = new Fixture().CustomizeAdminApi();
    }

    public CommandhandlerScenarioBase Build()
    {
        VmerWerdGeregistreerdScenario.additionalEvents.AddRange(Events);

        return VmerWerdGeregistreerdScenario;
    }

    public VmerScenarioBuilder GivenActieveVereniging()
    {
        var today = DateTime.Today;
        var randomDateInFuture = today.AddDays(_fixture.Create<int>());

        var startdatumWerdGewijzigd = _fixture.Create<StartdatumWerdGewijzigd>() with
        {
            Startdatum = DateOnly.FromDateTime(randomDateInFuture),
        };

        Events.Add(startdatumWerdGewijzigd);

        return this;
    }
}
