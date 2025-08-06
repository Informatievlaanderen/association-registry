namespace AssociationRegistry.Test.E2E.Framework.TestClasses;

using DecentraalBeheer.Vereniging;
using Events;
using Vereniging;

public interface IScenario
{
    Task<KeyValuePair<string, IEvent[]>[]> GivenEvents(IVCodeService service);
}
