namespace AssociationRegistry.Test.Admin.Api.CommandHandler.Scenarios;

using AssociationRegistry.Framework;

public interface IScenario
{
    IEnumerable<IEvent> Events();
}
