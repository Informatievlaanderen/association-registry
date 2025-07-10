namespace AssociationRegistry.Public.ProjectionHost.Projections.Search;

using JasperFx.Events;

public interface IMartenEventsConsumer
{
    Task ConsumeAsync(IReadOnlyList<IEvent> streamActions);
}
