namespace AssociationRegistry.Test.Admin.Api.CommandHandler;

using AssociationRegistry.Test.Admin.Api.Scenarios;
using AssociationRegistry.Vereniging;

public class Given_A_Scenario_CommandHandlerFixture<TScenario> where TScenario: class, IScenario, new()
{
    public readonly Vereniging Vereniging;
    public readonly TScenario Scenario;

    public VerenigingRepositoryMock VerenigingRepositoryMock
        => new(Vereniging);

    public Given_A_Scenario_CommandHandlerFixture()
    {
        Vereniging = new Vereniging();
        Scenario = new TScenario();

        foreach (var evnt in Scenario.Events())
        {
            Vereniging.Apply((dynamic)evnt);
        }
    }
}
