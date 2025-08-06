namespace AssociationRegistry.Test.E2E.Scenarios.Givens;

using DecentraalBeheer.Vereniging;
using Vereniging;
using IEvent = Events.IEvent;

public class EmptyScenario : Framework.TestClasses.IScenario
{
    public async Task<KeyValuePair<string, IEvent[]>[]> GivenEvents(IVCodeService service)
        => [];
}
