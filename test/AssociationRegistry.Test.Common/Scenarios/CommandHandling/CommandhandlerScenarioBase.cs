namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling;

using DecentraalBeheer.Vereniging;
using Events;

public abstract class CommandhandlerScenarioBase : ICommandHandlerScenarioBase
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

public interface ICommandHandlerScenarioBase
{
    public IEnumerable<IEvent> Events();
}
