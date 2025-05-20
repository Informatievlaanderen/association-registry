namespace AssociationRegistry.Test.E2E.Scenarios.Givens;

using Vereniging;
using IEvent = Events.IEvent;

public class EmptyScenario : Framework.TestClasses.IScenario
{
    public async Task<KeyValuePair<string, IEvent[]>[]> GivenEvents(IVCodeService service)
        => [];
}
