namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.CommandHandling;

using AssociationRegistry.Framework;
using Vereniging;

public abstract class CommandhandlerScenarioBase
{
    public abstract VCode VCode { get; }
    public abstract IEnumerable<IEvent> Events();

    public VerenigingState GetVerenigingState()
    {
        var verenigingState = new VerenigingState();

        foreach (var evnt in Events())
        {
            verenigingState = verenigingState.Apply((dynamic)evnt);
        }

        return verenigingState;
    }
}
