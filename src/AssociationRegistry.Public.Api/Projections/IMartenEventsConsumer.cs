namespace AssociationRegistry.Test.Public.Api.IntegrationTests.Fixtures;

using System.Collections.Generic;
using System.Threading.Tasks;
using Marten.Events;

public interface IMartenEventsConsumer
{
    Task ConsumeAsync(IReadOnlyList<StreamAction> streamActions);
}
