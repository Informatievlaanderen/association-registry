namespace AssociationRegistry.Test.E2E.Framework.TestClasses;

using AssociationRegistry.Framework;
using Vereniging;

public interface IScenario
{
    Task<Dictionary<string, IEvent[]>> GivenEvents(IVCodeService service);
}
