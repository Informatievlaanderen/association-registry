namespace AssociationRegistry.Test.Admin.Api.CommandHandler.Given_A_VerenigingWerdGeregistreerd;

using Scenarios;
using Vereniging;

public class Given_A_Scenario_CommandHandlerFixture<TScenario> where TScenario: class, IScenario, new()
{
    private readonly Vereniging _vereniging;
    public TScenario Scenario { get; }

    public VerenigingRepositoryMock VerenigingRepositoryMock
        => new(_vereniging);

    public Given_A_Scenario_CommandHandlerFixture()
    {
        _vereniging = new Vereniging();
        Scenario = new TScenario();

        foreach (var evnt in Scenario.Events())
        {
            _vereniging.Apply((dynamic)evnt);
        }
    }
}
