namespace AssociationRegistry.Test.Admin.Api.CommandHandler.Scenarios;

using AssociationRegistry.Framework;

public class EmptyScenario : IScenario
{
    public IEnumerable<IEvent> Events()
        => Array.Empty<IEvent>();
}
