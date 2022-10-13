namespace AssociationRegistry.Test.Public.Api.IntegrationTests.Fixtures;

using Marten.Events;

public interface IMartenEventsConsumer
{
    Task ConsumeAsync(IReadOnlyList<StreamAction> streamActions);
}
