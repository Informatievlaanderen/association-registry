namespace AssociationRegistry.Test.E2E.Framework.TestClasses;

using AssociationRegistry.Framework;
using Events;
using Vereniging;

public interface IScenario
{
    Task<KeyValuePair<string, IEvent[]>[]> GivenEvents(IVCodeService service);
}
