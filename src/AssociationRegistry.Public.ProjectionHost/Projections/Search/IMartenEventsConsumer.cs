namespace AssociationRegistry.Public.ProjectionHost.Projections.Search;

using Marten.Events;

public interface IMartenEventsConsumer
{
    Task ConsumeAsync(IReadOnlyList<StreamAction> streamActions);
}
