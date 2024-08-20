namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling;

using AssociationRegistry.Framework;
using AssociationRegistry.Vereniging;

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
