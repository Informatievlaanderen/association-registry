namespace AssociationRegistry.Test.E2E.V2.Scenarios.Givens;

using AssociationRegistry.Framework;
using Vereniging;

public interface IScenario
{
    Task<Dictionary<string, IEvent[]>> GivenEvents(IVCodeService service);
}
