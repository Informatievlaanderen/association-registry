namespace AssociationRegistry.Test.E2E.Framework.TestClasses;

using Events;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vereniging;

public interface IScenario
{
    Task<KeyValuePair<string, IEvent[]>[]> GivenEvents(IVCodeService service);
}
