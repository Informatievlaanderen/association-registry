namespace AssociationRegistry.Test.Admin.Api.Scenarios;

using AssociationRegistry.Framework;

public interface IScenario
{
    IEnumerable<IEvent> Events();
}
