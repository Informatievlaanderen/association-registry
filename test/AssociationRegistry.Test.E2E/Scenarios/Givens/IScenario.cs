namespace AssociationRegistry.Test.E2E.Scenarios.Givens;

using Events;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vereniging;

public interface IScenario
{
    Task<Dictionary<string, IEvent[]>> GivenEvents(IVCodeService service);
}
