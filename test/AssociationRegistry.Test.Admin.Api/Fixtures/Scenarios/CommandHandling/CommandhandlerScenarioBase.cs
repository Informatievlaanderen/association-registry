namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.CommandHandling;

using AssociationRegistry.Framework;
using AssociationRegistry.Vereniging;

public abstract class CommandhandlerScenarioBase
{
    public abstract IEnumerable<IEvent> Events();

    public Vereniging GetVereniging()
    {
        var vereniging = new Vereniging();

        foreach (var evnt in Events()) vereniging.Apply((dynamic)evnt);

        return vereniging;
    }
}