namespace AssociationRegistry.Admin.ProjectionHost.Projections.Search;

using JasperFx.Events;

public interface IMartenEventsConsumer
{
    Task ConsumeAsync(IReadOnlyList<IEvent> streamActions);
}
